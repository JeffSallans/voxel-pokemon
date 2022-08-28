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
    /// Which message box to use to display
    /// </summary>
    public MessageType messageType;

    /// <summary>
    /// Function to call after displayed
    /// </summary>
    public Func<bool> eventToCallAfterMessage;

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
    private Queue<MessageEvent> messages = new Queue<MessageEvent>();

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

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
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
    /// Show the dialog with the given text. Can be called if an dialog is already open to queue a message. Callback is called when the user clicks after reading the message
    /// </summary>
    /// <param name="text">Message text to display (must be less than 180 characters or 5 lines)</param>
    public async Task ShowMessageAsync(string text)
    {
        var dialogFinished = false;
        ShowMessage(text, () =>
        {
            dialogFinished = true;
            return true;
        });

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
    public void ShowMessage(string text, Func<bool> callback = null)
    {
        print("DIALOG SHOW: "+text);
        var newMessage = new MessageEvent();
        newMessage.message = text;
        newMessage.messageType = MessageType.BottomMessage;
        newMessage.eventToCallAfterMessage = callback;
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
    /// <param name="text">Max 80 characters a line and 3 lines.</param>
    private void OpenNextMessage()
    {
        // Play talk sound
        talkAudioSource.Play();

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
        if (currentmessage?.eventToCallAfterMessage != null)
        {
            currentmessage.eventToCallAfterMessage();
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
    /// <param name="callback">Function to trigger after dialog is read</param>
    public void PromptShowMessage(string text, List<string> options, Func<bool> onConfirm, Func<bool> onCancel = null, Func<bool> callback = null)
    {
        print("DIALOG SHOW: " + text);
        var newMessage = new MessageEvent();
        newMessage.message = text;
        newMessage.messageType = MessageType.PromptMessage;
        newMessage.confirmationEventToCallAfterMessage = onConfirm;
        newMessage.cancelEventToCallAfterMessage = onCancel;
        newMessage.eventToCallAfterMessage = callback;
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
    public async Task PromptShowMessageAsync(string text, List<string> options, Func<bool> onConfirm, Func<bool> onCancel = null)
    {
        var dialogFinished = false;
        PromptShowMessage(text, options, onConfirm, onCancel, () =>
        {
            dialogFinished = true;
            return true;
        });

        while (!dialogFinished)
        {
            await Task.Yield();
        }
    }

    /// <summary>
    /// Handling the prompt response
    /// </summary>
    public void PromptCaptureClick()
    {
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
        }

        CaptureClick();
    }
}
