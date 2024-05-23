using System.Text;
using NiDUC_RS.RS_Coder;

namespace NiDUC_RS.UnitTests;

public class SampleTests {
    [SetUp]
    public void Setup() { }
    //e3c = t = zdolnosc korekcyjna
    [Test]
    public void losowe_bledy(){
        //generować losowe błędy w zakresie od jednego do t błędów ( RS od 1 do t błędnych symboli)
        //wygeneruj wiadomosc, najlepiej wlasna a nie losowa
        //wrzucic wiadomosc do kodera, ktory wygeneruje pakiet
        //zmienic pare wartosci w zakodowanej wiadomosci
        //wrzucic ta wiadomosc do dekodera i sprawdzic czy naprawilo czy nie
        const string msg = "1";
        var coder = new ReedSolomonCoder(3, 1);
        var packets= coder.SendBits(msg);
        // var expectedPacket = "001110011".PadLeft(21,'0');
        // Assert.That(packets[0], Is.EqualTo(expectedPacket));
        var random = Random.Shared.Next()%21;
        var wrongPacket = new StringBuilder(packets[0]);
        wrongPacket[random] = '0';
        var decoded = coder.ReceiveString(wrongPacket.ToString());
        Assert.That(decoded, Is.EqualTo(packets[0]));
    }
}