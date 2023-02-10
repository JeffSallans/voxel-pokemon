using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ICard : MonoBehaviour
{
    public virtual void onBattleStart(Card thisCard, BattleGameBoard battleGameBoard) { }

    /// <summary>
    /// When the human or computer player drawns a card from their deck
    /// </summary>
    public virtual void onAnyDraw(Card thisCard, Card cardDrawn, BattleGameBoard battleGameBoard, Pokemon activePokemon) { }

    /// <summary>
    /// When the human or computer player drawns this card from their deck
    /// </summary>
    public virtual void onThisDraw(Card thisCard, BattleGameBoard battleGameBoard, Pokemon activePokemon) { }


    /// <summary>
    /// When the rival player (human or computer) drawns a card from their deck
    /// </summary>
    public virtual void onOpponentDraw(Card thisCard, Card cardDrawn, BattleGameBoard battleGameBoard, Pokemon opponentActivePokemon) { }

    /// <summary>
    /// Return true to only use the ICard ability and not the commonPlay() function
    /// </summary>

    public virtual bool overridesPlayFunc() { return false; }

    /// <summary>
    /// Functionality to execute right before the commonPlay() function is called if that setting is configured. Return result coordinates misses.
    /// </summary>

    public virtual List<bool> play(Card thisCard, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets) { return targets.Select(t => false).ToList(); }

    /// <summary>
    /// When a card is removed from the owners hand, this happens after play(). A good time to handle card cleanup
    /// </summary>
    public virtual void onAnyDiscard(Card thisCard, Card discardedCard, BattleGameBoard battleGameBoard, bool wasPlayed) { }

    /// <summary>
    /// When this card is removed from the owners hand, this happens after play(). A good time to handle card cleanup
    /// </summary>
    public virtual void onThisDiscard(Card thisCard, BattleGameBoard battleGameBoard, bool wasPlayed) { }

    /// <summary>
    /// When the players turn ends
    /// </summary>
    public virtual void onTurnEnd(Card thisCard, BattleGameBoard battleGameBoard) { }

    /// <summary>
    /// When the rival turn ends
    /// </summary>
    public virtual void onOpponentTurnEnd(Card thisCard, BattleGameBoard battleGameBoard) { }

    /// <summary>
    /// When the game ends
    /// </summary>
    public virtual void onBattleEnd(Card thisCard, BattleGameBoard battleGameBoard) { }

    /// <summary>
    /// When a card finishes running play(), a good time to check side effects that happened
    /// </summary>
    public virtual void onAnyCardPlayed(Card thisCard, BattleGameBoard battleGameBoard, Card move, Pokemon user, Pokemon target) { }

    /// <summary>
    /// When this card finishes running play(), a good time to check side effects that happened
    /// </summary>
    public virtual void onThisCardPlayed(Card thisCard, BattleGameBoard battleGameBoard, Pokemon user, Pokemon target) { }

    /// <summary>
    /// Returns true if the given card is in the players hand
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    protected bool isCardInHand(Card card, BattleGameBoard battleGameBoard) {
        if (card.isPlayerPokemon) return battleGameBoard.hand.Contains(card);
        return battleGameBoard.opponent.opponentStrategyBot.hand.Any(move => move.card == card);
    }
}
