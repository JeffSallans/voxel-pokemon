using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Card;

/// <summary>
/// Add damage bonus based on the target's health
/// </summary>
public class CardAgainstHealthyBuff : ICard
{
    /// <summary>
    /// Damage to be dealt if against a healthy opponent
    /// </summary>
    public int bonusDamage = 0;

    /// <summary>
    /// 50 is 50% of the targets health
    /// </summary>
    public int percentThreashold = 50;

    /// <summary>
    /// True means dmg is applied if above threshold otherwise dmg applied if below
    /// </summary>
    public bool aboveThreshold = true;

    public override bool overridesPlayFunc() { return true; }

    public override List<bool> play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets)
    {

        var attackMissed = targets.Select(t =>
        {
            // Add dmg bonus if condition is met
            var totalDamage = card.damage;
            if (aboveThreshold && t.health/t.initHealth > percentThreashold)
            {
                totalDamage += bonusDamage;
            }
            else if (!aboveThreshold && t.health / t.initHealth < percentThreashold)
            {
                totalDamage += bonusDamage;
            }
            // Do damage normally
            return damageTarget(card, user, t, totalDamage);
        }).ToList();

        return attackMissed;
    }

    /// <summary>
    /// deals damage to target using bonus damage
    /// </summary>
    /// <param name="target"></param>
    private bool damageTarget(Card card, Pokemon user, Pokemon target, int damage)
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