using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to hook dialog UI to the game scripts
/// </summary>
public class WorldDialog : MonoBehaviour
{
    /// <summary>
    /// The textbox to update
    /// </summary>
    public UnityEngine.UI.Text textboxObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Show the dialog with the given text. Can be called if an dialog is already open.
    /// </summary>
    /// <param name="text">Max 80 characters a line and 3 lines.</param>
    public void OpenDialog(string text)
    {
        textboxObject.text = text;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the dialog
    /// </summary>
    public void CloseDialog()
    {
        gameObject.SetActive(false);
    }
}
