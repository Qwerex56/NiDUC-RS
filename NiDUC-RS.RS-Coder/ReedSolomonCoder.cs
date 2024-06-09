using NiDUC_RS.GaloisField;
using NiDUC_RS.GaloisField.Gf2Polynomial;
using NiDUC_RS.GaloisField.Gf2Tables;
using NiDUC_RS.RS_Coder.RsFormatters;

namespace NiDUC_RS.RS_Coder;

public class ReedSolomonCoder {
    /// <summary>
    /// Galois field parameter
    /// </summary>
    private readonly int _gfDegree;

    /// <summary>
    /// Error Code Correction Capability
    /// </summary>
    private readonly int _e3C;

    private Gf2Polynomial GenerativePoly { get; set; } = new();
    private Packet.PacketBuilder _packetBuilder;

    private int BlockLength => (int)(Math.Pow(2, _gfDegree) - 1);
    public int InformationLength => BlockLength - 2 * _e3C;

    // ReSharper disable once ConvertToAutoPropertyWhenPossible
    public int WordSize => _gfDegree;

    public ReedSolomonCoder(byte gfDegree, int e3C) {
        _gfDegree = gfDegree;
        _e3C = e3C;

        var primePoly = PrimitivePolynomialTable.GetPrimitivePolynomial(_gfDegree);

        // Make sure that we have GF2
        Gf2Math.SetGf2 = new(_gfDegree, primePoly);
        GenerateGenPoly();

        _packetBuilder = new(InformationLength * WordSize);
    }

    /// <summary>
    /// Reads file as binary form and splits it to encoded packet
    /// </summary>
    /// <param name="filePath">Path to file</param>
    /// <returns>encoded packet</returns>
    public List<string> SendFile(string filePath) {
        var fileFormatter = new RsFileFormatter(filePath);
        var packets = new List<string>();

        while (fileFormatter.CanRead()) {
            var bits = fileFormatter.ReadBits(InformationLength * WordSize);
            var encodedMessage = EncodeMessage(bits);
            packets.Add(encodedMessage);
        }

        return packets;
    }

    /// <summary>
    /// Reads simple message as binary and splits it to packets if needed
    /// </summary>
    /// <param name="message">message to send</param>
    /// <returns>encoded packets</returns>
    public List<string> SendString(string message) {
        var stringFormatter = new RsStringFormatter(message);
        var packets = new List<string>();

        while (stringFormatter.CanRead()) {
            var bits = stringFormatter.ReadBits(InformationLength * WordSize);
            var encodedMessage = EncodeMessage(bits);
            packets.Add(encodedMessage);
        }

        return packets;
    }

    /// <summary>
    /// Allows sending bits directly, splits message to packets if needed
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public List<string> SendBits(string message) {
        var stringFormatter = RsStringFormatter.FromBinaryString(message);
        var packets = new List<string>();

        while (stringFormatter.CanRead()) {
            var bits = stringFormatter.ReadBits(InformationLength * WordSize);
            var encodedMessage = EncodeMessage(bits);
            packets.Add(encodedMessage);
        }

        return packets;
    }

