namespace NiDUC_RS.GaloisField.Gf2Polynomial;

public class Gf2Polynomial {
    public List<PolynomialWord> Factors { get; private set; }

    public Gf2Polynomial GetWordAsPoly(int xExp) {
        Factors = AddSameWords(Factors).Factors;
        var word = from words in Factors
                   where words.XExp == xExp
                   select words;

        return new Gf2Polynomial(word.ToList());
    }

    public Gf2Polynomial(List<PolynomialWord>? words = null) {
        if (words is null) {
            Factors = [];

            return;
        }

        Factors = AddSameWords(words).Factors;
    }

    public int GetPolynomialDegree() {
        return Factors.Max(word => word.XExp);
    }

    public static Gf2Polynomial operator +(in Gf2Polynomial lhs, in Gf2Polynomial rhs) {
        var combinedPoly = lhs.Factors.Concat(rhs.Factors).ToList();

        return AddSameWords(combinedPoly);
    }

    public static Gf2Polynomial operator *(in Gf2Polynomial lhs, in Gf2Polynomial rhs) {
        var multipliedPoly = new Gf2Polynomial();

        foreach (var lhsWord in lhs.Factors) {
            foreach (var rhsWord in rhs.Factors) {
                multipliedPoly.Factors.Add(lhsWord * rhsWord);
            }
        }

        return AddSameWords(multipliedPoly);
    }

    public static Gf2Polynomial operator /(Gf2Polynomial lhs, Gf2Polynomial rhs) {
        if (lhs.GetPolynomialDegree() < rhs.GetPolynomialDegree()) {
            return new Gf2Polynomial();
        }

        var resultPoly = new Gf2Polynomial();
        var divisor = rhs.Factors.First();
        PopulateWithZeros(lhs);
        PopulateWithZeros(rhs);

        var maxIter = lhs.GetPolynomialDegree() - rhs.GetPolynomialDegree();
        for (var index = 0; index <= maxIter; ++index) {
            var item = lhs.Factors[index] / divisor;
            lhs += new Gf2Polynomial([item]) * rhs;
            resultPoly.Factors.Add(item);
        }

        resultPoly.Factors.RemoveAll(word => word.GfExp is null);
        rhs.Factors.RemoveAll(word => word.GfExp is null);
        lhs.Factors.RemoveAll(word => word.GfExp is null);
        return resultPoly;

        void PopulateWithZeros(in Gf2Polynomial poly) {
            var expList = (from word in poly.Factors select word.XExp).ToList();

            for (var exp = poly.GetPolynomialDegree(); exp >= 0; --exp) {
                if (expList.Contains(exp)) continue;

                poly.Factors.Add(new PolynomialWord(null, exp));
            }
        }
    }

    public static Gf2Polynomial operator %(Gf2Polynomial lhs, Gf2Polynomial rhs) {
        if (lhs.GetPolynomialDegree() < rhs.GetPolynomialDegree()) {
            return lhs;
        }

        var quotient = lhs / rhs;
        var remainder = quotient * rhs + lhs;

        // remainder.Factors.RemoveAll(word => word.GfExp is null);
        return remainder;
    }

    private static Gf2Polynomial AddSameWords(in List<PolynomialWord>? poly) {
        if (poly is null) return new Gf2Polynomial();

        var sortedWordsByXExp = new Dictionary<int, List<PolynomialWord>>();
        var summedPoly = new Gf2Polynomial();

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

        summedPoly.Factors = summedPoly.Factors.OrderByDescending(word => word.XExp).ToList();

        return summedPoly;
    }

    private static Gf2Polynomial AddSameWords(in Gf2Polynomial poly) {
        return AddSameWords(poly.Factors);
    }

    public string ToBinaryString(int wordSize) {
        var binaryString = string.Empty;

        foreach (var t in Factors) {
            var gfVal = Gf2Math.GaloisField?.GetValueByExponent(t.GfExp)?? 0;
            var word = Convert.ToString(gfVal, 2).PadLeft(wordSize, '0');
            binaryString += word;
        }

        return binaryString;
    }

    public override string ToString() {
        var polyString = string.Empty;

        foreach (var word in Factors) {
            if (word.GfExp != 0) {
                if (word.GfExp is null) {
                    polyString += "0";
                } else {
                    polyString += $"a^{word.GfExp}";
                }
            }

            if (word.XExp != 0) {
                polyString += $"x^{word.XExp} + ";
            }
        }

        return polyString.Remove(polyString.Length);
    }
}