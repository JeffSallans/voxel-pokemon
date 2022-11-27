using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Discard all cards from your hand
/// </summary>
public class CardFlamethrower : ICard
{
    public override bool overridesPlayFunc() { return false; }

    public override List<bool> play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets)
    {
        var userHand = user.hand;
        if (BattleGameBoardForge.IsAForgeGame())
        {
            userHand = battleGameBoard.hand;
        }

        // Skip discard if this is the only card in the hand
        if (user.hand.Count < 1) return targets.Select(t => false).ToList();

        // Discard each card in your hand
        for (var i = user.hand.Count - 1; i >= 0; i--)
        {
            if (user.hand[i] != card)
            {
                battleGameBoard.cardDiscard(user.hand[i], user.hand[i].owner, false);
            }
        }

        return targets.Select(t => false).ToList();
    }
}

