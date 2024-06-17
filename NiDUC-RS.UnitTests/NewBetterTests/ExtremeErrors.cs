using NiDUC_RS.RS_Coder;
using NUnit.Framework.Internal;

namespace NiDUC_RS.UnitTests.NewBetterTests;

/// <summary>
/// This class will test codewords for extreme number of errors
/// </summary>
public class ExtremeErrors {
    private const int MaxTests = 1_000_000;

    private readonly Logger _logger = new("ExtremeLogger", InternalTraceLevel.Info, TextWriter.Null);
    private readonly ReedSolomonCoder _coder = new(6, 4);

    [Test]
    public void TestUntilDecode() {
        var iterations = 0;

        while (iterations++ <= MaxTests) {
            var message = MessageRandomizer.RandomMessage(_coder);
            var packet = _coder.SendBits(message)[0];
            
            var messageWithError = MessageRandomizer.InsertRandomError(packet, 
                Random.Shared.Next(30, 40),
                _coder);

            try {
                var decodeMessage = _coder.DecodeMessage(messageWithError);
                Console.WriteLine($"Iteration: {iterations}\nDecoded message:\n{decodeMessage}\nShould be:\n{packet}\nDecoding result: DECODED\nDecoding result: {packet.Equals(decodeMessage)}");

                Console.WriteLine($"Packet: {packet}");
                Console.WriteLine($"Errors: {messageWithError}");
            } catch (Exception) {
                Console.WriteLine($"\n{iterations}Did no decoded\n");
                // ignored
            }
        }
    }
}