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
    private RsFileFormatter _rsFileFormatter;

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
        // _rsFileFormatter = new();
    }

    public void SendFile(string filePath) {
        
    }

    public void SendString(string message) {
        
    }

    public string ReceiveFile() {
        throw new NotImplementedException();
    }

    public string ReceiveString() {
        throw new NotImplementedException();
    }

    private string EncodeMessage(string message) {
        var polyMessage = StringToPolynomial(message);

        polyMessage *= new Gf2Polynomial([new(0, GenerativePoly.GetPolynomialDegree())]);
        var remainder = polyMessage % GenerativePoly;
        var codeWord = polyMessage + remainder;

        return codeWord.ToBinaryString(_gfDegree);
    }

    private Gf2Polynomial DecodeMessage() {
        throw new NotImplementedException();
    }

    private void SendBadPacketRequest(int packetId) {
        throw new NotImplementedException();
    }

    private Gf2Polynomial StringToPolynomial(string message) {
        const int @base = 2;
        
        var wordCount = message.Length / _gfDegree + (message.Length % _gfDegree != 0 ? 1 : 0);
        var elements = new byte[wordCount];
        
        for (var id = wordCount - 1; id >= 0; --id) {
            string value;
            try {
                value = message.Substring(id * _gfDegree, _gfDegree);
            }
            catch (ArgumentOutOfRangeException) {
                value = message[(id * _gfDegree)..];
            }
            
            elements[id] = Convert.ToByte(value, @base);
        }

        var exponent = elements.Length - 1;
        var poly = new Gf2Polynomial();
        
        foreach (var b in elements) {
            var alpha = Gf2Math.GaloisField?.GetByValue(b);
            poly += new Gf2Polynomial([new (alpha, exponent)]);
            
            --exponent;
        }

        return poly;
    }

    private void GenerateGenPoly() {
        var genPoly = new Gf2Polynomial([
            new (0, 1),
            new (1, 0)
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