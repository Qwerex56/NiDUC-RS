// See https://aka.ms/new-console-template for more information

using NiDUC_RS.GaloisField;

Gf2Math.SetGf2 = new Gf2LookUpTable(6, 0b1000011);

var g1 = new Gf2Math(62);
var g2 = new Gf2Math(5);

var result = g1 * g2;

Console.WriteLine(result.Exponent);

// With 't' parameter set to 1
var p1 = new Gf2Polynomial([new PolynomialWord(0, 0), new PolynomialWord(1, 0)]);
var p2 = new Gf2Polynomial([new PolynomialWord(0, 1), new PolynomialWord(2, 0)]);

var polyResult = p1 * p2;
Console.WriteLine(polyResult);
