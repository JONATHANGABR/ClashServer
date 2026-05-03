import time


class LoginParser:
    """
    LOGIN (10101) — versão 3.2803.3
    """

    @staticmethod
    def parse(payload: bytes) -> dict:
        offset = 0

        account_id = int.from_bytes(payload[offset:offset + 8], 'big')
        offset += 8

        token_len  = payload[offset]
        offset += 1
        pass_token = payload[offset:offset + token_len]
        offset += token_len

        client_ver = int.from_bytes(payload[offset:offset + 4], 'big')
        offset += 4

        locale = int.from_bytes(payload[offset:offset + 4], 'big')
        offset += 4

        return {
            "account_id":     account_id,
            "pass_token":     pass_token.hex(),
            "client_version": client_ver,
            "locale":         locale
        }

    @staticmethod
    def build_login_ok(account_id: int, token: str) -> bytes:
        """
        LOGIN_OK (20104)
        """
        payload  = account_id.to_bytes(8, 'big')
        payload += (0).to_bytes(4, 'big')

        token_bytes = token.encode()
        payload += len(token_bytes).to_bytes(1, 'big')
        payload += token_bytes

        payload += int(time.time()).to_bytes(4, 'big')

        return payload
