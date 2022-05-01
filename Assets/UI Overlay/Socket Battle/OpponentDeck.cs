using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public List<IOpponentMove> movesConfig;

    public IOpponentStrategy opponentStrategyBot;

    // Start is called before the first frame update
    void Awake()
    {
        if (party.Count == 0)
        {
            party = gameObject.GetComponentsInChildren<Pokemon>().ToList();
        }
        if (movesConfig.Count == 0)
        {
            movesConfig = gameObject.GetComponentsInChildren<IOpponentMove>().ToList();
        }
        // Turn off all energies
        var energies = gameObject.GetComponentsInChildren<Energy>().ToList();
        energies.ForEach(e => e.gameObject.SetActive(false));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
