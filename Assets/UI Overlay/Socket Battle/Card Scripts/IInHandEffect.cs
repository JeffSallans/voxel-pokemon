using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Template for the in hand effect
/// </summary>
public abstract class IInHandEffect : MonoBehaviour
{
    public bool isInHand = false;

    private Card card;

    private Pokemon handOwner;

    private BattleGameBoard battleGameBoard;

    // Start is called before the first frame update
    void Start()
    {
        card = gameObject.GetComponent<Card>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Add the in hand effect
    /// </summary>
    public virtual void OnStartup(BattleGameBoard battleGameBoard)
    {
        this.battleGameBoard = battleGameBoard;
    }

    /// <summary>
    /// Add the in hand effect
    /// </summary>
    public virtual void OnDraw(Card card, Pokemon user)
    {
        this.handOwner = user;
        isInHand = true;
    }

    /// <summary>
    /// Remove the in hand effect
    /// </summary>
    public virtual void OnDiscard()
    {

        isInHand = false;
    }


    /// <summary>
    /// Update in the in hand value on card draw
    /// </summary>
    public virtual void OnOtherCardDraw(Card card, Pokemon user)
    {

    }

    /// <summary>
    /// Update in the in hand value on card play
    /// </summary>
    public virtual void OnCardPlay(Card card, Pokemon user, Pokemon target)
    {

    }

    /// <summary>
    /// Update in the in hand value on opponent move
    /// </summary>
    public virtual void OnOpponentMove(OpponentMove move, Pokemon user, Pokemon target)
    {

    }
}
