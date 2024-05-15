using System.Text;

namespace NiDUC_RS.RS_Coder.RsFormatters;

public interface IRsFormatter {
    bool CanRead();
    string ReadBits(int count);
    
    string ReadBitsAt(int startIndex, int count);
    
    string ParseToString();
    
    void ToBinaryFile(string savePath);
    
    static string ParseToBinaryString(ReadOnlySpan<byte> data) {
        const char padChar = '0';
        const int padLen = 8;
        const int numBase = 2;
        
        var sb = new StringBuilder();
        // 1 - 0b1 => 0000 0001

        foreach (var b in data) {
            var byteString = Convert.ToString(b, numBase)
                                    .PadLeft(padLen, padChar);
            sb.Append(byteString);
        }

        return sb.ToString();
    }
}