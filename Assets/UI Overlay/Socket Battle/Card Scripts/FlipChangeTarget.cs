using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class FlipChangeTarget : IFlipButton
{
    /// <summary>
    /// The new possible targets of this card. Self, Bench, Team, ActiveOpponent, AnyOpponent, BenchOpponent, AnyPokemon
    /// </summary>
    public CardTargetType newTargetType = CardTargetType.AnyOpponent;

    private CardTargetType initialTargetType = CardTargetType.ActiveOpponent;

    public override void onFlipEvent(Card card, BattleGameBoard battleGameBoard)
    {
        base.onFlipEvent(card, battleGameBoard);

        initialTargetType = card.targetType;
        card.targetType = newTargetType;
    }

    public override void onUnflipEvent(Card card, BattleGameBoard battleGameBoard)
    {
        base.onUnflipEvent(card, battleGameBoard);

        card.targetType = initialTargetType;
    }
}
