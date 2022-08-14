using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    PartyMenu partyMenu;

    GameObject optionsMenu;

    // Start is called before the first frame update
    void Start()
    {
        partyMenu = GameObject.FindObjectOfType<PartyMenu>(true);
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
        optionsMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnReturn()
    {
        Close();
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
