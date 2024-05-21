using NiDUC_RS.GaloisField.Gf2Polynomial;
using NiDUC_RS.GaloisField.Gf2Tables;
using NiDUC_RS.RS_Coder;


// NiDUC_RS.GaloisField.Gf2Math.SetGf2 = new Gf2LookUpTable(3, 0b1011);
// var poly1 = new Gf2Polynomial(new PolynomialWord(0, 1));
// var poly2 = new Gf2Polynomial(new PolynomialWord(0, 3));
//
// Console.WriteLine(poly1 / poly2);
// Console.WriteLine(poly2 / poly1);

var reedSalomonCoder = new ReedSolomonCoder(6, 4);

const string msg = "h";
// const string encodedMsg = "000000001101100010"; // Error at idx = 4  0 -> 1, original string:  001101000010110
// const string mochnacki = "001010001010001110111"; // z Mochnackiego p. 2.8.2. 

var packets = reedSalomonCoder.SendBits("1");

Console.WriteLine(packets[0]);

foreach (var wPacket in packets.Select(packet => "1" + packet[1..^1] + "1")) {
    var str = reedSalomonCoder.ReceiveString(wPacket);
    Console.WriteLine(str);
}

Gf2Polynomial.FromBinaryString("1").ToBinaryString(); // 001000111 -> 0 null 5
// Console.WriteLine(reedSalomonCoder.ReceiveString(encodedMsg));

// 001111001010110 random value
// 001101000010110
// 001101000010110
// 01110011