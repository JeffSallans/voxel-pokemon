using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    /// The interaction event to disable after triggering this event. Usually a can't do event.
    /// </summary>
    public InteractionEvent altDisableInteractionEvent;

    /// <summary>
    /// Use as an alternative to altDisableInteractionEvent. Finds the event by name instead of reference.
    /// </summary>
    public string altDisableInteractionEventName;

    /// <summary>
    /// The next interaction event to enable after this one. Usually the next text to show after an event.
    /// </summary>
    public InteractionEvent nextInteractionEvent;

    /// <summary>
    /// Use as an alternative to nextInteractionEvent. Finds the event by name instead of reference.
    /// </summary>
    public string nextInteractionEventName;

    /// <summary>
    /// The alt next interaction event to enable after this one. Usually an item or door that is now interactable.
    /// </summary>
    public InteractionEvent nextInteractionEvent2;

    /// <summary>
    /// Use as an alternative to nextInteractionEvent2. Finds the event by name instead of reference.
    /// </summary>
    public string nextInteractionEvent2Name;

    /// <summary>
    /// The interaction event to immediately trigger after this event. Usually dialog after a battle.
    /// </summary>
    public InteractionEvent autoTriggerInteractionEvent;

    /// <summary>
    /// Use as an alternative to autoTriggerInteractionEvent. Finds the event by name instead of reference.
    /// </summary>
    public string autoTriggerInteractionEventName;


    // Start is called before the first frame update
    void Start()
    {
        NameUniquenessCheck();

        if (animator == null)
        {
            animator = gameObject.GetComponent<Animator>();
        }

        if (altDisableInteractionEventName != "")
        {
            altDisableInteractionEvent = GetInteractionEventByName(altDisableInteractionEventName);
            if (!altDisableInteractionEvent) throw new System.Exception("On event " + eventName + " cannot find interaction event: " + altDisableInteractionEventName);
        }

        if (nextInteractionEventName != "")
        {
            nextInteractionEvent = GetInteractionEventByName(nextInteractionEventName);
            if (!nextInteractionEvent) throw new System.Exception("On event " + eventName + " cannot find interaction event: " + nextInteractionEventName);
        }

        if (nextInteractionEvent2Name != "")
        {
            nextInteractionEvent2 = GetInteractionEventByName(nextInteractionEvent2Name);
            if (!nextInteractionEvent2) throw new System.Exception("On event " + eventName + " cannot find interaction event: " + nextInteractionEvent2Name);
        }

        if (autoTriggerInteractionEventName != "")
        {
            autoTriggerInteractionEvent = GetInteractionEventByName(autoTriggerInteractionEventName);
            if (!autoTriggerInteractionEvent) throw new System.Exception("On event " + eventName + " cannot find interaction event: " + autoTriggerInteractionEventName);
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

    /// <summary>
    /// Loads all Interaction Events and find the one for the given name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private void NameUniquenessCheck()
    {
        var allInteractionEvents = GameObject.FindObjectsOfType<InteractionEvent>();

        var result = allInteractionEvents.FirstOrDefault(e => e.eventName == eventName && e != this);

        if (result)
        {
            throw new System.Exception("On event " + eventName + " event name is already found by " + result.eventName + " with message " + result.message.FirstOrDefault());
        }
    }

    /// <summary>
    /// Loads all Interaction Events and find the one for the given name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private InteractionEvent GetInteractionEventByName(string name)
    {
        var allInteractionEvents = GameObject.FindObjectsOfType<InteractionEvent>();

        var result = allInteractionEvents.FirstOrDefault(e => e.eventName == name);

        return result;
    }
}
