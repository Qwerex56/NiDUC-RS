using NiDUC_RS.RS_Coder;

var reedSalomonCoder = new ReedSolomonCoder(3, 1);

const string msg = "h";
const string encodedMsg = "000000001101100010"; // Error at idx = 4  0 -> 1, original string:  001101000010110
const string mochnacki = "001010001010001110111"; // z Mochnackiego p. 2.8.2. 

reedSalomonCoder.SendString(msg);
Console.WriteLine(reedSalomonCoder.ReceiveString(encodedMsg));

// 001111001010110 random value
// 001101000010110