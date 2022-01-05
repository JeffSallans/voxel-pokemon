using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// How a card works
/// </summary>
public class Card : MonoBehaviour
{
    /// <summary>
    /// Name of the move for lookup purposes
    /// </summary>
    public string cardName;

    /// <summary>
    /// Description to render
    /// </summary>
    public string cardDesc
    {
        get {
            var descWithTemplates = initCardDescription + System.String.Join(",", gameCardModifiers) + System.String.Join(",", battleCardModifiers);
            var desc = descWithTemplates.Replace("{damage}", damage.ToString())
                    .Replace("{turn}", turn.ToString())
                    .Replace("{attack}", attackStat.ToString())
                    .Replace("{defense}", defenseStat.ToString())
                    .Replace("{specialAttack}", specialAttackStat.ToString())
                    .Replace("{specialdefense}", specialdefenseStat.ToString())
                    .Replace("{evasion}", evasionStat.ToString());
            return desc;
        }
    }

    /// <summary>
    /// Card changes that last a battle
    /// </summary>
    public List<string> battleCardModifiers;

    /// <summary>
    /// Card changes that last the rest of the game
    /// </summary>
    public List<string> gameCardModifiers;

    /// <summary>
    /// How much energy it takes to play
    /// </summary>
    public List<Energy> cost;

    /// <summary>
    /// Initial card description.  Use {damage} will be replaced with the damage variable. {turn} will be replaced with the turn variable.
    /// </summary>
    public string initCardDescription;

    /// <summary>
    /// The type of card and damage see EnergyType
    /// </summary>
    public string cardType;

    /// <summary>
    /// The type of damage "Physical" or "Special"
    /// </summary>
    public string damageType;

    /// <summary>
    /// The damage the card will do
    /// </summary>
    public int damage
    {
        get { return initDamage; }
    }

    /// <summary>
    /// Initial damage the card will do
    /// </summary>
    public int initDamage;

    /// <summary>
    /// The attack stat the card will change
    /// </summary>
    public int attackStat;

    /// <summary>
    /// The special attack stat the card will change
    /// </summary>
    public int specialAttackStat;

    /// <summary>
    /// The defense stat the card will change
    /// </summary>
    public int defenseStat;

    /// <summary>
    /// The special defense stat the card will change
    /// </summary>
    public int specialdefenseStat;

    /// <summary>
    /// The evasion stat the card will change
    /// </summary>
    public int evasionStat;

    /// <summary>
    /// How long a StatusEffect will be placed
    /// </summary>
    public int turn;

    /// <summary>
    /// Current energies during a battle
    /// </summary>
    public List<Energy> attachedEnergies;

    /// <summary>
    /// Energies at the start of battle
    /// </summary>
    public List<Energy> battleStartEnergies;

    /// <summary>
    /// Returns true if the card can be played with the usable energy
    /// </summary>
    public bool canBePlayed
    {
        get
        {
            var costAsString = cost?.Select(c => c.energyName)
                .GroupBy(
                    c => c,
                    c => c,
                    (name, _name) => new
                    {
                        energyName = name,
                        count = _name.Count()
                    }
                ).ToList();
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

    /// <summary>
    /// Where the card description should be rendered
    /// </summary>
    public TextMeshProUGUI descriptionGameObject;

    /// <summary>
    /// Button that plays the card
    /// </summary>
    public Button playButtonGameObject;

    /// <summary>
    /// A list of locations of all the attached energy
    /// </summary>
    public List<GameObject> energyLocations;

    private BattleGameBoard battleGameBoard;

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        descriptionGameObject.text = cardDesc;
        playButtonGameObject.interactable = canBePlayed;

        // Update attached Energies to placeholder locations
        var i = 0;
        attachedEnergies.ForEach(e =>
        {
            e.transform.localPosition = gameObject.transform.localPosition + energyLocations[i].transform.localPosition;
            e.transform.rotation = energyLocations[i].transform.rotation;
            i++;
        });

        // Update card position
        var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        if (isDragging) {
            transform.localPosition += (mousePosition - previousMousePosition);
        }
        previousMousePosition = mousePosition;
    }

    public void OnHoverEnter()
    {
        if (canBePlayed && !isSelected && !isDragging)
        {
            dragStartPosition = transform.position;
            transform.localPosition += new Vector3(0, 0, -50);
            isDragging = false;
            isSelected = true;
        }
    }

    public void OnHoverExit()
    {
        if (isSelected)
        {
            transform.position = dragStartPosition;
            isDragging = false;
            isSelected = false;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        dropEvent = collision.gameObject.GetComponent<DropEvent>();
        if (dropEvent?.eventType == "TargetPokemon")
        {
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
        if (canBePlayed && isSelected && !isDragging)
        {
            isDragging = true;
        }
    }

    public void onDrop()
    {
        if (!canBePlayed || !isDragging) return;

        if (dropEvent?.eventType == "TargetPokemon")
        {
            OnHoverExit();
            onPlay(battleGameBoard.activePokemon, dropEvent.targetPokemon);
        }
        else
        {
            OnHoverExit();
        }
    }

    public void onBattleStart(BattleGameBoard _battleGameBoard) {
        attachedEnergies = battleStartEnergies.ToList();
        battleGameBoard = _battleGameBoard;
    }

    public void onDraw(Pokemon activePokemon) { }

    public void onOpponentDraw(Pokemon opponentActivePokemon) { }

    public void onPlay(Pokemon user, Pokemon target)
    {
        print("PLAY: Used card " + cardName + " on " + target.pokemonName);
        if (!canBePlayed) { return; }
        battleGameBoard.onPlay(this, user, target);
    }

    public void play(Pokemon user, Pokemon target) {
        var dealtDamage = damage + user.attackStat - target.defenseStat;
        var newHealth = target.health - dealtDamage;
        target.health = Mathf.Max(newHealth, 0);
    }

    public void onTurnEnd() { }

    public void onOpponentTurnEnd() { }

    public void onBattleEnd() { }
}
