using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains the meta game configurations the player brings into a battle.  Includes current deck, base pokemon stats/state, items, and others.
/// </summary>
public class PlayerDeck : MonoBehaviour
{
    /// <summary>
    /// The pokemon to bring into battle, the first one in the list is the active one
    /// </summary>
    public List<Pokemon> party;

    /// <summary>
    /// The energies to bring into battle. (Should probably be computed)
    /// </summary>
    public List<Energy> energies;

    // Start is called before the first frame update
    void Awake()
    {
        if (party.Count == 0)
        {
            party = gameObject.GetComponentsInChildren<Pokemon>().ToList();
        }
        if (energies.Count == 0)
        {
            var allEnergies = gameObject.GetComponentsInChildren<Energy>().ToList();
            energies = allEnergies.Where(e => e.initCanBeDragged).ToList();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
