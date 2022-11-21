using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Charges the card on KO
/// </summary>
public class CardKOEventBuff : ICard
{
    /// <summary>
    /// Base damage
    /// </summary>
    public int baseDamage = 0;

    /// <summary>
    /// Number of damage to add for each stack
    /// </summary>
    public int damagePerKO = 0;

    /// <summary>
    /// The max number of stacks to add
    /// </summary>
    public int maxStacks = 1;

    private int numberOfKOs = 0;

    private int numberAliveOpponentsLastTurn = 0;

    public override bool overridesPlayFunc() { return true; }

    public override List<bool> play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets)
    {
        var damageAndBonus = baseDamage + damagePerKO * numberOfKOs;

        var attackMissed = targets.Select(t => damageTarget(damageAndBonus, card, user, t)).ToList();

        return attackMissed;
    }

    public override void onDraw(Card card, BattleGameBoard battleGameBoard, Pokemon activePokemon)
    {
        // Reset count when drawn
        if (card == GetComponent<Card>())
        {
            numberAliveOpponentsLastTurn = battleGameBoard.opponent.party.Where(p => !p.isFainted).Count();
            numberOfKOs = 0;
        }
    }

    public override void onTurnEnd(Card card, BattleGameBoard battleGameBoard)
    {
        // Update the KO count
        var newAliveCount = battleGameBoard.opponent.party.Where(p => !p.isFainted).Count();
        if (numberAliveOpponentsLastTurn != newAliveCount)
        {
            card.cardAnimator.SetTrigger("onFlip");
            numberOfKOs = numberAliveOpponentsLastTurn - newAliveCount;
            numberAliveOpponentsLastTurn = newAliveCount;
        }
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
