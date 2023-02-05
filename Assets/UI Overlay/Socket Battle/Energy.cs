using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

/// <summary>
/// Resource to play a move.  It can be attached to cards or pokemon.
/// </summary>
public class Energy : MonoBehaviour
{
    public string energyName;

    public string energyType;


    private bool _isUsed;

    /// <summary>
    /// True if the energy is used for this turn
    /// </summary>
    public bool isUsed
    {
        get { return _isUsed; }
        set
        {
            animator.SetBool("isUsed", value);
            _isUsed = value;
        }
    }

    /// <summary>
    /// Reference to the default object
    /// </summary>
    public Animator animator;

    /// <summary>
    /// True if the energy can be place at the beginning of a match
    /// </summary>
    public bool initCanBeDragged = true;

    /// <summary>
    /// True if the energy is draggable
    /// </summary>
    private bool canBeDragged = true;

    /// <summary>
    /// True if the mouse cursor is over the card
    /// </summary>
    private bool isSelected = false;

    /// <summary>
    /// True if the mouse is dragging the card
    /// </summary>
    private bool isDragging = false;

    /// <summary>
    /// The initial position of the card before the user action
    /// </summary>
    private Vector3 dragStartPosition = new Vector3();

    /// <summary>
    /// The dropEvent for the selected drag
    /// </summary>
    public DropEvent dropEvent;

    public bool inDebugMode = false;
    private float interactableDistance = 200;
    private int raycastLayer;

    private BattleGameBoard battleGameBoard;

    private Camera canvasCamera;

    private GameOptions gameOptions
    {
        get
        {
            if (battleGameBoard != null) return battleGameBoard.player.gameOptions;
            return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        raycastLayer = LayerMask.GetMask("UI Raycast");
    }

    // Update is called once per frame
    void Update()
    {
        // Update energy position
        if (isDragging)
        {
            canvasCamera = GameObject.Find("Canvas Camera").GetComponent<Camera>();
            var mousePosition = canvasCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100f));
            gameObject.transform.position = mousePosition;
        }

        // Check for drag & drop
        if (gameOptions && gameOptions.useCardDragControls)
        {
            // Check for drop
            if (isDragging && Input.GetKeyUp(KeyCode.Mouse0))
            {
                onDrop();
            }
        }
        // use two click controls
        else
        {
            // Check for drop
            if (isDragging && Input.GetKeyDown(KeyCode.Mouse0))
            {
                onDrop();
            }

            // Check for "drag"
            if (isSelected && !isDragging && Input.GetKeyDown(KeyCode.Mouse0))
            {
                onDragHelper();
            }
        }

