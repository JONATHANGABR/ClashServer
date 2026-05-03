using CRServer.Logic.Cards;

namespace CRServer.Logic.Chests;

public class ChestReward
{
    public long Gold { get; set; }
    public int Gems { get; set; }
    public List<CardReward> Cards { get; set; } = new();
}

public class CardReward
{
    public int CardId { get; set; }
    public int Count { get; set; }
}

public static class ChestRewardGenerator
{
    private static readonly Random _random = new();
    
    public static ChestReward GenerateReward(ChestType type, int playerArena)
    {
        var reward = new ChestReward();
        
        switch (type)
        {
            case ChestType.Wooden:
                reward.Gold = _random.Next(15, 30);
                reward.Cards = GenerateCards(3, playerArena);
                break;
                
            case ChestType.Silver:
                reward.Gold = _random.Next(60, 100);
                reward.Cards = GenerateCards(11, playerArena);
                break;
                
            case ChestType.Golden:
                reward.Gold = _random.Next(200, 350);
                reward.Cards = GenerateCards(42, playerArena);
                break;
                
            case ChestType.Giant:
                reward.Gold = _random.Next(800, 1200);
                reward.Cards = GenerateCards(108, playerArena);
                break;
                
            case ChestType.Magical:
                reward.Gold = _random.Next(500, 800);
                reward.Gems = _random.Next(5, 15);
                reward.Cards = GenerateCards(55, playerArena, guaranteeEpic: 1);
                break;
                
            case ChestType.SuperMagical:
                reward.Gold = _random.Next(3000, 5000);
                reward.Gems = _random.Next(50, 100);
                reward.Cards = GenerateCards(198, playerArena, guaranteeEpic: 6, guaranteeLegendary: 1);
                break;
                
            case ChestType.Epic:
                reward.Gold = _random.Next(1000, 1500);
                reward.Cards = GenerateCards(10, playerArena, onlyEpic: true);
                break;
                
            case ChestType.Legendary:
                reward.Gold = _random.Next(5000, 10000);
                reward.Gems = _random.Next(100, 200);
                reward.Cards = GenerateCards(1, playerArena, onlyLegendary: true);
                break;
        }
        
        return reward;
    }
    
    private static List<CardReward> GenerateCards(
        int totalCards,
        int arena,
        bool onlyEpic = false,
        bool onlyLegendary = false,
        int guaranteeEpic = 0,
        int guaranteeLegendary = 0)
    {
        var rewards = new List<CardReward>();
        var availableCards = CardManager.GetCardsByArena(arena).ToList();
        
        // Garantir épicas
        for (int i = 0; i < guaranteeEpic; i++)
        {
            var epicCards = availableCards.Where(c => c.Rarity == CardRarity.Epic).ToList();
            if (epicCards.Any())
            {
                var card = epicCards[_random.Next(epicCards.Count)];
                rewards.Add(new CardReward { CardId = card.Id, Count = 1 });
            }
        }
        
        // Garantir lendárias
        for (int i = 0; i < guaranteeLegendary; i++)
        {
            var legendaryCards = availableCards.Where(c => c.Rarity == CardRarity.Legendary).ToList();
            if (legendaryCards.Any())
            {
                var card = legendaryCards[_random.Next(legendaryCards.Count)];
                rewards.Add(new CardReward { CardId = card.Id, Count = 1 });
            }
        }
        
        // Preencher o resto
        int remaining = totalCards - rewards.Sum(r => r.Count);
        
        for (int i = 0; i < remaining; i++)
        {
            CardData card;
            
            if (onlyEpic)
            {
                var epicCards = availableCards.Where(c => c.Rarity == CardRarity.Epic).ToList();
                card = epicCards[_random.Next(epicCards.Count)];
            }
            else if (onlyLegendary)
            {
                var legendaryCards = availableCards.Where(c => c.Rarity == CardRarity.Legendary).ToList();
                card = legendaryCards[_random.Next(legendaryCards.Count)];
            }
            else
            {
                // Distribuição realista
                var roll = _random.Next(100);
                CardRarity rarity = roll switch
                {
                    < 75 => CardRarity.Common,     // 75%
                    < 95 => CardRarity.Rare,       // 20%
                    < 99 => CardRarity.Epic,       // 4%
                    _ => CardRarity.Legendary      // 1%
                };
                
                var cardsOfRarity = availableCards.Where(c => c.Rarity == rarity).ToList();
                if (!cardsOfRarity.Any()) cardsOfRarity = availableCards.Where(c => c.Rarity == CardRarity.Common).ToList();
                
                card = cardsOfRarity[_random.Next(cardsOfRarity.Count)];
            }
            
            var existing = rewards.FirstOrDefault(r => r.CardId == card.Id);
            if (existing != null)
                existing.Count++;
            else
                rewards.Add(new CardReward { CardId = card.Id, Count = 1 });
        }
        
        return rewards;
    }
}
