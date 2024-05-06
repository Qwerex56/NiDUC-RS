namespace NiDUC_RS.RS_Coder.Packet;

public class PacketBuilder {
    private int _packetSize;
    private int _maxPacketId;
    private int _informationSize;
    
    public PacketBuilder(int packetSize, int maxPacketId = 0) {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(packetSize, nameof(packetSize));
        ArgumentOutOfRangeException.ThrowIfNegative(maxPacketId, nameof(maxPacketId));

        if (maxPacketId == 0) {
            Console.WriteLine($"{nameof(maxPacketId)} is equal to 0, information about packet count will be lost.");
        }
        
        _packetSize = packetSize;
        _maxPacketId = maxPacketId;

        _informationSize = packetSize - maxPacketId;
    }
    
    public string BuildPacket(string packetContent) {
        throw new NotImplementedException();
    }

    public string ParsePacket(string packet) {
        throw new NotImplementedException();
    }
}