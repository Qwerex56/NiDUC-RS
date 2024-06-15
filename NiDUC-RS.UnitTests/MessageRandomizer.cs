using System.Text;
using NiDUC_RS.GaloisField;
using NiDUC_RS.GaloisField.Gf2Polynomial;
using NiDUC_RS.RS_Coder;

namespace NiDUC_RS.UnitTests;

public static class MessageRandomizer
{
    public static string RandomMessage(ReedSolomonCoder koder)
    {
        var message = new StringBuilder();
        for (var i = 0; i < koder.InformationLength * koder.WordSize; ++i)
        {
            var bit = Random.Shared.Next(2);
            message.Append(bit);
        }

        return message.ToString();
    }

    public static string InsertRandomError(string message, int errors, ReedSolomonCoder koder)
    {
        var poly = Gf2Polynomial.FromBinaryString(message);
        var errorPositions = new List<int>();
        
        for (var i = 0; i < errors;)
        {
            var randomPosition = Random.Shared.Next(koder.InformationLength);
            if (errorPositions.Contains(randomPosition))
            {
                continue;
            }
            
            errorPositions.Add(randomPosition);
            var gfExp = Random.Shared.Next(Gf2Math.GaloisField.Gf2MaxExponent);
            poly.Factors[randomPosition].GfExp = gfExp;
            ++i;
        }

        return poly.ToBinaryString();
    }

    public static string InsertError(StringBuilder bitMessage, List<Error>errors, ReedSolomonCoder koder)
    {
        foreach (var error in errors)
        {
            bitMessage.Remove(error.Position * koder.WordSize, koder.WordSize);
            bitMessage.Insert(error.Position * koder.WordSize, error.BitError.PadLeft(koder.WordSize, '0'));
        }

        return bitMessage.ToString();
    }
}

public record Error (int Position, string BitError);