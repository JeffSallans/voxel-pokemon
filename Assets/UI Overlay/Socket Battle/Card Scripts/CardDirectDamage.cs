using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Card;

/// <summary>
/// Deal direct damage to target ignoring, typing, stats, and multipiers
/// </summary>
public class CardDirectDamage : ICard
{
    /// <summary>
    /// Set to true if the override should also use the common card effect
    /// </summary>
    public bool useCommonCardEffect = false;

    /// <summary>
    /// Damage to be dealt
    /// </summary>
    public int damage = 0;

    public override bool overridesPlayFunc() { return !useCommonCardEffect; }

    public override List<bool> play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets)
    {
        var attackMissed = targets.Select(t => damageTarget(t)).ToList();

        return attackMissed;
    }

    /// <summary>
    /// deals direct damage to target
    /// </summary>
    /// <param name="target"></param>
    private bool damageTarget(Pokemon target)
    {
        var attackMissed = false;

        // Deal Damage if applicable
        if (damage > 0 && !target.isInvulnerable)
        {
            // Determine damage
            var dealtDamage = damage;

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
