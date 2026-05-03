using Newtonsoft.Json;
using Serilog;

namespace CRServer.Logic.Cards;

public static class CardManager
{
    private static readonly Dictionary<int, CardData> _cards = new();
    
    public static void LoadCards()
    {
        try
        {
            // Carregar do JSON
            var json = File.ReadAllText("Data/Cards/cards.json");
            var cards = JsonConvert.DeserializeObject<List<CardData>>(json);
            
            if (cards != null)
            {
                foreach (var card in cards)
                {
                    _cards[card.Id] = card;
                }
                Log.Information($"[CARDS] {_cards.Count} cartas carregadas!");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[CARDS] Erro ao carregar cartas! Usando dados padrão.");
            LoadDefaultCards();
        }
    }
    
    private static void LoadDefaultCards()
    {
        // Cartas básicas padrão (você vai substituir pelos dados reais depois)
        var defaultCards = new List<CardData>
        {
            new() { Id = 26000000, Name = "Knight", Rarity = CardRarity.Common, Type = CardType.Troop, ElixirCost = 3, Arena = 0 },
            new() { Id = 26000001, Name = "Archers", Rarity = CardRarity.Common, Type = CardType.Troop, ElixirCost = 3, Arena = 0 },
            new() { Id = 26000002, Name = "Goblins", Rarity = CardRarity.Common, Type = CardType.Troop, ElixirCost = 2, Arena = 1 },
            new() { Id = 26000003, Name = "Giant", Rarity = CardRarity.Rare, Type = CardType.Troop, ElixirCost = 5, Arena = 0 },
            new() { Id = 26000004, Name = "P.E.K.K.A", Rarity = CardRarity.Epic, Type = CardType.Troop, ElixirCost = 7, Arena = 4 },
            new() { Id = 26000005, Name = "Minions", Rarity = CardRarity.Common, Type = CardType.Troop, ElixirCost = 3, Arena = 0 },
            new() { Id = 26000006, Name = "Balloon", Rarity = CardRarity.Epic, Type = CardType.Troop, ElixirCost = 5, Arena = 2 },
            new() { Id = 26000007, Name = "Witch", Rarity = CardRarity.Epic, Type = CardType.Troop, ElixirCost = 5, Arena = 0 },
            new() { Id = 26000008, Name = "Barbarians", Rarity = CardRarity.Common, Type = CardType.Troop, ElixirCost = 5, Arena = 3 },
            new() { Id = 26000009, Name = "Golem", Rarity = CardRarity.Epic, Type = CardType.Troop, ElixirCost = 8, Arena = 6 },
            new() { Id = 26000010, Name = "Skeletons", Rarity = CardRarity.Common, Type = CardType.Troop, ElixirCost = 1, Arena = 2 },
            new() { Id = 26000011, Name = "Valkyrie", Rarity = CardRarity.Rare, Type = CardType.Troop, ElixirCost = 4, Arena = 1 },
            new() { Id = 26000012, Name = "Skeleton Army", Rarity = CardRarity.Epic, Type = CardType.Troop, ElixirCost = 3, Arena = 0 },
            new() { Id = 26000013, Name = "Bomber", Rarity = CardRarity.Common, Type = CardType.Troop, ElixirCost = 2, Arena = 0 },
            new() { Id = 26000014, Name = "Musketeer", Rarity = CardRarity.Rare, Type = CardType.Troop, ElixirCost = 4, Arena = 0 },
            new() { Id = 26000015, Name = "Baby Dragon", Rarity = CardRarity.Epic, Type = CardType.Troop, ElixirCost = 4, Arena = 0 },
            new() { Id = 26000016, Name = "Prince", Rarity = CardRarity.Epic, Type = CardType.Troop, ElixirCost = 5, Arena = 0 },
            new() { Id = 26000017, Name = "Wizard", Rarity = CardRarity.Rare, Type = CardType.Troop, ElixirCost = 5, Arena = 5 },
            new() { Id = 26000018, Name = "Mini P.E.K.K.A", Rarity = CardRarity.Rare, Type = CardType.Troop, ElixirCost = 4, Arena = 0 },
            new() { Id = 26000019, Name = "Spear Goblins", Rarity = CardRarity.Common, Type = CardType.Troop, ElixirCost = 2, Arena = 1 },
            new() { Id = 26000020, Name = "Giant Skeleton", Rarity = CardRarity.Epic, Type = CardType.Troop, ElixirCost = 6, Arena = 2 },
            new() { Id = 26000021, Name = "Hog Rider", Rarity = CardRarity.Rare, Type = CardType.Troop, ElixirCost = 4, Arena = 4 },
            new() { Id = 26000022, Name = "Minion Horde", Rarity = CardRarity.Common, Type = CardType.Troop, ElixirCost = 5, Arena = 4 },
            new() { Id = 26000023, Name = "Ice Wizard", Rarity = CardRarity.Legendary, Type = CardType.Troop, ElixirCost = 3, Arena = 5 },
            new() { Id = 26000024, Name = "Royal Giant", Rarity = CardRarity.Common, Type = CardType.Troop, ElixirCost = 6, Arena = 7 },
            new() { Id = 26000025, Name = "Guards", Rarity = CardRarity.Epic, Type = CardType.Troop, ElixirCost = 3, Arena = 7 },
            new() { Id = 26000026, Name = "Princess", Rarity = CardRarity.Legendary, Type = CardType.Troop, ElixirCost = 3, Arena = 5 },
            new() { Id = 26000027, Name = "Dark Prince", Rarity = CardRarity.Epic, Type = CardType.Troop, ElixirCost = 4, Arena = 7 },
            
            // Buildings
            new() { Id = 27000000, Name = "Cannon", Rarity = CardRarity.Common, Type = CardType.Building, ElixirCost = 3, Arena = 0 },
            new() { Id = 27000001, Name = "Goblin Hut", Rarity = CardRarity.Rare, Type = CardType.Building, ElixirCost = 5, Arena = 1 },
            new() { Id = 27000002, Name = "Mortar", Rarity = CardRarity.Common, Type = CardType.Building, ElixirCost = 4, Arena = 6 },
            new() { Id = 27000003, Name = "Inferno Tower", Rarity = CardRarity.Rare, Type = CardType.Building, ElixirCost = 5, Arena = 4 },
            new() { Id = 27000004, Name = "Bomb Tower", Rarity = CardRarity.Rare, Type = CardType.Building, ElixirCost = 4, Arena = 2 },
            new() { Id = 27000005, Name = "Barbarian Hut", Rarity = CardRarity.Rare, Type = CardType.Building, ElixirCost = 7, Arena = 3 },
            new() { Id = 27000006, Name = "Tesla", Rarity = CardRarity.Common, Type = CardType.Building, ElixirCost = 4, Arena = 4 },
            new() { Id = 27000007, Name = "Elixir Collector", Rarity = CardRarity.Rare, Type = CardType.Building, ElixirCost = 6, Arena = 6 },
            new() { Id = 27000008, Name = "X-Bow", Rarity = CardRarity.Epic, Type = CardType.Building, ElixirCost = 6, Arena = 3 },
            
            // Spells
            new() { Id = 28000000, Name = "Fireball", Rarity = CardRarity.Rare, Type = CardType.Spell, ElixirCost = 4, Arena = 0 },
            new() { Id = 28000001, Name = "Arrows", Rarity = CardRarity.Common, Type = CardType.Spell, ElixirCost = 3, Arena = 0 },
            new() { Id = 28000002, Name = "Rage", Rarity = CardRarity.Epic, Type = CardType.Spell, ElixirCost = 2, Arena = 2 },
            new() { Id = 28000003, Name = "Rocket", Rarity = CardRarity.Rare, Type = CardType.Spell, ElixirCost = 6, Arena = 3 },
            new() { Id = 28000004, Name = "Goblin Barrel", Rarity = CardRarity.Epic, Type = CardType.Spell, ElixirCost = 3, Arena = 1 },
            new() { Id = 28000005, Name = "Freeze", Rarity = CardRarity.Epic, Type = CardType.Spell, ElixirCost = 4, Arena = 4 },
            new() { Id = 28000006, Name = "Mirror", Rarity = CardRarity.Epic, Type = CardType.Spell, ElixirCost = 1, Arena = 5 },
            new() { Id = 28000007, Name = "Lightning", Rarity = CardRarity.Epic, Type = CardType.Spell, ElixirCost = 6, Arena = 1 },
            new() { Id = 28000008, Name = "Zap", Rarity = CardRarity.Common, Type = CardType.Spell, ElixirCost = 2, Arena = 5 },
            new() { Id = 28000009, Name = "Poison", Rarity = CardRarity.Epic, Type = CardType.Spell, ElixirCost = 4, Arena = 5 },
        };
        
        foreach (var card in defaultCards)
        {
            _cards[card.Id] = card;
        }
        
        Log.Information($"[CARDS] {_cards.Count} cartas padrão carregadas");
    }
    
    public static CardData? GetCardById(int id)
    {
        return _cards.GetValueOrDefault(id);
    }
    
    public static List<CardData> GetAllCards()
    {
        return _cards.Values.ToList();
    }
    
    public static List<CardData> GetCardsByArena(int arena)
    {
        return _cards.Values.Where(c => c.Arena <= arena).ToList();
    }
    
    public static List<CardData> GetCardsByRarity(CardRarity rarity)
    {
        return _cards.Values.Where(c => c.Rarity == rarity).ToList();
    }
}
