using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class OptionsMenu : MonoBehaviour
{
    PlayerDeck deck;

    /// <summary>
    /// To enable or disable based on the setting
    /// </summary>
    public Button dragYesButton;

    /// <summary>
    /// To enable or disable based on the setting
    /// </summary>
    public Button dragNoButton;

    /// <summary>
    /// To enable or disable based on the setting
    /// </summary>
    public Button instaYesButton;

    /// <summary>
    /// To enable or disable based on the setting
    /// </summary>
    public Button instaNoButton;

    private MenuCommon menuCommon;

    public void Awake()
    {
        menuCommon = gameObject.AddComponent<MenuCommon>();
        menuCommon.Initialize(Close);
    }

    /// <summary>
    /// Opens the options menu
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);

        menuCommon.EnableDialogMode();

        deck = menuCommon.Player.GetComponent<PlayerDeck>();

        SetupPlayer();
    }

    /// <summary>
    /// Closes the options menu
    /// </summary>
    public bool Close()
    {
        gameObject.SetActive(false);

        menuCommon.PlayCloseSound();
        menuCommon.DisableDialogMode();

        return false;
    }

    public void OnReturn()
    {
        Close();
    }

    private void SetupPlayer()
    {
        dragYesButton.interactable = !deck.gameOptions.useCardDragControls;
        dragNoButton.interactable = deck.gameOptions.useCardDragControls;
    }

    public void OnDragOption()
    {
        menuCommon.PlaySelectSound();

        deck.gameOptions.useCardDragControls = true;

        dragYesButton.interactable = !deck.gameOptions.useCardDragControls;
        dragNoButton.interactable = deck.gameOptions.useCardDragControls;
    }

    public void OnDoubleClickOption()
    {
        menuCommon.PlaySelectSound();

        deck.gameOptions.useCardDragControls = false;

        dragYesButton.interactable = !deck.gameOptions.useCardDragControls;
        dragNoButton.interactable = deck.gameOptions.useCardDragControls;
    }

    public void OnInstaDeathActivate()
    {
        menuCommon.PlaySelectSound();

        deck.gameOptions.instaDeathEnabled = true;

        instaYesButton.interactable = deck.gameOptions.instaDeathEnabled;
        instaNoButton.interactable = !deck.gameOptions.instaDeathEnabled;
    }

    public void OnInstaDeathDeactivate()
    {
        menuCommon.PlaySelectSound();

        deck.gameOptions.instaDeathEnabled = false;

        instaYesButton.interactable = deck.gameOptions.instaDeathEnabled;
        instaNoButton.interactable = !deck.gameOptions.instaDeathEnabled;
    }
}
