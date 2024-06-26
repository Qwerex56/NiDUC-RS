﻿using NiDUC_RS.GaloisField;
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

        _packetBuilder = new(InformationLength * WordSize, 16, 16);
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
            var bits = fileFormatter.ReadBits(_packetBuilder.InformationSize);
            var packet = _packetBuilder.BuildPacket(bits, packets.Count);
            
            var encodedMessage = EncodeMessage(packet);
            packets.Add(encodedMessage);
        }

        if (_packetBuilder.HasCache()) {
            var packet = _packetBuilder.BuildPacketFromCache(packets.Count);
            var encodedPacket = EncodeMessage(packet);
            
            packets.Add(encodedPacket);
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
            var packet = _packetBuilder.BuildPacket(bits, packets.Count);
            
            var encodedMessage = EncodeMessage(packet);
            packets.Add(encodedMessage);
        }

        if (_packetBuilder.HasCache()) {
            var packet = _packetBuilder.BuildPacketFromCache(packets.Count);
            var encodedPacket = EncodeMessage(packet);
            
            packets.Add(encodedPacket);
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

    public string ReceiveFile(string[] bitStringPacket) {
        var decodedMessage = string.Empty;

        foreach (var packet in bitStringPacket) {
            var decodedPacket = DecodeMessage(packet)[..^48];

            var receivedBits = _packetBuilder.ParsePacket(decodedPacket).packetData;

            decodedMessage += receivedBits;
        }

        var fileFormatter = RsFileFormatter.FromBinaryString(decodedMessage);
        var message = fileFormatter.ParseToString();

        var file = new FileStream("./parsed.png", FileMode.Create);

        for (var i = 0; i < message.Length; ++i) {
            var b = decodedMessage[(i * 8)..(i * 8 + 8)];
            file.WriteByte(Convert.ToByte(b, 2));
        }
        
        return message;
    }

    /// <summary>
    /// Decodes and glue all received packets
    /// </summary>
    /// <param name="bitStringPacket">encoded packet</param>
    /// <returns>Original message string</returns>
    public string ReceiveString(string[] bitStringPacket) {
        var decodedMessage = string.Empty;
        
        foreach (var packet in bitStringPacket) {
            var decodedPacket = DecodeMessage(packet)[..^48];

            var receivedBits = _packetBuilder.ParsePacket(decodedPacket).packetData;

            decodedMessage += receivedBits;
        }

        var stringFormatter = RsStringFormatter.FromBinaryString(decodedMessage);
        var message = stringFormatter.ParseToString();
        
        return message;
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

        var errPositions = BruteForceErrorsVals(ref errLocPoly, errCount);
        var syndromePoly = CreateSyndromePolynomial(syndromeVec);

        if (errPositions.Count != errCount) {
            throw new($"Cannot decode message, error count differ from error position count. Error count: {errCount}," +
                      $" error position count {errPositions.Count}");
        }

        var errEvaluator = FindErrorEvaluator(ref errLocPoly, ref syndromePoly);
        var errLocDerivative = errLocPoly.FormalDerivative();

        var errorPoly = new Gf2Polynomial();

        for (var i = 0; i < errCount; ++i) {
            var locator = new Gf2Math(errPositions[i]);

            var revLocator = new Gf2Math(Gf2Math.GaloisField.Gf2MaxExponent + 1 - locator.Exponent);
            var omegaEval = errEvaluator.EvalGf2Polynomial(revLocator);
            var derivativeValue = errLocDerivative.EvalGf2Polynomial(revLocator);

            var error = locator * omegaEval / derivativeValue;

            errorPoly += new PolynomialWord(error.Exponent, locator.Exponent ?? 0);
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
            new(0, 0)
        ]);
        var x = new PolynomialWord(0, 1);

        for (var exp = 1; exp < 2 * _e3C; ++exp) {
            var alpha = new PolynomialWord(exp, 0);
            var partPoly = new Gf2Polynomial([x, alpha]);
            genPoly *= partPoly;
        }

        GenerativePoly = genPoly;
    }

    private List<Gf2Math> CalculateSyndromeVector(in Gf2Polynomial poly) {
        var syndromeVector = new List<Gf2Math>();

        for (var i = 0; i < 2 * _e3C; ++i) {
            var polyEval = poly.EvalGf2Polynomial(new(i));
            syndromeVector.Add(polyEval);
        }

        return syndromeVector;
    }

    public Gf2Polynomial FindErrorLocator(in List<Gf2Math> syndromeVec) {
        var length = 0; // 0
        var lambda = new Gf2Polynomial(new PolynomialWord(0, 0)); // 1

        var auxPoly = new Gf2Polynomial(new PolynomialWord(0, 1)); // 1

        for (var r = 1; r <= 2 * _e3C; ++r) {
            var delta = new Gf2Math(); // 0

            for (var j = 1; j <= length; ++j) {
                var syn = syndromeVec[r - 1 - j];
                var lambdaJ = lambda[lambda.PolyLength - j - 1];
                delta += lambdaJ * syn;
            }

            delta += syndromeVec[r - 1];

            var prevLambda = lambda;

            var deltaAsPoly = new Gf2Polynomial(delta);


            if (delta.Exponent is not null) {
                lambda += deltaAsPoly * auxPoly;

                if (2 * length < r) {
                    length = r - length;

                    auxPoly = prevLambda * deltaAsPoly.ReverseExponents();
                }
            }

            auxPoly *= new PolynomialWord(0, 1);
        }

        return lambda;
    }

    private static List<int> BruteForceErrorsVals(ref Gf2Polynomial errLocator, int stopAfter) {
        var errLocatorRoots = new List<int>();

        for (var i = 0; i <= Gf2Math.GaloisField.Gf2MaxExponent; ++i) {
            if (errLocator.EvalGf2Polynomial(new(i)).Exponent is not null) continue;

            errLocatorRoots.Add(Gf2Math.GaloisField.Gf2MaxExponent + 1 - i);
        }

        return errLocatorRoots;
    }

    private Gf2Polynomial CreateSyndromePolynomial(in List<Gf2Math> syndromeVec) {
        var poly = new Gf2Polynomial();

        for (var exp = 0; exp < 2 * _e3C; ++exp) {
            poly += new PolynomialWord(syndromeVec[exp].Exponent, exp);
        }

        return poly;
    }

    private Gf2Polynomial FindErrorEvaluator(ref Gf2Polynomial errLocator, ref Gf2Polynomial syndromePolynomial) {
        var x2T = new Gf2Polynomial(new PolynomialWord(0, 2 * _e3C));

        return (errLocator * syndromePolynomial) % x2T;
    }
}