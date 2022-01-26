using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipButtonStat : IFlipButton
{
    public int attackCost;
    public int defenseCost;
    public int specialCost;
    public int evasionCost;

    public int damageIncrease;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool isFlipButtonEnabled(Card card, BattleGameBoard battleGameBoard)
    {
        if (attackCost > 0 && battleGameBoard?.activePokemon && battleGameBoard?.activePokemon.attackStat < attackCost) {
            return false;
        }
        if (defenseCost > 0 && battleGameBoard?.activePokemon && battleGameBoard?.activePokemon.defenseStat < defenseCost)
        {
            return false;
        }
        if (specialCost > 0 && battleGameBoard?.activePokemon && battleGameBoard?.activePokemon.specialStat < specialCost)
        {
            return false;
        }
        if (evasionCost > 0 && battleGameBoard?.activePokemon && battleGameBoard?.activePokemon.evasionStat < evasionCost)
        {
            return false;
        }

        return true;
    }

    public override void onFlipEvent(Card card, BattleGameBoard battleGameBoard)
    {
        card.initDamage += damageIncrease;
    }

    public override void onFlipButtonHoverEnter(Card card, BattleGameBoard battleGameBoard)
    {
        // Do Nothing
    }

    public override void onFlipButtonHoverExit(Card card, BattleGameBoard battleGameBoard)
    {
        // Do Nothing
    }

    public override void onFlipButtonPress(Card card, BattleGameBoard battleGameBoard)
    {
        battleGameBoard.activePokemon.attackStat -= attackCost;
        battleGameBoard.activePokemon.defenseStat -= defenseCost;
        battleGameBoard.activePokemon.specialStat -= specialCost;
        battleGameBoard.activePokemon.evasionStat -= evasionCost;
    }

    public override void onUnflipEvent(Card card, BattleGameBoard battleGameBoard)
    {
        card.initDamage -= damageIncrease;
    }

    public override void onPlay(Card card, BattleGameBoard battleGameBoard, Pokemon targetPokemon)
    {
        // Do Nothing
    }
}
