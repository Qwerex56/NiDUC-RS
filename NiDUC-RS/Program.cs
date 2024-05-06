using System.Collections;
using NiDUC_RS.GaloisField.Gf2Polynomial;
using NiDUC_RS.RS_Coder;
using NiDUC_RS.RS_Coder.RsFormatters;

var reedSalomonCoder = new ReedSolomonCoder(6, 1);
var msg = "01001000";

// var codedMsg = reedSalomonCoder.EncodeMessage(msg);
// Console.WriteLine(codedMsg);

// var ff = new RsFileFormatter(@"D:\\dev\\NiDUC-Examples\\HelloWorld.txt");
// ff.ToBinFile();