using NiDUC_RS.GaloisField.Gf2Tables;

namespace NiDUC_RS.GaloisField;

public class Gf2Math(int? exponent) {
    private static Gf2LookUpTable? _galoisField;
    public static Gf2LookUpTable SetGf2 {
        set => _galoisField = value;
    }

    public int? Exponent { get; } = exponent;

    public static Gf2Math operator +(Gf2Math exp1, Gf2Math exp2) {
        if (_galoisField is null) {
            throw new NullReferenceException();
        }

        if (exp1.Exponent == exp2.Exponent) {
            return new Gf2Math(null);
        }

        var elementA = _galoisField.GetValueByExponent(exp1.Exponent);
        var elementB = _galoisField.GetValueByExponent(exp2.Exponent);

        var result = elementA ^ elementB;

        return new Gf2Math(_galoisField.GetByValue(result).Item1);
    }

    public static Gf2Math operator -(Gf2Math lhs, Gf2Math rhs) {
        if (_galoisField is null) {
            throw new NullReferenceException();
        }

        return lhs + rhs;
    }

    public static Gf2Math operator *(Gf2Math lhs, Gf2Math rhs) {
        if (_galoisField is null) {
            throw new NullReferenceException();
        }

        var exp = lhs.Exponent + rhs.Exponent;
        if (lhs.Exponent is null || rhs.Exponent is null) {
            exp = null;
        }

        return new Gf2Math(exp % _galoisField.Gf2ElementsCount);
    }

    public static Gf2Math operator /(Gf2Math lhs, Gf2Math rhs) {
        if (_galoisField is null) {
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
        if (_galoisField is null) {
            throw new NullReferenceException();
        }

        var quot = lhs / rhs;
        var modulo = lhs - quot * rhs;

        return modulo;
    }
    
}