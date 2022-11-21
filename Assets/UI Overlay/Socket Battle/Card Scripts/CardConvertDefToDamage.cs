using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Card;

/// <summary>
/// Use defense stat to boost damage
/// </summary>
public class CardConvertDefToDamage : ICard
{
    /// <summary>
    /// Damage to be dealt
    /// </summary>
    public int damage = 0;

    /// <summary>
    /// The defense to add per stack
    /// </summary>
    public int damageBonusPerDefStack = 30;

    public override bool overridesPlayFunc() { return true; }

    public override List<bool> play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets)
    {
        var defStat = user.attachedStatus.Find(s => s.statusEffectName == "defenseStat");

        var defDamageBoost = defStat.stackCount * damageBonusPerDefStack;
        var damageAndBonus = damage + defDamageBoost;

        var attackMissed = targets.Select(t => damageTarget(damageAndBonus, card, user, t)).ToList();

        // Remove defense
        user.attachedStatus.Remove(defStat);

        return attackMissed;
    }

    /// <summary>
    /// deals damage to target using bonus damage
    /// </summary>
    /// <param name="target"></param>
    private bool damageTarget(int damageAndBonus, Card card, Pokemon user, Pokemon target)
    {
        var attackMissed = false;

        // Deal Damage if applicable
        if (damageAndBonus > 0 && !target.isInvulnerable)
        {
            // Determine damage
            var dealtDamage = Mathf.RoundToInt(damageAndBonus * user.attackMultStat / 100f * TypeChart.getEffectiveness(card, target))
                + (user.attackStat - target.defenseStat) * 10
                - target.blockStat;
            target.blockStat = Mathf.Max(-dealtDamage, 0);
            user.attackMultStat = 100;

            // Hit target
            var newHealth = target.health - Mathf.Max(dealtDamage, 0);
            target.health = Mathf.Min(Mathf.Max(newHealth, 0), target.initHealth);
        }
        if (damageAndBonus > 0 && target.isInvulnerable)
        {
            attackMissed = true;
        }

        return attackMissed;
    }
}

