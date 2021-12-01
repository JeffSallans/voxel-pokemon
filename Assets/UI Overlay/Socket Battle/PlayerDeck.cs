using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the meta game configurations the player brings into a battle.  Includes current deck, base pokemon stats/state, items, and others.
/// </summary>
public class PlayerDeck : MonoBehaviour
{
    /// <summary>
    /// The cards to bring into battle
    /// </summary>
    public List<Card> deck;

    /// <summary>
    /// The pokemon to bring into battle, the first one in the list is the active one
    /// </summary>
    public List<Pokemon> party;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
