using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentMove : IOpponentMove
{
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
        var target = (overrideTarget != null) ? overrideTarget : battleGameBoard.activePokemon;

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
        addStatHelper(target, "attackStat", attackStat);
        addStatHelper(target, "defenseStat", defenseStat);
        addStatHelper(target, "specialStat", specialStat);
        addStatHelper(target, "evasionStat", evasionStat);
        addStatHelper(target, "blockStat", blockStat);
        addStatHelper(target, "attackMultStat", attackMultStat);
        if (grantsInvulnerability)
        {
            target.attachedStatus.Add(new StatusEffect(target, null, "invulnerabilityEffect", new Dictionary<string, string>() {
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

        message = moveDescriptionWithTemplates.Replace("{damage}", Mathf.Max(dealtDamage, 0).ToString());
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
}
