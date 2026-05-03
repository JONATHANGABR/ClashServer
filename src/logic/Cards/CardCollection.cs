using Newtonsoft.Json;

namespace CRServer.Logic.Cards;

public class CardCollection
{
    public Dictionary<int, Card> Cards { get; set; } = new();
    
    public void AddCard(int cardId, int count = 1)
    {
        if (Cards.ContainsKey(cardId))
        {
            Cards[cardId].Count += count;
        }
        else
        {
            var cardData = CardManager.GetCardById(cardId);
            Cards[cardId] = new Card
            {
                CardId = cardId,
                Count = count,
                Level = 1,
                IsNew = true,
                Data = cardData
            };
        }
    }
    
    public bool UpgradeCard(int cardId, long playerGold)
    {
        if (!Cards.ContainsKey(cardId)) return false;
        
        var card = Cards[cardId];
        if (!card.CanUpgrade()) return false;
        
        int goldCost = card.GetUpgradeCost();
        if (playerGold < goldCost) return false;
        
        // Consumir cartas e subir nível
        card.Count -= card.Data!.GetUpgradeCardsRequired(card.Level);
        card.Level++;
        
        return true;
    }
    
    public string ToJson()
    {
        return JsonConvert.SerializeObject(Cards);
    }
    
    public static CardCollection FromJson(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return new CardCollection();
        
        try
        {
            var cards = JsonConvert.DeserializeObject<Dictionary<int, Card>>(json);
            return new CardCollection { Cards = cards ?? new() };
        }
        catch
        {
            return new CardCollection();
        }
    }
}
