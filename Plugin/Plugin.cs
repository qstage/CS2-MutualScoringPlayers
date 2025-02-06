using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;

namespace MutualScoringPlayers;

public class Plugin : BasePlugin
{
    public override string ModuleName { get; } = "MutualScoringPlayers";
    public override string ModuleVersion { get; } = "1.0.1";
    public override string ModuleAuthor { get; } = "xstage";

    private readonly Dictionary<CCSPlayerController, MutualScoring> mutualScoring_ = [];

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
        mutualScoring_[player] = new MutualScoring();

        foreach (var target in Utilities.GetPlayers())
        {
            if (target != player)
            {
                mutualScoring_.TryAdd(target, new());

                mutualScoring_[player].Init(target);
                mutualScoring_[target].Init(player);
            }
        }
    }

    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo _)
    {
        var victim = @event.Userid;
        var attacker = @event.Attacker;

        if (victim == null || attacker == null || victim == attacker)
            return HookResult.Continue;

        mutualScoring_[victim].ResetRow(attacker);
        mutualScoring_[attacker].IncrementScore(victim);

        if (!attacker.IsBot)
        attacker.PrintToChat(Localizer.ForPlayer(attacker, "Plugin.Attacker", Localizer["Plugin.Tag"], attacker.PlayerName, mutualScoring_[attacker].Kills[victim],
            mutualScoring_[victim].Kills[attacker], victim.PlayerName, mutualScoring_[attacker].KillsRow[victim]));

        if (!victim.IsBot)
        victim.PrintToChat(Localizer.ForPlayer(victim, "Plugin.Victim", Localizer["Plugin.Tag"], victim.PlayerName, mutualScoring_[victim].Kills[attacker],
            mutualScoring_[attacker].Kills[victim], attacker.PlayerName, mutualScoring_[attacker].KillsRow[victim]));

        return HookResult.Continue;
    }
}
