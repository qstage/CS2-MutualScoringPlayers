using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;

namespace MutualScoringPlayers;

public class Plugin : BasePlugin
{
    public override string ModuleName { get; } = "MutualScoringPlayers";
    public override string ModuleVersion { get; } = "1.0.3";
    public override string ModuleAuthor { get; } = "xstage";

    private readonly Dictionary<int, MutualScoring> mutualScoring_ = [];

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        RegisterListener<Listeners.OnClientPutInServer>(OnClientPutInServer);

        if (!hotReload) return;

        foreach (var player in Utilities.GetPlayers())
            OnClientPutInServer(player);
    }

    private void OnClientPutInServer(int slot)
    {
        var player = Utilities.GetPlayerFromSlot(slot);

        if (player == null) return;

        OnClientPutInServer(player);
    }

    private void OnClientPutInServer(CCSPlayerController player)
    {
        mutualScoring_[player.Slot] = new MutualScoring();

        foreach (var target in Utilities.GetPlayers())
        {
            if (target != player)
            {
                mutualScoring_.TryAdd(target.Slot, new());

                mutualScoring_[player.Slot].Init(target.Slot);
                mutualScoring_[target.Slot].Init(player.Slot);
            }
        }
    }

    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo _)
    {
        var victim = @event.Userid;
        var attacker = @event.Attacker;

        if (victim == null || attacker == null || victim == attacker)
            return HookResult.Continue;

        mutualScoring_[victim.Slot].ResetRow(attacker.Slot);
        mutualScoring_[attacker.Slot].IncrementScore(victim.Slot);

        if (!attacker.IsBot)
        attacker.PrintToChat(Localizer.ForPlayer(attacker, "Plugin.Attacker", Localizer["Plugin.Tag"], attacker.PlayerName, mutualScoring_[attacker.Slot].Kills[victim.Slot],
            mutualScoring_[victim.Slot].Kills[attacker.Slot], victim.PlayerName, mutualScoring_[attacker.Slot].KillsRow[victim.Slot]));

        if (!victim.IsBot)
        victim.PrintToChat(Localizer.ForPlayer(victim, "Plugin.Victim", Localizer["Plugin.Tag"], victim.PlayerName, mutualScoring_[victim.Slot].Kills[attacker.Slot],
            mutualScoring_[attacker.Slot].Kills[victim.Slot], attacker.PlayerName, mutualScoring_[attacker.Slot].KillsRow[victim.Slot]));

        return HookResult.Continue;
    }
}
