using NiDUC_RS.GaloisField;

namespace NiDUC_RS.RS_Coder;

public class ReedSalomonCoder {
    /// <summary>
    /// Galois field parameter
    /// </summary>
    private byte _gfDegree;
    
    /// <summary>
    /// Error Code Correction Capability
    /// </summary>
    private readonly int _e3C;

    private Gf2Polynomial _generativePoly = new ();
    
    public ReedSalomonCoder(byte gfDegree, int e3C) {
        _gfDegree = gfDegree;
        _e3C = e3C;
        
        var primitivePoly = PrimitivePolynomialTable.GetPrimitivePolynomial(_gfDegree);

        // Make sure that we have GF2
        Gf2Math.SetGf2 = new Gf2LookUpTable(_gfDegree, primitivePoly);
        GenerateGenPoly();
    }

    private void GenerateGenPoly() {
        var genPoly = new Gf2Polynomial([new PolynomialWord(0, 1),
                                        new PolynomialWord(1, 0)]);
        var x = new PolynomialWord(0, 1);
        
        for (var exp = 2; exp <= 2 * _e3C; ++exp) {
            var alpha = new PolynomialWord(exp, 0);
            var partPoly = new Gf2Polynomial([x, alpha]);
            genPoly *= partPoly;
        }

        Console.WriteLine(genPoly);
        _generativePoly = genPoly;
    }
}