using System.Text;

namespace NiDUC_RS.RS_Coder.RsFormatters;

public class RsStringFormatter : IRsFormatter {
    private string _binaryData;
    private int _cursor;

    private int Cursor {
        get => _cursor;
        set => _cursor = Math.Clamp(value, 0, _binaryData.Length);
    }

    public RsStringFormatter(string message) {
        var buff = Encoding.ASCII.GetBytes(message);
        _binaryData = IRsFormatter.ParseToBinaryString(buff);
    }

    public string ReadBits(int count) {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        var afterReadPos = Cursor + count;
        string subString;

        if (afterReadPos >= _binaryData.Length) {
            subString = _binaryData[Cursor..];
            Cursor = _binaryData.Length;

            return subString;
        }

        subString = _binaryData[Cursor..afterReadPos];
        Cursor = afterReadPos;

        return subString;
    }

    public bool CanRead() => _cursor < _binaryData.Length;
    
    public string ReadBitsAt(int startIndex, int count) {
        ArgumentOutOfRangeException.ThrowIfNegative(startIndex);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfLessThan(count, startIndex);

        var oldCursorPos = Cursor;
        Cursor = startIndex;

        var substring = ReadBits(count);
        Cursor = oldCursorPos;

        return substring;
    }

    public string ParseToString() {
        const int byteWidth = 8;
        
        var buffLength = (int)MathF.Ceiling(_binaryData.Length / 8F);
        var buff = new byte[buffLength];

        for (var i = 0; i < buffLength; ++i) {
            buff[i] = Convert.ToByte(ReadBitsAt(i * byteWidth, byteWidth));
        }

        return Encoding.ASCII.GetString(buff);
    }

    public void ToBinaryFile(string savePath) {
        throw new NotImplementedException();
    }

    public static RsStringFormatter FromBinaryString(string binString) {
        var padLeft = binString.Length % 8 != 0;
        binString = padLeft ? binString.PadLeft(8, '0') : binString;
        
        return new(string.Empty) {
            _binaryData = binString
        };
    }
}