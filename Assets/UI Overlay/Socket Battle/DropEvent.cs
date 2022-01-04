using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Details the box colider provides when dropped on
/// </summary>
public class DropEvent : MonoBehaviour
{
    /// <summary>
    /// Possible types include (TargetPokemon, TargetCard, TargetEnergySocket, TargetDeck)
    /// </summary>
    public string eventType;

    /// <summary>
    /// Populated on Start()
    /// </summary>
    public Pokemon targetPokemon;

    /// <summary>
    /// Populated on Start()
    /// </summary>
    public Card targetCard;

    /// <summary>
    /// Populated on Start()
    /// </summary>
    public Energy targetEnergySocket;

    /// <summary>
    /// Populated on Start()
    /// </summary>
    public GameObject targetGameObject;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
