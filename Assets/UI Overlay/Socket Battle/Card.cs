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
                    .Replace("{specialDefense}", specialDefenseStat.ToString())
                    .Replace("{evasion}", evasionStat.ToString())
                    .Replace("{block}", blockStat.ToString());
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
    public int specialDefenseStat;

    /// <summary>
    /// The evasion stat the card will change
    /// </summary>
    public int evasionStat;

    /// <summary>
    /// The block stat the card will change
    /// </summary>
    public int blockStat;

    /// <summary>
    /// How long a StatusEffect will be placed
    /// </summary>
    public int turn;

    /// <summary>
    /// The animation event to trigger on the user
    /// </summary>
    public string userAnimationType = "onAttack";

    /// <summary>
    /// The animation event to trigger on the target
    /// </summary>
    public string targetAnimationType = "onHit";

    /// <summary>
    /// The animation event to trigger on the target (used for particle or identifying effects)
    /// </summary>
    public string targetAnimationType2 = "";

    /// <summary>
    /// Set to override default functionality
    /// </summary>
    public ICard overrideFunctionality = null;

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
            // Check color energy count
            var costAsString = cost?.Select(c => c.energyName)
                .Where(e => e != "Normal")
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
            var coloredEnergyConditionMet = costAsString.All(c => usableAsString != null && usableAsString.ContainsKey(c.energyName) && c.count <= usableAsString?[c.energyName]?.count);

            // Check count for colorless check
            var hasEnoughUsableEnergyForColorless = cost?.Count <= battleGameBoard?.useableEnergy?.Count ;

            return coloredEnergyConditionMet && hasEnoughUsableEnergyForColorless;
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
            animateMoveCost("onToBeUsedHoverEnter");
        }
    }

    public void OnHoverExit()
    {
        if (isSelected)
        {
            transform.position = dragStartPosition;
            isDragging = false;
            isSelected = false;
            animateMoveCost("onToBeUsedHoverLeave");
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
            isDragging = false;
            isSelected = false;
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
        // Run override if applicable
        if (overrideFunctionality) { overrideFunctionality.play(this, battleGameBoard, user, target); return; }

        // Deal Damage if applicable
        if (damage > 0)
        {
            // Determine damage
            var dealtDamage = damage - target.blockStat;
            target.blockStat = Mathf.Max(-dealtDamage, 0);

            // Hit target
            var newHealth = target.health - Mathf.Max(dealtDamage, 0);
            target.health = Mathf.Max(newHealth, 0);
        }

        // Add status effects if applicable
        var totalStatChange = Mathf.Abs(attackStat) + Mathf.Abs(defenseStat) + Mathf.Abs(specialAttackStat) + Mathf.Abs(specialDefenseStat) + Mathf.Abs(evasionStat) + Mathf.Abs(blockStat);
        if (totalStatChange > 0)
        {
            // target.attackStat += attackStat;
            // target.defenseStat += defenseStat;
            // target.specialAttackStat += specialAttackStat;
            // target.specialDefenseStat += specialDefenseStat;
            // target.evasionStat += evasionStat;
            target.attachedStatus.Add(new StatusEffect(target, this, "blockStatEffect", new Dictionary<string, string>() {
                { "statType", "blockStat" },
                { "stackCount", blockStat.ToString() },
                { "turnsLeft", "1" }
            }));
        }

        if (userAnimationType != "") user.GetComponent<Animator>().SetTrigger(userAnimationType);
        if (targetAnimationType != "") target.GetComponent<Animator>().SetTrigger(targetAnimationType);
        if (targetAnimationType2 != "") target.GetComponent<Animator>().SetTrigger(targetAnimationType2);
    }

    public void onTurnEnd() { }

    public void onOpponentTurnEnd() { }

    public void onBattleEnd() { }

    /// <summary>
    /// Animate the card to a new location
    /// </summary>
    /// <param name="_targetPosition"></param>
    /// <param name="_distancePerSecond"></param>
    public void Translate(Vector3 _targetPosition, float _distancePerSecond = 150.0f)
    {
        gameObject.GetComponent<TranslationAnimation>().Translate(_targetPosition, _distancePerSecond);
    }

    /// <summary>
    /// Animate the energy that will be used
    /// </summary>
    /// <param name="move"></param>
    private void animateMoveCost(string animationName)
    {
        var coloredCost = cost.Where(e => e.energyName != "Normal").ToList();
        coloredCost.ForEach(energy => animateEnergyCost(energy, animationName));

        var colorlessCost = cost.Where(e => e.energyName == "Normal").ToList();
        colorlessCost.ForEach(energy => animateEnergyCost(energy, animationName));
    }

    /// <summary>
    /// Animate an energy that will be used
    /// </summary>
    /// <param name="energy"></param>
    /// <param name="animationName"></param>
    private void animateEnergyCost(Energy energy, string animationName)
    {
        // Subtract from common
        var target = battleGameBoard.commonEnergy.Where(e => !e.isUsed && e.energyName == energy.energyName).FirstOrDefault();
        if (target != null)
        {
            target.animator.SetTrigger(animationName);
            return;
        }
        // Subtract from active
        var targetOnPokemon = battleGameBoard.activePokemon.attachedEnergy.Where(e => !e.isUsed && e.energyName == energy.energyName).FirstOrDefault();
        if (targetOnPokemon != null)
        {
            targetOnPokemon.animator.SetTrigger(animationName);
            return;
        }
        var colorlessTargetOnPokemon = battleGameBoard.activePokemon.attachedEnergy.Where(e => !e.isUsed).FirstOrDefault();
        if (energy.energyName == "Normal" && colorlessTargetOnPokemon != null)
        {
            colorlessTargetOnPokemon.animator.SetTrigger(animationName);
            return;
        }
    }
}