        if (inDebugMode)
        {
            Ray ray = canvasCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, raycastLayer))
                print("I'm looking at " + hit.transform.name);
            else
                print("I'm looking at nothing!");
        }

        var newDropEvent = GetDropInteraction();
        OnHoverInteraction(newDropEvent);
    }

    public void OnHoverEnter()
    {
        if (canBeDragged && !isSelected && !isDragging)
        {
            dragStartPosition = transform.position;
            transform.localPosition += new Vector3(0, 0, -40);
            isDragging = false;
            isSelected = true;
        }
    }

    public void OnHoverExit()
    {
        if (canBeDragged && !isDragging)
        {
            transform.position = dragStartPosition;
            isDragging = false;
            isSelected = false;
        }
    }

    /// <summary>
    /// Checks if the user is hovering on something to interact with
    /// </summary>
    private DropEvent GetDropInteraction()
    {
        if (canvasCamera == null) return null;

        RaycastHit hit = new RaycastHit();
        var raycastHit = Physics.Raycast(canvasCamera.ScreenPointToRay(Input.mousePosition), out hit, interactableDistance, raycastLayer);

        // Check if ray hit
        if (!raycastHit) return null;

        // Check if we can get component from hit
        var result = hit.collider.GetComponent<DropEvent>();

        return result;
    }

    /// <summary>
    /// When the user "hovers" on something to interact with
    /// </summary>
    /// <param name="iEvent"></param>
    public void OnHoverInteraction(DropEvent newDropEvent)
    {
        // When we hover over something
        if (isDragging && newDropEvent?.eventType == "TargetPokemon" && canAttachEnergy(newDropEvent.targetPokemon))
        {
            dropEvent = newDropEvent;
            dropEvent.targetPokemon.hudAnimator.SetTrigger("onHoverEnter");
        }

        // When the hover event is gone
        if (newDropEvent == null && dropEvent != null)
        {
            dropEvent.targetPokemon.hudAnimator.SetTrigger("onHoverExit");
            dropEvent = null;
        }
    }

    public void onDrag()
    {
        if (gameOptions && gameOptions.useCardDragControls) onDragHelper();
    }

    private void onDragHelper()
    {
        if (canBeDragged)
        {
            isDragging = true;
        }
    }

    public void onDrop()
    {
        if (!canBeDragged) return;

        if (dropEvent?.eventType == "TargetPokemon" && canAttachEnergy(dropEvent.targetPokemon))
        {
            isDragging = false;
            isSelected = false;
            onEnergyPlay(dropEvent.targetPokemon);
        }
        else
        {
            isDragging = false;
            OnHoverExit();
        }
    }

    /// <summary>
    /// Event that triggers on the start of the battle workflow
    /// </summary>
    /// <param name="_deckBuilderAddCard"></param>
    public void onBattleStart(BattleGameBoard _battleGameBoard)
    {
        battleGameBoard = _battleGameBoard;
        canBeDragged = initCanBeDragged;
        transform.position = battleGameBoard.energyDeckLocation.transform.position;
    }

    /// <summary>
    /// Event that triggers on the start of the deck builder workflow
    /// </summary>
    /// <param name="_deckBuilderAddCard"></param>
    public void onDeckBuildStart(DeckBuilderAddCard _deckBuilderAddCard)
    {
        onBattleStart(null);
    }

    /// <summary>
    /// When the player clicks on the energy to attach
    /// </summary>
    public void onEnergyPlay(Pokemon target)
    {
        print("PLAY: Attach energy " + energyName + " to " + target.pokemonName);
        if (!canAttachEnergy(target)) { return; }
        battleGameBoard.onEnergyPlay(this, target);
    }

    /// <summary>
    /// Returns true if the energy can be attached to the given target
    /// </summary>
    public bool canAttachEnergy(Pokemon target)
    {
        if (target.isFainted) { return false; }

        var hasRoomToAttach = true;
        var supportsEnergyType = energyName == "Normal" || target.energyTypes.Contains(energyName);
        return hasRoomToAttach && supportsEnergyType;
    }

    /// <summary>
    /// When an energy is played
    /// </summary>
    /// <param name="target"></param>
    public void playEnergy(Pokemon target)
    {
        target.attachedEnergy.Add(this);

        canBeDragged = false;
        isUsed = false;
    }

    public void onTurnEnd() { }

    public void onOpponentTurnEnd() { }

    public void onBattleEnd() { }

    /// <summary>
    /// Animate the energy to a new location
    /// </summary>
    /// <param name="_targetPosition"></param>
    /// <param name="_distancePerSecond"></param>
    public void Translate(Vector3 _targetPosition, float _distancePerSecond = 150.0f)
    {
        if (_targetPosition == transform.position) return;
        var prevCanBeDragged = canBeDragged;
        gameObject.GetComponent<TranslationAnimation>().Translate(_targetPosition, _distancePerSecond,
            () => { canBeDragged = false; return true; },
            () => { canBeDragged = prevCanBeDragged; return true; }
        );
    }

    /// <summary>
    /// Change if this energy can be interacted with or not
    /// </summary>
    /// <param name="_canBeDragged"></param>
    public void SetCanBeDragged(bool _canBeDragged)
    {
        canBeDragged = _canBeDragged;
    }
}
