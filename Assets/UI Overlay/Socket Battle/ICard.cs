using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICard : MonoBehaviour
{
    public abstract void onDraw(Card card, BattleGameBoard battleGameBoard, Pokemon activePokemon);

    public abstract void onOpponentDraw(Card card, BattleGameBoard battleGameBoard, Pokemon opponentActivePokemon);

    public abstract void play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon target);

    public abstract void onTurnEnd(Card card, BattleGameBoard battleGameBoard);

    public abstract void onOpponentTurnEnd(Card card, BattleGameBoard battleGameBoard);

    public abstract void onBattleEnd(Card card, BattleGameBoard battleGameBoard);
}
