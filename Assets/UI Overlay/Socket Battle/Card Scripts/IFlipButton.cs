using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IFlipButton : MonoBehaviour
{
    /// <summary>
    /// Text to remove when flipped
    /// </summary>
    public string flipButtonText;

    public int attackCost;
    public int defenseCost;
    public int specialCost;
    public int evasionCost;

    /// <summary>
    /// True when the card is flipped and effect should take place
    /// </summary>
    protected bool isFlipped;

    // Start is called before the first frame update
    void Start()
    {
        isFlipped = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual string getFlipButtonText()
    {
        if (isFlipped) return "";
        return flipButtonText;
    }

    public virtual bool isCardFlipped()
    {
        return isFlipped;
    }

    public virtual bool isFlipButtonEnabled(Card card, BattleGameBoard battleGameBoard)
    {
        if (isFlipped) return false;

        if (attackCost > 0 && battleGameBoard?.activePokemon && battleGameBoard?.activePokemon.attackStat < attackCost)
        {
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

    public virtual void onFlipEvent(Card card, BattleGameBoard battleGameBoard)
    {
        isFlipped = true;
    }

    public virtual void onUnflipEvent(Card card, BattleGameBoard battleGameBoard)
    {
        isFlipped = false;
    }

    public virtual void onFlipButtonPress(Card card, BattleGameBoard battleGameBoard)
    {
        battleGameBoard.activePokemon.attackStat -= attackCost;
        battleGameBoard.activePokemon.defenseStat -= defenseCost;
        battleGameBoard.activePokemon.specialStat -= specialCost;
        battleGameBoard.activePokemon.evasionStat -= evasionCost;
    }

    public virtual void onPlay(Card card, BattleGameBoard battleGameBoard, Pokemon targetPokemon) { }

}
