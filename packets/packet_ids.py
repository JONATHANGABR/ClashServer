# ──────────────────────────────────────────
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
