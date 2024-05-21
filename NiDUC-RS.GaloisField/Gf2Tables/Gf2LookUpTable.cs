namespace NiDUC_RS.GaloisField.Gf2Tables;

public class Gf2LookUpTable {
    private readonly int[] _field;
    public int GfDegree { get; }
    public int MaxGf2Value => _field.Max();
    public int Gf2ElementsCount => _field.Length + 1;
    public int Gf2NonZeroElementsCount => _field.Length;
    public int Gf2MaxExponent => _field.Length - 1;

    /// <summary>
    /// Returns nth exponent of alpha in Galois field
    /// </summary>
    /// <param name="exp">
    ///     Exponent from which to retrieve value
    ///     NOTE: if exp is null, 0 is returned
    /// </param>
    /// <returns></returns>
    public int GetValueByExponent(int? exp = null) {
        if (exp is null) return 0;

        exp %= _field.Length;
        var value = _field[(int)exp];

        return value;
    }

    public int? GetByValue(int value = 0) {
        ArgumentOutOfRangeException
            .ThrowIfGreaterThan(value, 
                                MaxGf2Value,
                                $"Element with value {value} is not present in GF2^{GfDegree}");
        ArgumentOutOfRangeException
            .ThrowIfNegative(value, $"Element with value {value} is not present in GF2^{GfDegree}");
        
        if (value == 0) {
            return null;
        }

        var exp = 0;

        for (; exp < _field.Length; ++exp) {
            if (value == _field[exp]) break;
        }

        return exp;
    }

    /// <summary>
    /// Generates lookup table for GF(2^m). <br/>
    /// Note: algorithm doesnt check for primitive polynomial validity
    /// </summary>
    /// <param name="gfDegree">
    /// Elements in GF(2^m),
    /// m is clamped to value between [1, 16]
    /// </param>
    /// <param name="primitivePolynomial">
    /// Primal polynomial written as binary number,
    /// e.g. x^6 + x + 1 can be written as 1000011
    /// </param>
    public Gf2LookUpTable(int gfDegree, int primitivePolynomial) {
        const int minGfExp = 1; // Minimal number of exponents in GF2
        const int maxGfExp = 16; // Max byte sqrt

        GfDegree = int.Clamp(gfDegree, minGfExp, maxGfExp);

        var galoisElemCount = (int)MathF.Pow(2, GfDegree);
        _field = new int[galoisElemCount - 1];

        for (var exp = 0; exp < galoisElemCount - 1; ++exp) {
            var alpha = 0;

            try {
                alpha = _field[exp - 1];
                alpha <<= 1;
            } catch (IndexOutOfRangeException) {
                alpha += 1;
            } finally {
                if (alpha >= galoisElemCount) {
                    alpha ^= primitivePolynomial;
                }

                _field[exp] = alpha;
            }
        }
    }
}