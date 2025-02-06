using CounterStrikeSharp.API.Core;

namespace MutualScoringPlayers;

public class MutualScoring
{
    public Dictionary<CCSPlayerController, int> Kills { get; set; } = [];
    public Dictionary<CCSPlayerController, int> KillsRow { get; set; } = [];

    public void Init(CCSPlayerController target)
    {
        Kills.TryAdd(target, 0);
        KillsRow.TryAdd(target, 0);
    }

    public void IncrementScore(CCSPlayerController target)
    {
        Kills[target]++;
        KillsRow[target]++;
    }

    public void ResetRow(CCSPlayerController target) => KillsRow[target] = 0;
}