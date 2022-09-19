using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PauseMenu : MonoBehaviour
{
    private PartyMenu partyMenu;
    private OptionsMenu optionsMenu;
    private MenuCommon menuCommon;

    public void Awake()
    {
        menuCommon = gameObject.AddComponent<MenuCommon>();
        menuCommon.Initialize(() =>
        {
            OnReturn();
            return false;
        });
    }

    // Start is called before the first frame update
    public void Start()
    {
        partyMenu = FindObjectOfType<PartyMenu>(true);
        optionsMenu = FindObjectOfType<OptionsMenu>(true);
    }

    /// <summary>
    /// Call to open the Pause Menu
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);

        menuCommon.PlayOpenSound();
        menuCommon.EnableDialogMode();
    }

    /// <summary>
    /// Call to close the Pause Menu
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);

        menuCommon.DisableDialogMode();
    }

    public void OnParty()
    {
        menuCommon.PlaySelectSound();
        Close();
        partyMenu.Open();
    }

    public void OnOptions()
    {
        menuCommon.PlaySelectSound();
        optionsMenu.Open();
        gameObject.SetActive(false);
    }

    public void OnReturn()
    {
        menuCommon.PlayCloseSound();
        Close();
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
