namespace NiDUC_RS.GaloisField;

public class Gf2Math(int? exponent) {
    private static Gf2LookUpTable? _galoisField;
    public static Gf2LookUpTable SetGf2 {
        set => _galoisField = value;
    }
    
    private readonly int? _exponent = exponent;
    public int? Exponent => _exponent;

    public static Gf2Math operator +(Gf2Math exp1, Gf2Math exp2) {
        if (_galoisField is null) {
            throw new NullReferenceException();
        }

        if (exp1.Exponent == exp2.Exponent) {
            return new Gf2Math(null);
        }

        var (_, elementA) = _galoisField.GetByExponent(exp1._exponent);
        var (_, elementB) = _galoisField.GetByExponent(exp2._exponent);

        var result = (byte)(elementA ^ elementB);

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

        var exp = lhs._exponent + rhs._exponent;
        if (lhs._exponent is null || rhs._exponent is null) {
            exp = null;
        }

        return new Gf2Math(exp % _galoisField.Gf2ElementsCount);
    }

    public static Gf2Math operator /(Gf2Math lhs, Gf2Math rhs) {
        if (_galoisField is null) {
            throw new NullReferenceException();
        }

        if (rhs._exponent is null) {
            throw new DivideByZeroException();
        }

        if (lhs._exponent < rhs._exponent) {
            return lhs;
        }

        return lhs * new Gf2Math(-rhs._exponent);
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