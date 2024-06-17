using System.Text;
using NiDUC_RS.GaloisField;
using NiDUC_RS.GaloisField.Gf2Polynomial;
using NiDUC_RS.RS_Coder;

namespace NiDUC_RS.UnitTests;

public class FullDecoderTest {
    private const int TestCount = 1_000_000;

    private ReedSolomonCoder _coder;

    [SetUp]
    public void Setup() {
        _coder = new(6, 4);
    }

    //e3c = t = zdolnosc korekcyjna
    [Test]
    public void SingleError() {
        var corrected = 0;
        var uncorrected = 0;

        for (var i = 0; i < TestCount; ++i) {
            TestRandomPacket(ref corrected, ref uncorrected, 1);
        }

        Console.WriteLine($"Corrected msg's: {corrected}");
        Console.WriteLine($"Uncorrected msg's: {uncorrected}");
    }

    [Test]
    public void DoubleError() {
        var corrected = 0;
        var uncorrected = 0;

        for (var i = 0; i < TestCount; ++i) {
            TestRandomPacket(ref corrected, ref uncorrected, 2);
        }

        Console.WriteLine($"Corrected msg's: {corrected}");
        Console.WriteLine($"Uncorrected msg's: {uncorrected}");
    }

    [Test]
    public void TripleError() {
        var corrected = 0;
        var uncorrected = 0;

        for (var i = 0; i < TestCount; ++i) {
            TestRandomPacket(ref corrected, ref uncorrected, 3);
        }

        Console.WriteLine($"Corrected msg's: {corrected}");
        Console.WriteLine($"Uncorrected msg's: {uncorrected}");
    }

    [Test]
    public void QuadrupleError() {
        var corrected = 0;
        var uncorrected = 0;

        for (var i = 0; i < TestCount; ++i) {
            TestRandomPacket(ref corrected, ref uncorrected, 4);
        }

        Console.WriteLine($"Corrected msg's: {corrected}");
        Console.WriteLine($"Uncorrected msg's: {uncorrected}");
    }

    [Test]
    public void DefinedSingleError() {
        var corrected = 0;
        var uncorrected = 0;
        var error = new Error(0, "111111");

        //63 bo tyle mamy pozycji
        for (var i = 0; i < 63; ++i) {
            TestPacket(ref corrected, ref uncorrected, "1", error);
        }

        Console.WriteLine($"Corrected msg's: {corrected}");
        Console.WriteLine($"Uncorrected msg's: {uncorrected}");
    }

    [Test]
    public void DefinedDoubleError() {
        var corrected = 0;
        var uncorrected = 0;

        List<List<Error>> errorList = [
            [
                new Error(0, "111"), new Error(3, "111")
            ]
        ];

        foreach (var error in errorList) {
            TestPacket(ref corrected, ref uncorrected, "1", error);
        }

        Console.WriteLine($"Corrected msg's: {corrected}");
        Console.WriteLine($"Uncorrected msg's: {uncorrected}");
    }

    private void TestRandomPacket(ref int correctedMsgs, ref int uncorrectedMsgs, int errors) {
        var message = MessageRandomizer.RandomMessage(_coder);
        var packet = _coder.SendBits(message)[0];
        var packetWithErrors = MessageRandomizer.InsertRandomError(packet, errors, _coder);

        try {
            var recPacket = _coder.DecodeMessage(packetWithErrors);
            var result = string.Compare(recPacket, packet, StringComparison.Ordinal);

            if (result != 0) {
                recPacket = _coder.DecodeMessage(recPacket);
                Console.WriteLine(recPacket);
                throw new("Decode miss, message has been decoded but wrongly.");
            }
            
            correctedMsgs++;
        } catch (Exception e) {
            Console.WriteLine(e);
            Console.WriteLine($"Cannot decode!\n" +
                              $"Was:       {packetWithErrors}\n" +
                              $"Should Be: {packet}");
            uncorrectedMsgs++;
        }
    }

    private void TestPacket(ref int correctedMsgs, ref int uncorrectedMsgs, string message, List<Error> errors) {
        var packet = _coder.SendBits(message)[0];
        var packetWithErrors = MessageRandomizer.InsertError(new StringBuilder(packet), errors, _coder);

        try {
            _coder.DecodeMessage(packetWithErrors);

            correctedMsgs++;
        } catch (Exception e) {
            Console.WriteLine(e);
            Console.WriteLine($"Cannot decode!\n" +
                              $"Was:       {packetWithErrors}\n" +
                              $"Should Be: {packet}\n\n.");
            uncorrectedMsgs++;
        }
    }

    private void TestPacket(ref int correctedMsgs, ref int uncorrectedMsgs, string message, Error errors) {
        TestPacket(ref correctedMsgs, ref uncorrectedMsgs, message, [errors]);
    }
}