namespace NiDUC_RS.GaloisField.Gf2Polynomial;

public class Gf2Polynomial {
    public List<PolynomialWord> Factors { get; set; } = [];

    /// <summary>
    /// Creates empty polynomial.
    /// Use with math operations for place-holding values.
    /// </summary>
    public static Gf2Polynomial CreateEmpty => new() { Factors = [] };

    /// <summary>
    /// Creates GF2 polynomial filled with zeros.
    /// If Galois field is null, empty polynomial is created instead.
    /// </summary>
    public Gf2Polynomial() {
        PopulateWithZeros(this, Gf2Math.GaloisField.Gf2MaxExponent);
        Factors = Factors.OrderByDescending(word => word.XExp).ToList();
    }

    /// <summary>
    /// Creates polynomial from words, free exponents of 'x' fills with zeroes.
    /// </summary>
    /// <param name="words"></param>
    public Gf2Polynomial(List<PolynomialWord>? words = null) {
        Factors = AddSameWords(words).Factors;
        PopulateWithZeros(this, Gf2Math.GaloisField.Gf2MaxExponent);
    }

    /// <summary>
    /// Casts polynomial word to polynomial and fills other 'x' exponents with zeroes.
    /// </summary>
    /// <param name="word"></param>
    public Gf2Polynomial(PolynomialWord? word = null) {
        Factors.Add(word ?? new(null, 0));
        PopulateWithZeros(this, Gf2Math.GaloisField.Gf2MaxExponent);
    }

    /// <summary>
    /// Finds polynomial degree with non-zero factor.
    /// </summary>
    public int GetPolynomialDegree() {
        try {
            return Factors.Where(word => word.GfExp is not null).Max(word => word.XExp);
        } catch (ArgumentOutOfRangeException) {
            return 0;
        } catch (InvalidOperationException) {
            return 0;
        }
    }

    /// <summary>
    /// Counts all bits which value is equals 1.
    /// Mainly used to calculate syndrome weight.
    /// </summary>
    public int Population => Factors.Sum(word => int.PopCount(Gf2Math.GaloisField.GetValueByExponent(word.GfExp)));
    
    /// <summary>
    /// Counts all not null elements of polynomial
    /// </summary>
    public int NotNullWordCount => Factors.Count(word => word.GfExp is not null);

    /// <summary>
    /// Used to perform left cycle shifts on GF2 polynomials/vectors.
    /// Note that GF2 polynomial is not aware of max 'x' power.
    /// </summary>
    /// <param name="shifts">count of shifts.</param>
    /// <param name="maxXPow">x exponent cap.</param>
    /// <returns>itself</returns>
    public Gf2Polynomial LeftCycleShift(int shifts, int maxXPow) {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxXPow, 1);

        foreach (var word in Factors.Where(word => (word.XExp += shifts) > maxXPow)) {
            word.XExp -= maxXPow;
        }

        Factors = Factors.OrderByDescending(word => word.XExp).ToList();

