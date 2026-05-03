namespace CRServer.Logic.Cards;

public enum CardRarity
{
    Common = 0,
    Rare = 1,
    Epic = 2,
    Legendary = 3
}

public enum CardType
{
    Troop = 0,
    Building = 1,
    Spell = 2
}

public class CardData
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public CardRarity Rarity { get; set; }
    public CardType Type { get; set; }
    public int ElixirCost { get; set; }
    public int Arena { get; set; }
    public string IconSWF { get; set; } = "";
    
    // Levels (1-14 para Common, 1-12 para Rare, etc)
    public int MaxLevel => Rarity switch
    {
        CardRarity.Common => 14,
        CardRarity.Rare => 12,
        CardRarity.Epic => 9,
        CardRarity.Legendary => 6,
        _ => 14
    };
    
    // Cartas necessárias para upgrade
    public int GetUpgradeCardsRequired(int currentLevel)
    {
        return Rarity switch
        {
            CardRarity.Common => currentLevel switch
            {
                1 => 2, 2 => 4, 3 => 10, 4 => 20, 5 => 50,
                6 => 100, 7 => 200, 8 => 400, 9 => 800,
                10 => 1000, 11 => 2000, 12 => 5000, 13 => 10000,
                _ => 0
            },
            CardRarity.Rare => currentLevel switch
            {
                1 => 2, 2 => 4, 3 => 10, 4 => 20, 5 => 50,
                6 => 100, 7 => 200, 8 => 400, 9 => 800,
                10 => 1000, 11 => 2000, _ => 0
            },
            CardRarity.Epic => currentLevel switch
            {
                1 => 2, 2 => 4, 3 => 10, 4 => 20, 5 => 50,
                6 => 100, 7 => 200, 8 => 400, _ => 0
            },
            CardRarity.Legendary => currentLevel switch
            {
                1 => 2, 2 => 4, 3 => 10, 4 => 20, 5 => 40, _ => 0
            },
            _ => 0
        };
    }
    
    // Ouro necessário para upgrade
    public int GetUpgradeGoldRequired(int currentLevel)
    {
        return Rarity switch
        {
            CardRarity.Common => currentLevel switch
            {
                1 => 5, 2 => 20, 3 => 50, 4 => 150, 5 => 400,
                6 => 1000, 7 => 2000, 8 => 4000, 9 => 8000,
                10 => 10000, 11 => 20000, 12 => 50000, 13 => 100000,
                _ => 0
            },
            CardRarity.Rare => currentLevel switch
            {
                1 => 50, 2 => 150, 3 => 400, 4 => 1000, 5 => 2000,
                6 => 4000, 7 => 8000, 8 => 10000, 9 => 20000,
                10 => 50000, 11 => 100000, _ => 0
            },
            CardRarity.Epic => currentLevel switch
            {
                1 => 400, 2 => 1000, 3 => 2000, 4 => 4000,
                5 => 8000, 6 => 10000, 7 => 20000, 8 => 50000, _ => 0
            },
            CardRarity.Legendary => currentLevel switch
            {
                1 => 5000, 2 => 20000, 3 => 50000, 4 => 100000, 5 => 200000, _ => 0
            },
            _ => 0
        };
    }
}
