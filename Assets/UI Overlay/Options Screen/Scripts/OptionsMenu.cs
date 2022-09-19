using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class OptionsMenu : MonoBehaviour
{
    PlayerDeck deck;

    /// <summary>
    /// All the menues for the party pokemon
    /// </summary>
    public Button dragYesButton;

    /// <summary>
    /// All the menues for the party pokemon
    /// </summary>
    public Button dragNoButton;

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
}
