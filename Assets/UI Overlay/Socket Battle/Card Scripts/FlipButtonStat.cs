using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipButtonStat : IFlipButton
{

    public int damageIncrease;
    public int blockIncrease;
    public int multiplierIncrease;
    public int userHealIncrease;

    public override void onFlipEvent(Card card, BattleGameBoard battleGameBoard)
    {
        base.onFlipEvent(card, battleGameBoard);
        card.initDamage += damageIncrease;
        card.blockStat += blockIncrease;
        card.attackMultStat += multiplierIncrease;
        card.userHeal += userHealIncrease;
    }

    public override void onUnflipEvent(Card card, BattleGameBoard battleGameBoard)
    {
        base.onUnflipEvent(card, battleGameBoard);
        card.initDamage -= damageIncrease;
        card.blockStat -= blockIncrease;
        card.attackMultStat -= multiplierIncrease;
        card.userHeal -= userHealIncrease;
    }
}
