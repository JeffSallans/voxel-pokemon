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

    /// <summary>
    /// Populated on Start(), can call onDrag, onHoverEnter, onHoverExit, onDropSuccess, onDropStop
    /// </summary>
    public Animator targetAnimator;

    // Start is called before the first frame update
    void Start()
    {
        if (targetAnimator == null)
        {
            targetAnimator = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}