using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The sound to play when the player steps on it
/// </summary>
[RequireComponent(typeof(MeshCollider))]
public class WalkingSound : MonoBehaviour
{
    /// <summary>
    /// The player collider to trigger the sound
    /// </summary>
    public BoxCollider playerCollider;
    
    /// <summary>
    /// Sounds to activate when the player is walking
    /// </summary>
    public AudioSource[] audioSourceList;

    /// <summary>
    /// Seconds between the audio sounds playing
    /// </summary>
    public float secondsBetweenAudioSounds;

    /// <summary>
    /// Used for debugging to see if the walking audio should be playing
    /// </summary>
    public bool playerOnSurface = false;

    /// <summary>
    /// Used for debugging to see if the walking audio should be playing
    /// </summary>
    public bool audioShouldBePlaying = false;

    /// <summary>
    /// Used for debugging to see if the walking audio should be playing
    /// </summary>
    public int lastIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var playerAnimation = playerCollider.gameObject.GetComponent<Animator>();
        if (audioShouldBePlaying && playerAnimation.GetBool("isWalking"))
        {
            StartCoroutine(playAudio());
        }
    }

    // Show the textbox when the player enters
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == playerCollider.gameObject)
        {
            playerOnSurface = true;
            audioShouldBePlaying = true;
        }
    }
    
    // Show the textbox when the player enters
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject == playerCollider.gameObject)
        {
            playerOnSurface = false;
        }
    }

    /// <summary>
    /// Coroutine function that triggers a walking sound.
    /// </summary>
    /// <returns>Enumerator to be used by coroutines</returns>
    IEnumerator playAudio()
    {
        // Handle empty sound list
        if (audioSourceList.Length == 0) yield return new WaitForSeconds(secondsBetweenAudioSounds);

        // Random sounds
        //var index = (int) Mathf.Floor(Random.value * audioSourceList.Length);

        // Looping sounds
        var index = (lastIndex + 1) % audioSourceList.Length;

        var audioSource = (AudioSource) audioSourceList.GetValue(index);
        audioSource.Play();
        audioShouldBePlaying = false;
        yield return new WaitForSeconds(secondsBetweenAudioSounds);
        audioShouldBePlaying = playerOnSurface;
        lastIndex = index;
    }
}
