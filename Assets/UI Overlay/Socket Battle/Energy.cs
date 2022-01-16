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

    public Pokemon attachedToPokemon;

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
    /// The last position of the mouse to compute the delta
    /// </summary>
    private Vector3 previousMousePosition = new Vector3();

    /// <summary>
    /// The dropEvent for the selected drag
    /// </summary>
    private DropEvent dropEvent;

    private BattleGameBoard battleGameBoard;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Update energy position
        var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        if (isDragging)
        {
            transform.localPosition += (mousePosition - previousMousePosition);
        }
        previousMousePosition = mousePosition;
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
        if (canBeDragged)
        {
            transform.position = dragStartPosition;
            isDragging = false;
            isSelected = false;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        var triggeredDropEvent = collision.gameObject.GetComponent<DropEvent>();

        if (triggeredDropEvent?.eventType == "TargetPokemon" && canAttachEnergy(triggeredDropEvent.targetPokemon))
        {
            dropEvent = collision.gameObject.GetComponent<DropEvent>();
            dropEvent.targetPokemon.GetComponent<Animator>().SetTrigger("onHoverEnter");
        }
    }

    void OnTriggerExit(Collider collision)
    {
        var triggeredDropEvent = collision.gameObject.GetComponent<DropEvent>();
        if (triggeredDropEvent != dropEvent) return;

        if (dropEvent?.eventType == "TargetPokemon")
        {
            dropEvent.targetPokemon.GetComponent<Animator>().SetTrigger("onHoverExit");
        }
        dropEvent = null;
    }

    public void onDrag()
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
            OnHoverExit();
        }
    }

    public void onBattleStart(BattleGameBoard _battleGameBoard)
    {
        battleGameBoard = _battleGameBoard;
        canBeDragged = initCanBeDragged;
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
        var hasRoomToAttach = target.attachedEnergy.Count < target.maxNumberOfAttachedEnergy;
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
        attachedToPokemon = target;
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
        gameObject.GetComponent<TranslationAnimation>().Translate(_targetPosition, _distancePerSecond);
    }
}
