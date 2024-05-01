using NiDUC_RS.GaloisField.Gf2Polynomial;
using NiDUC_RS.RS_Coder;

var reedSalomonCoder = new ReedSalomonCoder(3, 1);
var msg = new Gf2Polynomial([
                                new (0, 4),
                                new (2, 3),
                                new (6, 2),
                                new (2, 1),
                                new (6, 0)
                            ]);

var codedMsg = reedSalomonCoder.EncodeMessage(msg);
Console.WriteLine(codedMsg);