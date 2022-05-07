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
    public string targetType = "ActiveOpponent";

    public int attackCost;
    public int defenseCost;
    public int specialCost;
    public int evasionCost;

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
    /// How long a StatusEffect will be placed
    /// </summary>
    public int turn = 99;

    /// <summary>
    /// Set to true if the status affects the user instead of the target
    /// </summary>
    public bool statusAffectsUser = false;

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
            var costIsPaid = actingPokemon.attackStat >= attackCost &&
                actingPokemon.defenseStat >= defenseCost &&
                actingPokemon.specialStat >= specialCost &&
                actingPokemon.evasionStat >= evasionCost;
            return costIsPaid;
        }
    }

    public override string playMove()
    {
        var moveMessage = "";
        var target = getTarget();

        if (target.isInvulnerable && damage > 0)
        {
            if (userAnimationType != "") actingPokemon.GetComponent<Animator>().SetTrigger(userAnimationType);
            return actingPokemon.pokemonName + " missed. " + target.pokemonName + " is invulnerable.";
        }

        // Pay cost
        actingPokemon.attackStat -= attackCost;
        actingPokemon.defenseStat -= defenseCost;
        actingPokemon.specialStat -= specialCost;
        actingPokemon.evasionStat -= evasionCost;

        // Determine damage
        commonCardPlay(actingPokemon, target, out moveMessage);

        if (userAnimationType != "") actingPokemon.GetComponent<Animator>().SetTrigger(userAnimationType);
        if (targetAnimationType != "") target.GetComponent<Animator>().SetTrigger(targetAnimationType);
        if (targetAnimationType2 != "") target.GetComponent<Animator>().SetTrigger(targetAnimationType2);

        return moveMessage;
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
            dealtDamage = Mathf.RoundToInt(damage * user.attackMultStat / 100f * TypeChart.getEffectiveness(this, target)) - target.blockStat;
            target.blockStat = Mathf.Max(-dealtDamage, 0);
            user.attackMultStat = 100;

            // Hit target
            var newHealth = target.health - Mathf.Max(dealtDamage, 0);
            target.health = Mathf.Max(newHealth, 0);
        }
        if (damage > 0 && target.isInvulnerable)
        {
            attackMissed = true;
        }

        // Add status effects if applicable
        var statusTarget = (statusAffectsUser) ? user : target;
        addStatHelper(statusTarget, "attackStat", attackStat);
        addStatHelper(statusTarget, "defenseStat", defenseStat);
        addStatHelper(statusTarget, "specialStat", specialStat);
        addStatHelper(statusTarget, "evasionStat", evasionStat);
        addStatHelper(statusTarget, "blockStat", blockStat);
        addStatHelper(statusTarget, "attackMultStat", attackMultStat);
        if (grantsInvulnerability)
        {
            target.attachedStatus.Add(new StatusEffect(statusTarget, null, "invulnerabilityEffect", new Dictionary<string, string>() {
                { "statType", "invulnerability" },
                { "stackCount", "1" },
                { "turnsLeft", "1" }
            }));
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

    private void addStatHelper(Pokemon target, string statName, int statValue)
    {
        if (statValue > 0)
        {
            target.attachedStatus.Add(new StatusEffect(target, null, statName + "Effect", new Dictionary<string, string>() {
                { "statType", statName.ToString() },
                { "stackCount", statValue.ToString() },
                { "turnsLeft", turn.ToString() }
            }));
        }
    }

    /// <summary>
    /// Returns the target of the move
    /// </summary>
    /// <returns></returns>
    private Pokemon getTarget()
    {
        var opponentBench = battleGameBoard.player.party.Where(p => !p.isFainted && p != battleGameBoard.activePokemon).ToList();
        var opponentTeam = battleGameBoard.player.party.Where(p => !p.isFainted).ToList();
        if (overrideTarget)
        {
            return overrideTarget;
        }
        else if (targetType == "ActiveOpponent")
        {
            return battleGameBoard.activePokemon;
        }
        else if (targetType == "BenchOpponent" && opponentBench.Count > 0)
        {
            var randomTarget = UnityEngine.Random.Range(0, opponentBench.Count - 1);
            return opponentBench[randomTarget];
        }
        else if (targetType == "AnyOpponent")
        {
            var randomTarget = UnityEngine.Random.Range(0, opponentTeam.Count - 1);
            return opponentTeam[randomTarget];
        }
        else if (targetType == "Self")
        {
            return battleGameBoard.opponentActivePokemon;
        }

        return battleGameBoard.activePokemon;
    }
}
