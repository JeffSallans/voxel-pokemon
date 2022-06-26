using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

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
    /// Function to call after displayed
    /// </summary>
    public Func<bool> eventToCallAfterMessage;
}

/// <summary>
/// Used to hook dialog UI to the game scripts
/// </summary>
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        // Open dialog if it is not already open
        dialogObject.SetActive(true);
        clickCaptureObject.SetActive(true);

        textboxObject.text = UnicodeUtil.replaceWithUnicode(currentmessage?.message);

        // Show text if not displayed
        textboxObject.gameObject.SetActive(true);        
    }

    /// <summary>
    /// Show the dialog with the given text. Can be called if an dialog is already open.
    /// </summary>
    public void CaptureClick()
    {
        if (currentmessage.eventToCallAfterMessage != null)
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
    }

    /// <summary>
    /// Clears out all messages and hides the dialog
    /// </summary>
    public void CloseDialog()
    {
        messages.Clear();
        HideDialog();
    }
}
