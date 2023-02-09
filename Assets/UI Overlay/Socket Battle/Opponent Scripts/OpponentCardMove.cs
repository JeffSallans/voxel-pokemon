using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Track all the info for an opponent card
/// </summary>
public class OpponentCardMove
{
    /// <summary>
    /// The pokemon targetted by this move
    /// </summary>
    public Pokemon selectedTargetPokemon;

    /// <summary>
    /// The current priority ranking to determin if this card should be played
    /// </summary>
    public float turnPriority;

    /// <summary>
    /// The card that will be played
    /// </summary>
    public Card card;

    public virtual bool canUseMove
    {
        get {
            if (selectedTargetPokemon == null)
            {
                Debug.LogWarning("canUseMove called before selectedTargetPokemon is set");
                return false;
            }
            return card.canBePlayed && card.canTarget(selectedTargetPokemon);
        }
    }
}
