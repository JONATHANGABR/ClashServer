class RC4:
    """
    Clash Royale 3.x usa RC4.
    Cada sessão tem sua própria instância (estado independente).
    """

    def __init__(self, key: bytes):
        self.S = list(range(256))
        j = 0
        for i in range(256):
            j = (j + self.S[i] + key[i % len(key)]) % 256
            self.S[i], self.S[j] = self.S[j], self.S[i]
        self.i = 0
        self.j = 0

    def process(self, data: bytes) -> bytes:
        """Encripta ou desencripta (RC4 é simétrico)."""
        S = self.S
        i, j = self.i, self.j
        result = []

        for byte in data:
            i = (i + 1) % 256
            j = (j + S[i]) % 256
            S[i], S[j] = S[j], S[i]
            result.append(byte ^ S[(S[i] + S[j]) % 256])

        self.i, self.j = i, j
        self.S = S
        return bytes(result)
