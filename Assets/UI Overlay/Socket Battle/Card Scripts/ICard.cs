using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICard : MonoBehaviour
{
    public virtual void onBattleStart(Card card, BattleGameBoard battleGameBoard) { }

    public virtual void onDraw(Card card, BattleGameBoard battleGameBoard, Pokemon activePokemon) { }

    public virtual void onOpponentDraw(Card card, BattleGameBoard battleGameBoard, Pokemon opponentActivePokemon) { }

    public virtual bool overridesPlayFunc() { return false; }

    public virtual void play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon target) { }

    public virtual void onDiscard(Card card, BattleGameBoard battleGameBoard, bool wasPlayed) { }

    public virtual void onTurnEnd(Card card, BattleGameBoard battleGameBoard) { }

    public virtual void onOpponentTurnEnd(Card card, BattleGameBoard battleGameBoard) { }

    public virtual void onBattleEnd(Card card, BattleGameBoard battleGameBoard) { }

    public virtual void onCardPlayed(Card card, BattleGameBoard battleGameBoard, Card move, Pokemon user, Pokemon target) { }
}
