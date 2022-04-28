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

    /// <summary>
    /// Trakes the scene loading in the background
    /// </summary>
    private AsyncOperation sceneAsync;

    /// <summary>
    /// The reference to the previous scene opponent
    /// </summary>
    private GameObject prevSceneOpponent;

    /// <summary>
    /// The reference to the previous scene player
    /// </summary>
    private GameObject prevScenePlayer;

    /// <summary>
    /// The reference to the previous scene name
    /// </summary>
    private string prevSceneName;

    /// <summary>
    /// The reference to the previous scene camera position to avoid a sliding transition on load
    /// </summary>
    private Vector3 prevSceneCameraPosition;
    private Quaternion prevSceneCameraRotation;

    // Start is called before the first frame update
    void Start()
    {
        if (interactionHoverText == null || crosshairText == null)
        {
            print("Interaction Checker: Disabled - interactionHoverText and crosshairText are undefined");
            this.enabled = false;
            return;
        }

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
        if (playerCamera == null) return null;

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
            var opponent = iEvent.gameObject;
            StartCoroutine(LoadScene(iEvent.sceneName, opponent));
            activeEvent = null;
            return true;
        });

    }

    IEnumerator LoadScene(string sceneName, GameObject opponent)
    {
        yield return new WaitForSeconds(1);

        // Set Previous Scene data
        prevSceneName = SceneManager.GetActiveScene().name;
        prevSceneOpponent = Instantiate(opponent);
        prevScenePlayer = GameObject.Find("Player Dad");
        prevSceneCameraPosition = Camera.main.gameObject.transform.position;
        prevSceneCameraRotation = Camera.main.gameObject.transform.rotation;

        // Copy opponent to be moved as a global value
        prevSceneOpponent.name = "Opponent";

        DontDestroyOnLoad(prevScenePlayer);
        DontDestroyOnLoad(prevSceneOpponent);
        SceneManager.LoadScene(sceneName);
        this.enabled = false;
        
    }

    /// <summary>
    /// Return to the previous scene
    /// </summary>
    /// <returns></returns>
    public void LoadPreviousScene()
    {
        if (prevSceneName == null || prevSceneName == "") return;
        StartCoroutine(LoadPreviousSceneHelper());
    }

    IEnumerator LoadPreviousSceneHelper()
    {
        yield return new WaitForSeconds(0.5f);

        Destroy(prevSceneOpponent);
        SceneManager.LoadScene(prevSceneName);

        // Remove previous scene player
        var newScene = SceneManager.GetActiveScene();
        while (newScene.name != prevSceneName)
        {
            yield return new WaitForSeconds(0.3f);
            newScene = SceneManager.GetActiveScene();
        }
        var rootObjects = newScene.GetRootGameObjects();
        foreach (var obj in rootObjects)
        {
            if (obj.name == "Player Dad")
            {
                // Copy things over
                gameObject.GetComponent<ThirdPersonMovement>().cam = obj.GetComponent<ThirdPersonMovement>().cam;
                playerCamera = obj.GetComponent<InteractionChecker>().playerCamera;
                worldDialog = obj.GetComponent<InteractionChecker>().worldDialog;
                crosshairText = obj.GetComponent<InteractionChecker>().crosshairText;
                interactionHoverText = obj.GetComponent<InteractionChecker>().interactionHoverText;

                // Delete duplicate
                Destroy(obj);

                // Move updated Player Dad into scene
                SceneManager.MoveGameObjectToScene(gameObject, newScene);

                // Setup third person camera
                Camera.main.gameObject.transform.position = prevSceneCameraPosition;
                Camera.main.gameObject.transform.rotation = prevSceneCameraRotation;
                GameObject.FindObjectOfType<Cinemachine.CinemachineFreeLook>().m_Follow = prevScenePlayer.transform.Find("camera_focus").transform;
                GameObject.FindObjectOfType<Cinemachine.CinemachineFreeLook>().m_LookAt = prevScenePlayer.transform.Find("camera_focus").transform;
            }
        }

        prevSceneName = "";
        this.enabled = true;
    }
}
