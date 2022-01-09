using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the meta game configurations the computer brings into a battle.  Includes base pokemon stats/state, possible moves, strategy bot, items, and others.
/// </summary>
public class OpponentDeck : MonoBehaviour
{
    /// <summary>
    /// The trainer name
    /// </summary>
    public string opponentName;

    /// <summary>
    /// The pokemon to bring into battle, the first one in the list is the active one
    /// </summary>
    public List<Pokemon> party;

    /// <summary>
    /// The possible moves for this opponent to battle with
    /// </summary>
    public List<OpponentMove> movesConfig;

    public OpponentBot opponentStrategyBot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
