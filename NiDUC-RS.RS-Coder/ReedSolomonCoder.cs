using NiDUC_RS.GaloisField;
using NiDUC_RS.GaloisField.Gf2Polynomial;
using NiDUC_RS.GaloisField.Gf2Tables;

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

    private Gf2Polynomial _generativePoly = new();
    public Gf2Polynomial GenerativePoly => _generativePoly;

    public ReedSolomonCoder(byte gfDegree, int e3C) {
        _gfDegree = gfDegree;
        _e3C = e3C;

        var primitivePoly = PrimitivePolynomialTable.GetPrimitivePolynomial(_gfDegree);

        // Make sure that we have GF2
        Gf2Math.SetGf2 = new Gf2LookUpTable(_gfDegree, primitivePoly);
        GenerateGenPoly();
    }

    public string EncodeMessage(string message) {
        var polyMessage = StringToPolynomial(message);

        polyMessage *= new Gf2Polynomial([new(0, _generativePoly.GetPolynomialDegree())]);
        var remainder = polyMessage % GenerativePoly;
        var codeWord = polyMessage + remainder;

        return codeWord.ToBinaryString(_gfDegree);
    }

    private Gf2Polynomial StringToPolynomial(string message) {
        var wordCount = message.Length / _gfDegree + (message.Length % _gfDegree != 0 ? 1 : 0);
        var elements = new byte[wordCount];

        for (var i = wordCount - 1; i >= 0; --i) {
            string value;
            try {
                value = message.Substring(i * _gfDegree, _gfDegree);
            }
            catch (ArgumentOutOfRangeException) {
                value = message.Substring(i * _gfDegree);
            }
            
            elements[i] = Convert.ToByte(value, 2);
        }

        var exponent = elements.Length - 1;
        var poly = new Gf2Polynomial();
        
        foreach (var b in elements) {
            var alpha = Gf2Math.GaloisField?.GetByValue(b);
            poly.Factors.Add(new (alpha, exponent));
            
            --exponent;
        }

        return poly;
    }

    private void GenerateGenPoly() {
        var genPoly = new Gf2Polynomial([
            new PolynomialWord(0, 1),
            new PolynomialWord(1, 0)
        ]);
        var x = new PolynomialWord(0, 1);

        for (var exp = 2; exp <= 2 * _e3C; ++exp) {
            var alpha = new PolynomialWord(exp, 0);
            var partPoly = new Gf2Polynomial([x, alpha]);
            genPoly *= partPoly;
        }

        _generativePoly = genPoly;
    }
}