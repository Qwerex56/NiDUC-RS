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
    
    public bool CanRead() => _cursor < _binaryData.Length;
    
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
        var padLeft = _binaryData.Length % 8 != 0;
        _binaryData = padLeft ? _binaryData.PadLeft(_binaryData.Length + _binaryData.Length % 8, '0') : 
                        _binaryData;

        var binStringLength = _binaryData.Length / 8;
        var message = string.Empty;
        var bytes = new byte[binStringLength];
        
        for (var i = 0; i < binStringLength; ++i) {
            var bits = Convert.ToByte(_binaryData[(i * 8)..(i * 8 + 8)], 2);
            bytes[i] = bits;
        }

        message += Encoding.UTF8.GetString(bytes);

        return message;
    }

    public void ToBinaryFile(string savePath) {
        throw new NotImplementedException();
    }

    public static RsStringFormatter FromBinaryString(string binString) {
        var padLeft = binString.Length % 8 != 0;
        binString = padLeft ? binString.PadLeft(8, '0') : binString;

        if (padLeft) {
            var padLength = binString.Length + binString.Length % 8;
            binString = binString.PadLeft(padLength, '0');
        }

        var formatter = new RsStringFormatter(string.Empty) {
            _binaryData = binString
        };

        return formatter;
    }
}