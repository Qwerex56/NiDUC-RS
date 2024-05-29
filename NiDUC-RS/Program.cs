using NiDUC_RS.GaloisField;
using NiDUC_RS.GaloisField.Gf2Polynomial;
using NiDUC_RS.RS_Coder;

var reedSalomonCoder = new ReedSolomonCoder(3, 2);
var packet = reedSalomonCoder.SendBits("1")[0];
const string erroredPacket = "011000001011001010011";

var syndromes = reedSalomonCoder.CalculateSyndromeVector(Gf2Polynomial.FromBinaryString(erroredPacket));
var locator = reedSalomonCoder.FindErrorLocator(syndromes);
var positions = reedSalomonCoder.BruteForceErrorsVals(ref locator, locator.GetPolynomialDegree());



foreach (var syn in reedSalomonCoder.CalculateSyndromeVector(Gf2Polynomial.FromBinaryString(erroredPacket))) {
    Console.Write(syn + "; ");
}

Console.WriteLine($"Original:   {packet}");
Console.WriteLine($"With error: {erroredPacket}");
Console.WriteLine($"Corrected:  {reedSalomonCoder.ReceiveString(erroredPacket)}");