namespace NiDUC_RS.GaloisField;

// ReSharper disable once InconsistentNaming
public class GF2Math(int? exponent) {
    private static GaloisFieldLookUpTable? _galoisField;
    private readonly int? _exponent = exponent;

    public GaloisFieldLookUpTable SetGaloisField {
        set => _galoisField = value;
    }

    public static GF2Math operator +(GF2Math exp1, GF2Math exp2) {
        if (_galoisField is null) {
            // TODO: Add exception
            throw new Exception("Create GF(2) first.");
        }

        var (_, elementA) = _galoisField.GetByExponent(exp1._exponent);
        var (_, elementB) = _galoisField.GetByExponent(exp2._exponent);

        var result = (byte)(elementA ^ elementB);

        return new GF2Math(_galoisField.GetByValue(result).Item1);
    }

    public static GF2Math operator -(GF2Math lhs, GF2Math rhs) {
        if (_galoisField is null) {
            // TODO: Add exception
            throw new Exception("Create GF(2) first.");
        }

        return lhs + rhs;
    }

    public static GF2Math operator *(GF2Math lhs, GF2Math rhs) {
        if (_galoisField is null) {
            // TODO: Add exception
            throw new Exception("Create GF(2) first.");
        }

        var exp = lhs._exponent + rhs._exponent;
        if (lhs._exponent is null || rhs._exponent is null) {
            exp = null;
        }

        return new GF2Math(exp);
    }

    public static GF2Math operator /(GF2Math lhs, GF2Math rhs) {
        if (_galoisField is null) {
            // TODO: Add exception
            throw new Exception("Create GF(2) first.");
        }

        if (rhs._exponent is null) {
            throw new DivideByZeroException();
        }

        if (lhs._exponent < rhs._exponent) {
            return lhs;
        }

        return lhs * new GF2Math(-rhs._exponent);
    }

    public static GF2Math operator %(GF2Math lhs, GF2Math rhs) {
        if (_galoisField is null) {
            throw new Exception("Create GF(2) first");
        }

        var quot = lhs / rhs;
        var modulo = lhs - quot * rhs;

        return modulo;
    }
    
}