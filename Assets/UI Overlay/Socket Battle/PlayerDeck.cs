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

    /// <summary>
    /// The energies to bring into battle. (Should probably be computed)
    /// </summary>
    public List<Energy> energies;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
