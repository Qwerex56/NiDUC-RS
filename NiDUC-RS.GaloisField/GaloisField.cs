namespace NiDUC_RS.GaloisField;

public class GaloisField {
    private readonly Dictionary<byte, byte> _field = [];

    /// <summary>
    /// Returns nth exponent of alpha in Galois field
    /// </summary>
    /// <param name="exp">
    /// Exponent from which to retrieve value
    /// NOTE: if exp is null, 0 is returned
    /// </param>
    /// <returns></returns>
    public (byte?, byte) GetGaloisWord(byte? exp = null) => exp is null ? (null, 0) : (exp, _field[(byte)exp]);

    /// <summary>
    /// Generates lookup table for GF(2^m)
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
        m = byte.Clamp(1, 16, m);
        
        for (byte i = 0; i < m; ++i) {
            _field.Add(i, (byte)(1 << i));
        }
    }
}