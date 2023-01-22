using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Discard a specific number of cards from your hand
/// </summary>
public class CardDiscard : ICard
{
    /// <summary>
    /// Number of cards to discard from your hand
    /// </summary>
    public int numberToDiscard = 1;
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
        var discardedCount = 0;
        for (var i = user.hand.Count - 1; i >= 0; i--)
        {
            if (user.hand[i] != card)
            {
                battleGameBoard.cardDiscard(user.hand[i], user.hand[i].owner, false);
                discardedCount++;
                if (discardedCount == numberToDiscard) break;
            }
        }

        return targets.Select(t => false).ToList();
    }
}

