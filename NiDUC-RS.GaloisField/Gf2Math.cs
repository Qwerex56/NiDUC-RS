using NiDUC_RS.GaloisField.Gf2Tables;

namespace NiDUC_RS.GaloisField;

public class Gf2Math(int? exponent) {
    public static Gf2LookUpTable SetGf2 {
        set => GaloisField = value;
    }

    public static Gf2LookUpTable GaloisField { get; private set; } =
        new(6, PrimitivePolynomialTable.GetPrimitivePolynomial(6));

    public int? Exponent { get; } = exponent;

    public static Gf2Math operator +(Gf2Math exp1, Gf2Math exp2) {
        if (GaloisField is null) {
            throw new NullReferenceException();
        }

        if (exp1.Exponent == exp2.Exponent) {
            return new Gf2Math(null);
        }

        var elementA = GaloisField.GetValueByExponent(exp1.Exponent);
        var elementB = GaloisField.GetValueByExponent(exp2.Exponent);

        var result = elementA ^ elementB;

        return new Gf2Math(GaloisField.GetByValue(result));
    }

    public static Gf2Math operator -(Gf2Math lhs, Gf2Math rhs) {
        if (GaloisField is null) {
            throw new NullReferenceException();
        }

        return lhs + rhs;
    }

    public static Gf2Math operator *(Gf2Math lhs, Gf2Math rhs) {
        if (GaloisField is null) {
            throw new NullReferenceException();
        }

        var exp = lhs.Exponent + rhs.Exponent;

        if (lhs.Exponent is null || rhs.Exponent is null) {
            exp = null;
        }

        return new (exp % (GaloisField.Gf2MaxExponent + 1));
    }

    public static Gf2Math operator /(Gf2Math lhs, Gf2Math rhs) {
        if (GaloisField is null) {
            throw new NullReferenceException();
        }

        if (rhs.Exponent is null) {
            throw new DivideByZeroException();
        }

        if (lhs.Exponent < rhs.Exponent) {
            return lhs;
        }

        return lhs * new Gf2Math(-rhs.Exponent);
    }

    public static Gf2Math operator %(Gf2Math lhs, Gf2Math rhs) {
        if (GaloisField is null) {
            throw new NullReferenceException();
        }

        var quot = lhs / rhs;
        var modulo = lhs - quot * rhs;

        return modulo;
    }
}