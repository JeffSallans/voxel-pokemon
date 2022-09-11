using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Discard all cards from your hand
/// </summary>
public class CardFlamethrower : ICard
{
    public override bool overridesPlayFunc() { return false; }

    public override void play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon target)
    {
        // Skip discard if this is the only card in the hand
        if (user.hand.Count < 1) return;

        // Discard each card in your hand
        for (var i = user.hand.Count - 1; i >= 0; i--)
        {
            if (user.hand[i] != card)
            {
                battleGameBoard.cardDiscard(user.hand[i], user.hand[i].owner, false);
            }
        }
    }
}

