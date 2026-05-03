import socket
import threading
import struct

class FakeDNS:
    """
    Responde queries DNS de game.clashroyaleapp.com
    apontando para o IP do seu servidor.
    """
    def __init__(self, server_ip: str, game_domain: str, logger, port: int = 53):
        self.server_ip = server_ip
        self.game_domain = game_domain
        self.logger = logger
        self.port = port

    def start(self):
        thread = threading.Thread(target=self._run, daemon=True)
        thread.start()
        self.logger.info(f"🌐 DNS Falso ouvindo na porta {self.port}")

    def _run(self):
        sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        sock.bind(("0.0.0.0", self.port))

        while True:
            try:
                data, addr = sock.recvfrom(512)
                response = self._build_response(data)
                if response:
                    sock.sendto(response, addr)
                    self.logger.debug(f"[DNS] {addr} → {self.game_domain} = {self.server_ip}")
            except Exception as e:
                self.logger.error(f"[DNS] Erro: {e}")

    def _build_response(self, query: bytes) -> bytes:
        # Header da resposta DNS
        tid = query[:2]
        flags = b'\x81\x80'  # QR=1 (resposta), OPCODE=0, AA=0, RD=1, RA=1
        qdcount = query[4:6]
        ancount = b'\x00\x01'
        nscount = b'\x00\x00'
        arcount = b'\x00\x00'

        # Copiar a question section da query
        question = query[12:]

        # Resource Record (resposta A)
        rr = (
            b'\xc0\x0c' +              # pointer pra question
            b'\x00\x01' +              # tipo A
            b'\x00\x01' +              # classe IN
            b'\x00\x00\x00\x3c' +      # TTL: 60s
            b'\x00\x04' +              # rdlength: 4 bytes (IPv4)
            socket.inet_aton(self.server_ip)
        )

        return tid + flags + qdcount + ancount + nscount + arcount + question + rr