        return this;
    }

    /// <summary>
    /// Used to perform right cycle shifts on GF2 polynomials/vectors.
    /// Note that GF2 polynomial is not aware of max 'x' power.
    /// </summary>
    /// <param name="shifts">count of shifts.</param>
    /// <param name="maxXPow">x exponent cap.</param>
    /// <returns>itself</returns>
    public Gf2Polynomial RightCycleShift(int shifts, int maxXPow) {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxXPow, 1);

        foreach (var word in Factors.Where(word => (word.XExp -= shifts) < 0)) {
            word.XExp += maxXPow;
        }

        Factors = Factors.OrderByDescending(word => word.XExp).ToList();

        return this;
    }

    /// <summary>
    /// Creates binary string.
    /// Note that GF2 polynomial is not aware of bit width of GF2 elements.
    /// </summary>
    /// <param name="wordSize">this value indicates how many bits is single GF2 word occupying</param>
    /// <returns>Binary representation of polynomial using string</returns>
    public string ToBinaryString() {
        var binaryString = string.Empty;
        var wordSize = Gf2Math.GaloisField.GfDegree;

        foreach (var t in Factors) {
            var gfVal = Gf2Math.GaloisField?.GetValueByExponent(t.GfExp) ?? 0;
            var word = Convert.ToString(gfVal, 2).PadLeft(wordSize, '0');
            binaryString += word;
        }

        return binaryString;
    }

    public static Gf2Polynomial operator +(in Gf2Polynomial lhs, in Gf2Polynomial rhs) {
        var combinedPoly = lhs.Factors.Concat(rhs.Factors).ToList();

        return AddSameWords(combinedPoly);
    }

    public static Gf2Polynomial operator +(in Gf2Polynomial lhs, in PolynomialWord rhs) {
        return lhs + new Gf2Polynomial(rhs);
    }

    public static Gf2Polynomial operator -(in Gf2Polynomial lhs, in Gf2Polynomial rhs) {
        return lhs + rhs;
    }

    public static Gf2Polynomial operator -(in Gf2Polynomial lhs, in PolynomialWord rhs) {
        return lhs + new Gf2Polynomial(rhs);
    }

    public static Gf2Polynomial operator *(in Gf2Polynomial lhs, in Gf2Polynomial rhs) {
        var multipliedPoly = CreateEmpty;

        foreach (var lhsWord in lhs.Factors) {
            if (lhsWord.GfExp is null) continue;

            foreach (var rhsWord in rhs.Factors) {
                if (rhsWord.GfExp is null) continue;
                multipliedPoly.Factors.Add(lhsWord * rhsWord);
            }
        }

        return AddSameWords(multipliedPoly);
    }

    public static Gf2Polynomial operator *(in Gf2Polynomial lhs, in PolynomialWord rhs) {
        return lhs * new Gf2Polynomial(rhs);
    }

    public static Gf2Polynomial operator /(Gf2Polynomial lhs, in Gf2Polynomial rhs) {
        if (rhs.Factors.All(word => word.GfExp is null)) throw new DivideByZeroException();

        if (lhs.GetPolynomialDegree() < rhs.GetPolynomialDegree()) {
            return new();
        }

        var resultPoly = CreateEmpty;
        var divisor = rhs.Factors.First(word => word.GfExp is not null);
        PopulateWithZeros(lhs, Gf2Math.GaloisField.Gf2MaxExponent);
        PopulateWithZeros(rhs, Gf2Math.GaloisField.Gf2MaxExponent);

        for (var index = 0; index < lhs.Factors.Count; ++index) {
            var item = lhs.Factors[index] / divisor;

            if (item.XExp < 0) break;

            lhs += new Gf2Polynomial(item) * rhs;
            resultPoly.Factors.Add(item);
        }

        PopulateWithZeros(resultPoly, Gf2Math.GaloisField.Gf2MaxExponent);

        return resultPoly;
    }

    public static Gf2Polynomial operator /(Gf2Polynomial lhs, PolynomialWord rhs) {
        return lhs / new Gf2Polynomial(rhs);
    }

    public static Gf2Polynomial operator %(Gf2Polynomial lhs, Gf2Polynomial rhs) {
        if (lhs.GetPolynomialDegree() < rhs.GetPolynomialDegree()) {
            return lhs;
        }

        var quotient = lhs / rhs;
        var remainder = quotient * rhs - lhs;

        PopulateWithZeros(remainder, Gf2Math.GaloisField.Gf2MaxExponent);

        return remainder;
    }

    public static Gf2Polynomial operator %(Gf2Polynomial lhs, PolynomialWord rhs) {
        return lhs % new Gf2Polynomial(rhs);
    }

    public static Gf2Polynomial FromBinaryString(string binString) {
        var wordLength = Gf2Math.GaloisField.GfDegree;

        var binStringLenMod = binString.Length % wordLength;

        if (binStringLenMod != 0) {
            binString = binString.PadLeft(binString.Length + wordLength - binStringLenMod, '0');
        }

        var poly = CreateEmpty;
        var wordCount = binString.Length / wordLength;

        for (var exp = 0; exp < wordCount; ++exp) {
            var binAlpha = binString.Substring(exp * wordLength, wordLength);
            var alpha = Gf2Math.GaloisField.GetByValue(Convert.ToInt32(binAlpha, 2));

            poly.Factors.Add(new(alpha, wordCount - 1 - exp));
        }

        PopulateWithZeros(poly, Gf2Math.GaloisField.Gf2MaxExponent);

        return poly;
    }

    private static Gf2Polynomial AddSameWords(in List<PolynomialWord>? poly) {
        if (poly is null) return new();

        var sortedWordsByXExp = new Dictionary<int, List<PolynomialWord>>();
        var summedPoly = CreateEmpty;

        foreach (var word in poly.Where(word => !sortedWordsByXExp.ContainsKey(word.XExp))) {
            var sameWords = from w in poly
                            where w.XExp == word.XExp
                            select w;

            sortedWordsByXExp.Add(word.XExp, sameWords.ToList());
        }

        foreach (var sortedWords in sortedWordsByXExp.Values) {
            var summedWords = new PolynomialWord(null, sortedWords.First().XExp);
            summedWords = sortedWords.Aggregate(summedWords, (current, word) => current + word);

            summedPoly.Factors.Add(summedWords);
        }

        PopulateWithZeros(summedPoly, Gf2Math.GaloisField.Gf2MaxExponent);

        return summedPoly;
    }

    private static Gf2Polynomial AddSameWords(in Gf2Polynomial poly) {
        return AddSameWords(poly.Factors);
    }

    private static void PopulateWithZeros(in Gf2Polynomial poly, int maxXPow) {
        var expList = (from word in poly.Factors select word.XExp).ToList();

        for (var exp = maxXPow; exp >= 0; --exp) {
            if (expList.Contains(exp)) continue;

            poly.Factors.Add(new(null, exp));
        }

        poly.Factors = poly.Factors.OrderByDescending(word => word.XExp).ToList();
    }

    public override string ToString() {
        var polyString = string.Empty;

        foreach (var word in Factors) {
            if (word.GfExp is null) {
                polyString += "0";
            } else {
                polyString += $"a^{word.GfExp}";
            }

            if (word.XExp != 0) {
                polyString += $"x^{word.XExp} + ";
            }
        }

        return polyString.Remove(polyString.Length);
    }
}