using NiDUC_RS.RS_Coder;

var reedSalomonCoder = new ReedSolomonCoder(3, 2);
var packet = reedSalomonCoder.SendBits("1")[0];
const string erroredPacket = "011100001011001010011";

Console.WriteLine($"Original:   {packet}");
Console.WriteLine($"With error: {erroredPacket}");
Console.WriteLine($"Corrected:  {reedSalomonCoder.DecodeMessage(erroredPacket)}");