class ClientPackets:
    # ── Conexão ──────────────────────────────────
    CLIENT_HELLO      = 10100   # Handshake inicial
    LOGIN             = 10101   # Login com device token
    LOGOUT            = 10102   # Logout
    KEEP_ALIVE        = 10108   # Heartbeat (enviado a cada ~10s)

    # ── Home / Perfil ─────────────────────────────
    GO_HOME           = 14201   # Solicitar dados da home

    # ── Batalha ───────────────────────────────────
    BATTLE_EVENT      = 14102   # Evento durante batalha (colocar carta)
    SURRENDER         = 14113   # Rendição

    # ── Champions (novo na 3.x) ───────────────────
    ACTIVATE_ABILITY  = 14116   # Ativar habilidade do Champion

    # ── Social ────────────────────────────────────
    CHAT_MESSAGE      = 14715   # Chat do clã
    JOIN_CLAN         = 14316   # Entrar em clã


class ServerPackets:
    # ── Conexão ──────────────────────────────────
    SERVER_HELLO      = 20100   # Resposta ao handshake
    LOGIN_OK          = 20104   # Login aceito
    LOGIN_FAILED      = 20103   # Login recusado
    KEEP_ALIVE_OK     = 20108   # Resposta ao heartbeat

    # ── Home / Perfil ─────────────────────────────
    OWN_HOME_DATA     = 24101   # Dados completos do jogador
    ALLIANCE_DATA     = 24129   # Dados do clã

    # ── Batalha ───────────────────────────────────
    BATTLE_START      = 24100   # Início de batalha
    BATTLE_MESSAGE    = 24103   # Evento de batalha do servidor
    BATTLE_RESULT     = 22280   # Resultado final da batalha

    # ── Erros ─────────────────────────────────────
    SERVER_ERROR      = 20117   # Erro genérico
    OUT_OF_SYNC       = 22275   # Dessincronização de batalha# ──────────────────────────────────────────
# CLIENT → SERVER (mensagens que o jogo manda)
# ──────────────────────────────────────────
class ClientPackets:
    CLIENT_HELLO       = 10100  # Primeira mensagem ao conectar
    LOGIN              = 10101  # Login com token/account id
    KEEP_ALIVE         = 10108  # Heartbeat / ping
    CHAT_MESSAGE       = 14715  # Mensagem no chat global/clã
    BATTLE_EVENT       = 14102  # Evento de batalha (colocar carta, etc.)
    REQUEST_BATTLE     = 14201  # Solicitar batalha 1v1
    REQUEST_2V2        = 14202  # Solicitar batalha 2v2
    SURRENDER          = 14113  # Desistir da batalha

# ──────────────────────────────────────────
# SERVER → CLIENT (respostas que você envia)
# ──────────────────────────────────────────
class ServerPackets:
    SERVER_HELLO       = 20100  # Resposta ao CLIENT_HELLO
    LOGIN_OK           = 20104  # Login aceito
    LOGIN_FAILED       = 20103  # Login recusado
    KEEP_ALIVE_OK      = 20108  # Resposta ao heartbeat
    OWN_HOME_DATA      = 24101  # Dados do perfil/base do jogador
    ENEMY_HOME_DATA    = 24115  # Dados do oponente
    BATTLE_START       = 24100  # Inicia a batalha
    BATTLE_MESSAGE     = 24103  # Evento de batalha do servidor
    CHAT_MESSAGE_OK    = 24715  # Confirmação de chat
    SERVER_ERROR       = 20117  # Erro genérico
