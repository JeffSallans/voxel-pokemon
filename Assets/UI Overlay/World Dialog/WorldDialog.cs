using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public enum MessageType { BottomMessage, PromptMessage }

/// <summary>
/// Details for the world message display event
/// </summary>
public class MessageEvent
{
    /// <summary>
    /// Text to display (must be less than 5 lines)
    /// </summary>
    public string message;

    /// <summary>
    /// Person label to display (must be less than 50 characters)
    /// </summary>
    public string messagePersonName;

    /// <summary>
    /// Audio to play upon display, if not the default talk sound.
    /// </summary>
    public AudioClip messageSound;

    /// <summary>
    /// Which message box to use to display
    /// </summary>
    public MessageType messageType;

    /// <summary>
    /// Function to call after displayed
    /// </summary>
    public Func<bool, bool> eventToCallAfterMessage;

    /// <summary>
    /// Function to call if yes was selected
    /// </summary>
    public Func<bool> confirmationEventToCallAfterMessage;

    /// <summary>
    /// Function to call if no was selected
    /// </summary>
    public Func<bool> cancelEventToCallAfterMessage;
}

/// <summary>
/// Used to hook dialog UI to the game scripts
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class WorldDialog : MonoBehaviour
{
    /// <summary>
    /// The dialog background to show
    /// </summary>
    public GameObject dialogObject;

    /// <summary>
    /// Click Capture Element
    /// </summary>
    public GameObject clickCaptureObject;

    /// <summary>
    /// The textbox to update
    /// </summary>
    public TextMeshProUGUI textboxObject;

    /// <summary>
    /// The dialog background to show
    /// </summary>
    public GameObject promptDialogBackground;

    /// <summary>
    /// The textbox to update with the dialog message
    /// </summary>
    public TextMeshProUGUI promptTextboxObject;

    /// <summary>
    /// The option dialog background to show
    /// </summary>
    public GameObject promptOptionBackground;

    /// <summary>
    /// The options to display
    /// </summary>
    public List<GameObject> promptOptionObject;

    /// <summary>
    /// True if we should wait for keyboard presses
    /// </summary>
    public bool promptIsCapturingInput = false;

    /// <summary>
    /// The current selection
    /// </summary>
    public int promptCurrentSelection = 0;

    /// <summary>
    /// True when it is the first iteration capturing input. Use this to avoid key presses triggering
    /// </summary>
    private bool isFirstCycleCapturingInput = true;

    /// <summary>
    /// The message to display
    /// </summary>
    private MessageEvent currentmessage {
        get
        {
            return (messages.Count > 0) ? messages.Peek() : null;
        }
    }

    /// <summary>
    /// The message queue to display for each click
    /// </summary>
    private Queue<MessageEvent> messages;

    /// <summary>
    /// Sound to play when the dialog opens
    /// </summary>
    public AudioSource talkAudioSource;

    /// <summary>
    /// Sound to play when selecting a yes/no option
    /// </summary>
    public AudioSource menuingAudioSource;

    /// <summary>
    /// Sounds to play when selecting yes
    /// </summary>
    public AudioSource selectAudioSource;

    /// <summary>
    /// Sounds to play when selecting no
    /// </summary>
    public AudioSource cancelAudioSource;

    /// <summary>
    /// Background for name
    /// </summary>
    public GameObject personNameObject;

    /// <summary>
    /// Text that hold the name
    /// </summary>
    public TextMeshProUGUI personNameTextbox;

    private void Awake()
    {
        messages = new Queue<MessageEvent>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (promptIsCapturingInput && isFirstCycleCapturingInput)
        {
            isFirstCycleCapturingInput = false;
        }
        else if (promptIsCapturingInput) {
            isFirstCycleCapturingInput = false;

            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.E))
            {
                promptOptionObject[promptCurrentSelection].GetComponent<Animator>().SetTrigger("Pressed");
                PromptCaptureClick();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                // Play menu sound
                menuingAudioSource.Play();

                promptOptionObject[promptCurrentSelection].GetComponent<Animator>().SetTrigger("Normal");
                promptCurrentSelection = Mathf.Abs(promptCurrentSelection - 1) % promptOptionObject.Count;
                promptOptionObject[promptCurrentSelection].GetComponent<Animator>().SetTrigger("Highlighted");
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                // Play menu sound
                menuingAudioSource.Play();

                promptOptionObject[promptCurrentSelection].GetComponent<Animator>().SetTrigger("Normal");
                promptCurrentSelection = (promptCurrentSelection + 1) % promptOptionObject.Count;
                promptOptionObject[promptCurrentSelection].GetComponent<Animator>().SetTrigger("Highlighted");
            }
        } else
        {
            isFirstCycleCapturingInput = true;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CaptureClick();
            }
        }
    }

    /// <summary>
    /// Show all the dialog with the given text. Can be called if an dialog is already open to queue a message. Callback is called when the user clicks after reading the message
    /// </summary>
    /// <param name="iEvent">The event to display messages for</param>
    public async Task ShowAllMessagesAsync(InteractionEvent iEvent) {
        for (var index = 0; index < iEvent.message.Count; index++)
        {
            var message = iEvent.message[index];
            AudioClip sound = null;
            if (index < iEvent.messageSounds.Count) sound = iEvent.messageSounds[index];

            string personName = null;
            if (index < iEvent.messageName.Count) personName = iEvent.messageName[index];

            await ShowMessageAsync(message, sound, personName);
        }
    }

    /// <summary>
    /// Show the dialog with the given text. Can be called if an dialog is already open to queue a message. Callback is called when the user clicks after reading the message
    /// </summary>
    /// <param name="text">Message text to display (must be less than 180 characters or 5 lines)</param>
    /// <param name="sound">If non-null, AudioClip to play instead of the default.</param>
    /// <param name="personName">If non-null, a person name label will show with the text</param>
    public async Task ShowMessageAsync(string text, AudioClip sound = null, string personName = null)
    {
        var dialogFinished = false;
        ShowMessage(text, (t) =>
        {
            dialogFinished = true;
            return true;
        }, sound, personName);

        while(!dialogFinished)
        {
            await Task.Yield();
        }
    }

    /// <summary>
    /// Show the dialog with the given text. Can be called if an dialog is already open to queue a message. Callback is called when the user clicks after reading the message
    /// </summary>
    /// <param name="text">Message text to display (must be less than 180 characters or 5 lines)</param>
    /// <param name="callback">Function to trigger after dialog is read</param>
    /// <param name="sound">If non-null, AudioClip to play instead of the default.</param>
    /// <param name="personName">If non-null, a person name label will show with the text</param>
    public void ShowMessage(string text, Func<bool, bool> callback = null, AudioClip sound = null, string personName = null)
    {
        print($"PROMPT SHOW: {text} ({(sound == null ? "default sound" : sound.name)})");
        var newMessage = new MessageEvent
        {
            message = text,
            messagePersonName = personName,
            messageSound = sound,
            messageType = MessageType.BottomMessage,
            eventToCallAfterMessage = callback
        };
        messages.Enqueue(newMessage);

        // Open if current message is null
        if (currentmessage == newMessage)
        {
            OpenNextMessage();
        }
    }

    /// <summary>
    /// Show the dialog with the given text. Can be called if an dialog is already open.
    /// </summary>
    private void OpenNextMessage()
    {
        // Play talk sound
        if (currentmessage.messageSound == null)
        {
            talkAudioSource.Play();
        }
        else
        {
            var playerObject = FindObjectOfType<CharacterController>().gameObject;
            var gameOptions = playerObject.GetComponent<GameOptions>();

            talkAudioSource.PlayOneShot(currentmessage.messageSound, gameOptions.musicVolume);
        }

        // Open dialog if it is not already open
        if (currentmessage.messageType == MessageType.BottomMessage)
        {
            dialogObject.SetActive(true);
            clickCaptureObject.SetActive(true);

            textboxObject.text = UnicodeUtil.replaceWithUnicode(currentmessage?.message);

            promptDialogBackground.SetActive(false);
            promptOptionBackground.SetActive(false);
            promptOptionObject.ForEach(p => p.SetActive(false));
            promptIsCapturingInput = false;

            promptTextboxObject.gameObject.SetActive(false);
            promptTextboxObject.text = "";

            if (currentmessage.messagePersonName != null && currentmessage.messagePersonName != "")
            {
                personNameObject.SetActive(true);
                personNameTextbox.gameObject.SetActive(true);
                personNameTextbox.text = UnicodeUtil.replaceWithUnicode(currentmessage.messagePersonName);
            }
        }
        else
        {
            dialogObject.SetActive(false);
            clickCaptureObject.SetActive(false);

            textboxObject.text = "";

            promptDialogBackground.SetActive(true);
            promptOptionBackground.SetActive(true);
            promptOptionObject.ForEach(p => p.SetActive(true));
            promptIsCapturingInput = true;
            promptCurrentSelection = 0;
            promptOptionObject[promptCurrentSelection].GetComponent<Animator>().SetTrigger("Highlighted");

            promptTextboxObject.gameObject.SetActive(true);
            promptTextboxObject.text = UnicodeUtil.replaceWithUnicode(currentmessage?.message);

            if (currentmessage.messagePersonName != null && currentmessage.messagePersonName != "")
            {
                personNameObject.SetActive(true);
                personNameTextbox.gameObject.SetActive(true);
                personNameTextbox.text = UnicodeUtil.replaceWithUnicode(currentmessage.messagePersonName);
            }
        }


        // Show text if not displayed
        textboxObject.gameObject.SetActive(true);

        // Check if we are capturing decisions
        promptIsCapturingInput = (currentmessage.messageType == MessageType.PromptMessage);
    }

    /// <summary>
    /// Show the dialog with the given text. Can be called if an dialog is already open.
    /// </summary>
    public void CaptureClick()
    {
        CaptureClick(true);
    }


    /// <summary>
    /// Show the dialog with the given text. Can be called if an dialog is already open.
    /// </summary>
    public void CaptureClick(bool promptSelectedYes)
    {
        if (currentmessage?.eventToCallAfterMessage != null)
        {
            currentmessage.eventToCallAfterMessage(promptSelectedYes);
        }
        if (messages.Count > 0) { messages.Dequeue(); }

        // Check to display next message
        var possibleNewMessage = (messages.Count > 0) ? messages.Peek() : null;
        if (possibleNewMessage != null)
        {
            OpenNextMessage();
        }
        // If none exit
        else
        {
            HideDialog();
        }
    }

    /// <summary>
    /// Hides the dialog
    /// </summary>
    private void HideDialog()
    {
        print("HIDE DIALOG");
        dialogObject.SetActive(false);
        clickCaptureObject.SetActive(false);
        textboxObject.text = "";
        textboxObject.gameObject.SetActive(false);

        promptDialogBackground.SetActive(false);
        promptOptionBackground.SetActive(false);
        promptOptionObject.ForEach(p => p.SetActive(false));
        promptIsCapturingInput = false;

        promptTextboxObject.text = "";
        promptTextboxObject.gameObject.SetActive(false);

        personNameObject.SetActive(false);
        personNameTextbox.gameObject.SetActive(false);
        personNameTextbox.text = "";
    }

    /// <summary>
    /// Clears out all messages and hides the dialog
    /// </summary>
    public void CloseDialog()
    {
        messages.Clear();
        HideDialog();
    }

    /// <summary>
    /// Show the dialog with a yes/no prompt. Can be called if an dialog is already open to queue a message.
    /// </summary>
    /// <param name="text">Message text to display (must be less than 180 characters or 5 lines)</param>
    /// <param name="options">The options to show to select</param>
    /// <param name="onConfirm">Function to trigger on space press when Yes is selected</param>
    /// <param name="onCancel">Function to trigger on space press when No is selected</param>
    /// <param name="callback">Function to trigger after dialog is read, return true if the user selected yes and false if the user selects no</param>
    /// <param name="sound">If non-null, AudioClip to play instead of the default.</param>
    /// <param name="personName">If non-null, a person name label will show with the text</param>
    public void PromptShowMessage(string text, List<string> options, Func<bool> onConfirm, Func<bool> onCancel = null, Func<bool, bool> callback = null, AudioClip sound = null, string personName = null)
    {
        print($"PROMPT SHOW: {text} ({(sound == null ? "default sound" : sound.name)})");
        var newMessage = new MessageEvent
        {
            message = text,
            messagePersonName = personName,
            messageSound = sound,
            messageType = MessageType.PromptMessage,
            confirmationEventToCallAfterMessage = onConfirm,
            cancelEventToCallAfterMessage = onCancel,
            eventToCallAfterMessage = callback
        };
        messages.Enqueue(newMessage);

        // Open if current message is null
        if (currentmessage == newMessage)
        {
            OpenNextMessage();
        }
    }

    /// <summary>
    /// Show the dialog with a yes/no prompt. Can be called if an dialog is already open to queue a message.
    /// </summary>
    /// <param name="text">Message text to display (must be less than 180 characters or 5 lines)</param>
    /// <param name="options">The options to show to select</param>
    /// <param name="onConfirm">Function to trigger on space press when Yes is selected</param>
    /// <param name="onCancel">Function to trigger on space press when No is selected</param>
    /// <param name="sound">If non-null, AudioClip to play instead of the default.</param>
    /// <param name="personName">If non-null, a person name label will show with the text</param>
    public async Task<bool> PromptShowMessageAsync(string text, List<string> options, Func<bool> onConfirm, Func<bool> onCancel = null, AudioClip sound = null, string personName = null)
    {
        var dialogFinished = false;
        var promptSelectedYes = true;
        PromptShowMessage(text, options, onConfirm, onCancel, (selectedYes) =>
        {
            dialogFinished = true;
            promptSelectedYes = selectedYes;
            return true;
        }, sound, personName);

        while (!dialogFinished)
        {
            await Task.Yield();
        }
 
        return promptSelectedYes;
    }

    /// <summary>
    /// Handling the prompt response
    /// </summary>
    public void PromptCaptureClick()
    {
        var selectedYes = true;
        if (promptCurrentSelection == 0)
        {
            // Play select sound
            selectAudioSource.Play();

            currentmessage.confirmationEventToCallAfterMessage();
        }
        else if (promptCurrentSelection == 1)
        {
            // Play cancel sound
            cancelAudioSource.Play();

            currentmessage.cancelEventToCallAfterMessage();
            selectedYes = false;
        }

        CaptureClick(selectedYes);
    }
}
