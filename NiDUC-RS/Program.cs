using NiDUC_RS.RS_Coder;

var reedSalomonCoder = new ReedSolomonCoder(3, 2);
var packets = reedSalomonCoder.SendBits("1")[0];
const string erroredPacket = "011010001011001010011";

Console.WriteLine($"Original:   {packets}");
Console.WriteLine($"With error: {erroredPacket}");
Console.WriteLine($"Corrected:  {reedSalomonCoder.ReceiveString(erroredPacket)}");