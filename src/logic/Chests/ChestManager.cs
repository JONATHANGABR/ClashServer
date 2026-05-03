using Serilog;

namespace CRServer.Logic.Chests;

public static class ChestManager
{
    private static readonly Random _random = new();
    
    // Ciclo de baús (200 batalhas)
    private static readonly List<ChestType> _chestCycle = GenerateChestCycle();
    
    private static List<ChestType> GenerateChestCycle()
    {
        var cycle = new List<ChestType>();
        
        // 180 baús silver/gold
        for (int i = 0; i < 180; i++)
        {
            cycle.Add(i % 4 == 0 ? ChestType.Golden : ChestType.Silver);
        }
        
        // Inserir baús especiais em posições fixas
        cycle[10] = ChestType.Giant;
        cycle[40] = ChestType.Magical;
        cycle[80] = ChestType.Giant;
        cycle[120] = ChestType.Magical;
        cycle[160] = ChestType.Giant;
        cycle[180] = ChestType.SuperMagical;
        
        // Completar com silver
        while (cycle.Count < 200)
        {
            cycle.Add(ChestType.Silver);
        }
        
        return cycle;
    }
    
    public static ChestType GetNextChestType(int battleCount)
    {
        int index = battleCount % _chestCycle.Count;
        return _chestCycle[index];
    }
}
