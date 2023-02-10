using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Card;

/// <summary>
/// Changes the card target if a condition is met
/// </summary>
public class CardStatConditionTargetChange : ICard
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
    /// The possible targets of this card. Self, Bench, Team, ActiveOpponent, AnyOpponent, BenchOpponent, AnyPokemon,
    /// </summary>
    public CardTargetType newTargetType = CardTargetType.AnyOpponent;

    /// <summary>
    /// Base damage of card
    /// </summary>
    private CardTargetType baseTargetType;

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
        baseTargetType = card.targetType;
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

    public override void onThisCardPlayed(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon target)
    {
        base.onThisCardPlayed(card, battleGameBoard, user, target);

        card.targetType = baseTargetType;
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
            card.targetType = newTargetType;
        }

        if (card.currentStacks > 0 && !statusConditionMet)
        {
            card.cardAnimator.SetTrigger("onFlip");
            card.currentStacks--;
            card.targetType = baseTargetType;
        }
    }
}
