using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FadeInOnSceneLoad : MonoBehaviour
{
    /// <summary>
    /// Animator with onFadeIn and onFadeOut triggers to be called on scene transitions
    /// </summary>
    private Animator sceneTransitionAnimator;

    /// <summary>
    /// Reference to the current scene cursor
    /// </summary>
    private OverlayCursor cursor;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        sceneTransitionAnimator = gameObject.GetComponent<Animator>();
        sceneTransitionAnimator.SetTrigger("onFadeIn");

        cursor = GameObject.FindObjectOfType<OverlayCursor>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Transition the scene view to hidden
    /// </summary>
    public void FadeIn()
    {
        sceneTransitionAnimator.SetTrigger("onFadeIn");
    }

    /// <summary>
    /// Transition the scene view to visible
    /// </summary>
    public void FadeOut()
    {
        sceneTransitionAnimator.SetTrigger("onFadeOut");
    }
}
