using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Charges the card for each card in your hand
/// </summary>
public class CardHandSizeBuff : ICard
{
    /// <summary>
    /// Number of damage to add for each stack
    /// </summary>
    public int damagePer = 0;

    /// <summary>
    /// The attack stat to add for each stack
    /// </summary>
    public int attackStatPer = 0;

    /// <summary>
    /// The defense stat to add for each stack
    /// </summary>
    public int defenseStatPer = 0;

    /// <summary>
    /// Number of userHeal to add for each stack
    /// </summary>
    public int userHealPer = 0;

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

    private Card cardReference = null;

    public override bool overridesPlayFunc() { return false; }

    public override void onBattleStart(Card card, BattleGameBoard battleGameBoard)
    {
        base.onBattleStart(card, battleGameBoard);

        if (usesThisOverrideInstance(card))
        {
            cardReference = card;

            baseDamage = card.damage;
            baseAttack = card.attackStat;
            baseDefense = card.defenseStat;
            baseUserHeal = card.userHeal;

            currentStacks = 0;
        }
    }

    /// <summary>
    /// Revert to the base stats on discard
    /// </summary>
    /// <param name="card"></param>
    /// <param name="battleGameBoard"></param>
    /// <param name="wasPlayed"></param>
    public override void onDiscard(Card card, BattleGameBoard battleGameBoard, bool wasPlayed)
    {
        base.onDiscard(card, battleGameBoard, wasPlayed);

        card.cardAnimator.SetTrigger("onFlip");

        cardReference.damage -= damagePer;
        cardReference.attackStat -= attackStatPer;
        cardReference.defenseStat -= defenseStatPer;
        cardReference.userHeal -= userHealPer;

        currentStacks--;
    }

    /// <summary>
    /// Track the base stats on draw
    /// </summary>
    /// <param name="card"></param>
    /// <param name="battleGameBoard"></param>
    /// <param name="activePokemon"></param>
    public override void onDraw(Card card, BattleGameBoard battleGameBoard, Pokemon activePokemon)
    {
        base.onDraw(card, battleGameBoard, activePokemon);

        if (currentStacks < maxStacks)
        {
            card.cardAnimator.SetTrigger("onFlip");

            cardReference.damage += damagePer;
            cardReference.attackStat += attackStatPer;
            cardReference.defenseStat += defenseStatPer;
            cardReference.userHeal += userHealPer;

            currentStacks++;
        }
    }
}
