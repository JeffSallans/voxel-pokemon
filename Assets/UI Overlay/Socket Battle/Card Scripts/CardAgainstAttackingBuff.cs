using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Card;

/// <summary>
/// Add damage bonus
/// </summary>
public class CardAgainstAttackingBuff : ICard
{
    /// <summary>
    /// Damage to be dealt if against an attacking opponent
    /// </summary>
    public int damage = 0;

    public override bool overridesPlayFunc() { return true; }

    public override List<bool> play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets)
    {

        // Miss if the selected target is not attacking
        if (battleGameBoard.opponent.opponentStrategyBot.nextOpponentMove.card.owner != selectedTarget)
        {
            return targets.Select(t => true).ToList();
        }

        // Otherwise do damage normally
        var attackMissed = targets.Select(t =>
        {
            return damageTarget(card, user, t);
        }).ToList();

        return attackMissed;
    }

    /// <summary>
    /// deals damage to target using bonus damage
    /// </summary>
    /// <param name="target"></param>
    private bool damageTarget(Card card, Pokemon user, Pokemon target)
    {
        var attackMissed = false;

        // Deal Damage if applicable
        if (damage > 0 && !target.isInvulnerable)
        {
            // Determine damage
            var dealtDamage = Mathf.RoundToInt(damage * user.attackMultStat / 100f * TypeChart.getEffectiveness(card, target))
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

        return attackMissed;
    }
}