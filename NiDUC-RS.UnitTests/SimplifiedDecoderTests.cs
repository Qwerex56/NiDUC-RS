using System.Text;
using NiDUC_RS.GaloisField;
using NiDUC_RS.GaloisField.Gf2Polynomial;
using NiDUC_RS.RS_Coder;

namespace NiDUC_RS.UnitTests;

public class SimplifiedDecoderTests
{
    private ReedSolomonCoder _koder = new ReedSolomonCoder(6, 4);

    private readonly int _testCount = 1000;
    
    [SetUp]
    public void Setup()
    {
    }

    //e3c = t = zdolnosc korekcyjna
    [Test]
    public void SingleError()
    {
        //generować losowe błędy w zakresie od jednego do t błędów ( RS od 1 do t błędnych symboli)
        //wygeneruj wiadomosc, najlepiej wlasna a nie losowa
        //wrzucic wiadomosc do kodera, ktory wygeneruje pakiet
        //zmienic pare wartosci w zakodowanej wiadomosci
        //wrzucic ta wiadomosc do dekodera i sprawdzic czy naprawilo czy nie
        var corrected = 0;
        var uncorrected = 0;

        for (var i = 0; i < _testCount; ++i)
        {
            TestRandomPacket(ref corrected, ref uncorrected, 1);
        }
        
        Console.WriteLine($"Corrected msgs: {corrected}");
        Console.WriteLine($"Uncorrected msgs: {uncorrected}");
    }
    
    [Test]
    public void DoubleError()
    {
        var corrected = 0;
        var uncorrected = 0;

        for (var i = 0; i < _testCount; ++i)
        {
            TestRandomPacket(ref corrected, ref uncorrected, 2);
        }
        
        Console.WriteLine($"Corrected msgs: {corrected}");
        Console.WriteLine($"Uncorrected msgs: {uncorrected}");
    }
    [Test]
    public void TripleError()
    {
        var corrected = 0;
        var uncorrected = 0;

        for (var i = 0; i < _testCount; ++i)
        {
            TestRandomPacket(ref corrected, ref uncorrected, 3);
        }
        
        Console.WriteLine($"Corrected msgs: {corrected}");
        Console.WriteLine($"Uncorrected msgs: {uncorrected}");
    }
    [Test]
    public void QuadrupleError()
    {
        var corrected = 0;
        var uncorrected = 0;

        for (var i = 0; i < _testCount; ++i)
        {
            TestRandomPacket(ref corrected, ref uncorrected, 4);
        }
        
        Console.WriteLine($"Corrected msgs: {corrected}");
        Console.WriteLine($"Uncorrected msgs: {uncorrected}");
    }
    private void TestRandomPacket(ref int correctedMsgs, ref int uncorrectedMsgs, int errors)
    {
        var message = MessageRandomizer.RandomMessage(_koder);
        var packet = _koder.SendBits(message)[0];
        var packetWithErrors = MessageRandomizer.InsertRandomError(packet, errors, _koder);

        try
        {
            var decodedBits = _koder.SimplifiedDecodeMessage(packetWithErrors);
            correctedMsgs++;
        }
        catch (Exception)
        {
            Console.WriteLine($"Cannot decode, {packetWithErrors}");
            uncorrectedMsgs++;
        }
    }
    
}