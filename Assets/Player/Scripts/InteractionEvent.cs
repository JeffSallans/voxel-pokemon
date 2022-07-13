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

    /// <summary>
    /// The pokemon to add if the event type is AddPokemon
    /// </summary>
    public string pokemonToAdd;
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
    public string eventTypeString
    {
        get
        {
            return eventType.ToString();
        }
    }
    public enum PossibleEventTypes { Message, Battle, SceneChange, Question, AddPokemon }
    public PossibleEventTypes eventType = PossibleEventTypes.Message;

    /// <summary>
    /// Returns true if all the required fields
    /// </summary>
    private bool allRequiredFieldsSet
    {
        get
        {
            if (eventName == "") return false;
            if (eventType == PossibleEventTypes.Message)
            {
                return message.Count >= 0;
            }
            else if (eventType == PossibleEventTypes.Battle)
            {
                return message.Count >= 0 && sceneName != "" && opponent != null;
            }
            else if (eventType == PossibleEventTypes.SceneChange)
            {
                return message.Count >= 0 && sceneName != "";
            }
            else if (eventType == PossibleEventTypes.Question)
            {
                return message.Count >= 0 && options.Count == 2 && optionFirstTriggerInteractionEvent != null && autoTriggerInteractionEvent == null;
            }
            else if (eventType == PossibleEventTypes.AddPokemon)
            {
                return message.Count >= 0 && pokemonToAdd != "";
            }

            return true;  
        }
    }

    /// <summary>
    /// Display text before the user clicks on the target
    /// </summary>
    public string interactionHint;

    /// <summary>
    /// Display text when the user clicks as a global message
    /// </summary>
    public List<string> message;

    /// <summary>
    /// (REQUIRED by Battle) Opponent details if the event is a battle
    /// </summary>
    public OpponentDeck opponent;

    /// <summary>
    /// (REQUIRED by Battle and SceneChange) Scene to navigate to if it is a battle or another event
    /// </summary>
    public string sceneName;

    /// <summary>
    /// (REQUIRED by SceneChange) Player position to copy to
    /// </summary>
    public string scenePlayerName;

    /// <summary>
    /// (Optional) The name of the boolean to toggle on if active
    /// </summary>
    public string animationBooleanName;

    /// <summary>
    /// (Optional) The animation controller to update
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
    /// (Optional) The interaction event to disable after triggering this event. Usually a can't do event.
    /// </summary>
    public InteractionEvent altDisableInteractionEvent;

    /// <summary>
    /// (Optional) Use as an alternative to altDisableInteractionEvent. Finds the event by name instead of reference.
    /// </summary>
    public string altDisableInteractionEventName;

    /// <summary>
    /// (Optional) The next interaction event to enable after this one. Usually the next text to show after an event.
    /// </summary>
    public InteractionEvent nextInteractionEvent;

    /// <summary>
    /// (Optional) Use as an alternative to nextInteractionEvent. Finds the event by name instead of reference.
    /// </summary>
    public string nextInteractionEventName;

    /// <summary>
    /// (Optional) The alt next interaction event to enable after this one. Usually an item or door that is now interactable.
    /// </summary>
    public InteractionEvent nextInteractionEvent2;

    /// <summary>
    /// (Optional) Use as an alternative to nextInteractionEvent2. Finds the event by name instead of reference.
    /// </summary>
    public string nextInteractionEvent2Name;

    /// <summary>
    /// (Optional for Message, Battle, SceneChange, AddPokemon) The interaction event to immediately trigger after this event. Usually dialog after a battle.
    /// </summary>
    public InteractionEvent autoTriggerInteractionEvent;

    /// <summary>
    /// (Optional) Use as an alternative to autoTriggerInteractionEvent. Finds the event by name instead of reference.
    /// </summary>
    public string autoTriggerInteractionEventName;

    /// <summary>
    /// (Required for Question) Display text for each option
    /// </summary>
    public List<string> options;

    /// <summary>
    /// (Required for Question) The interaction event to trigger after selecting option 1. Usually confirmation event.
    /// </summary>
    public InteractionEvent optionFirstTriggerInteractionEvent;

    /// <summary>
    /// (Optional for Question) The interaction event to trigger after selecting option 1. Usually confirmation event.
    /// </summary>
    public string optionFirstTriggerInteractionEventName;
    
    /// <summary>
    /// (Required for Question) The interaction event to trigger after selecting option 2. Usually confirmation event.
    /// </summary>
    public InteractionEvent optionSecondTriggerInteractionEvent;

    /// <summary>
    /// (Optional for Question) The interaction event to trigger after selecting option 2. Usually confirmation event.
    /// </summary>
    public string optionSecondTriggerInteractionEventName;

    /// <summary>
    /// (Required for AddPokemon) The name of the pokemon to instatate on the player.
    /// </summary>
    public string pokemonToAdd = "";

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

        if (optionFirstTriggerInteractionEventName != "")
        {
            optionFirstTriggerInteractionEvent = GetInteractionEventByName(optionFirstTriggerInteractionEventName);
            if (!optionFirstTriggerInteractionEvent) throw new System.Exception("On event " + eventName + " cannot find interaction event: " + optionFirstTriggerInteractionEventName);
        }

        if (optionSecondTriggerInteractionEventName != "")
        {
            optionSecondTriggerInteractionEvent = GetInteractionEventByName(optionSecondTriggerInteractionEventName);
            if (!optionSecondTriggerInteractionEvent) throw new System.Exception("On event " + eventName + " cannot find interaction event: " + optionSecondTriggerInteractionEventName);
        }

        // Give user a warning if the settings
        if (!allRequiredFieldsSet)
        {
            throw new System.Exception("On event " + eventName + " missing required fields");
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
        result.eventType = eventTypeString;
        result.sceneName = sceneName;
        result.disableOnReturn = disableOnReturn;
        result.removeOnReturn = removeOnReturn;
        result.pokemonToAdd = pokemonToAdd;

        result.interactionEventEnabled = enabled;
        result.gameObjectActive = gameObject.activeSelf;

        return result;
    }

    /// <summary>
    /// Copies the interaction event no component data into InteractionEvent
    /// </summary>
    /// <param name="iEventData"></param>
    public void CopyInteractionEventValues(InteractionEventNoComponent iEventData)
    {
        eventName = iEventData.eventName;
        //TODO: add getter for eventTypeString
        //eventTypeString = iEventData.eventType;
        sceneName = iEventData.sceneName;
        disableOnReturn = iEventData.disableOnReturn;
        removeOnReturn = iEventData.removeOnReturn;
        pokemonToAdd = iEventData.pokemonToAdd;

        enabled = iEventData.interactionEventEnabled;
        gameObject.SetActive(iEventData.gameObjectActive);
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
