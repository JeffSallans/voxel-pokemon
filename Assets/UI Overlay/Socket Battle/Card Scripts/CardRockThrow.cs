using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Card to add damage based on cards played earlier this turn
/// </summary>
public class CardRockThrow : ICard
{
    /// <summary>
    /// Number of damage to add for each stack
    /// </summary>
    public int damagePerCardPlayed = 0;

    /// <summary>
    /// True if the count should reset when turn end is pressed
    /// </summary>
    public bool countResetsAtEndOfTurn = false;

    /// <summary>
    /// The max number of stacks to add
    /// </summary>
    public int maxStacks = 4;
    
    private int numberOfCardsPlayed = 0;

    public override void onBattleStart(Card card, BattleGameBoard battleGameBoard)
    {
        numberOfCardsPlayed = 0;
    }

    public override void onCardPlayed(Card card, BattleGameBoard battleGameBoard, Card move, Pokemon user, Pokemon target)
    {
        // Increment damage if the card is in your hand
        if (isCardInHand(card, battleGameBoard) && card != move && numberOfCardsPlayed < maxStacks)
        {
            card.cardAnimator.SetTrigger("onFlip");
            numberOfCardsPlayed++;
            card.initDamage += damagePerCardPlayed;
        }

        // Reset damage if card is played
        if (card == move)
        {
            resetCard(card);
        }
    }

    public override void onDraw(Card card, BattleGameBoard battleGameBoard, Pokemon activePokemon)
    {
        base.onDraw(card, battleGameBoard, activePokemon);

        // Reset count when drawn
        if (card == GetComponent<Card>())
        {
            numberOfCardsPlayed = 0;
        }
    }

    public override void onTurnEnd(Card card, BattleGameBoard battleGameBoard) {
        if (countResetsAtEndOfTurn)
        {
            resetCard(card);
        }
    }

    private void resetCard(Card card)
    {
        card.cardAnimator.SetTrigger("onFlip");
        card.initDamage -= numberOfCardsPlayed * damagePerCardPlayed;
        numberOfCardsPlayed = 0;
    }
}
