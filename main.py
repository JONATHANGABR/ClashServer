import json

from logger      import setup_logger
from server      import ClashRoyaleServer
from dns_server  import FakeDNS
from database.db import Database


def load_config(path: str = "config.json") -> dict:
    with open(path, "r") as f:
        return json.load(f)


def main():
    config = load_config()
    logger = setup_logger(config["server_name"], config["debug"])

    logger.info("⚔️  Iniciando Clash Royale Private Server...")
    logger.info(f"📡 Host: {config['host']} | Porta: {config['port']}")

    db = Database(config["db_path"], logger)

    dns = FakeDNS(
        server_ip=config["your_server_ip"],
        game_domain=config["game_domain"],
        logger=logger,
        port=config["dns_port"]
    )
    dns.start()

    srv = ClashRoyaleServer(config, logger, db)
    srv.start()


if __name__ == "__main__":
    main()
