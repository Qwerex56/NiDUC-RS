namespace NiDUC_RS.GaloisField;

public class GaloisField {
    private readonly Dictionary<byte, byte> _field = [];
    private byte M { get; }

    /// <summary>
    /// Returns nth exponent of alpha in Galois field
    /// </summary>
    /// <param name="exp">
    /// Exponent from which to retrieve value
    /// NOTE: if exp is null, 0 is returned
    /// </param>
    /// <returns></returns>
    public (byte?, byte) GetGaloisWord(byte? exp = null) =>
        exp is null ? (null, 0) : (exp, _field[(byte)(exp % (MathF.Pow(2, M) - 1))!]);

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
    public GaloisField(byte m, byte primalPolynomial) {
        // TODO: Get rid of magic values
        const byte minGfExp = 1;
        const byte maxGfExp = 16;
        
        m = byte.Clamp(m, minGfExp, maxGfExp);
        M = m;

        var galoisElemCount = (int)MathF.Pow(2, M);
        
        for (var exp = 0; exp < galoisElemCount - 1; ++exp) {
            if (_field.TryGetValue((byte)(exp - 1), out var alpha)) {
                alpha <<= 1;
            } else {
                alpha += 1;
            }
            
            if (alpha >= galoisElemCount) {
                alpha ^= primalPolynomial;
            }
            
            _field.Add((byte)exp, alpha);
        }
    }
}