using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class OptionsMenu : MonoBehaviour
{
    PlayerDeck player;

    /// <summary>
    /// All the menues for the party pokemon
    /// </summary>
    public Button dragYesButton;

    /// <summary>
    /// All the menues for the party pokemon
    /// </summary>
    public Button dragNoButton;

    /// <summary>
    /// Sound to play when option is clicked
    /// </summary>
    public AudioSource selectAudioSource;

    /// <summary>
    /// Sound to play when close is clicked
    /// </summary>
    public AudioSource closeAudioSource;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Opens the pause menu
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);

        selectAudioSource.Play();

        GameObject.FindObjectOfType<OverlayCursor>().showCursor = true;
        GameObject.FindObjectOfType<CinemachineFreeLook>().enabled = false;
        var playerObject = GameObject.FindObjectOfType<CharacterController>().gameObject;
        playerObject.GetComponent<InteractionChecker>().enabled = false;
        playerObject.GetComponent<FallToGround>().enabled = false;
        playerObject.GetComponent<CharacterController>().enabled = false;
        playerObject.GetComponent<ThirdPersonMovement>().enabled = false;
        playerObject.GetComponent<AnimationExample>().enabled = false;

        player = playerObject.GetComponent<PlayerDeck>();

        SetupPlayer();
    }

    /// <summary>
    /// Closes the pause menu
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
        GameObject.FindObjectOfType<OverlayCursor>().showCursor = false;
        GameObject.FindObjectOfType<CinemachineFreeLook>().enabled = true;
        var playerObject = GameObject.FindObjectOfType<CharacterController>().gameObject;
        playerObject.GetComponent<InteractionChecker>().enabled = true;
        playerObject.GetComponent<FallToGround>().enabled = true;
        playerObject.GetComponent<CharacterController>().enabled = true;
        playerObject.GetComponent<ThirdPersonMovement>().enabled = true;
        playerObject.GetComponent<AnimationExample>().enabled = true;

        PackupPlayer();
    }

    private void SetupPlayer()
    {
        dragYesButton.interactable = !player.gameOptions.useCardDragControls;
        dragNoButton.interactable = player.gameOptions.useCardDragControls;
    }

    private void PackupPlayer()
    {

    }

    public void OnDragOption()
    {
        selectAudioSource.Play();

        player.gameOptions.useCardDragControls = true;

        dragYesButton.interactable = !player.gameOptions.useCardDragControls;
        dragNoButton.interactable = player.gameOptions.useCardDragControls;
    }

    public void OnDoubleClickOption()
    {
        selectAudioSource.Play();

        player.gameOptions.useCardDragControls = false;

        dragYesButton.interactable = !player.gameOptions.useCardDragControls;
        dragNoButton.interactable = player.gameOptions.useCardDragControls;
    }


    public void OnReturn()
    {
        closeAudioSource.Play();
        Close();
    }
}
