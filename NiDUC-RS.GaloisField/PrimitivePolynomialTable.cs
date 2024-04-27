namespace NiDUC_RS.GaloisField;

/// <summary>
/// Table of first 16 primitive polynomials for GF(2^m)
/// </summary>
public static class PrimitivePolynomialTable {
    private readonly static Dictionary<byte, int> PolynomialTable = new(
                                                                        new[] {
                                                                            new KeyValuePair<byte, int>(1, 0b0011),
                                                                            new(2, 0b0111),
                                                                            new(3, 0b1011),
                                                                            new(4, 0b0001_0011),
                                                                            new(5, 0b0010_0101),
                                                                            new(6, 0b0100_0011),
                                                                            new(7, 0b1000_0011),
                                                                            new(8, 0b0001_0110_0011),
                                                                            new(9, 0b0010_0001_0001),
                                                                            new(10, 0b0100_0000_1001),
                                                                            new(11, 0b1000_0000_0101),
                                                                            new(12, 0b0001_0000_1001_1001),
                                                                            new(13, 0b0010_0000_0001_1011),
                                                                            new(14, 0b0101_1000_0000_0011),
                                                                            new(15, 0b1000_0000_0000_0011),
                                                                            new(16, 0b0001_0000_0000_0010_1101),
                                                                        }
                                                                       );

    public static int GetPrimitivePolynomial(byte degree) {
        if (!PolynomialTable.TryGetValue(degree, out var polynomial)) {
            throw new IndexOutOfRangeException("Non existing degree");
        }

        return polynomial;
    }
}