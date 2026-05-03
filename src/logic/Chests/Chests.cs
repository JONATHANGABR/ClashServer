namespace CRServer.Logic.Chests;

public enum ChestType
{
    Wooden = 0,
    Silver = 1,
    Golden = 2,
    Giant = 3,
    Magical = 4,
    SuperMagical = 5,
    Epic = 6,
    Legendary = 7
}

public class Chest
{
    public int Id { get; set; }
    public ChestType Type { get; set; }
    public int Slot { get; set; } // 0-3 (4 slots)
    public DateTime? UnlockStartTime { get; set; }
    public bool IsUnlocking => UnlockStartTime.HasValue;
    public bool IsUnlocked => UnlockStartTime.HasValue && 
                               DateTime.UtcNow >= UnlockStartTime.Value.AddSeconds(GetUnlockDuration());
    
    public int GetUnlockDuration()
    {
        return Type switch
        {
            ChestType.Wooden => 0,           // Instantâneo
            ChestType.Silver => 3 * 3600,    // 3 horas
            ChestType.Golden => 8 * 3600,    // 8 horas
            ChestType.Giant => 12 * 3600,    // 12 horas
            ChestType.Magical => 12 * 3600,  // 12 horas
            ChestType.SuperMagical => 24 * 3600,  // 24 horas
            ChestType.Epic => 12 * 3600,     // 12 horas
            ChestType.Legendary => 24 * 3600, // 24 horas
            _ => 0
        };
    }
    
    public int GetGemUnlockCost()
    {
        if (!IsUnlocking) return 0;
        
        var remainingSeconds = (UnlockStartTime!.Value.AddSeconds(GetUnlockDuration()) - DateTime.UtcNow).TotalSeconds;
        if (remainingSeconds <= 0) return 0;
        
        // 1 gema por minuto
        return (int)Math.Ceiling(remainingSeconds / 60.0);
    }
}
