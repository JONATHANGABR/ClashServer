import secrets

from packets.packet_ids  import ClientPackets, ServerPackets
from packets.parser      import Packet
from packets.hello_parser import HelloParser
from packets.login_parser import LoginParser


class PacketHandler:
    def __init__(self, logger, db):
        self.logger = logger
        self.db     = db

    def handle(self, packet: Packet, session) -> list[Packet]:
        pid = packet.packet_id
        self.logger.debug(f"[{session.addr}] ← Packet {pid} ({len(packet.payload)}b)")

        routes = {
            ClientPackets.CLIENT_HELLO:     self.on_hello,
            ClientPackets.LOGIN:            self.on_login,
            ClientPackets.KEEP_ALIVE:       self.on_keep_alive,
            ClientPackets.BATTLE_EVENT:     self.on_battle_event,
            ClientPackets.ACTIVATE_ABILITY: self.on_champion_ability,
            ClientPackets.SURRENDER:        self.on_surrender,
        }

        handler = routes.get(pid)
        if handler:
            return handler(packet, session)

        self.logger.warning(f"[{session.addr}] ⚠️  Packet {pid} não tratado")
        return []

    # ── Handlers ────────────────────────────────────────────────────────────

    def on_hello(self, packet: Packet, session) -> list[Packet]:
        info = HelloParser.parse(packet.payload)
        self.logger.info(
            f"[{session.addr}] CLIENT_HELLO | "
            f"proto={info['protocol_version']} | "
            f"ver={info['client_version']} | "
            f"fp={info['fingerprint']}"
        )

        if info['client_version'] != 534:
            self.logger.warning(
                f"[{session.addr}] Versão incorreta: {info['client_version']} "
                f"(esperado: 534)"
            )

        payload = HelloParser.build_server_hello()
        return [Packet(ServerPackets.SERVER_HELLO, payload)]

    def on_login(self, packet: Packet, session) -> list[Packet]:
        info = LoginParser.parse(packet.payload)
        self.logger.info(
            f"[{session.addr}] LOGIN | "
            f"account_id={info['account_id']} | "
            f"ver={info['client_version']}"
        )

        player = self.db.get_or_create_player(info['account_id'])
        session.player = player

        new_token = secrets.token_hex(20)

        login_ok_payload = LoginParser.build_login_ok(
            account_id=info['account_id'],
            token=new_token
        )
        home_payload = self._build_home_data(player)

        return [
            Packet(ServerPackets.LOGIN_OK,      login_ok_payload),
            Packet(ServerPackets.OWN_HOME_DATA, home_payload),
        ]

    def on_keep_alive(self, packet: Packet, session) -> list[Packet]:
        return [Packet(ServerPackets.KEEP_ALIVE_OK, b'')]

    def on_battle_event(self, packet: Packet, session) -> list[Packet]:
        self.logger.debug(f"[{session.addr}] BATTLE_EVENT recebido")
        # TODO: repassar evento para o oponente
        return []

    def on_champion_ability(self, packet: Packet, session) -> list[Packet]:
        self.logger.info(f"[{session.addr}] CHAMPION ABILITY ativada")
        # TODO: processar habilidade (Mighty Miner, etc.)
        return []

    def on_surrender(self, packet: Packet, session) -> list[Packet]:
        self.logger.info(f"[{session.addr}] SURRENDER recebido")
        # TODO: encerrar batalha e enviar BATTLE_RESULT
        return []

    # ── Helpers ─────────────────────────────────────────────────────────────

    def _build_home_data(self, player: dict) -> bytes:
        """
        TODO: Montar payload real com protobuf / análise do APK.
        Por ora retorna placeholder.
        """
        return b'\x00' * 64