    public string ReceiveFile() {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Decodes and glue all received packets
    /// </summary>
    /// <param name="bitStringPacket">encoded pacet</param>
    /// <returns>Original message string</returns>
    public string ReceiveString(string bitStringPacket) {
        return SimplifiedDecodeMessage(bitStringPacket);
    }

    private string EncodeMessage(string message) {
        var polyMessage = Gf2Polynomial.FromBinaryString(message);

        polyMessage *= new Gf2Polynomial(new PolynomialWord(0, GenerativePoly.GetPolynomialDegree()));
        var remainder = polyMessage % GenerativePoly;
        var codeWord = polyMessage + remainder;

        return codeWord.ToBinaryString();
    }

    public string DecodeMessage(string bitStringMessage) {
        var poly = Gf2Polynomial.FromBinaryString(bitStringMessage);
        var syndromeVec = CalculateSyndromeVector(poly);

        // There is not any errors
        if (syndromeVec.Max(syn => syn.Exponent) == 0) {
            return poly.ToBinaryString();
        }

        var errLocPoly = FindErrorLocator(syndromeVec);
        var errCount = errLocPoly.GetPolynomialDegree();

        if (errCount > _e3C) {
            throw new($"Cannot decode message, error count in message {errCount}");
        }

        var errPositions = BruteForceErrorsVals(ref errLocPoly, errCount);
        var syndromePoly = CreateSyndromePolynomial(syndromeVec);

        var errEvaluator = FindErrorEvaluator(ref errLocPoly, ref syndromePoly);
        var errLocDerivative = errLocPoly.FormalDerivative();

        var errorPoly = new Gf2Polynomial();

        for (var i = 0; i < errCount; ++i) {
            var locator = new Gf2Math(errPositions[i]);

            var revLocator = new Gf2Math(Gf2Math.GaloisField.Gf2MaxExponent + 1 - locator.Exponent);

            var error = locator * errEvaluator.EvalGf2Polynomial(revLocator) /
                        errLocDerivative.EvalGf2Polynomial(revLocator);

            errorPoly += new PolynomialWord(error.Exponent, revLocator.Exponent ?? 0);
        }

        var correctedMsg = poly + errorPoly;

        return correctedMsg.ToBinaryString();
    }

    public string SimplifiedDecodeMessage(string bitStringMessage) {
        var polyMessage = Gf2Polynomial.FromBinaryString(bitStringMessage);

        for (var i = 0; i < BlockLength; ++i) {
            var syndrome = polyMessage % GenerativePoly;
            var syndromePop = syndrome.NotNullWordCount;

            if (syndromePop <= _e3C) {
                polyMessage += syndrome;
                polyMessage.LeftCycleShift(i + 1, BlockLength);

                return polyMessage.ToBinaryString();
            }

            polyMessage.RightCycleShift(1, BlockLength);
        }

        throw new("Cannot decode message");
    }

    private void SendBadPacketRequest(int packetId) {
        throw new NotImplementedException();
    }

    private void GenerateGenPoly() {
        var genPoly = new Gf2Polynomial([
            new(0, 1),
            new(1, 0)
        ]);
        var x = new PolynomialWord(0, 1);

        for (var exp = 2; exp <= 2 * _e3C; ++exp) {
            var alpha = new PolynomialWord(exp, 0);
            var partPoly = new Gf2Polynomial([x, alpha]);
            genPoly *= partPoly;
        }

        GenerativePoly = genPoly;
    }

    private List<Gf2Math> CalculateSyndromeVector(in Gf2Polynomial poly) {
        var syndromeVector = new List<Gf2Math>();

        for (var i = 1; i <= 2 * _e3C; ++i) {
            var polyEval = poly.EvalGf2Polynomial(new(i));
            syndromeVector.Add(polyEval);
        }

        return syndromeVector;
    }

    private Gf2Polynomial FindErrorLocator(in List<Gf2Math> syndromeVec) {
        var length = 0; // 0
        var lambda = new Gf2Polynomial(new PolynomialWord(0, 0)); // 1

        var auxPoly = new Gf2Polynomial(new PolynomialWord(0, 0)); // 1

        for (var r = 1; r <= 2 * _e3C; ++r) {
            var delta = new Gf2Math(); // 0

            for (var j = 0; j <= length; ++j) {
                var syn = syndromeVec[r - 1 - j];
                var lambdaJ = lambda[lambda.PolyLength - j - 1];
                delta += lambdaJ * syn;
            }

            if (delta.Exponent is null) break;

            var prevLambda = lambda;
            var deltaAsPoly = new Gf2Polynomial(delta);

            lambda -= deltaAsPoly * auxPoly * new PolynomialWord(0, 1);

            if (2 * length <= r - 1) {
                length = r - length;
                auxPoly = prevLambda * deltaAsPoly.ReverseExponents();
            } else {
                // length = length;
                auxPoly *= new PolynomialWord(0, 1);
            }

        }

        return lambda;
    }

    private List<Gf2Math> ChienSearch(in Gf2Polynomial errLocator) {
        throw new NotImplementedException();
    }

    private static List<int> BruteForceErrorsVals(ref Gf2Polynomial errLocator, int stopAfter) {
        var errLocatorRoots = new List<int>();

        for (var i = 0; i <= Gf2Math.GaloisField.Gf2MaxExponent; ++i) {
            if (errLocator.EvalGf2Polynomial(new(i)).ToString() != new Gf2Math().ToString()) continue;

            errLocatorRoots.Add(i);
        }

        return errLocatorRoots;
    }

    private Gf2Polynomial CreateSyndromePolynomial(in List<Gf2Math> syndromeVec) {
        var poly = new Gf2Polynomial();

        for (var exp = 0; exp < 2 * _e3C; ++exp) {
            poly += new PolynomialWord(syndromeVec[exp].Exponent, exp + 1);
            // poly += new PolynomialWord(syndromeVec[exp].Exponent, 2 * _e3C - exp);
        }

        return poly;
    }

    private Gf2Polynomial FindErrorEvaluator(ref Gf2Polynomial errLocator, ref Gf2Polynomial syndromePolynomial) {
        var x2TPlus1 = new Gf2Polynomial(new PolynomialWord(0, 2 * _e3C + 1));

        return (errLocator * syndromePolynomial) % x2TPlus1;
    }
}