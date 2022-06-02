using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interaction event without component to better store data without errors between scenes
/// </summary>
public class InteractionEventNoComponent
{
    /// <summary>
    /// Unique identifier for the event
    /// </summary>
    public string eventName;

    /// <summary>
    /// Possible types include (Message, Battle, SceneChange)
    /// </summary>
    public string eventType;

    /// <summary>
    /// Scene to navigate to if it is a battle or another event
    /// </summary>
    public string sceneName;

    /// <summary>
    /// True if the game object should be hidden on return
    /// </summary>
    public bool removeOnReturn;

    /// <summary>
    /// True if the interaction event should be disabled on return
    /// </summary>
    public bool disableOnReturn;

    /// <summary>
    /// True if the game object is active
    /// </summary>
    public bool gameObjectActive;

    /// <summary>
    /// True if the interactionEvent is enabled
    /// </summary>
    public bool interactionEventEnabled;
}

/// <summary>
/// Used to configured world events the user can interact with
/// </summary>
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

    /// <summary>
    /// True if the game object should be hidden on return
    /// </summary>
    public bool removeOnReturn;

    /// <summary>
    /// True if the interaction event should be disabled on return
    /// </summary>
    public bool disableOnReturn;

    /// <summary>
    /// The next interaction event to enable after this one
    /// </summary>
    public InteractionEvent nextInteractionEvent;

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

    /// <summary>
    /// Return the interaction event with the class data without a component
    /// </summary>
    /// <returns></returns>
    public InteractionEventNoComponent GetInteractionEventWithoutComponent()
    {
        var result = new InteractionEventNoComponent();
        result.eventName = eventName;
        result.eventType = eventType;
        result.sceneName = sceneName;
        result.disableOnReturn = disableOnReturn;
        result.removeOnReturn = removeOnReturn;

        result.interactionEventEnabled = enabled;
        result.gameObjectActive = gameObject.activeSelf;

        return result;
    }
}
