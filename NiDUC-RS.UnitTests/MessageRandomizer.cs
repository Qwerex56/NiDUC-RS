using System.Text;
using NiDUC_RS.GaloisField;
using NiDUC_RS.GaloisField.Gf2Polynomial;
using NiDUC_RS.RS_Coder;

namespace NiDUC_RS.UnitTests;

public static class MessageRandomizer {
    public static string RandomMessage(ReedSolomonCoder coder) {
        var message = new StringBuilder();

        for (var i = 0; i < coder.InformationLength * coder.WordSize; ++i) {
            var bit = Random.Shared.Next(2);
            message.Append(bit);
        }

        return message.ToString();
    }

    public static string InsertRandomError(string message, int errors, ReedSolomonCoder coder) {
        var poly = Gf2Polynomial.FromBinaryString(message);
        var errorPositions = new List<int>();

        for (var i = 0; i < errors;) {
            var randomPosition = Random.Shared.Next(coder.InformationLength);

            if (errorPositions.Contains(randomPosition)) {
                continue;
            }

            errorPositions.Add(randomPosition);
            var gfExp = Random.Shared.Next(Gf2Math.GaloisField.Gf2MaxExponent);
            poly.Factors[randomPosition].GfExp = gfExp;
            ++i;
        }

        return poly.ToBinaryString();
    }

    public static string InsertError(StringBuilder bitMessage, List<Error> errors, ReedSolomonCoder coder) {
        foreach (var error in errors) {
            bitMessage.Remove(error.Position * coder.WordSize, coder.WordSize);
            bitMessage.Insert(error.Position * coder.WordSize, error.BitError.PadLeft(coder.WordSize, '0'));
        }

        return bitMessage.ToString();
    }
}

public record Error(int Position, string BitError);