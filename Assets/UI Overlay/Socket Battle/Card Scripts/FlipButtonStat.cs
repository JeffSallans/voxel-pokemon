using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipButtonStat : IFlipButton
{
    /// <summary>
    /// Text to remove when flipped
    /// </summary>
    public string flipButtonText;

    public int attackCost;
    public int defenseCost;
    public int specialCost;
    public int evasionCost;

    public int damageIncrease;
    public int blockIncrease;
    public int multiplierIncrease;

    /// <summary>
    /// True when the card is flipped and effect should take place
    /// </summary>
    private bool isFlipped;

    // Start is called before the first frame update
    void Start()
    {
        isFlipped = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override string getFlipButtonText()
    {
        if (isFlipped) return "";
        return flipButtonText;
    }

    public override bool isCardFlipped()
    {
        return isFlipped;
    }

    public override bool isFlipButtonEnabled(Card card, BattleGameBoard battleGameBoard)
    {
        if (isFlipped) return false;

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
        isFlipped = true;
        card.initDamage += damageIncrease;
        card.blockStat += blockIncrease;
        card.attackMultStat += multiplierIncrease;
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
        isFlipped = false;
        card.initDamage -= damageIncrease;
        card.blockStat -= blockIncrease;
        card.attackMultStat -= multiplierIncrease;
    }
}
