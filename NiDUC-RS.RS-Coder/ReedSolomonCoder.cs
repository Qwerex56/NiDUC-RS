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

    private Gf2Polynomial DecodeMessage(string bitStringMessage) {
        throw new NotImplementedException();
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
}