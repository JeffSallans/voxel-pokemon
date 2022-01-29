using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IFlipButton : MonoBehaviour
{
    public virtual string getFlipButtonText() { return "";  }

    public virtual bool isCardFlipped() { return false; }

    public virtual bool isFlipButtonEnabled(Card card, BattleGameBoard battleGameBoard) { return false; }

    public virtual void onFlipButtonPress(Card card, BattleGameBoard battleGameBoard) { }

    public virtual void onFlipEvent(Card card, BattleGameBoard battleGameBoard) { }

    public virtual void onUnflipEvent(Card card, BattleGameBoard battleGameBoard) { }

    public virtual void onPlay(Card card, BattleGameBoard battleGameBoard, Pokemon targetPokemon) { }

}
