namespace CRServer.Logic.Cards;

public class Card
{
    public int CardId { get; set; }
    public int Count { get; set; } = 0;
    public int Level { get; set; } = 1;
    public bool IsNew { get; set; } = false;
    
    public CardData? Data { get; set; }
    
    public bool CanUpgrade()
    {
        if (Data == null) return false;
        if (Level >= Data.MaxLevel) return false;
        return Count >= Data.GetUpgradeCardsRequired(Level);
    }
    
    public int GetUpgradeCost()
    {
        return Data?.GetUpgradeGoldRequired(Level) ?? 0;
    }
}
