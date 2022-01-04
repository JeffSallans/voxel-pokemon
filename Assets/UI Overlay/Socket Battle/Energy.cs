using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
            defaultObject.SetActive(!value);
            usedObject.SetActive(value);
            _isUsed = value;
        }
    }

    public Pokemon attachedToPokemon;

    /// <summary>
    /// Reference to the default object
    /// </summary>
    public GameObject defaultObject;

    /// <summary>
    /// Reference to the used object
    /// </summary>
    public GameObject usedObject;

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
    /// True if the card is over an interactable space
    /// </summary>
    private bool isOverDropZone = false;

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

    /// <summary>
    /// Returns true if the energy can be played with the usable energy
    /// </summary>
    public bool canBePlayed
    {
        get
        {
            var costAsString = battleGameBoard?.commonEnergy?.GetRange(0, 1)
                .Select(c => new { energyName = c.energyName, count = 1 })
                .ToList();
            var usableAsString = battleGameBoard?.useableEnergy?.Select(e => e.energyName)
                ?.GroupBy(
                    c => c,
                    c => c,
                    (name, _name) => new
                    {
                        energyName = name,
                        count = _name.Count()
                    }
                )?.ToDictionary(e => e.energyName);
            var result = costAsString.All(c => usableAsString != null && usableAsString.ContainsKey(c.energyName) && c.count <= usableAsString?[c.energyName]?.count);
            return result;
        }
    }

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
            transform.localPosition += new Vector3(0, 0, -50);
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        dropEvent = collision.gameObject.GetComponent<DropEvent>();
        if (dropEvent?.eventType == "TargetPokemon")
        {
            dropEvent.targetPokemon.GetComponent<Animator>().SetTrigger("onHoverEnter");
            isOverDropZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        var triggeredDropEvent = collision.gameObject.GetComponent<DropEvent>();
        if (triggeredDropEvent != dropEvent) return;

        if (dropEvent?.eventType == "TargetPokemon")
        {
            dropEvent.targetPokemon.GetComponent<Animator>().SetTrigger("onHoverExit");
            isOverDropZone = false;
        }
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

        isDragging = false;
        if (isOverDropZone)
        {
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
    public void onEnergyPlayEvent()
    {
        print("energy play");
        if (!canBePlayed) { return; }
        battleGameBoard.onEnergyPlay(this, battleGameBoard.activePokemon);
    }

    /// <summary>
    /// When an energy is played
    /// </summary>
    /// <param name="target"></param>
    public void onEnergyPlay(Pokemon target)
    {
        target.attachedEnergy.Add(this);

        canBeDragged = false;
        isUsed = true;
        attachedToPokemon = target;
    }

    public void onTurnEnd() { }

    public void onOpponentTurnEnd() { }

    public void onBattleEnd() { }
}
