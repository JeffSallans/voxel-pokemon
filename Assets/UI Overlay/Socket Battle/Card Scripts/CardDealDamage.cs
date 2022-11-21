using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Card;

/// <summary>
/// Deal damage separate from common card damage
/// </summary>
public class CardDealDamage : ICard
{
    /// <summary>
    /// Set to true if the override should also use the common card effect
    /// </summary>
    public bool useCommonCardEffect = false;

    /// <summary>
    /// Damage to be dealt
    /// </summary>
    public int damage = 0;

    /// <summary>
    /// Target to remove energies from, used to be more flexible if we want to remove energies from the user after an attack
    /// </summary>
    public CardTargetType targetType = CardTargetType.ActiveOpponent;

    public override bool overridesPlayFunc() { return !useCommonCardEffect; }

    public override List<bool> play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets)
    {
        var dmgTargets = card.getTarget(targetType, selectedTarget);

        dmgTargets.ForEach(t => damageTarget(card, user, t));

        return targets.Select(t => false).ToList();
    }

    /// <summary>
    /// deals direct damage to target
    /// </summary>
    /// <param name="target"></param>
    private void damageTarget(Card card, Pokemon user, Pokemon target)
    {
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
            //attackMissed = true;
        }
    }
}
