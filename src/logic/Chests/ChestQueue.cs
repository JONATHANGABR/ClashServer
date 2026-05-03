using Newtonsoft.Json;

namespace CRServer.Logic.Chests;

public class ChestQueue
{
    public List<Chest> Chests { get; set; } = new();
    private const int MaxSlots = 4;
    
    public bool IsFull => Chests.Count >= MaxSlots;
    
    public bool CanAddChest => !IsFull;
    
    public Chest? GetUnlockingChest()
    {
        return Chests.FirstOrDefault(c => c.IsUnlocking);
    }
    
    public bool AddChest(ChestType type)
    {
        if (IsFull) return false;
        
        int slot = 0;
        while (Chests.Any(c => c.Slot == slot))
            slot++;
        
        Chests.Add(new Chest
        {
            Id = Chests.Count > 0 ? Chests.Max(c => c.Id) + 1 : 1,
            Type = type,
            Slot = slot
        });
        
        return true;
    }
    
    public bool StartUnlock(int chestId)
    {
        var chest = Chests.FirstOrDefault(c => c.Id == chestId);
        if (chest == null || chest.IsUnlocking) return false;
        
        // Só pode ter 1 baú desbloqueando
        if (GetUnlockingChest() != null) return false;
        
        chest.UnlockStartTime = DateTime.UtcNow;
        return true;
    }
    
    public bool UnlockWithGems(int chestId, ref int playerGems)
    {
        var chest = Chests.FirstOrDefault(c => c.Id == chestId);
        if (chest == null || !chest.IsUnlocking) return false;
        
        int gemCost = chest.GetGemUnlockCost();
        if (playerGems < gemCost) return false;
        
        playerGems -= gemCost;
        chest.UnlockStartTime = DateTime.UtcNow.AddSeconds(-chest.GetUnlockDuration());
        
        return true;
    }
    
    public ChestReward? CollectChest(int chestId, int playerArena)
    {
        var chest = Chests.FirstOrDefault(c => c.Id == chestId);
        if (chest == null || !chest.IsUnlocked) return null;
        
        var reward = ChestRewardGenerator.GenerateReward(chest.Type, playerArena);
        Chests.Remove(chest);
        
        return reward;
    }
    
    public string ToJson()
    {
        return JsonConvert.SerializeObject(Chests);
    }
    
    public static ChestQueue FromJson(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return new ChestQueue();
        
        try
        {
            var chests = JsonConvert.DeserializeObject<List<Chest>>(json);
            return new ChestQueue { Chests = chests ?? new() };
        }
        catch
        {
            return new ChestQueue();
        }
    }
}
