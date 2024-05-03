using System.Text;

namespace NiDUC_RS.RS_Coder.FileFormatter;

public class RsFileFormatter {
    private readonly string _filePath;
    private readonly string _binaryFileFormat;
    private int _cursor;

    public long Length => _binaryFileFormat.Length;

    public RsFileFormatter(string filePath) {
        _filePath = filePath;
        var fs = new FileStream(filePath, FileMode.Open);
        var sb = new StringBuilder();
        
        for (var i = 0; i < fs.Length; ++i) {
            var bitData = Convert.ToString((byte)fs.ReadByte(), 2).PadLeft(8, '0');
            sb.Append(bitData);
        }

        _binaryFileFormat = sb.ToString();
    }

    /// <summary>
    /// Reads n bits and moves cursor to last red bit.
    /// If _cursor + count indicates position not within the range in this instance
    /// the range to end of string is returned.
    /// </summary>
    /// <param name="count">number of bits to read</param>
    /// <returns>n bits in string representation</returns>
    /// <exception cref="ArgumentOutOfRangeException">is thrown when count is less negative</exception>
    public string ReadBytes(int count) {
        string bitStream;
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        try {
            bitStream = _binaryFileFormat.Substring(_cursor, count);
            _cursor += count;
        }
        catch (ArgumentOutOfRangeException) {
            bitStream = _binaryFileFormat[_cursor..];
            _cursor = _binaryFileFormat.Length;
        }
        
        return bitStream;
    }

    /// <summary>
    /// Returns sub range of file as bit string but does not move _cursor
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public string ReadBytesAt(int startIndex, int count) {
        return _binaryFileFormat.Substring(startIndex, count);
    }

    public void ToBinFile() {
        var path = _filePath + ".rscode";
        File.WriteAllText(path, _binaryFileFormat);
    }
}