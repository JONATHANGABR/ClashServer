class HelloParser:
    """
    CLIENT_HELLO (10100) — versão 3.2803.3
    Número interno da versão: 534
    """

    @staticmethod
    def parse(payload: bytes) -> dict:
        proto_version  = int.from_bytes(payload[0:4], 'big')
        client_version = int.from_bytes(payload[4:8], 'big')

        fp_len      = payload[8]
        fingerprint = payload[9:9 + fp_len].decode('utf-8', errors='ignore')

        return {
            "protocol_version": proto_version,
            "client_version":   client_version,
            "fingerprint":      fingerprint
        }

    @staticmethod
    def build_server_hello() -> bytes:
        """
        SERVER_HELLO (20100)
        session_key (32 bytes) + server_version (4 bytes)
        """
        session_key    = b'\x00' * 32
        server_version = (534).to_bytes(4, 'big')
        return session_key + server_version
