using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    /// <summary>
    /// Unique identifier for the event
    /// </summary>
    public string eventName;

    /// <summary>
    /// Possible types include (Message, Battle, SceneChange?)
    /// </summary>
    public string eventType; //

    /// <summary>
    /// Display text before the user clicks on the target
    /// </summary>
    public string interactionHint;

    /// <summary>
    /// Display text when the user clicks as a global message
    /// </summary>
    public List<string> message;

    /// <summary>
    /// Opponent details if the event is a battle
    /// </summary>
    public OpponentDeck opponent;

    /// <summary>
    /// Scene to navigate to if it is a battle or another event
    /// </summary>
    public string sceneName;

    /// <summary>
    /// The name of the boolean to toggle on if active
    /// </summary>
    public string animationBooleanName;

    /// <summary>
    /// The animation controller to update
    /// </summary>
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        if (animator == null)
        {
            animator = gameObject.GetComponent<Animator>();
        }  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
