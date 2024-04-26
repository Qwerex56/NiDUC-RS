namespace NiDUC_RS.GaloisField;

public class GaloisFieldLookUpTable {
    private readonly byte[] _field;
    private byte M { get; }

    /// <summary>
    /// Returns nth exponent of alpha in Galois field
    /// </summary>
    /// <param name="exp">
    /// Exponent from which to retrieve value
    /// NOTE: if exp is null, 0 is returned
    /// </param>
    /// <returns></returns>
    public (int?, byte) GetByExponent(int? exp = null) {
        if (exp is null) return (null, 0);

        exp %= _field.Length;
        var value = _field[(int)exp];

        return (exp, value);
    }

    public (byte?, byte) GetByValue(byte value = 0) {
        if (value >= MathF.Pow(2, M))
            throw new ArgumentException($"Trying to access not existing element of GF(2^{M})");

        var exp = 0;

        for (; exp < _field.Length; ++exp) {
            if (value == _field[exp]) break;
        }

        return ((byte)exp, value);
    }

    /// <summary>
    /// Generates lookup table for GF(2^m). <br/>
    /// Note: algorithm doesnt check for primitive polynomial validity
    /// </summary>
    /// <param name="m">
    /// Elements in GF(2^m),
    /// m is clamped to value between [1, 16]
    /// </param>
    /// <param name="primalPolynomial">
    /// Primal polynomial written as binary number,
    /// e.g. x^6 + x + 1 can be written as 1000011
    /// </param>
    public GaloisFieldLookUpTable(byte m, byte primalPolynomial) {
        // TODO: Get rid of magic values
        const byte minGfExp = 1; // Minimal number of exponents in GF2
        const byte maxGfExp = 16; // Max byte sqrt

        m = byte.Clamp(m, minGfExp, maxGfExp);
        M = m;

        var galoisElemCount = (int)MathF.Pow(2, M);
        _field = new byte[galoisElemCount - 1];

        for (var exp = 0; exp < galoisElemCount - 1; ++exp) {
            var alpha = (byte)0;

            try {
                alpha = _field[exp - 1];
                alpha <<= 1;
            } catch (IndexOutOfRangeException e) {
                alpha += 1;
            } finally {
                if (alpha >= galoisElemCount) {
                    alpha ^= primalPolynomial;
                }

                _field[exp] = alpha;
            }
        }
        
        Console.Write("");
    }
}