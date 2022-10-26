using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the user indicated settings
/// </summary>
public class GameOptions : MonoBehaviour
{
    /// <summary>
    /// Set to true if the user should click and drag to play cards. 
    /// If false, the user will click once to pick up a card and click again to play it.
    /// </summary>
    public bool useCardDragControls = false;

    /// <summary>
    /// Set to true if the user will return to main menu on death.
    /// </summary>
    public bool instaDeathEnabled = false;

    /// <summary>
    /// Level at which to play background music.
    /// </summary>
    public float musicVolume = DefaultMusicVolume;
    public const float DefaultMusicVolume = 0.5f;

    /// <summary>
    /// Level at which to play sound effects.
    /// </summary>
    public float soundEffectVolume = DefaultSoundEffectVolume;
    public const float DefaultSoundEffectVolume = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
