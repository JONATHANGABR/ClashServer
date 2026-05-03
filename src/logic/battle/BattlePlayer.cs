using CRServer.Logic.Player;

namespace CRServer.Logic.Battle;

public class BattlePlayer
{
    public long AccountId { get; set; }
    public string Name { get; set; } = "Jogador";
    public int Trophies { get; set; } = 0;
    public int Level { get; set; } = 1;
    public int CrownsTaken { get; set; } = 0;
    public int TowerHp { get; set; } = 3000;
    public bool IsBot { get; set; } = false;
    public bool SurrenderedOrLeft { get; set; } = false;
    public List<int> Deck { get; set; } = new();
    public Network.Session? Session { get; set; }

    // Construtor para jogador real
    public static BattlePlayer FromPlayer(PlayerData player, Network.Session session)
    {
        return new BattlePlayer
        {
            AccountId = player.AccountId,
            Name = player.Name ?? "Jogador",
            Trophies = player.Trophies,
            Level = player.Level,
            Session = session,
            IsBot = false,
            Deck = player.Deck ?? Bot.BotDecks.GetRandomDeck()
        };
    }

    // Construtor para bot
    public static BattlePlayer CreateBot(int trophies)
    {
        var random = new Random();
        var botNames = new[]
        {
            "Knight Bot", "Archer Bot", "Giant Bot",
            "Wizard Bot", "Dragon Bot", "Pekka Bot"
        };

        return new BattlePlayer
        {
            AccountId = -(random.NextInt64(1000000, 9999999)),
            Name = botNames[random.Next(botNames.Length)],
            Trophies = trophies,
            Level = random.Next(8, 14),
            IsBot = true,
            Session = null,
            Deck = Bot.BotDecks.GetRandomDeck()
        };
    }
}
