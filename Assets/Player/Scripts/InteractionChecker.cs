using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ThirdPersonMovement))]
public class InteractionChecker : MonoBehaviour
{
    public float interactableDistance = 30;

    public bool removeCursor = true;

    public bool inDebugMode = false;

    public float hoverRed;
    public float hoverGreen;
    public float hoverBlue;
    public float hoverAlpha;

    private ThirdPersonMovement thirdPersonMovement;

    public Camera playerCamera;

    public WorldDialog worldDialog;

    public TextMeshProUGUI crosshairText;

    public float red;
    public float green;
    public float blue;
    public float alpha;

    public TextMeshProUGUI interactionHoverText;

    /// <summary>
    /// Target the user "hovers" on
    /// </summary>
    public InteractionEvent hoverPossibleEvent;

    /// <summary>
    /// The event that is active, to avoid triggering it twice
    /// </summary>
    private InteractionEvent activeEvent = null;

    private Dictionary<string, string> interactionFlags = new Dictionary<string, string>();

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = !removeCursor;
        red = crosshairText.color.r;
        green = crosshairText.color.g;
        blue = crosshairText.color.b;
        alpha = crosshairText.color.a;

        thirdPersonMovement = GetComponent<ThirdPersonMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inDebugMode)
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
                print("I'm looking at " + hit.transform.name);
            else
                print("I'm looking at nothing!");
        }

        var hoverInteraction = GetHoverInteraction();
        OnHoverInteraction(hoverInteraction);
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnInteractionClick(hoverPossibleEvent);
        }
    }

    /// <summary>
    /// Checks if the user is hovering on something to interact with
    /// </summary>
    private InteractionEvent GetHoverInteraction()
    {
        RaycastHit hit = new RaycastHit();
        var raycastHit = Physics.Raycast(playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out hit, interactableDistance);
        
        // Check if ray hit
        if (!raycastHit) return null;

        // Check if we can get component from hit
        var result = hit.collider.GetComponent<InteractionEvent>();

        return result;
    }

    /// <summary>
    /// When the user "hovers" on something to interact with
    /// </summary>
    /// <param name="iEvent"></param>
    public void OnHoverInteraction(InteractionEvent iEvent)
    {
        if (iEvent == null || activeEvent != null)
        {
            hoverPossibleEvent = null;
            interactionHoverText.text = "";
            crosshairText.color = new Color(red, green, blue, alpha);
        }
        else if (iEvent != hoverPossibleEvent)
        {
            hoverPossibleEvent = iEvent;
            interactionHoverText.text = iEvent.interactionHint;
            crosshairText.color = new Color(hoverRed, hoverGreen, hoverBlue, hoverAlpha);
        }
    }

    /// <summary>
    /// When the user interacts with something
    /// </summary>
    /// <param name="iEvent"></param>
    private void OnInteractionClick(InteractionEvent iEvent)
    {
        if (hoverPossibleEvent == null) { return; }
        if (hoverPossibleEvent.eventType == "Message")
        {
            OnMessage(hoverPossibleEvent);
        } else if (hoverPossibleEvent.eventType == "Battle")
        {
            OnBattle(hoverPossibleEvent);
        }
        hoverPossibleEvent = null;
    }

    private void OnMessage(InteractionEvent iEvent)
    {
        activeEvent = hoverPossibleEvent;
        if (iEvent.animator) { iEvent.animator.SetBool(iEvent.animationBooleanName, true); }

        thirdPersonMovement.enabled = false;
        OnMessageHelper(iEvent, 0);
    }

    private void OnMessageHelper(InteractionEvent iEvent, int messageIndex)
    {
        // Base case the message index is the end of the list
        if (messageIndex == iEvent.message.Count)
        {
            thirdPersonMovement.enabled = true;
            if (iEvent.animator) { iEvent.animator.SetBool(iEvent.animationBooleanName, false); }
            activeEvent = null;
            return;
        }

        // Recursive case: display message and then increment index
        worldDialog.ShowMessage(iEvent.message[messageIndex], () =>
        {
            OnMessageHelper(iEvent, messageIndex + 1);
            return true;
        });
    }

    private void OnBattle(InteractionEvent iEvent)
    {
        activeEvent = hoverPossibleEvent;
        if (iEvent.animator) { iEvent.animator.SetBool(iEvent.animationBooleanName, true); }

        thirdPersonMovement.enabled = false;
        worldDialog.ShowMessage(iEvent.message[0], () =>
        {
            thirdPersonMovement.enabled = true;

            if (iEvent.animator) { iEvent.animator.SetBool(iEvent.animationBooleanName, false); }
            SceneManager.LoadScene(iEvent.sceneName);
            activeEvent = null;
            return true;
        });

    }
}
