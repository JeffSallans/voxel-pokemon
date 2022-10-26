using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Sets the properties of interaction checker for a spawn
/// </summary>
[RequireComponent(typeof(InteractionChecker))]
public class SpawnLoader : MonoBehaviour
{

    /// <summary>
    /// Sets the properties a user would do for each spawn prefab
    /// </summary>
    private void Awake()
    {
        var interactionChecker = gameObject.GetComponent<InteractionChecker>();

        if (interactionChecker.playerCamera == null)
        {
            interactionChecker.playerCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }

        if (interactionChecker.worldDialog == null)
        {
            interactionChecker.worldDialog = GameObject.FindObjectOfType<WorldDialog>();
        }

        if (interactionChecker.crosshairText == null)
        {
            interactionChecker.crosshairText = GameObject.Find("crosshair").GetComponent<TextMeshProUGUI>();
        }

        if (interactionChecker.interactionHoverText == null)
        {
            interactionChecker.interactionHoverText = GameObject.Find("interaction-hint-message").GetComponent<TextMeshProUGUI>();
        }

        if (interactionChecker.sceneTransitionAnimator == null)
        {
            interactionChecker.sceneTransitionAnimator = GameObject.FindObjectOfType<FadeInOnSceneLoad>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
