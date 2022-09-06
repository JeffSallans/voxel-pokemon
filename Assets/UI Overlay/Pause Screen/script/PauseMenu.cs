using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PauseMenu : MonoBehaviour
{
    PartyMenu partyMenu;

    OptionsMenu optionsMenu;

    public AudioSource openAudioSource;
    public AudioSource selectAudioSource;
    public AudioSource closeAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        partyMenu = GameObject.FindObjectOfType<PartyMenu>(true);
        optionsMenu = GameObject.FindObjectOfType<OptionsMenu>(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Call to open the Pause Menu
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);

        openAudioSource.Play();

        GameObject.FindObjectOfType<OverlayCursor>().showCursor = true;
        GameObject.FindObjectOfType<CinemachineFreeLook>().enabled = false;
        var player = GameObject.FindObjectOfType<CharacterController>().gameObject;
        player.GetComponent<InteractionChecker>().enabled = false;
        player.GetComponent<FallToGround>().enabled = false;  
        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<ThirdPersonMovement>().enabled = false;
        player.GetComponent<AnimationExample>().enabled = false;
    }

    /// <summary>
    /// Call to close the Pause Menu
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
        GameObject.FindObjectOfType<OverlayCursor>().showCursor = false;
        GameObject.FindObjectOfType<CinemachineFreeLook>().enabled = true;
        var player = GameObject.FindObjectOfType<CharacterController>().gameObject;
        player.GetComponent<InteractionChecker>().enabled = true;
        player.GetComponent<FallToGround>().enabled = true;
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<ThirdPersonMovement>().enabled = true;
        player.GetComponent<AnimationExample>().enabled = true;
    }

    public void OnParty()
    {
        Close();
        partyMenu.Open();
    }

    public void OnOptions()
    {
        selectAudioSource.Play();
        optionsMenu.Open();
        gameObject.SetActive(false);
    }

    public void OnReturn()
    {
        closeAudioSource.Play();
        Close();
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
