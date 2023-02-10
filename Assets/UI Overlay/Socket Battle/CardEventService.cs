using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Coordinates all the game events to send to the cards for different bonuses
/// </summary>
public class CardEventService : MonoBehaviour
{
    /// <summary>
    /// Reference to battle game board
    /// </summary>
    private BattleGameBoard battleGameBoard;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Call before any other event for on battle start
    /// </summary>
    /// <param name="_battleGameBoard"></param>
    public void onBattleStart(BattleGameBoard _battleGameBoard)
    {
        Debug.Log("Battle started.");
        battleGameBoard = _battleGameBoard;
    }

    /// <summary>
    /// Return the complete list of cards for the owner of the given card. If it was the players card this would return allCards, otherwise it would return opponent.initDeck.
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private List<Card> getCardsForEventOwner(Card card)
    {
        return getCardsForEventOwner(card.isPlayerPokemon);
    }

    /// <summary>
    /// Return the complete list of cards for the owner of the given card. If it was the players card this would return allCards, otherwise it would return opponent.initDeck.
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private List<Card> getCardsForEventOwner(bool isEventOwnerPlayer)
    {
        var playerCards = battleGameBoard.allCards;
        var opponentCards = battleGameBoard.opponent.initDeck;

        if (isEventOwnerPlayer) return playerCards;
        return opponentCards;
    }

    /// =====================================
    /// ICard Events to trigger
    /// ======================================

    /// <summary>
    /// When any card is drawn for the given human or computer player
    /// </summary>
    /// <param name="cardDrawn"></param>
    /// <param name="activePokemon"></param>
    public void onDraw(Card cardDrawn, Pokemon activePokemon)
    {
        Debug.Log("Player/Computer drew " + cardDrawn.cardName);

        var cardList = getCardsForEventOwner(cardDrawn);
        cardList.ForEach(card =>
        {
            if (card.overrideFunctionality && card == cardDrawn) { card.overrideFunctionality.onThisDraw(card, battleGameBoard, activePokemon); }
            if (card.overrideFunctionality) { card.overrideFunctionality.onAnyDraw(card, cardDrawn, battleGameBoard, activePokemon); }
        });
    }

    public void onOpponentDraw(Card cardDrawn, Pokemon opponentActivePokemon)
    {
        var isRivalPlayerDrawing = !cardDrawn.isPlayerPokemon;
        var cardList = getCardsForEventOwner(isRivalPlayerDrawing);
        cardList.ForEach(card =>
        {
            if (card.overrideFunctionality) { card.overrideFunctionality.onOpponentDraw(card, cardDrawn, battleGameBoard, opponentActivePokemon); }
        });
    }

    public void onTurnEnd()
    {
        Debug.Log("Player turn ended.");
        var isPlayer = true;
        var playerCardList = getCardsForEventOwner(isPlayer);
        playerCardList.ForEach(card =>
        {
            if (card.overrideFunctionality) { card.overrideFunctionality.onTurnEnd(card, battleGameBoard); }
        });

        var opponentCardList = getCardsForEventOwner(!isPlayer);
        opponentCardList.ForEach(card =>
        {
            if (card.overrideFunctionality) { card.overrideFunctionality.onOpponentTurnEnd(card, battleGameBoard); }
        });
    }

    public void onOpponentTurnEnd()
    {
        Debug.Log("Opponent turn ended.");
        var isPlayer = true;
        var playerCardList = getCardsForEventOwner(isPlayer);
        playerCardList.ForEach(card =>
        {
            if (card.overrideFunctionality) { card.overrideFunctionality.onOpponentTurnEnd(card, battleGameBoard); }
        });

        var opponentCardList = getCardsForEventOwner(!isPlayer);
        opponentCardList.ForEach(card =>
        {
            if (card.overrideFunctionality) { card.overrideFunctionality.onTurnEnd(card, battleGameBoard); }
        });
    }

    public void onBattleEnd()
    {
        Debug.Log("Battle ended.");
        var isPlayer = true;
        var playerCardList = getCardsForEventOwner(isPlayer);
        playerCardList.ForEach(card =>
        {
            if (card.overrideFunctionality) { card.overrideFunctionality.onBattleEnd(card, battleGameBoard); }
        });

        var opponentCardList = getCardsForEventOwner(!isPlayer);
        opponentCardList.ForEach(card =>
        {
            if (card.overrideFunctionality) { card.overrideFunctionality.onBattleEnd(card, battleGameBoard); }
        });
    }

    public void onCardPlayed(Card move, Pokemon user, Pokemon target)
    {
        var cardList = getCardsForEventOwner(move);
        cardList.ForEach(card =>
        {
            if (card.overrideFunctionality && card == move) { card.overrideFunctionality.onThisCardPlayed(card, battleGameBoard, user, target); }
            if (card.overrideFunctionality) { card.overrideFunctionality.onAnyCardPlayed(card, battleGameBoard, move, user, target); }
        });
    }

    public void onDiscard(Card discardedCard, bool wasPlayed)
    {
        var cardList = getCardsForEventOwner(discardedCard);
        cardList.ForEach(card =>
        {
            if (card.overrideFunctionality && card == discardedCard) { card.overrideFunctionality.onThisDiscard(card, battleGameBoard, wasPlayed); }
            if (card.overrideFunctionality) { card.overrideFunctionality.onAnyDiscard(card, discardedCard, battleGameBoard, wasPlayed); }
        });
    }
}
