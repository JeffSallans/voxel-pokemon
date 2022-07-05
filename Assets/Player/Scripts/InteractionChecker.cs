using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Threading.Tasks;

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
    /// Animator with onFadeIn and onFadeOut triggers to be called on scene transitions
    /// </summary>
    public FadeInOnSceneLoad sceneTransitionAnimator;

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
    /// The reference to the previous scene active event
    /// </summary>
    private InteractionEvent prevActiveEvent;

    /// <summary>
    /// The key is the interaction state name, the value includes if it is enabled or not and if the game object is active or not
    /// </summary>
    private Dictionary<string, InteractionEventNoComponent> prevInteractionEventStates;

    /// <summary>
    /// The reference to the previous scene opponent
    /// </summary>
    private GameObject prevSceneOpponent;

    /// <summary>
    /// The reference to the previous scene player
    /// </summary>
    private GameObject prevScenePlayer;
    /// <summary>
    /// The location of the previous player to account for any elevation if the player fell down
    /// </summary>
    private Vector3 prevScenePlayerPos;

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

        if (sceneTransitionAnimator == null)
        {
            sceneTransitionAnimator = GameObject.FindObjectOfType<FadeInOnSceneLoad>();
        }

        Cursor.visible = !removeCursor;
        red = crosshairText.color.r;
        green = crosshairText.color.g;
        blue = crosshairText.color.b;
        alpha = crosshairText.color.a;

        thirdPersonMovement = GetComponent<ThirdPersonMovement>();

        prevInteractionEventStates = new Dictionary<string, InteractionEventNoComponent>();
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
        var resultList = hit.collider.GetComponents<InteractionEvent>();
        var result = resultList.FirstOrDefault(e => e.enabled);

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
    /// Call to trigger an interaction event
    /// </summary>
    /// <param name="iEvent"></param>
    public void OnInteractionEvent(InteractionEvent iEvent)
    {
        hoverPossibleEvent = iEvent;
        OnInteractionClick(iEvent);
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
            OnMessageAsync(hoverPossibleEvent);
        } else if (hoverPossibleEvent.eventType == "Battle")
        {
            OnBattle(hoverPossibleEvent);
        } else if (hoverPossibleEvent.eventType == "SceneChange")
        {
            OnSceneChange(hoverPossibleEvent);
        }
        hoverPossibleEvent = null;
    }

    private async void OnMessageAsync(InteractionEvent iEvent)
    {
        activeEvent = hoverPossibleEvent;
        if (iEvent.animator) { iEvent.animator.SetBool(iEvent.animationBooleanName, true); }

        thirdPersonMovement.enabled = false;
        foreach(var message in iEvent.message)
        {
            await worldDialog.ShowMessageAsync(message);
        }

        postInteracationChanges(iEvent);

        // Finish reading state
        thirdPersonMovement.enabled = true;
        if (iEvent.animator) { iEvent.animator.SetBool(iEvent.animationBooleanName, false); }
        activeEvent = null;
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
            StartCoroutine(LoadScene(iEvent, iEvent.sceneName, opponent, true));
            activeEvent = null;
            return true;
        });

    }

    private void OnSceneChange(InteractionEvent iEvent)
    {
        activeEvent = hoverPossibleEvent;
        if (iEvent.animator) { iEvent.animator.SetBool(iEvent.animationBooleanName, true); }

        thirdPersonMovement.enabled = false;
        worldDialog.ShowMessage(iEvent.message[0], () =>
        {
            thirdPersonMovement.enabled = true;

            if (iEvent.animator) { iEvent.animator.SetBool(iEvent.animationBooleanName, false); }
            StartCoroutine(LoadScene(iEvent, iEvent.sceneName));
            activeEvent = null;
            return true;
        });

    }

    IEnumerator LoadScene(InteractionEvent iEvent, string sceneName, GameObject opponent = null, bool movePlayerToNewScene = false)
    {
        sceneTransitionAnimator.FadeOut();
        yield return new WaitForSeconds(1);

        // Set Previous Scene data
        prevActiveEvent = iEvent;
        var interactionEvents = GameObject.FindObjectsOfType<InteractionEvent>();
        print(interactionEvents.Length);
        prevInteractionEventStates = interactionEvents.Select(i => i.GetInteractionEventWithoutComponent()).ToDictionary(i => i.eventName);
        prevSceneName = SceneManager.GetActiveScene().name;
        prevSceneOpponent = (opponent) ? Instantiate(opponent) : null;
        prevScenePlayer = GameObject.Find("Player Dad");
        prevScenePlayerPos = prevScenePlayer.transform.position;
        prevSceneCameraPosition = Camera.main.gameObject.transform.position;
        prevSceneCameraRotation = Camera.main.gameObject.transform.rotation;

        // Copy opponent to be moved as a global value
        if (prevSceneOpponent) { prevSceneOpponent.name = "Opponent"; }

        DontDestroyOnLoad(prevScenePlayer);
        prevActiveEvent.gameObject.transform.parent = null;
        DontDestroyOnLoad(prevActiveEvent.gameObject);
        prevActiveEvent.gameObject.SetActive(false);
        if (prevSceneOpponent) { DontDestroyOnLoad(prevSceneOpponent); }
        SceneManager.LoadScene(sceneName);

        // If player is not moving to new scene copy data over
        if (!movePlayerToNewScene)
        {
            // Wait for new scene to load
            var newScene = SceneManager.GetActiveScene();
            while (newScene.name != sceneName)
            {
                yield return new WaitForSeconds(0.3f);
                newScene = SceneManager.GetActiveScene();
            }

            // Set new scene animator
            sceneTransitionAnimator = GameObject.FindObjectOfType<FadeInOnSceneLoad>();
            
            // Find root objects
            var rootObjects = newScene.GetRootGameObjects();
            foreach (var obj in rootObjects)
            {
                var battleGameBoard = obj.GetComponent<BattleGameBoard>();
                var deckBuildGameBoard = obj.GetComponent<DeckBuilderAddCard>();
                if (obj.name == "Battle Canvas" && battleGameBoard)
                {
                    battleGameBoard.playerInteractionChecker = this;
                }
                else if (obj.name == "Battle Canvas" && deckBuildGameBoard)
                {
                    deckBuildGameBoard.playerInteractionChecker = this;
                }
            }
        }
        else
        {
            // Wait for new scene to load
            var newScene = SceneManager.GetActiveScene();
            while (newScene.name != sceneName)
            {
                yield return new WaitForSeconds(0.3f);
                newScene = SceneManager.GetActiveScene();
            }

            // Set new scene animator
            sceneTransitionAnimator = GameObject.FindObjectOfType<FadeInOnSceneLoad>();
        }

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
        sceneTransitionAnimator.FadeOut();
        yield return new WaitForSeconds(1f);

        if (prevSceneOpponent) { Destroy(prevSceneOpponent); }
        SceneManager.LoadScene(prevSceneName);

        // Remove previous scene player
        var newScene = SceneManager.GetActiveScene();
        while (newScene.name != prevSceneName)
        {
            yield return new WaitForSeconds(0.3f);
            newScene = SceneManager.GetActiveScene();
        }

        // Set new scene animator
        sceneTransitionAnimator = GameObject.FindObjectOfType<FadeInOnSceneLoad>();

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

                // Setup pos
                gameObject.transform.position = prevScenePlayerPos;
            }
        }

        // Update the game events when returning to the scene
        var interactionEventList = GameObject.FindObjectsOfType<InteractionEvent>();
        foreach (var interactionEvent in interactionEventList)
        {
            // Update the other events
            InteractionEventNoComponent prevEventState = null;
            prevInteractionEventStates.TryGetValue(interactionEvent.eventName, out prevEventState);

            // If event state doesn't exist it must of been deleted
            if (prevEventState == null)
            {
                Destroy(interactionEvent.gameObject);
                SceneManager.MoveGameObjectToScene(prevActiveEvent.gameObject, SceneManager.GetActiveScene());             
            }
            else
            {
                interactionEvent.enabled = prevEventState.interactionEventEnabled;
                interactionEvent.gameObject.SetActive(prevEventState.gameObjectActive);
            }

            // Update the active event
            if (interactionEvent?.gameObject?.name == prevActiveEvent?.gameObject?.name && interactionEvent?.gameObject.GetComponent<InteractionEvent>()?.eventName == prevActiveEvent?.eventName)
            {
                postInteracationChanges(interactionEvent);
            }
        }

        // Remove previous interaction
        if (prevActiveEvent && prevActiveEvent.removeOnReturn && prevActiveEvent.gameObject)
        {
            Destroy(prevActiveEvent.gameObject);
        } else if (prevActiveEvent && prevActiveEvent.disableOnReturn)
        {
            prevActiveEvent.gameObject.SetActive(true);
            prevActiveEvent.enabled = false;
            if (prevActiveEvent.nextInteractionEvent) prevActiveEvent.nextInteractionEvent.enabled = true;
            Destroy(prevActiveEvent.gameObject);
        }

        // Disable previous interaction

        prevSceneName = "";
        this.enabled = true;
    }

    /// <summary>
    /// Updates all the interaction events with the results from the events
    /// </summary>
    /// <param name="iEvent"></param>
    private void postInteracationChanges(InteractionEvent iEvent)
    {
        // When the event triggers a different event
        if (iEvent.disableOnReturn)
        {
            iEvent.enabled = false;
        }

        // When removing to avoid replay
        if (iEvent.removeOnReturn)
        {
            Destroy(iEvent?.gameObject);
        }

        // Remove an interaction
        if (iEvent.altDisableInteractionEvent)
        {
            iEvent.altDisableInteractionEvent.enabled = false;
        }

        // Enable a new interaction
        if (iEvent.nextInteractionEvent)
        {
            iEvent.nextInteractionEvent.gameObject.SetActive(true);
            iEvent.nextInteractionEvent.enabled = true;
        }

        // Enable a second interaction
        if (iEvent.nextInteractionEvent2)
        {
            iEvent.nextInteractionEvent2.gameObject.SetActive(true);
            iEvent.nextInteractionEvent2.enabled = true;
        }

        // Trigger another event
        if (iEvent.autoTriggerInteractionEvent)
        {
            iEvent.autoTriggerInteractionEvent.gameObject.SetActive(true);
            iEvent.autoTriggerInteractionEvent.enabled = true;
            OnInteractionEvent(iEvent.autoTriggerInteractionEvent);
        }
    }
}
