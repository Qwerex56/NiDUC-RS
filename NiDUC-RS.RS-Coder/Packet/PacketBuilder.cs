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
    /// Tells how many bits are from message, useful in parsing
    /// </summary>
    private readonly int _messageBitsInPacket;
    
    /// <summary>
    /// Cache for future packet
    /// </summary>
    private string _overflowCache = string.Empty;

    public int InformationSize => _informationSize;

    public PacketBuilder(int packetSize, int packetIdSize = 0, int messageBitsInPacket = 0) {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(packetSize, nameof(packetSize));
        ArgumentOutOfRangeException.ThrowIfNegative(packetIdSize, nameof(packetIdSize));

        if (packetIdSize == 0) {
            Console.WriteLine($"{nameof(packetIdSize)} is 0, information about packet ID will be lost.");
        }

        if (messageBitsInPacket == 0) {
            Console.WriteLine($"{nameof(messageBitsInPacket)} is 0, information about valid bits in packet will be lost.");
        }

        _packetSize = packetSize;
        _packetIdSize = packetIdSize;
        _messageBitsInPacket = messageBitsInPacket;

        _informationSize = packetSize - packetIdSize - _messageBitsInPacket;
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
        var packetBitLength = Convert.ToString(packetContent.Length, 2)
                                .PadLeft(_messageBitsInPacket, '0');
        
        var packet = idBits + packetBitLength + packetContent;

        if (packet.Length >= _packetSize) {
            return packet;
        }

        var fillLength = _packetSize - packet.Length;
        var fillStart = _packetIdSize + _messageBitsInPacket;
        packet = packet.Insert(fillStart, "0".PadLeft(fillLength, '0'));

        return packet;
    }

    public string BuildPacketFromCache(int packetId) {
        return BuildPacket(string.Empty, packetId);
    }

    public (int packetId, string packetData) ParsePacket(string packet) {
        var idBits = packet[.._packetIdSize]; // initially 0..8
        var bitLength = packet[_packetIdSize..(_messageBitsInPacket + _packetIdSize)]; // 8..24

        var packetId = Convert.ToInt32(idBits, 2);
        var packetBitLength = Convert.ToInt32(bitLength, 2);

        var dataBytes = packet[^packetBitLength..];

        return (packetId, dataBytes);
    }

    public bool HasCache() => _overflowCache.Length != 0;
}