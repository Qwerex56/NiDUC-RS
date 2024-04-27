namespace NiDUC_RS.GaloisField;

// TODO: Add division and modulo
public class Gf2Polynomial {
    private List<PolynomialWord> _factors;

    public Gf2Polynomial(List<PolynomialWord>? words = null) {
        if (words is null) {
            _factors = [];

            return;
        }

        _factors = AddSameWords(words)._factors;
    }

    public static Gf2Polynomial operator +(in Gf2Polynomial lhs, in Gf2Polynomial rhs) {
        var combinedPoly = lhs._factors.Concat(rhs._factors) as List<PolynomialWord>;

        return AddSameWords(combinedPoly);
    }

    public static Gf2Polynomial operator *(in Gf2Polynomial lhs, in Gf2Polynomial rhs) {
        var multipliedPoly = new Gf2Polynomial();

        foreach (var lhsWord in lhs._factors) {
            foreach (var rhsWord in rhs._factors) {
                var alpha = new Gf2Math(lhsWord.GfExp) * new Gf2Math(rhsWord.GfExp);
                var variable = lhsWord.XExp + rhsWord.XExp;

                multipliedPoly._factors.Add(new PolynomialWord(alpha.Exponent, variable));
            }
        }

        return AddSameWords(multipliedPoly);
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

            foreach (var word in sortedWords) {
                var w1 = new Gf2Math(word.GfExp);
                var w2 = new Gf2Math(summedWords.GfExp);

                summedWords.GfExp = (w1 + w2).Exponent;
            }

            summedPoly._factors.Add(summedWords);
        }

        summedPoly._factors = summedPoly._factors.OrderByDescending(word => word.XExp).ToList();

        return summedPoly;
    }

    private static Gf2Polynomial AddSameWords(in Gf2Polynomial poly) {
        return AddSameWords(poly._factors);
    }

    public override string ToString() {
        var polyString = _factors.Aggregate(string.Empty, 
                                            (current, word) => current + $"a^{word.GfExp}x^{word.XExp} + ");

        return polyString.Remove(polyString.Length - 3);
    }
}