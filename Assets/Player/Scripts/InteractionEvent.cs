using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// Interaction event without component to better store data without errors between scenes
/// </summary>
[Serializable]
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
    /// Possible types include (to be activated, to be enabled, active and enabled, completed)
    /// </summary>
    public string eventStatus;

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
        set
        {
            Enum.TryParse(value, out eventType);
        }
    }
    public enum PossibleEventTypes { Message, Battle, SceneChange, Question, AddPokemon }
    public PossibleEventTypes eventType = PossibleEventTypes.Message;

    /// <summary>
    /// Possible types include (to be activated, to be enabled, active and enabled, completed)
    /// </summary>
    public string eventStatusString
    {
        get
        {
            return eventStatus.ToString();
        }
        set
        {
            Enum.TryParse(value, out eventStatus);
        }
    }
    public enum PossibleEventStatus { ToBeActivated, ToBeEnabled, ActiveAndEnabled, Completed }
    public PossibleEventStatus eventStatus = PossibleEventStatus.ToBeActivated;

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
                if (messageSounds.Count == 0) return message.Count >= 0;
                return message.Count >= 0 && messageSounds.Count == message.Count;
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
    /// Display name text with the message
    /// </summary>
    public List<string> messageName;

    /// <summary>
    /// Display text when the user clicks as a global message
    /// </summary>
    public List<string> message;

    /// <summary>
    /// Companion to message - if non-null, use it instead of the default WorldDialog talkAudioSource.
    /// </summary>
    public List<AudioClip> messageSounds;

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
    /// (Optional) The name of the reaction animation trigger to call. If set reactionIdle will be called at the end of the event
    /// </summary>
    public string animationReactionTriggerName;

    /// <summary>
    /// (Optional) The name of the reaction idle animation trigger to call at the end of the event.
    /// </summary>
    public string animationReactionIdleTriggerName = "reactionIdle";

    /// <summary>
    /// (Optional) The animation controller to update
    /// </summary>
    public Animator reactionAnimator;

    /// <summary>
    /// True if the game object should be hidden on return
    /// </summary>
    public bool removeOnReturn;

    /// <summary>
    /// True if the interaction event should be disabled on return
    /// </summary>
    public bool disableOnReturn;

    /// <summary>
    /// (Optional) Used to remove objects like barriers blocking a path.
    /// </summary>
    public List<string> dependsOnToSetInactive = new List<string>();

    /// <summary>
    /// (Optional) Used to disable alternative event options. Or if the same event was on two objects.
    /// </summary>
    public List<string> dependsOnToDisable = new List<string>();

    /// <summary>
    /// (Optional) Used to chain text dialogs. Usually the previous text event to before this one.
    /// </summary>
    public List<string> dependsOnToEnable = new List<string>();

    /// <summary>
    /// (Optional) Used to add barriers to a path or show an object before it is interactable.
    /// </summary>
    public List<string> dependsOnToSetActive = new List<string>();

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

    /// <summary>
    /// (Required for AddPokemon) The message to display when the pokemon was added.
    /// </summary>
    public string pokemonToAddSuccessMessage {
        get
        {
            return "** {pokemon} joined the team **".Replace("{pokemon}", pokemonToAdd);
        }
    }

    /// <summary>
    /// (Required for AddPokemon) The message to display when the pokemon can't be added.
    /// </summary>
    public string pokemonToAddPartyFullMessage
    {
        get
        {
            return "** Team is full, {pokemon} can't join **".Replace("{pokemon}", pokemonToAdd);
        }
    }

    void Awake()
    {
        if (eventName == null || eventName == "")
        {
            eventName = "DefaultEvent_" + Guid.NewGuid();
        }
    }

    void Start()
    {
        StartHelper();
    }


    // Start is called before the first frame update
    private void StartHelper()
    {
        NameUniquenessCheck();

        if (animator == null)
        {
            animator = gameObject.GetComponent<Animator>();
        }

        if (autoTriggerInteractionEventName != null && autoTriggerInteractionEventName != "")
        {
            autoTriggerInteractionEvent = GetInteractionEventByName(autoTriggerInteractionEventName);
            if (!autoTriggerInteractionEvent) throw new System.Exception("On event " + eventName + " cannot find auto interaction event: " + autoTriggerInteractionEventName);
        }

        if (optionFirstTriggerInteractionEventName != null && optionFirstTriggerInteractionEventName != "")
        {
            optionFirstTriggerInteractionEvent = GetInteractionEventByName(optionFirstTriggerInteractionEventName);
            if (!optionFirstTriggerInteractionEvent) throw new System.Exception("On event " + eventName + " cannot find interaction event: " + optionFirstTriggerInteractionEventName);
        }

        if (optionSecondTriggerInteractionEventName != null && optionSecondTriggerInteractionEventName != "")
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
        result.eventStatus = eventStatusString;
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

        eventTypeString = iEventData.eventType;
        eventStatusString = iEventData.eventStatus;
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
        var allInteractionEvents = GameObject.FindObjectsOfType<InteractionEvent>(true);

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
    public static InteractionEvent GetInteractionEventByName(string name)
    {
        var allInteractionEvents = GameObject.FindObjectsOfType<InteractionEvent>(true);

        var result = allInteractionEvents.FirstOrDefault(e => e.eventName == name.Trim());

        return result;
    }

    /// <summary>
    /// Updates the event gameobject and enabled property based on which dependent events have been completed
    /// </summary>
    /// <param name="allEvents"></param>
    public void UpdateInteractionFromEventStatus(List<InteractionEventNoComponent> allEvents)
    {
        // Make sure default status is accurate
        if (eventStatus == PossibleEventStatus.ToBeActivated)
        {
            if (gameObject.activeSelf && enabled)
            {
                eventStatus = PossibleEventStatus.ActiveAndEnabled;
            }
            else if (gameObject.activeSelf)
            {
                eventStatus = PossibleEventStatus.ToBeEnabled;
            }
        }

        var completedEvents = allEvents.Where(e => e.eventStatus == PossibleEventStatus.Completed.ToString());

        // Check if event names are valid
        dependsOnToDisable.Union(dependsOnToEnable)
           .Union(dependsOnToSetActive)
           .Union(dependsOnToSetInactive)
           .ToList()
           .ForEach(dependsEventName =>
           {
               var interactionEvent = GetInteractionEventByName(dependsEventName);
               if (!interactionEvent) Debug.LogWarning("On event " + eventName + " cannot find interaction event: " + dependsEventName);
           });

        // Handle the case of showing an event object
        if (eventStatus == PossibleEventStatus.ToBeActivated)
        {
            var shouldBeActivated = completedEvents.Any(e => dependsOnToSetActive.Contains(e.eventName));
            var shouldBeEnabled = completedEvents.Any(e => dependsOnToEnable.Contains(e.eventName));
            var shouldBeInactive = completedEvents.Any(e => dependsOnToSetInactive.Contains(e.eventName));

            if (shouldBeActivated && enabled)
            {
                gameObject.SetActive(true);
                eventStatus = PossibleEventStatus.ActiveAndEnabled;
            }
            else if (shouldBeActivated)
            {
                gameObject.SetActive(true);
                eventStatus = PossibleEventStatus.ToBeEnabled;
            }
            else if (shouldBeEnabled)
            {
                enabled = true;
                eventStatus = PossibleEventStatus.ToBeEnabled;
            }
            else if (shouldBeInactive)
            {
                gameObject.SetActive(false);
                eventStatus = PossibleEventStatus.Completed;
            }
        }
        
        // Handle the case of making an event interactable
        if (eventStatus == PossibleEventStatus.ToBeEnabled)
        {
            var shouldBeEnabled = completedEvents.Any(e => dependsOnToEnable.Contains(e.eventName));
            var shouldBeInactive = completedEvents.Any(e => dependsOnToSetInactive.Contains(e.eventName));

            if (shouldBeEnabled)
            {
                enabled = true;
                gameObject.SetActive(true);
                eventStatus = PossibleEventStatus.ActiveAndEnabled;
            }
            else if (shouldBeInactive)
            {
                gameObject.SetActive(false);
                eventStatus = PossibleEventStatus.Completed;
            }
        }
        
        // Handle the case of completing a event
        if (eventStatus == PossibleEventStatus.ActiveAndEnabled)
        {
            var shouldBeDisabled = completedEvents.Any(e => dependsOnToDisable.Contains(e.eventName));
            var shouldBeInactive = completedEvents.Any(e => dependsOnToSetInactive.Contains(e.eventName));

            if (shouldBeDisabled)
            {
                enabled = false;
                eventStatus = PossibleEventStatus.Completed;
            }

            if (shouldBeInactive)
            {
                gameObject.SetActive(false);
                eventStatus = PossibleEventStatus.Completed;
            }
        }

        // Handle the case of completing a event
        if (eventStatus == PossibleEventStatus.Completed)
        {
            var shouldBeInactive = completedEvents.Any(e => dependsOnToSetInactive.Contains(e.eventName));

            if (shouldBeInactive)
            {
                gameObject.SetActive(false);
                eventStatus = PossibleEventStatus.Completed;
            }
        }
    }
}
