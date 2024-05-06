namespace NiDUC_RS.RS_Coder.Packet;

public class PacketBuilder {
    /// <summary>
    /// Overall packet size
    /// </summary>
    private readonly int _packetSize;
    
    /// <summary>
    /// Space occupied by packet Idx bits
    /// </summary>
    private readonly int _packetIdSize;
    
    /// <summary>
    /// Space occupied by packet information
    /// </summary>
    private readonly int _informationSize;
    
    /// <summary>
    /// Cache for future packet
    /// </summary>
    private string _overflowCache = string.Empty;

    public PacketBuilder(int packetSize, int packetIdSize = 0) {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(packetSize, nameof(packetSize));
        ArgumentOutOfRangeException.ThrowIfNegative(packetIdSize, nameof(packetIdSize));

        if (packetIdSize == 0) {
            Console.WriteLine($"{nameof(packetIdSize)} is equal to 0, information about packet count will be lost.");
        }

        _packetSize = packetSize;
        _packetIdSize = packetIdSize;

        _informationSize = packetSize - packetIdSize;
    }

    public string BuildPacket(string packetContent, int packetId = 0) {
        if (!_overflowCache.Equals(string.Empty)) {
            packetContent = packetContent.Insert(0, _overflowCache);
            _overflowCache = string.Empty;
        }
        
        if (packetContent.Length > _informationSize) {
            Console.WriteLine($"{nameof(packetContent)} is too big to fit in packet. " +
                              $"Overflowed content will be cached and send with next packet");
            _overflowCache = packetContent[_informationSize..];
            packetContent = packetContent[.._informationSize];
        }

        if (_informationSize == _packetSize) {
            return packetContent;
        }

        var idBits = Convert.ToString(packetId, 2)
                            .PadLeft(_packetIdSize, '0');
        var packet = idBits + packetContent;

        return packet;
    }

    public (int packetId, byte[] packetData) ParsePacket(string packet) {
        var idBits = packet[.._packetIdSize];
        var data = packet[_packetIdSize..];

        var packetId = Convert.ToInt32(idBits, 2);
        var dataSize = (int)MathF.Ceiling(data.Length / 8F);

        var dataBytes = new byte[dataSize];
        
        for (var i = 0; i < dataSize; ++i) {
            byte b;
            try {
                b = Convert.ToByte(data.Substring(i * 8, 8), 2);

            } catch (ArgumentOutOfRangeException) {
                b = Convert.ToByte(data[(i * 8)..], 2);
            }

            dataBytes[i] = b;
        }

        return (packetId, dataBytes);
    }
}