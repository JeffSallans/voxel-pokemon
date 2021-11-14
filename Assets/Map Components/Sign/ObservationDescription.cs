using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The text to display when the player "interacts" with it
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class ObservationDescription : MonoBehaviour
{
    /// <summary>
    /// The player collider to trigger the text
    /// </summary>
    public CapsuleCollider playerCollider;

    /// <summary>
    /// The textbox to render
    /// </summary>
    public WorldDialog worldTextBox;

    /// <summary>
    /// The text to display when a player.  Max 80 characters a line and 3 lines.
    /// </summary>
    public string signText;

    /// <summary>
    /// How long the text stays on the screen after leaving the collision
    /// </summary>
    public float textDisplayDuration = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Show the textbox when the player enters
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == playerCollider.gameObject)
        {
            worldTextBox.OpenDialog(signText);
        }
    }

    // Hide the textbox after the player leaves
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject == playerCollider.gameObject)
        {
            worldTextBox.CloseDialog();
        }
    }
}
