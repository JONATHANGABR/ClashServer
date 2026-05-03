import socket
import threading
from packets.parser import Packet
from packets.handler import PacketHandler
from crypto.rc4 import RC4

class Session:
    def __init__(self, conn, addr, rc4_key: bytes):
        self.conn = conn
        self.addr = addr
        self.rc4_in  = RC4(rc4_key)   # desencripta o que vem do cliente
        self.rc4_out = RC4(rc4_key)   # encripta o que vai pro cliente
        self.player = None

class ClashRoyaleServer:
    def __init__(self, config: dict, logger, db):
        self.host    = config["host"]
        self.port    = config["port"]
        self.rc4_key = config["rc4_key"].encode()
        self.logger  = logger
        self.handler = PacketHandler(logger, db)

    def start(self):
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        sock.bind((self.host, self.port))
        sock.listen(100)
        self.logger.info(f"⚔️  Clash Royale Server → {self.host}:{self.port}")

        try:
            while True:
                conn, addr = sock.accept()
                self.logger.info(f"🔌 Conectado: {addr}")
                session = Session(conn, addr, self.rc4_key)
                threading.Thread(
                    target=self._handle,
                    args=(session,),
                    daemon=True
                ).start()
        except KeyboardInterrupt:
            self.logger.info("🛑 Servidor encerrado.")
        finally:
            sock.close()

    def _handle(self, session: Session):
        try:
            with session.conn:
                while True:
                    # Lê o header (7 bytes)
                    raw_header = self._recv_exact(session.conn, 7)
                    if not raw_header:
                        break

                    # Desencripta o header
                    header = session.rc4_in.process(raw_header)

                    payload_len = int.from_bytes(header[2:5], 'big')

                    # Lê o payload
                    raw_payload = self._recv_exact(session.conn, payload_len)
                    payload = session.rc4_in.process(raw_payload) if raw_payload else b''

                    # Monta e processa o pacote
                    packet = Packet.from_bytes(header + payload)
                    responses = self.handler.handle(packet, session)

                    # Envia respostas encriptadas
                    for resp in responses:
                        raw = session.rc4_out.process(resp.to_bytes())
                        session.conn.sendall(raw)

        except ConnectionResetError:
            self.logger.warning(f"⚠️  {session.addr} desconectou.")
        except Exception as e:
            self.logger.error(f"❌ Erro [{session.addr}]: {e}")
        finally:
            self.logger.info(f"🔌 Desconectado: {session.addr}")

    def _recv_exact(self, conn, n: int) -> bytes:
        buf = b''
        while len(buf) < n:
            chunk = conn.recv(n - len(buf))
            if not chunk:
                return b''
            buf += chunk
        return buf
