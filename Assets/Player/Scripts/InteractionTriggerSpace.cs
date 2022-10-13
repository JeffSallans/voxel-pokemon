using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class InteractionTriggerSpace : MonoBehaviour
{
    /// <summary>
    /// The interaction event name to trigger when walking on this space
    /// </summary>
    public string interactionEventToTriggerName;

    /// <summary>
    /// The interaction event to trigger when walking on this space
    /// </summary>
    public InteractionEvent interactionEventToTrigger;

    /// <summary>
    /// The user's interaction checker
    /// </summary>
    private InteractionChecker interactionChecker;


    private bool hasTriggeredEvent;

    // Start is called before the first frame update
    void Start()
    {
        hasTriggeredEvent = false;
        if (interactionEventToTriggerName != null && interactionEventToTriggerName != "")
        {
            interactionEventToTrigger = InteractionEvent.GetInteractionEventByName(interactionEventToTriggerName);
            if (!interactionEventToTrigger) throw new System.Exception("On event " + name + " cannot find next interaction event: " + interactionEventToTriggerName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!hasTriggeredEvent && interactionEventToTrigger && interactionEventToTrigger.enabled)
        {
            // Check if two players exist, this in invalid event trigger
            if (FindObjectsOfType<CharacterController>().Count() > 1)
            {
                Debug.LogWarning("Event triggered when more than one character controller is active. This is probably because the event was trigged by a spawn.");
                return;
            }

            var interactionChecker = FindObjectOfType<CharacterController>().gameObject.GetComponent<InteractionChecker>();
            interactionChecker.OnInteractionEvent(interactionEventToTrigger);

            // Set camera focus
            //var thirdPersonCamera = GameObject.FindObjectOfType<Cinemachine.CinemachineFreeLook>();
            //thirdPersonCamera.LookAt = interactionEventToTrigger.gameObject.transform;

            // Remove focus after two seconds
             
            hasTriggeredEvent = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        hasTriggeredEvent = false;
    }
}
