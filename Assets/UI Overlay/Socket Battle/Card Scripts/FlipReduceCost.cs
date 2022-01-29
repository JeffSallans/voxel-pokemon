using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FlipReduceCost : IFlipButton
{
    /// <summary>
    /// The energy reference that is removed when reducing cost
    /// </summary>
    private Energy removedEnergy;

    public override void onFlipEvent(Card card, BattleGameBoard battleGameBoard)
    {
        base.onFlipEvent(card, battleGameBoard);
        removedEnergy = card.cost.Last();
        card.cost.Remove(removedEnergy);
        removedEnergy.gameObject.SetActive(false);
    }

    public override void onUnflipEvent(Card card, BattleGameBoard battleGameBoard)
    {
        base.onUnflipEvent(card, battleGameBoard);
        removedEnergy.gameObject.SetActive(true);
        card.cost.Add(removedEnergy);
        removedEnergy = null;
    }
}
