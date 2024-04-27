namespace NiDUC_RS.GaloisField;

public record PolynomialWord(int? GfExp, int XExp) {
    public int? GfExp { get; set; } = GfExp;
    public int XExp { get; set; } = XExp;
}