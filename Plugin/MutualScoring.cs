namespace MutualScoringPlayers;

public class MutualScoring
{
    public Dictionary<int, int> Kills { get; set; } = [];
    public Dictionary<int, int> KillsRow { get; set; } = [];

    public void Init(int slot)
    {
        Kills.TryAdd(slot, 0);
        KillsRow.TryAdd(slot, 0);
    }

    public void IncrementScore(int slot)
    {
        Kills[slot]++;
        KillsRow[slot]++;
    }

    public void ResetRow(int target) => KillsRow[target] = 0;
}