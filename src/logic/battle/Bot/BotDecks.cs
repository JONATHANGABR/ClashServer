namespace CRServer.Logic.Battle.Bot;

public static class BotDecks
{
    // Baralhos pré-definidos para os bots
    // IDs das cartas conforme o protocolo do CR 3.2803.3
    private static readonly List<List<int>> Decks = new()
    {
        // Deck 1 — Hog Rider Cycle
        new List<int> { 26000000, 26000006, 26000025, 26000005,
                         26000011, 26000002, 26000009, 26000047 },

        // Deck 2 — Giant Double Prince
        new List<int> { 26000003, 26000041, 26000042, 26000015,
                         26000007, 26000009, 26000029, 26000011 },

        // Deck 3 — Golem Beatdown
        new List<int> { 26000017, 26000041, 26000025, 26000015,
                         26000009, 26000007, 26000011, 26000029 },

        // Deck 4 — Bait Deck
        new List<int> { 26000031, 26000001, 26000023, 26000044,
                         26000005, 26000011, 26000009, 26000025 },

        // Deck 5 — Balloon Freeze
        new List<int> { 26000019, 26000037, 26000006, 26000009,
                         26000015, 26000025, 26000011, 26000047 },
    };

    public static List<int> GetRandomDeck()
    {
        var random = new Random();
        return Decks[random.Next(Decks.Count)];
    }

    public static List<int> GetDeckByIndex(int index)
    {
        if (index < 0 || index >= Decks.Count)
            return GetRandomDeck();
        return Decks[index];
    }
}
