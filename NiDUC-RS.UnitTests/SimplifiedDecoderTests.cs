using System.Text;
using NiDUC_RS.GaloisField;
using NiDUC_RS.GaloisField.Gf2Polynomial;
using NiDUC_RS.RS_Coder;

namespace NiDUC_RS.UnitTests;

public class SimplifiedDecoderTests
{
    private const int TestCount = 1000;
    
    private readonly ReedSolomonCoder _coder = new(6, 4);

    [SetUp]
    public void Setup()
    {
    }

    //e3c = t = zdolnosc korekcyjna
    [Test]
    public void SingleError()
    {
        var corrected = 0;
        var uncorrected = 0;

        for (var i = 0; i < TestCount; ++i)
        {
            TestRandomPacket(ref corrected, ref uncorrected, 1);
        }
        
        Console.WriteLine($"Corrected msg's: {corrected}");
        Console.WriteLine($"Uncorrected msg's: {uncorrected}");
    }
    
    [Test]
    public void DoubleError()
    {
        var corrected = 0;
        var uncorrected = 0;

        for (var i = 0; i < TestCount; ++i)
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

        for (var i = 0; i < TestCount; ++i)
        {
            TestRandomPacket(ref corrected, ref uncorrected, 3);
        }
        
        Console.WriteLine($"Corrected msg's: {corrected}");
        Console.WriteLine($"Uncorrected msg's: {uncorrected}");
    }
    [Test]
    public void QuadrupleError()
    {
        var corrected = 0;
        var uncorrected = 0;

        for (var i = 0; i < TestCount; ++i)
        {
            TestRandomPacket(ref corrected, ref uncorrected, 4);
        }
        
        Console.WriteLine($"Corrected msg's: {corrected}");
        Console.WriteLine($"Uncorrected msg's: {uncorrected}");
    }
    private void TestRandomPacket(ref int correctedMsgs, ref int uncorrectedMsgs, int errors)
    {
        var message = MessageRandomizer.RandomMessage(_coder);
        var packet = _coder.SendBits(message)[0];
        var packetWithErrors = MessageRandomizer.InsertRandomError(packet, errors, _coder);
        
        try
        {
            _coder.SimplifiedDecodeMessage(packetWithErrors);
            correctedMsgs++;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine($"Cannot decode!\n" +
                              $"Was:       {packetWithErrors}\n" +
                              $"Should Be: {packet}");
            uncorrectedMsgs++;
        }
    }
    
}