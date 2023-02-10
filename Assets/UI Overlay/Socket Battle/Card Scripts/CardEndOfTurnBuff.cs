using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Charges the card at the end of the turn
/// </summary>
public class CardEndOfTurnBuff : ICard
{
    /// <summary>
    /// Number of damage to add for each stack
    /// </summary>
    public int damagePerTurn = 0;

    /// <summary>
    /// The attack stat to add for each stack
    /// </summary>
    public int attackStatPerTurn = 0;

    /// <summary>
    /// The defense stat to add for each stack
    /// </summary>
    public int defenseStatPerTurn = 0;

    /// <summary>
    /// Number of userHeal to add for each stack
    /// </summary>
    public int userHealPerTurn = 0;

    /// <summary>
    /// The max number of stacks to add
    /// </summary>
    public int maxStacks = 1;

    /// <summary>
    /// Number of times a stack was applied
    /// </summary>
    private int currentStacks = 0;

    /// <summary>
    /// Base damage of card
    /// </summary>
    private int baseDamage = 0;

    /// <summary>
    /// Base attack of card
    /// </summary>
    private int baseAttack = 0;

    /// <summary>
    /// Base defense of card
    /// </summary>
    private int baseDefense = 0;

    /// <summary>
    /// Base userHeal of card
    /// </summary>
    private int baseUserHeal = 0;

    public override bool overridesPlayFunc() { return false; }

    /// <summary>
    /// Revert to the base stats on discard
    /// </summary>
    /// <param name="card"></param>
    /// <param name="battleGameBoard"></param>
    /// <param name="wasPlayed"></param>
    public override void onThisDiscard(Card card, BattleGameBoard battleGameBoard, bool wasPlayed)
    {
        base.onThisDiscard(card, battleGameBoard, wasPlayed);

        card.damage = baseDamage;
        card.attackStat = baseAttack;
        card.defenseStat = baseDefense;
        card.userHeal = baseUserHeal;

        currentStacks = 0;
    }

    /// <summary>
    /// Track the base stats on draw
    /// </summary>
    /// <param name="card"></param>
    /// <param name="battleGameBoard"></param>
    /// <param name="activePokemon"></param>
    public override void onThisDraw(Card card, BattleGameBoard battleGameBoard, Pokemon activePokemon)
    {
        base.onThisDraw(card, battleGameBoard, activePokemon);

        baseDamage = card.damage;
        baseAttack = card.attackStat;
        baseDefense = card.defenseStat;
        baseUserHeal = card.userHeal;

        currentStacks = 0;
    }

    /// <summary>
    /// Increment the stats on turn end
    /// </summary>
    /// <param name="card"></param>
    /// <param name="battleGameBoard"></param>
    public override void onTurnEnd(Card card, BattleGameBoard battleGameBoard)
    {
        if (battleGameBoard.hand.Contains(card) && currentStacks < maxStacks)
        {
            card.cardAnimator.SetTrigger("onFlip");
            currentStacks++;
            card.damage += damagePerTurn;
            card.attackStat += attackStatPerTurn;
            card.defenseStat += defenseStatPerTurn;
            card.userHeal += userHealPerTurn;
        }
    }
}
