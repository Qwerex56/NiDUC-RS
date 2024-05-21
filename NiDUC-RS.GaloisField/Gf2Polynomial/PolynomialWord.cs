namespace NiDUC_RS.GaloisField.Gf2Polynomial;

public record PolynomialWord(int? GfExp, int XExp) {
    public int XExp { get; set; } = XExp;

    public static PolynomialWord operator +(PolynomialWord lhs, PolynomialWord rhs) {
        if (lhs.XExp != rhs.XExp) throw new ArgumentException("Different exponents");

        return lhs with { GfExp = (new Gf2Math(lhs.GfExp) + new Gf2Math(rhs.GfExp)).Exponent };
    }

    public static PolynomialWord operator -(PolynomialWord lhs, PolynomialWord rhs) {
        return lhs + rhs;
    }

    public static PolynomialWord operator *(PolynomialWord lhs, PolynomialWord rhs) {
        return new PolynomialWord((new Gf2Math(lhs.GfExp) * new Gf2Math(rhs.GfExp)).Exponent,
                                  lhs.XExp + rhs.XExp);
    }

    public static PolynomialWord operator /(PolynomialWord lhs, PolynomialWord rhs) {
        if (rhs.GfExp is null) throw new DivideByZeroException();
        
        return new PolynomialWord((new Gf2Math(lhs.GfExp) / new Gf2Math(rhs.GfExp)).Exponent,
                                  lhs.XExp - rhs.XExp);
    }
}