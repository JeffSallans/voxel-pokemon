using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Charges the card if a condition is met
/// </summary>
public class CardStatConditionBuff : ICard
{
    /// <summary>
    /// The attack value to trigger this flip
    /// </summary>
    public int attackCost = 0;

    /// <summary>
    /// The defense value to trigger this flip
    /// </summary>
    public int defenseCost = 0;

    /// <summary>
    /// The special value to trigger this flip
    /// </summary>
    public int specialCost = 0;

    /// <summary>
    /// True when the card cost should be spent instead of just measured as a requirement
    /// </summary>
    public bool spendCost = true;

    /// <summary>
    /// Number of damage to add for each stack
    /// </summary>
    public int damageBuff = 0;

    /// <summary>
    /// The attack stat to add for each stack
    /// </summary>
    public int attackStatBuff = 0;

    /// <summary>
    /// The defense stat to add for each stack
    /// </summary>
    public int defenseStatBuff = 0;

    /// <summary>
    /// Number of userHeal to add for each stack
    /// </summary>
    public int userHealBuff = 0;

    /// <summary>
    /// Set to true if the trap condition should be set
    /// </summary>
    public bool setTrapBuff = false;

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
    /// Set base stats
    /// </summary>
    /// <param name="card"></param>
    /// <param name="battleGameBoard"></param>
    public override void onBattleStart(Card card, BattleGameBoard battleGameBoard)
    {
        base.onBattleStart(card, battleGameBoard);

        // Set base stat
        baseDamage = card.damage;
        baseAttack = card.attackStat;
        baseDefense = card.defenseStat;
        baseUserHeal = card.userHeal;
    }

    public override void onAnyDraw(Card card, Card drawnCard, BattleGameBoard battleGameBoard, Pokemon activePokemon)
    {
        base.onAnyDraw(card, drawnCard, battleGameBoard, activePokemon);

        checkStatusConditionChange(card, battleGameBoard);
    }

    public override List<bool> play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets)
    {
        if (spendCost && card.currentStacks > 0)
        {
            user.attackStat -= attackCost;
            user.defenseStat -= defenseCost;
            user.specialStat -= specialCost;
        }

        return base.play(card, battleGameBoard, user, selectedTarget, targets);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="card"></param>
    /// <param name="battleGameBoard"></param>
    /// <param name="move"></param>
    /// <param name="user"></param>
    /// <param name="target"></param>
    public override void onAnyCardPlayed(Card card, BattleGameBoard battleGameBoard, Card move, Pokemon user, Pokemon target)
    {
        base.onAnyCardPlayed(card, battleGameBoard, move, user, target);

        checkStatusConditionChange(card, battleGameBoard);
    }

    public override void onThisCardPlayed(Card thisCard, BattleGameBoard battleGameBoard, Pokemon user, Pokemon target)
    {
        base.onThisCardPlayed(thisCard, battleGameBoard, user, target);

        thisCard.damage = baseDamage;
        thisCard.attackStat = baseAttack;
        thisCard.defenseStat = baseDefense;
        thisCard.userHeal = baseUserHeal;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="card"></param>
    /// <param name="battleGameBoard"></param>
    public override void onOpponentTurnEnd(Card card, BattleGameBoard battleGameBoard)
    {
        base.onOpponentTurnEnd(card, battleGameBoard);

        checkStatusConditionChange(card, battleGameBoard);
    }


    private void checkStatusConditionChange(Card card, BattleGameBoard battleGameBoard)
    {
        var attackConditionMet = attackCost > 0 && card.owner && card.owner.attackStat >= attackCost;
        var defenseConditionMet = defenseCost > 0 && card.owner && card.owner.defenseStat >= defenseCost;
        var specialConditionMet = specialCost > 0 && card.owner && card.owner.specialStat >= specialCost;
        var statusConditionMet = attackConditionMet || defenseConditionMet || specialConditionMet;

        if (card.currentStacks < card.maxStacks && statusConditionMet)
        {
            card.cardAnimator.SetTrigger("onFlip");
            card.currentStacks++;
            card.damage += damageBuff;
            card.attackStat += attackStatBuff;
            card.defenseStat += defenseStatBuff;
            card.userHeal += userHealBuff;
            card.applyTrap = setTrapBuff ? true : card.applyTrap;
        }

        if (card.currentStacks > 0 && !statusConditionMet)
        {
            card.cardAnimator.SetTrigger("onFlip");
            card.currentStacks--;
            card.damage -= damageBuff;
            card.attackStat -= attackStatBuff;
            card.defenseStat -= defenseStatBuff;
            card.userHeal -= userHealBuff;
            card.applyTrap = setTrapBuff ? false : card.applyTrap;
        }
    }
}
