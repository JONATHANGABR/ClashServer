from packets.packet_ids import ClientPackets, ServerPackets
from packets.parser import Packet
import struct

class PacketHandler:
    def __init__(self, logger, db):
        self.logger = logger
        self.db = db

    def handle(self, packet: Packet, session) -> list[Packet]:
        pid = packet.packet_id
        self.logger.debug(f"[{session.addr}] ← Packet {pid} ({len(packet.payload)}b)")

        routes = {
            ClientPackets.CLIENT_HELLO: self.on_hello,
            ClientPackets.LOGIN:        self.on_login,
            ClientPackets.KEEP_ALIVE:   self.on_keep_alive,
            ClientPackets.BATTLE_EVENT: self.on_battle_event,
        }

        handler = routes.get(pid)
        if handler:
            return handler(packet, session)

        self.logger.warning(f"[{session.addr}] Packet {pid} não tratado!")
        return []

    def on_hello(self, packet: Packet, session) -> list[Packet]:
        self.logger.info(f"[{session.addr}] → CLIENT_HELLO")
        response = Packet(ServerPackets.SERVER_HELLO, b'\x00' * 8)
        return [response]

    def on_login(self, packet: Packet, session) -> list[Packet]:
        self.logger.info(f"[{session.addr}] → LOGIN")

        # Responder com LOGIN_OK + dados do home
        login_ok   = Packet(ServerPackets.LOGIN_OK, b'\x00' * 4)
        home_data  = Packet(ServerPackets.OWN_HOME_DATA, self._build_home_data(session))
        return [login_ok, home_data]

    def on_keep_alive(self, packet: Packet, session) -> list[Packet]:
        return [Packet(ServerPackets.KEEP_ALIVE_OK, b'')]

    def on_battle_event(self, packet: Packet, session) -> list[Packet]:
        self.logger.info(f"[{session.addr}] → BATTLE_EVENT")
        # Reenviar evento para o oponente (em batalhas reais)
        return []

    def _build_home_data(self, session) -> bytes:
        # TODO: serializar com protobuf ou formato binário do CR
        return b'\x00' * 32
