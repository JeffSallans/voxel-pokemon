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
        if (usesThisOverrideInstance(card))
        {
            baseTargetType = card.targetType;
        }
    }

    public override void onDraw(Card card, BattleGameBoard battleGameBoard, Pokemon activePokemon)
    {
        base.onDraw(card, battleGameBoard, activePokemon);

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
    public override void onCardPlayed(Card card, BattleGameBoard battleGameBoard, Card move, Pokemon user, Pokemon target)
    {
        base.onCardPlayed(card, battleGameBoard, move, user, target);

        if (move == card)
        {
            card.targetType = baseTargetType;
        }
        else
        {
            checkStatusConditionChange(card, battleGameBoard);
        }
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
        var cardCanBeTriggered = usesThisOverrideInstance(card);

        var attackConditionMet = attackCost > 0 && card.owner && card.owner.attackStat >= attackCost;
        var defenseConditionMet = defenseCost > 0 && card.owner && card.owner.defenseStat >= defenseCost;
        var specialConditionMet = specialCost > 0 && card.owner && card.owner.specialStat >= specialCost;
        var statusConditionMet = attackConditionMet || defenseConditionMet || specialConditionMet;

        if (cardCanBeTriggered && card.currentStacks < card.maxStacks && statusConditionMet)
        {
            card.cardAnimator.SetTrigger("onFlip");
            card.currentStacks++;
            card.targetType = newTargetType;
        }

        if (cardCanBeTriggered && card.currentStacks > 0 && !statusConditionMet)
        {
            card.cardAnimator.SetTrigger("onFlip");
            card.currentStacks--;
            card.targetType = baseTargetType;
        }
    }
}
