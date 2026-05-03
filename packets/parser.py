# Formato do header Clash Royale (7 bytes):
# [2 bytes] Packet ID  (big-endian)
# [3 bytes] Payload length (big-endian)
# [2 bytes] Version / unknown

class Packet:
    HEADER_SIZE = 7

    def __init__(self, packet_id: int, payload: bytes = b''):
        self.packet_id = packet_id
        self.payload = payload

    @staticmethod
    def from_bytes(data: bytes) -> 'Packet':
        if len(data) < Packet.HEADER_SIZE:
            raise ValueError("Dados insuficientes para montar o pacote.")

        packet_id     = int.from_bytes(data[0:2], 'big')
        payload_len   = int.from_bytes(data[2:5], 'big')
        # data[5:7] = version (ignorado por ora)
        payload       = data[7: 7 + payload_len]

        return Packet(packet_id, payload)

    def to_bytes(self) -> bytes:
        header = (
            self.packet_id.to_bytes(2, 'big') +
            len(self.payload).to_bytes(3, 'big') +
            (0).to_bytes(2, 'big')  # version
        )
        return header + self.payload

    def __repr__(self):
        return f"<Packet id={self.packet_id} size={len(self.payload)}>"
