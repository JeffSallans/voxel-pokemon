using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFlamethrower : ICard
{
    public override bool overridesPlayFunc() { return false; }

    public override void play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon target)
    {
        // Skip discard if this is the only card in the hand
        if (user.hand.Count < 1) return;

        // Discard another card in our hand
        var moveIndex = user.hand.IndexOf(card);
        var discardTargetIndex = Random.Range(0, user.hand.Count - 1);

        while (moveIndex == discardTargetIndex && user.hand[discardTargetIndex] == null)
        {
            discardTargetIndex = Random.Range(0, user.hand.Count - 1);
        }

        battleGameBoard.cardDiscard(user.hand[discardTargetIndex], user.hand[discardTargetIndex].owner, false);
    }
}

