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
    public bool addTrapStatus;

    public override void onFlipEvent(Card card, BattleGameBoard battleGameBoard)
    {
        base.onFlipEvent(card, battleGameBoard);
        card.damage += damageIncrease;
        card.blockStat += blockIncrease;
        card.attackMultStat += multiplierIncrease;
        card.userHeal += userHealIncrease;
        if (addTrapStatus) card.applyTrap = true;
    }

    public override void onUnflipEvent(Card card, BattleGameBoard battleGameBoard)
    {
        base.onUnflipEvent(card, battleGameBoard);
        card.damage -= damageIncrease;
        card.blockStat -= blockIncrease;
        card.attackMultStat -= multiplierIncrease;
        card.userHeal -= userHealIncrease;
        if (addTrapStatus) card.applyTrap = false;
    }
}
