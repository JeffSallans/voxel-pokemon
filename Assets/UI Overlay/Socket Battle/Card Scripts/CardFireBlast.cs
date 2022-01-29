using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFireBlast : ICard
{
    public int maxStacks = 3;
    public int damagePerDiscard = 32;
    private int numberOfTimesDiscarded = 0;

    public override void onBattleStart(Card card, BattleGameBoard battleGameBoard)
    {
        numberOfTimesDiscarded = 0;
    }

    public override void onDiscard(Card card, BattleGameBoard battleGameBoard, bool wasPlayed)
    {
        if (!wasPlayed && numberOfTimesDiscarded < maxStacks)
        {
            numberOfTimesDiscarded++;
            card.initDamage += damagePerDiscard;
        }
    }
}
