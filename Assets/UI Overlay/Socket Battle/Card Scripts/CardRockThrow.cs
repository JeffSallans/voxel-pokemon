using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Card to add damage based on cards played earlier this turn
/// </summary>
public class CardRockThrow : ICard
{
    public int damagePerCardPlayed = 0;
    private int numberOfCardsPlayed = 0;

    public override void onBattleStart(Card card, BattleGameBoard battleGameBoard)
    {
        numberOfCardsPlayed = 0;
    }

    public override void onCardPlayed(Card card, BattleGameBoard battleGameBoard, Card move, Pokemon user, Pokemon target)
    {
        if (card != move)
        {
            numberOfCardsPlayed++;
            card.initDamage += damagePerCardPlayed;
        }
    }

    public override void onTurnEnd(Card card, BattleGameBoard battleGameBoard) {
        card.initDamage -= numberOfCardsPlayed * damagePerCardPlayed;
        numberOfCardsPlayed = 0;
    }
}
