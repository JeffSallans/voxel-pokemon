using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RareCandy : MonoBehaviour
{
    /// <summary>
    /// Max level a pokemon can be
    /// </summary>
    public int maxLevel = 20;

    /// <summary>
    /// The canvas camera to map mouse position to
    /// </summary>
    private Camera canvasCamera;

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

    private float interactableDistance = 200;

    /// <summary>
    /// Layer to reference for raycasting
    /// </summary>
    private int raycastLayer;

    /// <summary>
    /// Targeted Drop Event
    /// </summary>
    private DropEvent dropEvent;

    /// <summary>
    /// The deck builder coordinator
    /// </summary>
    private DeckBuilderAddCard deckBuilderAddCard;

    // Start is called before the first frame update
    void Start()
    {
        deckBuilderAddCard = GameObject.FindObjectOfType<DeckBuilderAddCard>();
        raycastLayer = LayerMask.GetMask("UI Raycast");
    }

    // Update is called once per frame
    void Update()
    {
        // Update card position
        if (isDragging)
        {
            canvasCamera = GameObject.Find("Canvas Camera").GetComponent<Camera>();
            var mousePosition = canvasCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100f));
            gameObject.transform.position = mousePosition;
        }

        // Check for drop
        if (isDragging && Input.GetKeyUp(KeyCode.Mouse0))
        {
            onDrop();
        }

        var newDropEvent = GetDropInteraction();
        OnHoverInteraction(newDropEvent);
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
        if (isDragging && newDropEvent?.eventType == "TargetPokemon" && canTarget(newDropEvent.targetPokemon))
        {
            dropEvent = newDropEvent;
            dropEvent.targetPokemon.GetComponent<Animator>().SetTrigger("onHoverEnter");
        }

        // When the hover event is gone
        if (newDropEvent == null && dropEvent != null)
        {
            dropEvent.targetPokemon.GetComponent<Animator>().SetTrigger("onHoverExit");
            dropEvent = null;
        }
    }

    public void OnHoverEnter()
    {
        if (!isSelected && !isDragging)
        {
            dragStartPosition = transform.position;
            transform.localPosition += new Vector3(0, 0, -20);
            isDragging = false;
            isSelected = true;
        }
    }

    public void OnHoverExit()
    {
        if (!isDragging)
        {
            transform.position = dragStartPosition;
            isDragging = false;
            isSelected = false;
        }
    }

    /// <summary>
    /// On mouse press
    /// </summary>
    public void onDrag()
    {
        isDragging = true;
    }

    /// <summary>
    /// On mouse release
    /// </summary>
    public void onDrop()
    {
        if (dropEvent?.eventType == "TargetPokemon" && canTarget(dropEvent.targetPokemon))
        {
            isDragging = false;
            isSelected = false;
            deckBuilderAddCard.onCandyPlay(this, dropEvent.targetPokemon);
        }
        else
        {
            isDragging = false;
            transform.position = dragStartPosition;
            isSelected = false;
        }
    }

    /// <summary>
    /// Returns true if you can level up a pokemon
    /// </summary>
    /// <param name="targetPokemon"></param>
    /// <returns></returns>
    private bool canTarget(Pokemon targetPokemon)
    {
        return targetPokemon.level < maxLevel;
    }

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
