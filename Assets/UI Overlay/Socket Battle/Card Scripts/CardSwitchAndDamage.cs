using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Card switch functionality and damage active opponent
/// </summary>
public class CardSwitchAndDamage : ICard
{
    public override bool overridesPlayFunc() { return true; }

    public override List<bool> play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets) {
        // Switch board roles
        battleGameBoard.switchPokemon(user, selectedTarget);

        var attackMissed = new List<bool>();
        attackMissed.Add(damageTarget(card, user, card.otherActivePokemon));

        return attackMissed;
    }


    /// <summary>
    /// deals direct damage to target
    /// </summary>
    /// <param name="target"></param>
    private bool damageTarget(Card card, Pokemon user, Pokemon target)
    {
        var attackMissed = false;

        // Deal Damage if applicable
        if (card.damage > 0 && !target.isInvulnerable)
        {
            // Determine damage
            var dealtDamage = Mathf.RoundToInt(card.damage * user.attackMultStat / 100f * TypeChart.getEffectiveness(card, target))
                + (user.attackStat - target.defenseStat) * 10
                - target.blockStat;
            target.blockStat = Mathf.Max(-dealtDamage, 0);
            user.attackMultStat = 100;

            // Hit target
            var newHealth = target.health - Mathf.Max(dealtDamage, 0);
            target.health = Mathf.Min(Mathf.Max(newHealth, 0), target.initHealth);
        }
        if (card.damage > 0 && target.isInvulnerable)
        {
            attackMissed = true;
        }

        return attackMissed;
    }
}
