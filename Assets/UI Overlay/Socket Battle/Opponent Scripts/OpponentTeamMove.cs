using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// DEPRECATED: Use the team target with OpponentMove
/// </summary>
public class OpponentTeamMove : IOpponentMove
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

    public override List<string> playMove()
    {
        var moveMessage = "";
        var teamPokemon = battleGameBoard.opponent.party.Where(p => !p.isFainted && p != actingPokemon).ToList();

        // Pay cost
        actingPokemon.attackStat -= attackCost;
        actingPokemon.defenseStat -= defenseCost;
        actingPokemon.specialStat -= specialCost;
        actingPokemon.evasionStat -= evasionCost;

        // Play card on self
        commonCardPlay(actingPokemon, actingPokemon, out moveMessage);
        if (userAnimationType != "") actingPokemon.modelAnimator.SetTrigger(userAnimationType);

        // Play card on team
        teamPokemon.ForEach(t =>
        {
            var tempMessage = "";
            commonCardPlay(actingPokemon, t, out tempMessage);
            if (targetAnimationType != "") t.modelAnimator.SetTrigger(targetAnimationType);
            if (targetAnimationType2 != "") t.modelAnimator.SetTrigger(targetAnimationType2);
        });

        return new List<string> { moveMessage + " to the team" };
    }

    /// <summary>
    /// The same as OpponentMove
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    /// <param name="message"></param>
    /// <returns></returns>
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
            target.health = Mathf.Max(newHealth, 0);
        }
        if (damage > 0 && target.isInvulnerable)
        {
            attackMissed = true;
        }

        // Add status effects if applicable
        addStatHelper(target, "attackStat", attackStat, 6);
        addStatHelper(target, "defenseStat", defenseStat, 6);
        addStatHelper(target, "specialStat", specialStat, 6);
        addStatHelper(target, "evasionStat", evasionStat, 6);
        addStatHelper(target, "blockStat", blockStat, target.initHealth);
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

        message = moveDescriptionWithTemplates.Replace("{damage}", Mathf.Max(dealtDamage, 0).ToString())
            .Replace("{userHeal}", Mathf.Max(userHeal, 0).ToString())
            .Replace("{userName}", actingPokemon.pokemonName)
            .Replace("{attack}", attackStat.ToString())
            .Replace("{special}", specialStat.ToString())
            .Replace("{defense}", defenseStat.ToString())
            .Replace("{evasion}", evasionStat.ToString())
            .Replace("{userHeal}", userHeal.ToString())
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
}
