using System.Text;
using NiDUC_RS.GaloisField;
using NiDUC_RS.GaloisField.Gf2Polynomial;
using NiDUC_RS.RS_Coder;

namespace NiDUC_RS.UnitTests;

public class SampleTests
{
    private ReedSolomonCoder koder = new ReedSolomonCoder(6, 4);

    [SetUp]
    public void Setup()
    {
    }

    //e3c = t = zdolnosc korekcyjna
    [Test]
    public void losowe_bledy()
    {
        //generować losowe błędy w zakresie od jednego do t błędów ( RS od 1 do t błędnych symboli)
        //wygeneruj wiadomosc, najlepiej wlasna a nie losowa
        //wrzucic wiadomosc do kodera, ktory wygeneruje pakiet
        //zmienic pare wartosci w zakodowanej wiadomosci
        //wrzucic ta wiadomosc do dekodera i sprawdzic czy naprawilo czy nie
        var corrected = 0;
        var uncorrected = 0;

        for (var i = 0; i < 100; ++i)
        {
            TestRandomPacket(ref corrected, ref uncorrected, 3);
        }
        
        Console.WriteLine($"Corrected msgs: {corrected}");
        Console.WriteLine($"Uncorrected msgs: {uncorrected}");
    }

    private void TestRandomPacket(ref int correctedMsgs, ref int uncorrectedMsgs, int errors)
    {
        var message = RandomMessage();
        var packet = koder.SendBits(message)[0];
        var packetWithErrors = WprowadzanieBledu(packet, errors);

        try
        {
            var decodedBits = koder.SimplifiedDecodeMessage(packetWithErrors);
            correctedMsgs++;
        }
        catch (Exception)
        {
            Console.WriteLine($"Cannot decode, {packetWithErrors}");
            uncorrectedMsgs++;
        }
    }

    private string RandomMessage()
    {
        var message = new StringBuilder();
        for (var i = 0; i < koder.InformationLength * koder.WordSize; ++i)
        {
            var bit = Random.Shared.Next(2);
            message.Append(bit);
        }

        return message.ToString();
    }

    private string WprowadzanieBledu(string message, int errors = 1)
    {
        var poly = Gf2Polynomial.FromBinaryString(message);
        for (var i = 0; i < errors; ++i)
        {
            // TODO: Check if position was generated before
            var randomPosition = Random.Shared.Next(koder.InformationLength);
            var gfExp = Random.Shared.Next(Gf2Math.GaloisField.Gf2MaxExponent);
            poly.Factors[randomPosition] = poly.Factors[randomPosition] with
            {
                GfExp = gfExp
            };
        }

        return poly.ToBinaryString();
    }
}