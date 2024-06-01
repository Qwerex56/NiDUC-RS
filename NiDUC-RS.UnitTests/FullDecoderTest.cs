using System.Text;
using NiDUC_RS.GaloisField;
using NiDUC_RS.GaloisField.Gf2Polynomial;
using NiDUC_RS.RS_Coder;

namespace NiDUC_RS.UnitTests;

public class FullDecoderTest
{
    private ReedSolomonCoder _koder;
    
    private readonly int _testCount = 1000;
    
    [SetUp]
    public void Setup()
    {
        _koder = new ReedSolomonCoder(3, 1);
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

    [Test]
    public void DefinedSingleError()
    {
        var corrected = 0;
        var uncorrected = 0;
        var error = new Error(0, "111111");
        
        
        for (var i = 0; i < 63; ++i) //63 bo tyle mamy pozycji
        {
            Error foo = new Error(0, "0"); // 0 "000000"
            Error bar = foo with { Position = i};
        }
        
        Console.WriteLine($"Corrected msgs: {corrected}");
        Console.WriteLine($"Uncorrected msgs: {uncorrected}");
    }
    
    [Test]
    public void DefinedDoubleError()
    {
        var corrected = 0;
        var uncorrected = 0;

        List<List<Error>> errorList = [
            [
                new Error(0, "111"), new Error(3, "111")
            ]];

        foreach (var error in errorList)
        {
            TestPacket(ref corrected, ref uncorrected, "1", error);
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
            var decodedBits = _koder.DecodeMessage(packetWithErrors);
            correctedMsgs++;
        }
        catch (Exception)
        {
            Console.WriteLine($"Cannot decode, {packetWithErrors}");
            uncorrectedMsgs++;
        }
    }

    private void TestPacket(ref int correctedMsgs, ref int uncorrectedMsgs, string message, List<Error>errors)
    {
        var packet = _koder.SendBits(message)[0];
        var packetWithErrors = MessageRandomizer.InsertError(new StringBuilder(packet), errors, _koder);
        
        try
        {
            var decodedBits = _koder.DecodeMessage(packetWithErrors);
            Console.WriteLine($"Good:      {packet}\n" +
                              $"Wrong:     {packetWithErrors}\n" +
                              $"Corrected: {decodedBits}");
            correctedMsgs++;
        }
        catch (Exception)
        {
            Console.WriteLine($"Cannot decode, {packetWithErrors}");
            uncorrectedMsgs++;
        }
    }
    
}