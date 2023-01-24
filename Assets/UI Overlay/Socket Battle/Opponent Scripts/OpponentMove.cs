using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OpponentMove : IOpponentMove
{
    /// <summary>
    /// The move to show above the attacking pokemon
    /// </summary>
    override public string moveDescription
    {
        get
        {
            
            var desc = base.moveDescription
                .Replace("{userName}", actingPokemon.pokemonName)
                .Replace("{attack}", attackStat.ToString())
                .Replace("{special}", specialStat.ToString())
                .Replace("{defense}", defenseStat.ToString())
                .Replace("{evasion}", evasionStat.ToString())
                .Replace("{userHeal}", userHeal.ToString())
                .Replace("{block}", blockStat.ToString())
                .Replace("{damage}", damage.ToString());
            return desc;
        }
    }

    /// <summary>
    /// The possible targets of this card. Self, Bench, Team, ActiveOpponent, AnyOpponent, BenchOpponent, AnyPokemon, OverrideTarget
    /// </summary>
    public MoveTargetType targetType = MoveTargetType.ActiveOpponent;

    public enum MoveTargetType
    {
        Self,
        Bench,
        Team,
        ActiveOpponent,
        AnyOpponent,
        BenchOpponent,
        AnyPokemon,
        BenchAll,
        TeamAll,
        AnyOpponentAll,
        BenchOpponentAll,
        AnyPokemonAll,
        OverrideTarget,
    }

    public int attackCost;
    public int defenseCost;
    public int specialCost;
    public int evasionCost;

    /// <summary>
    /// The energy stat the card will change
    /// </summary>
    public int energyStat;

    /// <summary>
    /// The attack stat the card will change
    /// </summary>
    public int attackStat;

    /// <summary>
    /// The special stat the card will change
    /// </summary>
    public int specialStat;

    /// <summary>
    /// The defense stat the card will change
    /// </summary>
    public int defenseStat;

    /// <summary>
    /// The evasion stat the card will change
    /// </summary>
    public int evasionStat;

    /// <summary>
    /// The block stat the card will change
    /// </summary>
    public int blockStat;

    /// <summary>
    /// The attack multiplier to apply to the next attack * 100
    /// </summary>
    public int attackMultStat;

    /// <summary>
    /// How much the user is healed
    /// </summary>
    public int userHeal;

    /// <summary>
    /// True if the user gets immunity to the next attack
    /// </summary>
    public bool grantsInvulnerability;

    /// <summary>
    /// The number of energies to remove from the target
    /// </summary>
    public int removeEnergyCount;

    /// <summary>
    /// How long a StatusEffect will be placed
    /// </summary>
    public int turn = 99;

    /// <summary>
    /// Set to true if the status affects the user instead of the target
    /// </summary>
    public bool statusAffectsUser = false;

    /// <summary>
    /// Set to true if the status affects the user's bench instead of the target
    /// </summary>
    public bool statusAffectsBothBench = false;

    /// <summary>
    /// The number of times a move was used before. Incremented at the end of playMove
    /// </summary>
    private int numberOfTimesUsed = 0;

    /// <summary>
    /// Set to true if the move will only be used once
    /// </summary>
    public bool isSingleUse = false;

    /// <summary>
    /// Set to true if the move will end with a random energy being assigned to a party member
    /// </summary>
    public bool endOfTurnAddRandomEnergy = false;

    /// <summary>
    /// (Optional) Set to create a new target instead of battleGameBoard.activePokemon
    /// </summary>
    public Pokemon overrideTarget;

    /// <summary>
    /// Returns true if the move can be used
    /// </summary>
    public override bool canUseMove
    {
        get
        {
            // Check single use
            if (isSingleUse && numberOfTimesUsed > 0) return false;

            // Check if self is trapped
            if (actingPokemon.isTrapped && switchInOnUse && battleGameBoard?.opponentActivePokemon != actingPokemon) return false;

            // Check if active is trapped
            if (battleGameBoard.opponentActivePokemon.isTrapped && switchInOnUse && battleGameBoard?.opponentActivePokemon != actingPokemon) return false;

            var costIsPaid = actingPokemon.attachedEnergy.Count < energyMax &&
                actingPokemon.attachedEnergy.Count >= energyRequirement &&
                actingPokemon.attackStat >= attackCost &&
                actingPokemon.defenseStat >= defenseCost &&
                actingPokemon.specialStat >= specialCost &&
                actingPokemon.evasionStat >= evasionCost;
            return costIsPaid;
        }
    }

    /// <summary>
    /// Executes the opponent move
    /// </summary>
    /// <returns></returns>
    public override List<string> playMove()
    {
        var targetList = getTarget();

        var moveMessageList = targetList.Select(target =>
        {
            var moveMessage = "";

            if (target.isInvulnerable && damage > 0)
            {
                if (userAnimationType != "") actingPokemon.modelAnimator.SetTrigger(userAnimationType);
                return actingPokemon.pokemonName + " missed. " + target.pokemonName + " is invulnerable.";
            }

            // Pay cost
            if (discardEnergyOnUse)
            {
                var targetsToRemove = actingPokemon.attachedEnergy.GetRange(actingPokemon.attachedEnergy.Count - energyRequirement, energyRequirement);
                targetsToRemove.ForEach(t =>
                {
                    t.Translate(battleGameBoard.energyDiscardLocation.transform.position);
                    // TODO: these should maybe be destroyed once off screen
                });
                actingPokemon.attachedEnergy.RemoveRange(actingPokemon.attachedEnergy.Count - energyRequirement, energyRequirement);
            }
            actingPokemon.attackStat -= attackCost;
            actingPokemon.defenseStat -= defenseCost;
            actingPokemon.specialStat -= specialCost;
            actingPokemon.evasionStat -= evasionCost;

            // Determine damage
            commonCardPlay(actingPokemon, target, out moveMessage);

            if (userAnimationType != "") actingPokemon.modelAnimator.SetTrigger(userAnimationType);
            if (targetAnimationType != "") target.modelAnimator.SetTrigger(targetAnimationType);
            if (targetAnimationType2 != "") target.modelAnimator.SetTrigger(targetAnimationType2);
            
            return moveMessage;
        }).ToList();

        // Add energy to random target
        if (endOfTurnAddRandomEnergy)
        {
            var aliveParty = battleGameBoard.opponent.party.Where(p => !p.isFainted).ToList();
            var energyStatRandomTarget = aliveParty[UnityEngine.Random.Range(0, aliveParty.Count)];
            addEnergy(energyStatRandomTarget);
            moveMessageList.Add("Added energy to " + energyStatRandomTarget.pokemonName);
        }

        // Update move metadata
        numberOfTimesUsed++;

        if (moveMessageList.Count == 0) return new List<string> { "" };
        return moveMessageList;
    }


    private bool commonCardPlay(Pokemon user, Pokemon target, out string message)
    {
        message = "";
        var attackMissed = false;
        var dealtDamage = 0;

        // Deal Damage if applicable
        if (damage > 0 && !target.isInvulnerable)
        {
            // Determine damage
            dealtDamage = Mathf.RoundToInt(damage * user.attackMultStat / 100f * TypeChart.getEffectiveness(this, target))
                + (user.attackStat - target.defenseStat) * 10
                - target.blockStat;
            target.blockStat = Mathf.Max(-dealtDamage, 0);
            user.attackMultStat = 100;

            // Hit target
            var newHealth = target.health - Mathf.Max(dealtDamage, 0);
            target.health = Mathf.Min(Mathf.Max(newHealth, 0), target.initHealth);
        }
        if (damage > 0 && target.isInvulnerable)
        {
            attackMissed = true;
        }
        if (damage < 0)
        {
            var newHealth = target.health - damage;
            target.health = Mathf.Min(Mathf.Max(newHealth, 0), target.initHealth);
        }

        // Add status effects if applicable
        var statusTarget = target;
        if (statusAffectsUser) {
            statusTarget = user;
        }
        else if (statusAffectsBothBench && battleGameBoard.opponent.party.Count >= 1) {
            statusTarget = battleGameBoard.opponent.party[1];
        }
        addStatHelper(statusTarget, "attackStat", attackStat, 6);
        addStatHelper(statusTarget, "defenseStat", defenseStat, 6);
        addStatHelper(statusTarget, "specialStat", specialStat, 6);
        addStatHelper(statusTarget, "evasionStat", evasionStat, 6);
        addStatHelper(statusTarget, "blockStat", blockStat, statusTarget.initHealth);
        addStatHelper(statusTarget, "attackMultStat", attackMultStat);
        if (grantsInvulnerability)
        {
            target.attachedStatus.Add(new StatusEffect(statusTarget, null, "invulnerabilityEffect", new Dictionary<string, string>() {
                { "statType", "invulnerability" },
                { "stackCount", "1" },
                { "turnsLeft", "1" }
            }));
        }
        if (removeEnergyCount > 0)
        {
            var actualEnergyNumberToRemove = Mathf.Min(removeEnergyCount, statusTarget.attachedEnergy.Count);
            for (var i = 0; i < actualEnergyNumberToRemove; i++)
            {
                statusTarget.DiscardEnergy(0);
            }
        }

        // Add energy effects if applicable
        for (var i = 0; i < energyStat; i++)
        {
            addEnergy(statusTarget);
        }

        // DO THE SAME THING FOR THE SECOND PARTY MEMBER
        if (statusAffectsBothBench && battleGameBoard.opponent.party.Count >= 2)
        {
            statusTarget = battleGameBoard.opponent.party[2];
            addStatHelper(statusTarget, "attackStat", attackStat, 6);
            addStatHelper(statusTarget, "defenseStat", defenseStat, 6);
            addStatHelper(statusTarget, "specialStat", specialStat, 6);
            addStatHelper(statusTarget, "evasionStat", evasionStat, 6);
            addStatHelper(statusTarget, "blockStat", blockStat, statusTarget.initHealth);
            addStatHelper(statusTarget, "attackMultStat", attackMultStat);
            if (grantsInvulnerability)
            {
                target.attachedStatus.Add(new StatusEffect(statusTarget, null, "invulnerabilityEffect", new Dictionary<string, string>() {
                    { "statType", "invulnerability" },
                    { "stackCount", "1" },
                    { "turnsLeft", "1" }
                }));
            }

            // Add energy effects if applicable
            for (var i = 0; i < energyStat; i++)
            {
                addEnergy(statusTarget);
            }
        }


        // Heal/dmg user if possible
        if (!attackMissed && userHeal > 0)
        {
            var newHealth = user.health + userHeal;
            user.health = Mathf.Min(user.initHealth, newHealth);
        }

        message = moveDescriptionWithTemplates.Replace("{damage}", Mathf.Max(dealtDamage, 0).ToString())
            .Replace("{userHeal}", Mathf.Max(userHeal, 0).ToString())
            .Replace("{userName}", actingPokemon.pokemonName)
            .Replace("{attack}", attackStat.ToString())
            .Replace("{special}", specialStat.ToString())
            .Replace("{defense}", defenseStat.ToString())
            .Replace("{evasion}", evasionStat.ToString())
            .Replace("{block}", blockStat.ToString());
        return attackMissed;
    }

    /// <summary>
    /// Help set the stats
    /// </summary>
    /// <param name="target"></param>
    /// <param name="statName"></param>
    /// <param name="statValue"></param>
    /// <param name="maxStack">set to -1 to have no max stack</param>
    private void addStatHelper(Pokemon target, string statName, int statValue, int maxStack = -1)
    {
        if (statValue > 0)
        {
            target.attachedStatus.Add(new StatusEffect(target, null, statName + "Effect", new Dictionary<string, string>() {
                { "statType", statName.ToString() },
                { "stackCount", statValue.ToString() },
                { "maxStack", maxStack.ToString() },
                { "turnsLeft", turn.ToString() }
            }));
        }
    }

    private void addEnergy(Pokemon target)
    {
        // Don't add energy if we are already maxed
        if (target.maxNumberOfAttachedEnergy == target.attachedEnergy.Count) return;

        var energyParent = target.gameObject;
        var energy = Instantiate(battleGameBoard.opponent.normalEnergyPrefab, energyParent.transform);
        energy.transform.position = battleGameBoard.energyDeckLocation.transform.position;
        target.attachedEnergy.Add(energy.GetComponent<Energy>());
    }

    /// <summary>
    /// Returns the target of the move
    /// </summary>
    /// <returns></returns>
    private List<Pokemon> getTarget()
    {
        var bench = battleGameBoard.opponent.party.Where(p => !p.isFainted && p != battleGameBoard.activePokemon).ToList();
        var team = battleGameBoard.opponent.party.Where(p => !p.isFainted).ToList();

        var opponentBench = battleGameBoard.player.party.Where(p => !p.isFainted && p != battleGameBoard.activePokemon).ToList();
        var opponentTeam = battleGameBoard.player.party.Where(p => !p.isFainted).ToList();
        if (overrideTarget)
        {
            return new List<Pokemon>() { overrideTarget };
        }
        else if (targetType == MoveTargetType.ActiveOpponent)
        {
            return new List<Pokemon>() { battleGameBoard.activePokemon };
        }
        else if (targetType == MoveTargetType.Bench && opponentBench.Count > 0)
        {
            var randomTarget = UnityEngine.Random.Range(0, bench.Count - 1);
            return new List<Pokemon>() { bench[randomTarget] };
        }
        else if (targetType == MoveTargetType.Team)
        {
            var randomTarget = UnityEngine.Random.Range(0, team.Count - 1);
            return new List<Pokemon>() { team[randomTarget] };
        }
        else if (targetType == MoveTargetType.BenchOpponent && opponentBench.Count > 0)
        {
            var randomTarget = UnityEngine.Random.Range(0, opponentBench.Count - 1);
            return new List<Pokemon>() { opponentBench[randomTarget] };
        }
        else if (targetType == MoveTargetType.AnyOpponent)
        {
            var randomTarget = UnityEngine.Random.Range(0, opponentTeam.Count - 1);
            return new List<Pokemon>() { opponentTeam[randomTarget] };
        }
        else if (targetType == MoveTargetType.Self)
        {
            return new List<Pokemon>() { battleGameBoard.opponentActivePokemon };
        }
        else if (targetType == MoveTargetType.TeamAll)
        {
            return team;
        }
        else if (targetType == MoveTargetType.BenchAll)
        {
            return bench;
        }
        else if (targetType == MoveTargetType.AnyOpponentAll)
        {
            return opponentTeam;
        }
        else if (targetType == MoveTargetType.BenchOpponentAll)
        {
            return opponentBench;
        }
        else if (targetType == MoveTargetType.AnyPokemonAll)
        {
            return team.Union(opponentTeam).ToList();
        }

        return new List<Pokemon>() { battleGameBoard.activePokemon };
    }
}
