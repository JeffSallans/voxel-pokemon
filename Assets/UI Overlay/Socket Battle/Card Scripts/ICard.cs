using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ICard : MonoBehaviour
{
    public virtual void onBattleStart(Card card, BattleGameBoard battleGameBoard) { }

    public virtual void onDraw(Card card, BattleGameBoard battleGameBoard, Pokemon activePokemon) { }

    public virtual void onOpponentDraw(Card card, BattleGameBoard battleGameBoard, Pokemon opponentActivePokemon) { }

    public virtual bool overridesPlayFunc() { return false; }

    public virtual List<bool> play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets) { return targets.Select(t => false).ToList(); }

    public virtual void onDiscard(Card card, BattleGameBoard battleGameBoard, bool wasPlayed) { }

    public virtual void onTurnEnd(Card card, BattleGameBoard battleGameBoard) { }

    public virtual void onOpponentTurnEnd(Card card, BattleGameBoard battleGameBoard) { }

    public virtual void onBattleEnd(Card card, BattleGameBoard battleGameBoard) { }

    public virtual void onCardPlayed(Card card, BattleGameBoard battleGameBoard, Card move, Pokemon user, Pokemon target) { }

    /// <summary>
    /// Returns true if the given card is using this override functionality
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    protected bool usesThisOverrideInstance(Card card) { return card.overrideFunctionality == this; }

    /// <summary>
    /// Returns true if the given card is in the players hand
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    protected bool isCardInHand(Card card, BattleGameBoard battleGameBoard) { return battleGameBoard.hand.Contains(card); }
}
