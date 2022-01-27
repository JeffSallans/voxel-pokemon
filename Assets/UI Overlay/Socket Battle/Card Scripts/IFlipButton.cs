using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IFlipButton : MonoBehaviour
{
    public abstract string getFlipButtonText();

    public abstract bool isCardFlipped();

    public abstract bool isFlipButtonEnabled(Card card, BattleGameBoard battleGameBoard);

    public abstract void onFlipButtonHoverEnter(Card card, BattleGameBoard battleGameBoard);

    public abstract void onFlipButtonHoverExit(Card card, BattleGameBoard battleGameBoard);

    public abstract void onFlipButtonPress(Card card, BattleGameBoard battleGameBoard);

    public abstract void onFlipEvent(Card card, BattleGameBoard battleGameBoard);

    public abstract void onUnflipEvent(Card card, BattleGameBoard battleGameBoard);

    public abstract void onPlay(Card card, BattleGameBoard battleGameBoard, Pokemon targetPokemon);

}
