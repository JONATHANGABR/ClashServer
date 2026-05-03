# 🏰 CR-Server — Servidor de Clash Royale 3.2803.3

<div align="center">


[![License](https://img.shields.io/badge/license-GPL--3.0-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-orange)](https://mysql.com)
[![Stars](https://img.shields.io/github/stars/JONATHANGABR/CR-Server?style=social)](https://github.com/JONATHANGABR/ClashServer/stargazers)
[![Issues](https://img.shields.io/github/issues/JONATHANGABR/CR-Server)](https://github.com/JONATHANGABR/ClashServer/issues)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](https://github.com/JONATHANGABR/ClashServer/pulls)

**Servidor privado open-source de Clash Royale para a versão 3.2803.3**  
Escrito em C# com .NET 8.0 e banco de dados MySQL

[📦 Instalação](#-instalação) •
[⚙️ Configuração](#️-configuração) •
[🚀 Rodando](#-rodando-o-servidor) •
[📊 Status](#-status-do-projeto) •
[🤝 Contribuindo](#-contribuindo)

</div>

---

## 📌 Sobre o Projeto

O **CR-Server** é um servidor privado open-source para o jogo **Clash Royale**, desenvolvido do zero em **C# (.NET 8.0)** com suporte a banco de dados **MySQL**.

O projeto foi criado com o objetivo de ser uma base limpa, organizada e expansível para estudo de:

- 📡 Protocolos de rede TCP/UDP
- 🔬 Engenharia reversa de aplicações mobile
- 💾 Gerenciamento de banco de dados MySQL
- 🏗️ Arquitetura de servidores de jogos

> ⚠️ **Aviso Legal:** Este projeto é exclusivamente para fins **educacionais**.  
> Criar servidores privados viola os [Termos de Serviço da Supercell](https://supercell.com/en/terms-of-service/).  
> Não somos afiliados, endossados ou patrocinados pela Supercell.

---

## ✨ Features

### ✅ Implementado
- [x] Sistema de rede TCP (aceitar múltiplos clientes)
- [x] Decodificação do protocolo Piranha (header + payload)
- [x] Sistema de login e autenticação
- [x] Criação automática de conta nova
- [x] KeepAlive (conexão estável com o cliente)
- [x] Banco de dados MySQL integrado
- [x] Sistema de logs (console + arquivo)
- [x] Suporte a Docker
- [x] Configuração via `config.json`

### 🚧 Em Desenvolvimento
- [ ] Batalhas PvP
- [x] Batalhas PvE (contra bots)
- [ ] Sistema de clãs completo
- [ ] Sistema de cartas e baralhos
- [ ] Loja e baús
- [ ] Chat global e de clã
- [ ] Torneios e desafios
- [ ] Painel de administração web

---

## 🛠️ Tecnologias

| Tecnologia | Versão | Uso |
|---|---|---|
| **C#** | .NET 8.0 | Linguagem principal |
| **MySQL** | 8.0+ | Banco de dados |
| **Serilog** | 3.x | Sistema de logs |
| **MySqlConnector** | 2.x | Conexão MySQL |
| **Newtonsoft.Json** | 13.x | Leitura de configs |
| **Docker** | Latest | Containerização |

---

## 📋 Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL 8.0+](https://dev.mysql.com/downloads/mysql/)
- [Git](https://git-scm.com/downloads)
- [Docker](https://www.docker.com/) *(opcional)*

---

## 📦 Instalação

### Clonando o Repositório

```bash
# Clonar repositório
git clone https://github.com/SeuUser/CR-Server.git

# Entrar na pasta
cd CR-Server

⚙️ Configuração
1. Configurar o banco de dados

# Entrar no MySQL
mysql -u root -p

# Criar usuário e banco
CREATE DATABASE clashroyale;
CREATE USER 'crserver'@'localhost' IDENTIFIED BY 'SuaSenha123';
GRANT ALL PRIVILEGES ON clashroyale.* TO 'crserver'@'localhost';
FLUSH PRIVILEGES;
EXIT;

# Importar schema
mysql -u crserver -p clashroyale < database/schema.sql

2. Configurar o servidor

Edite o arquivo config.json na raiz do projeto:

JSON

{
  "Server": {
    "Host": "0.0.0.0",
    "Port": 9339,
    "MaxPlayers": 1000,
    "Maintenance": false,
    "MaintenanceMessage": "Servidor em manutenção!"
  },
  "Database": {
    "Host": "localhost",
    "Port": 3306,
    "Username": "crserver",
    "Password": "SuaSenha123",
    "Database": "clashroyale"
  },
  "Game": {
    "Version": "3.2803.3",
    "DefaultGold": 100000000,
    "DefaultGems": 100000000,
    "DefaultLevel": 1,
    "MaxLevel": 14,
    "DefaultTrophies": 0,
    "AllCardsUnlocked": true
  }
}
