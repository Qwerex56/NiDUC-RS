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
    private int InformationLength => BlockLength - 2 * _e3C;

    // ReSharper disable once ConvertToAutoPropertyWhenPossible
    private int WordSize => _gfDegree;

    public ReedSolomonCoder(byte gfDegree, int e3C) {
        _gfDegree = gfDegree;
        _e3C = e3C;

        var primePoly = PrimitivePolynomialTable.GetPrimitivePolynomial(_gfDegree);

        // Make sure that we have GF2
        Gf2Math.SetGf2 = new(_gfDegree, primePoly);
        GenerateGenPoly();

        _packetBuilder = new(InformationLength * WordSize);
    }

    public void SendFile(string filePath) {
        throw new NotImplementedException();
    }

    public void SendString(string message) {
        var stringFormatter = new RsStringFormatter(message);
        var counter = 0;

        while (stringFormatter.CanRead()) {
            var bits = stringFormatter.ReadBits(InformationLength * WordSize);
            var encodedMessage = EncodeMessage(bits);
            Console.Write($"Packet {counter++}: ");
            Console.WriteLine(encodedMessage);
        }

        Console.WriteLine("All packets has been sent");
    }

    public string ReceiveFile() {
        throw new NotImplementedException();
    }

    public string ReceiveString(string bitStringPacket) {
        // TODO: i had something on mind but never mind
        return SimplifiedDecodeMessage(bitStringPacket);
    }

    private string EncodeMessage(string message) {
        var polyMessage = StringToPolynomial(message);

        polyMessage *= new Gf2Polynomial([new(0, GenerativePoly.GetPolynomialDegree)]);
        var remainder = polyMessage % GenerativePoly;
        var codeWord = polyMessage + remainder;

        return codeWord.ToBinaryString();
    }

    private Gf2Polynomial DecodeMessage(string bitStringMessage) {
        throw new NotImplementedException();
    }

    private string SimplifiedDecodeMessage(string bitStringMessage) {
        for (var i = 0; i < InformationLength; ++i) {
            var polyMessage = StringToPolynomial(bitStringMessage);
            var syndrome = (polyMessage % GenerativePoly).ToBinaryString();
            var syndromePop = syndrome.Count(ch => ch == '1');

            if (syndromePop <= _e3C) {
                polyMessage += StringToPolynomial(syndrome);
                bitStringMessage = polyMessage.ToBinaryString();

                var shiftedString = bitStringMessage[..(i * WordSize)];
                bitStringMessage = bitStringMessage[(i * WordSize)..];

                return bitStringMessage + shiftedString;
            }

            var last = bitStringMessage[^WordSize..];
            bitStringMessage = bitStringMessage[..^WordSize];
            bitStringMessage = bitStringMessage.Insert(0, last);
        }

        throw new("Cannot decode message");
    }

    private void SendBadPacketRequest(int packetId) {
        throw new NotImplementedException();
    }

    private Gf2Polynomial StringToPolynomial(string message) {
        const int @base = 2;

        while (message.Length % WordSize != 0) {
            message = "0" + message;
        }

        var wordCount = message.Length / _gfDegree;
        var elements = new byte[wordCount];

        for (var id = 1; id <= wordCount; ++id) {
            string value;

            try {
                value = message.Substring(message.Length - id * _gfDegree, _gfDegree);
            } catch (ArgumentOutOfRangeException) {
                value = message[(id * _gfDegree)..];
            }

            elements[id - 1] = Convert.ToByte(value, @base);
        }

        var exponent = 0;
        var poly = new Gf2Polynomial();

        foreach (var b in elements) {
            var alpha = Gf2Math.GaloisField?.GetByValue(b);
            poly += new Gf2Polynomial([new(alpha, exponent)]);

            ++exponent;
        }

        return poly;
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