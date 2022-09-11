using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipChangeTarget : IFlipButton
{
    /// <summary>
    /// The new possible targets of this card. Self, Bench, Team, ActiveOpponent, AnyOpponent, BenchOpponent, AnyPokemon
    /// </summary>
    public string newTargetType = "AnyOpponent";

    private string initialTargetType = "";

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
