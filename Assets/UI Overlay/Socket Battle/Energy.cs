using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resource to play a move.  It can be attached to cards or pokemon.
/// </summary>
public class Energy : MonoBehaviour
{
    public string energyName;

    public string energyType;

    /// <summary>
    /// True if the energy is used for this turn
    /// </summary>
    public bool isUsed;

    public Card attachedToCard;

    public Pokemon attachedToPokemon;
       
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
