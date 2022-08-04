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
    /// The name of the previous scene active event
    /// </summary>
    public string prevActiveEventName;

    /// <summary>
    /// The key is the interaction state name, the value includes if it is enabled or not and if the game object is active or not
    /// </summary>
    private Dictionary<string, InteractionEventNoComponent> prevInteractionEventStates = null;

    /// <summary>
    /// The reference to the previous scene opponent
    /// </summary>
    private GameObject prevSceneOpponent;

    /// <summary>
    /// The reference to the previous scene player
    /// </summary>
    public GameObject prevScenePlayer;
    /// <summary>
    /// The location of the previous player to account for any elevation if the player fell down
    /// </summary>
    public Vector3 prevScenePlayerPos;

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

        if (prevInteractionEventStates == null)
        {
            prevInteractionEventStates = new Dictionary<string, InteractionEventNoComponent>();
        }

        prevActiveEventName = "";
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
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
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
        activeEvent = null;
        OnInteractionClick(iEvent);
    }

    /// <summary>
    /// When the user interacts with something
    /// </summary>
    /// <param name="iEvent"></param>
    private void OnInteractionClick(InteractionEvent iEvent)
    {
        if (hoverPossibleEvent == null || activeEvent != null) { return; }
        if (hoverPossibleEvent.eventTypeString == "Message")
        {
            OnMessageAsync(hoverPossibleEvent);
        }
        else if (hoverPossibleEvent.eventTypeString == "Battle")
        {
            OnBattle(hoverPossibleEvent);
        }
        else if (hoverPossibleEvent.eventTypeString == "SceneChange")
        {
            OnSceneChange(hoverPossibleEvent);
        }
        else if (hoverPossibleEvent.eventTypeString == "Question")
        {
            OnQuestionAsync(hoverPossibleEvent);
        } else if (hoverPossibleEvent.eventTypeString == "AddPokemon")
        {
            OnAddPokemonAsync(hoverPossibleEvent);
        }
    }

    private async void OnMessageAsync(InteractionEvent iEvent)
    {
        activeEvent = hoverPossibleEvent;
        if (iEvent.animator && iEvent.animationBooleanName != "") { iEvent.animator.SetBool(iEvent.animationBooleanName, true); }

        thirdPersonMovement.enabled = false;
        foreach(var message in iEvent.message)
        {
            await worldDialog.ShowMessageAsync(message);
        }

        postInteracationChanges(iEvent);
    }

    private void OnBattle(InteractionEvent iEvent)
    {
        activeEvent = hoverPossibleEvent;
        if (iEvent.animator && iEvent.animationBooleanName != "") { iEvent.animator.SetBool(iEvent.animationBooleanName, true); }

        thirdPersonMovement.enabled = false;
        worldDialog.ShowMessage(iEvent.message[0], () =>
        {
            thirdPersonMovement.enabled = true;

            if (iEvent.animator && iEvent.animationBooleanName != "") { iEvent.animator.SetBool(iEvent.animationBooleanName, false); }
            var opponent = iEvent.gameObject;
            StartCoroutine(LoadScene(iEvent, iEvent.sceneName, opponent, true));
            activeEvent = null;
            hoverPossibleEvent = null;
            return true;
        });

    }

    /// <summary>
    /// Shows a text box with a question at the end
    /// </summary>
    /// <param name="iEvent"></param>
    private async void OnQuestionAsync(InteractionEvent iEvent)
    {
        activeEvent = hoverPossibleEvent;
        if (iEvent.animator && iEvent.animationBooleanName != "") { iEvent.animator.SetBool(iEvent.animationBooleanName, true); }

        thirdPersonMovement.enabled = false;
        foreach (var message in iEvent.message)
        {
            if (message == iEvent.message.Last())
            {
                await worldDialog.PromptShowMessageAsync(message,
                    iEvent.options,
                    () => { iEvent.autoTriggerInteractionEvent = iEvent.optionFirstTriggerInteractionEvent; return true; },
                    () => { iEvent.autoTriggerInteractionEvent = iEvent.optionSecondTriggerInteractionEvent; return true; }
                );
            } else
            {
                await worldDialog.ShowMessageAsync(message);
            }
        }

        postInteracationChanges(iEvent);
    }

    /// <summary>
    /// Creates a pokemon from the given list and add to the player's party
    /// </summary>
    /// <param name="iEvent"></param>
    private async void OnAddPokemonAsync(InteractionEvent iEvent)
    {
        activeEvent = hoverPossibleEvent;
        if (iEvent.animator && iEvent.animationBooleanName != "") { iEvent.animator.SetBool(iEvent.animationBooleanName, true); }

        thirdPersonMovement.enabled = false;
        foreach (var message in iEvent.message)
        {
            await worldDialog.ShowMessageAsync(message);
        }

        // Select current player
        var player = GameObject.FindObjectOfType<PlayerDeck>();
        var pokemonWasAdded = player.AddPokemon(iEvent.pokemonToAdd);

        if (pokemonWasAdded)
        {
            await worldDialog.ShowMessageAsync(iEvent.pokemonToAddSuccessMessage);
        }
        else
        {
            await worldDialog.ShowMessageAsync(iEvent.pokemonToAddPartyFullMessage);
        }

        postInteracationChanges(iEvent);
    }

    private async void OnSceneChange(InteractionEvent iEvent)
    {
        activeEvent = hoverPossibleEvent;
        if (iEvent.animator && iEvent.animationBooleanName != "") { iEvent.animator.SetBool(iEvent.animationBooleanName, true); }

        thirdPersonMovement.enabled = false;
        foreach (var message in iEvent.message)
        {
            await worldDialog.ShowMessageAsync(message);
        }

        // Load Scene
        StartCoroutine(LoadScene(iEvent, iEvent.sceneName, null, iEvent.scenePlayerName != ""));

        thirdPersonMovement.enabled = true;
        if (iEvent.animator && iEvent.animationBooleanName != "") { iEvent.animator.SetBool(iEvent.animationBooleanName, false); }
        activeEvent = null;
    }

    IEnumerator LoadScene(InteractionEvent iEvent, string sceneName, GameObject opponent = null, bool movePlayerToNewScene = false)
    {
        sceneTransitionAnimator.FadeOut();
        yield return new WaitForSeconds(1);

        // Set Previous Scene data
        prevActiveEventName = iEvent.eventName;
        var interactionEvents = GameObject.FindObjectsOfType<InteractionEvent>(true);
        print(interactionEvents.Length);
        var interactionEventsStates = interactionEvents.Select(i => i.GetInteractionEventWithoutComponent()).ToList();
        foreach (var interaction in interactionEventsStates)
        { 
            prevInteractionEventStates[interaction.eventName] = interaction;
        }
        prevSceneName = SceneManager.GetActiveScene().name;
        prevScenePlayer = gameObject;
        prevScenePlayerPos = prevScenePlayer.transform.position;
        prevSceneCameraPosition = Camera.main.gameObject.transform.position;
        prevSceneCameraRotation = Camera.main.gameObject.transform.rotation;

        // Turn-off movement to avoid spawn issues
        prevScenePlayer.GetComponent<FallToGround>().enabled = false;
        prevScenePlayer.GetComponent<CharacterController>().enabled = false;

        DontDestroyOnLoad(prevScenePlayer);
        prevSceneOpponent = opponent;
        if (prevSceneOpponent) {
            prevSceneOpponent.transform.parent = null;
            prevSceneOpponent.SetActive(true);
            DontDestroyOnLoad(prevSceneOpponent);
        }
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

            // Delete any Player Dad records
            var rootObjects = newScene.GetRootGameObjects();
            foreach (var obj in rootObjects)
            {
                if (obj.name == "Player Dad")
                {
                    Destroy(obj);
                    break;
                }
            }

            // Move location to spawn location
            if (iEvent.scenePlayerName != "")
            {
                // Get references
                var playerSpawnObject = GameObject.Find(iEvent.scenePlayerName);
                if (playerSpawnObject == null) throw new System.Exception("Unable to find spawn point " + iEvent.scenePlayerName);
                var playerSpawn = playerSpawnObject.GetComponent<PlayerDeck>();

                // Setup third person camera
                GameObject.FindObjectOfType<Cinemachine.CinemachineFreeLook>().m_Follow = transform.Find("camera_focus").transform;
                GameObject.FindObjectOfType<Cinemachine.CinemachineFreeLook>().m_LookAt = transform.Find("camera_focus").transform;

                // Copy things over
                playerCamera = playerSpawn.GetComponent<InteractionChecker>().playerCamera;
                worldDialog = playerSpawn.GetComponent<InteractionChecker>().worldDialog;
                crosshairText = playerSpawn.GetComponent<InteractionChecker>().crosshairText;
                interactionHoverText = playerSpawn.GetComponent<InteractionChecker>().interactionHoverText;
                thirdPersonMovement.cam = playerCamera?.transform;

                // Setup pos
                gameObject.transform.position = playerSpawn.transform.position;

                // Remove player spawn
                playerSpawnObject.SetActive(false);
            }

            // Update the game events when returning to the scene
            var interactionEventList = GameObject.FindObjectsOfType<InteractionEvent>(true);
            foreach (var interactionEvent in interactionEventList)
            {
                // Update the other events
                InteractionEventNoComponent prevEventState = null;
                prevInteractionEventStates.TryGetValue(interactionEvent.eventName, out prevEventState);

                if (prevEventState != null)
                {
                    interactionEvent.CopyInteractionEventValues(prevEventState);
                }
            }

            // Set new scene animator
            sceneTransitionAnimator = GameObject.FindObjectOfType<FadeInOnSceneLoad>();
        }

        if (iEvent.scenePlayerName == "") this.enabled = false;

        // Turn-off movement to avoid spawn issues
        prevScenePlayer.GetComponent<FallToGround>().enabled = true;
        prevScenePlayer.GetComponent<CharacterController>().enabled = true;
    }

    /// <summary>
    /// Return to the title screen
    /// </summary>
    public void LoadTitleScreen()
    {
        StartCoroutine(LoadTitleScreenHelper());
    }

    IEnumerator LoadTitleScreenHelper()
    {
        sceneTransitionAnimator.FadeOut();
        yield return new WaitForSeconds(1f);

        if (prevSceneOpponent) { Destroy(prevSceneOpponent); }
        if (prevScenePlayer) { Destroy(prevScenePlayer); }

        SceneManager.LoadScene("gls_title");
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
        // Turn-off movement to avoid spawn issues
        prevScenePlayer.GetComponent<FallToGround>().enabled = false;
        prevScenePlayer.GetComponent<CharacterController>().enabled = false;

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

        // Delete any Player Dad records
        var rootObjects = newScene.GetRootGameObjects();
        foreach (var obj in rootObjects)
        {
            if (obj.name == "Player Dad")
            {
                Destroy(obj);
                break;
            }
        }

        // Set new scene animator
        sceneTransitionAnimator = GameObject.FindObjectOfType<FadeInOnSceneLoad>();

        // Get references
        var playerSpawnObject = GameObject.Find("PlayerSpawn");
        if (playerSpawnObject == null) throw new System.Exception("Unable to find spawn point PlayerSpawn");
        var playerSpawn = playerSpawnObject.GetComponent<PlayerDeck>();

        // Setup third person camera
        GameObject.FindObjectOfType<Cinemachine.CinemachineFreeLook>().m_Follow = transform.Find("camera_focus").transform;
        GameObject.FindObjectOfType<Cinemachine.CinemachineFreeLook>().m_LookAt = transform.Find("camera_focus").transform;

        // Copy things over
        playerCamera = playerSpawn.GetComponent<InteractionChecker>().playerCamera;
        worldDialog = playerSpawn.GetComponent<InteractionChecker>().worldDialog;
        crosshairText = playerSpawn.GetComponent<InteractionChecker>().crosshairText;
        interactionHoverText = playerSpawn.GetComponent<InteractionChecker>().interactionHoverText;
        thirdPersonMovement.cam = playerCamera?.transform;

        // Remove player spawn
        playerSpawnObject.SetActive(false);

        // Move updated Player Dad into scene
        SceneManager.MoveGameObjectToScene(gameObject, newScene);

        // Setup third person camera
        Camera.main.gameObject.transform.position = prevSceneCameraPosition;
        Camera.main.gameObject.transform.rotation = prevSceneCameraRotation;
        GameObject.FindObjectOfType<Cinemachine.CinemachineFreeLook>().m_Follow = prevScenePlayer.transform.Find("camera_focus").transform;
        GameObject.FindObjectOfType<Cinemachine.CinemachineFreeLook>().m_LookAt = prevScenePlayer.transform.Find("camera_focus").transform;

        // Setup pos
        gameObject.transform.position = prevScenePlayerPos;

        // Update the game events when returning to the scene
        var interactionEventList = GameObject.FindObjectsOfType<InteractionEvent>(true);
        foreach (var interactionEvent in interactionEventList)
        {
            // Update the other events
            InteractionEventNoComponent prevEventState = null;
            prevInteractionEventStates.TryGetValue(interactionEvent.eventName, out prevEventState);

            // If event state doesn't exist it must of been deleted
            if (prevEventState == null)
            {
            }
            else
            {
                interactionEvent.CopyInteractionEventValues(prevEventState);
            }
        }

        // Update the active event
        if (prevActiveEventName != "")
        {
            var prevActiveEvent = interactionEventList.Where(e => e.eventName == prevActiveEventName).FirstOrDefault();
            postInteracationChanges(prevActiveEvent);
        }

        prevSceneName = "";
        prevActiveEventName = "";
        this.enabled = true;

        prevScenePlayer.GetComponent<FallToGround>().enabled = true;
        prevScenePlayer.GetComponent<CharacterController>().enabled = true;
    }

    /// <summary>
    /// Updates all the interaction events with the results from the events
    /// </summary>
    /// <param name="iEvent"></param>
    private void postInteracationChanges(InteractionEvent iEvent)
    {
        // Remove interaction lock
        hoverPossibleEvent = null;

        // When the event triggers a different event
        if (iEvent.disableOnReturn)
        {
            iEvent.enabled = false;
        }

        // When removing to avoid replay
        if (iEvent.removeOnReturn)
        {
            iEvent.gameObject.SetActive(false);
        }

        // Remove an interaction
        if (iEvent.altDisableInteractionEvent)
        {
            iEvent.altDisableInteractionEvent.enabled = false;
        }

        // Remove an interaction
        if (iEvent.alt2DisableInteractionEvent)
        {
            iEvent.alt2DisableInteractionEvent.enabled = false;
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

        // Finish reading state
        if (hoverPossibleEvent == null)
        {
            thirdPersonMovement.enabled = true;
            if (iEvent.animator && iEvent.animationBooleanName != "") { iEvent.animator.SetBool(iEvent.animationBooleanName, false); }
            activeEvent = null;
        }
    }
}
