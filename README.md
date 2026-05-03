# 🏰 Clash Royale Private Server

Servidor privado para **Clash Royale versão 3.2803.3** (Março 2022).

## ⚠️ Aviso Legal

Este projeto é apenas para fins educacionais. Clash Royale e todos os ativos relacionados são propriedade da Supercell.

## 📋 Características

- ✅ Socket TCP na porta 9339
- ✅ Criptografia RC4
- ✅ Sistema de autenticação (CLIENT_HELLO + LOGIN)
- ✅ DNS falso (redireciona game.clashroyaleapp.com)
- ✅ Carregamento de CSV do jogo (cartas, arenas, etc.)
- ⏳ Sistema de batalha (em desenvolvimento)
- ⏳ Matchmaking 1v1/2v2 (planejado)

## 🚀 Instalação

### Requisitos

- Python 3.10+
- Acesso root (para porta 53 do DNS)

### Passos

```bash
# 1. Clonar o repositório
git clone https://github.com/JONATHANGABR/ClashRoyale-Server
cd ClashServer

# 2. Instalar dependências
pip install -r requirements.txt

# 3. Configurar
cp config.json.example config.json
# Editar config.json com seu IP local

# 4. Rodar (precisa de root para porta 53)
sudo python main.py
