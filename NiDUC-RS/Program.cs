using NiDUC_RS.GaloisField;
using NiDUC_RS.GaloisField.Gf2Polynomial;
using NiDUC_RS.RS_Coder;

var reedSalomonCoder = new ReedSolomonCoder(6, 4);

//
// var filePackets = reedSalomonCoder
//     .SendFile(@"C:\Users\qwere\Pictures\Screenshots\Zrzut ekranu 2024-06-01 233243.png");
//     
// var receivedPackets = reedSalomonCoder.ReceiveFile(filePackets.ToArray());
