import sqlite3
import time


class Database:
    def __init__(self, db_path: str, logger):
        self.logger = logger
        self.conn   = sqlite3.connect(db_path, check_same_thread=False)
        self._create_tables()
        logger.info(f"🗄️  Banco de dados conectado: {db_path}")

    def _create_tables(self):
        self.conn.executescript("""
            CREATE TABLE IF NOT EXISTS players (
                id          INTEGER PRIMARY KEY AUTOINCREMENT,
                account_id  INTEGER UNIQUE,
                token       TEXT,
                name        TEXT    DEFAULT 'Player',
                level       INTEGER DEFAULT 1,
                trophies    INTEGER DEFAULT 0,
                gold        INTEGER DEFAULT 999999,
                gems        INTEGER DEFAULT 999999,
                elixir      INTEGER DEFAULT 999999,
                created_at  INTEGER
            );

            CREATE TABLE IF NOT EXISTS decks (
                id          INTEGER PRIMARY KEY AUTOINCREMENT,
                player_id   INTEGER,
                card_id     INTEGER,
                slot        INTEGER,
                level       INTEGER DEFAULT 14,
                FOREIGN KEY (player_id) REFERENCES players(id)
            );

            CREATE TABLE IF NOT EXISTS battles (
                id          INTEGER PRIMARY KEY AUTOINCREMENT,
                player1_id  INTEGER,
                player2_id  INTEGER,
                winner_id   INTEGER,
                started_at  INTEGER,
                ended_at    INTEGER
            );
        """)
        self.conn.commit()

    def get_or_create_player(self, account_id: int) -> dict:
        cursor = self.conn.cursor()

        cursor.execute(
            "SELECT * FROM players WHERE account_id = ?",
            (account_id,)
        )
        row = cursor.fetchone()

        if row:
            columns = [d[0] for d in cursor.description]
            return dict(zip(columns, row))

        cursor.execute(
            "INSERT INTO players (account_id, name, created_at) VALUES (?, 'Player', ?)",
            (account_id, int(time.time()))
        )
        self.conn.commit()
        self.logger.info(f"🆕 Novo jogador criado: account_id={account_id}")

        return self.get_or_create_player(account_id)
