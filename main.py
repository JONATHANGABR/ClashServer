import json
from logger import setup_logger
from server import ClashRoyaleServer
from dns_server import FakeDNS
from database.db import Database

def main():
    with open("config.json") as f:
        config = json.load(f)

    logger = setup_logger(config["server_name"], config["debug"])
    logger.info("⚔️  Iniciando Clash Royale Private Server...")

    # Banco de dados
    db = Database(config["db_path"], logger)

    # DNS falso (redireciona game.clashroyaleapp.com → seu IP)
    dns = FakeDNS(
        server_ip=config["your_server_ip"],
        game_domain=config["game_domain"],
        logger=logger,
        port=config["dns_port"]
    )
    dns.start()

    # Servidor principal
    srv = ClashRoyaleServer(config, logger, db)
    srv.start()

if __name__ == "__main__":
    main()
