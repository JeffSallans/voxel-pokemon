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
                    .Replace("{special}", specialStat.ToString())
                    .Replace("{evasion}", evasionStat.ToString())
                    .Replace("{block}", blockStat.ToString())
                    .Replace("{attackMult}", attackMultStat.ToString())
                    .Replace("{userHeal}", userHeal.ToString())
                    .Replace("{flipText}", flipButtonFunctionality?.getFlipButtonText());
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
    /// The theme of the card (Rush, Control, Support, Combo)
    /// </summary>
    public string theme;

    /// <summary>
    /// The possible targets of this card. Self, Bench, Team, ActiveOpponent, AnyOpponent, BenchOpponent, AnyPokemon,
    /// </summary>
    public string targetType;

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
    /// The special stat the card will change
    /// </summary>
    public int specialStat;

    /// <summary>
    /// The defense stat the card will change
    /// </summary>
    public int defenseStat;

    /// <summary>
    /// The evasion stat the card will change
    /// </summary>
    public int evasionStat;

    /// <summary>
    /// The block stat the card will change
    /// </summary>
    public int blockStat;

    /// <summary>
    /// The attack multiplier to apply to the next attack * 100
    /// </summary>
    public int attackMultStat;

    /// <summary>
    /// How much the user is healed
    /// </summary>
    public int userHeal;

    /// <summary>
    /// True if the user gets immunity to the next attack
    /// </summary>
    public bool grantsInvulnerability;

    /// <summary>
    /// How long a StatusEffect will be placed
    /// </summary>
    public int turn;

    /// <summary>
    /// Set to true if the status affects the user instead of the target
    /// </summary>
    public bool statusAffectsUser = false;

    /// <summary>
    /// True if the card will be removed from the game after playing
    /// </summary>
    public bool isSingleUse = false;

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
    /// Button that flips a card
    /// </summary>
    public IFlipButton flipButtonFunctionality = null;

    /// <summary>
    /// Current energies during a battle
    /// </summary>
    public List<Energy> attachedEnergies;

    /// <summary>
    /// Energies at the start of battle
    /// </summary>
    public List<Energy> battleStartEnergies;

    /// <summary>
    /// Animation controller for the card
    /// </summary>
    public Animator cardAnimator;

    /// <summary>
    /// Returns true if the card can be played with the usable energy
    /// </summary>
    public bool canBePlayed
    {
        get
        {
            // Override this check if DeckBuilder is here
            if (inDeckBuilderWorkflow) { return true; }

            // Override this check for a different one
            if (canBePlayedOverride != null) { return canBePlayedOverride(); }

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
    /// Set to a function if we should override the card canBePlayed functionality
    /// </summary>
    public Func<bool> canBePlayedOverride = null;

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

    public Func<bool> _onDragFunc = null;

    public Func<bool> _onDropFunc = null;

    private Pokemon _owner;
    /// <summary>
    /// The pokemon that owns the card
    /// </summary>
    public Pokemon owner { get { return _owner; } }

    private BattleGameBoard battleGameBoard;

    /// <summary>
    /// True if the mouse cursor is over the card
    /// </summary>
    public bool isSelected = false;

    /// <summary>
    /// True if the mouse is dragging the card
    /// </summary>
    public bool isDragging = false;

    /// <summary>
    /// The initial position of the card before the user action
    /// </summary>
    private Vector3 dragStartPosition = new Vector3();

    /// <summary>
    /// The dropEvent for the selected drag
    /// </summary>
    private DropEvent dropEvent;

    private float interactableDistance = 200;
    private int raycastLayer;

    /// <summary>
    /// The canvas camera to map mouse position to
    /// </summary>
    private Camera canvasCamera;

    /// <summary>
    /// A reference to deck builder workflow if exists
    /// </summary>
    private object deckBuilderAddCard;

    /// <summary>
    /// True if the card is in the deck builder workflow
    /// </summary>
    private bool inDeckBuilderWorkflow
    {
        get
        {
            return deckBuilderAddCard != null;
        }
    }

    private void Awake()
    {
        battleGameBoard = GameObject.FindObjectOfType<BattleGameBoard>();
        deckBuilderAddCard = GameObject.FindObjectOfType<DeckBuilderAddCard>();
    }

    // Start is called before the first frame update
    void Start()
    {
        raycastLayer = LayerMask.GetMask("UI Raycast");
        cardAnimator.SetBool("hideFlipButton", flipButtonFunctionality == null);
    }

    // Update is called once per frame
    void Update()
    {
        descriptionGameObject.text = UnicodeUtil.replaceWithUnicode(cardDesc);
        playButtonGameObject.interactable = canBePlayed;

        // Update cost Energies to placeholder locations
        var i = 0;
        cost.ForEach(e =>
        {
            e.transform.localScale = energyLocations[i].transform.localScale;
            e.transform.localRotation = energyLocations[i].transform.localRotation;
            e.transform.position = energyLocations[i].transform.position;
            i++;
        });

        // Update card position
        if (isDragging) {
            canvasCamera = GameObject.Find("Canvas Camera").GetComponent<Camera>();
            var mousePosition = canvasCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100f));
            gameObject.transform.position = mousePosition + new Vector3(0, 0, -5);
        }

        // Check for drop
        if (isDragging && Input.GetKeyUp(KeyCode.Mouse0))
        {
            onDrop();
        }

        // Update flip button activeness
        if (flipButtonFunctionality != null)
        {
            var flipButtonEnabled = isFlipButtonEnabled();
            if (flipButtonEnabled != cardAnimator.GetBool("isFlipButtonEnabled"))
            {
                cardAnimator.SetBool("isFlipButtonEnabled", flipButtonEnabled);
            }
        }

        var newDropEvent = GetDropInteraction();
        OnHoverInteraction(newDropEvent);
    }

    public void OnHoverEnter()
    {
        if (!isSelected && !isDragging)
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
        if (!isDragging)
        {
            transform.position = dragStartPosition;
            isDragging = false;
            isSelected = false;
            animateMoveCost("onToBeUsedHoverLeave");
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
        if (inDeckBuilderWorkflow && newDropEvent?.eventType == "TargetPokemon")
        {
            dropEvent = newDropEvent;
            FindObjectOfType<HoverAndDragMessageTarget>().OnHoverEnter(new HoverAndDragEvent()
            {
                eventType = dropEvent.eventType,
                dropEvent = dropEvent,
                targetCard = this
            });
        }
        // Standard battle hover
        else if (isDragging && newDropEvent?.eventType == "TargetPokemon" && canBePlayed && canTarget(newDropEvent.targetPokemon))
        {
            dropEvent = newDropEvent;
            var isSuperEffective = damage > 0 && TypeChart.getEffectiveness(this, dropEvent.targetPokemon) > 1;
            var isNotVeryEffective = damage > 0 && TypeChart.getEffectiveness(this, dropEvent.targetPokemon) < 1;
            dropEvent.targetPokemon.GetComponent<Animator>().SetBool("isSuperEffective", isSuperEffective);
            dropEvent.targetPokemon.GetComponent<Animator>().SetBool("isNotVeryEffective", isNotVeryEffective);
            dropEvent.targetPokemon.GetComponent<Animator>().SetTrigger("onHoverEnter");

        }
        // Discard hover
        else if (isDragging && newDropEvent?.eventType == "Discard")
        {
            dropEvent = newDropEvent;
            dropEvent.targetGameObject.GetComponent<Animator>().SetTrigger("onHoverEnter");
        }

        // When the hover event is gone
        if (newDropEvent == null && dropEvent != null && inDeckBuilderWorkflow && dropEvent?.eventType == "TargetPokemon")
        {
            FindObjectOfType<HoverAndDragMessageTarget>().OnHoverExit(new HoverAndDragEvent()
            {
                eventType = dropEvent.eventType,
                dropEvent = dropEvent,
                targetCard = this
            });
            dropEvent = null;
        }
        else if (newDropEvent == null && dropEvent != null && dropEvent.eventType == "TargetPokemon")
        {
            dropEvent.targetPokemon.GetComponent<Animator>().SetTrigger("onHoverExit");
            dropEvent = null;
        }
        // Discard hover exit
        else if (newDropEvent == null && dropEvent != null && dropEvent.eventType == "Discard")
        {
            dropEvent.targetGameObject.GetComponent<Animator>().SetTrigger("onHoverExit");
            dropEvent = null;
        }
    }

    public void onDrag()
    {
        isDragging = true;
        if (_onDragFunc != null) { _onDragFunc(); }
    }

    public void onDrop()
    {
        // When you drop on a pokemon
        if (inDeckBuilderWorkflow && dropEvent?.eventType == "TargetPokemon")
        {
            isDragging = false;
            isSelected = false;
            FindObjectOfType<HoverAndDragMessageTarget>().OnDrop(new HoverAndDragEvent()
            {
                eventType = dropEvent.eventType,
                dropEvent = dropEvent,
                targetCard = this
            });
        }
        // Standard battle drop
        else if (dropEvent?.eventType == "TargetPokemon" && canBePlayed && canTarget(dropEvent.targetPokemon))
        {
            isDragging = false;
            isSelected = false;
            onPlay(battleGameBoard.activePokemon, dropEvent.targetPokemon);
        }
        // Discard
        else if (dropEvent?.eventType == "Discard")
        {
            battleGameBoard.cardDiscard(this, battleGameBoard.activePokemon, false);
            isDragging = false;
            isSelected = false;
        }
        else
        {
            isDragging = false;
            isSelected = false;
            OnHoverExit();
            if (_onDropFunc != null) { _onDropFunc(); }
        }
    }

    /// <summary>
    /// Call when battle starts to setup the game
    /// </summary>
    public void onBattleStart(BattleGameBoard _battleGameBoard) {
        attachedEnergies = battleStartEnergies.ToList();
        battleGameBoard = _battleGameBoard;
        deckBuilderAddCard = null;

        // Calculate owner
        _owner = battleGameBoard.player.party.Find(pokemon => pokemon.deck.Contains(this));

        if (overrideFunctionality) { overrideFunctionality.onBattleStart(this, battleGameBoard); }
    }

    /// <summary>
    /// Call when deck builder is finished to reset card state
    /// </summary>
    public void onDeckBuilderAddCardEnd()
    {
        deckBuilderAddCard = null;
    }

    public void onPlay(Pokemon user, Pokemon target)
    {
        print("PLAY: Used card " + cardName + " on " + target.pokemonName);
        if (!canBePlayed) { return; }
        battleGameBoard.onPlay(this, user, target);
    }

    public void play(Pokemon user, Pokemon target) {
        var attackMissed = false;

        // Run override if applicable
        if (overrideFunctionality) {
            overrideFunctionality.play(this, battleGameBoard, user, target);
        }
        // Don't use default functionality if specified
        var useCommonCardEffect = overrideFunctionality?.overridesPlayFunc() != true;
        if (useCommonCardEffect) {
            commonCardPlay(user, target);
        }      

        if (userAnimationType != "") user.GetComponent<Animator>().SetTrigger(userAnimationType);
        if (!attackMissed)
        {
            if (targetAnimationType != "") target.GetComponent<Animator>().SetTrigger(targetAnimationType);
            if (targetAnimationType2 != "") target.GetComponent<Animator>().SetTrigger(targetAnimationType2);
        }

        // Trigger flip functionality if applicable
        if (flipButtonFunctionality != null && flipButtonFunctionality.isFlipButtonEnabled(this, battleGameBoard))
        {
            flipButtonFunctionality.onPlay(this, battleGameBoard, target);
        }
    }

    private bool commonCardPlay(Pokemon user, Pokemon target)
    {
        var attackMissed = false;

        // Deal Damage if applicable
        if (damage > 0 && !target.isInvulnerable)
        {
            // Determine damage
            var dealtDamage = Mathf.RoundToInt(damage * user.attackMultStat / 100f * TypeChart.getEffectiveness(this, target)) - target.blockStat;
            target.blockStat = Mathf.Max(-dealtDamage, 0);
            user.attackMultStat = 100;

            // Hit target
            var newHealth = target.health - Mathf.Max(dealtDamage, 0);
            target.health = Mathf.Max(newHealth, 0);
        }
        if (damage > 0 && target.isInvulnerable)
        {
            attackMissed = true;
        }

        // Add status effects if applicable
        var statusTarget = (statusAffectsUser) ? user : target;
        addStatHelper(statusTarget, "attackStat", attackStat);
        addStatHelper(statusTarget, "defenseStat", defenseStat);
        addStatHelper(statusTarget, "specialStat", specialStat);
        addStatHelper(statusTarget, "evasionStat", evasionStat);
        addStatHelper(statusTarget, "blockStat", blockStat);
        addStatHelper(statusTarget, "attackMultStat", attackMultStat);
        if (grantsInvulnerability)
        {
            statusTarget.attachedStatus.Add(new StatusEffect(statusTarget, this, "invulnerabilityEffect", new Dictionary<string, string>() {
                { "statType", "invulnerability" },
                { "stackCount", "1" },
                { "turnsLeft", "1" }
            }));
        }

        // Heal/dmg user if possible
        if (!attackMissed && userHeal > 0)
        {
            user.health += userHeal;
        }

        return attackMissed;
    }

    private void addStatHelper(Pokemon target, string statName, int statValue)
    {
        if (statValue > 0)
        {
            target.attachedStatus.Add(new StatusEffect(target, this, statName + "Effect", new Dictionary<string, string>() {
                { "statType", statName.ToString() },
                { "stackCount", statValue.ToString() },
                { "turnsLeft", turn.ToString() }
            }));
        }
    }

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
        if (inDeckBuilderWorkflow) { return; }

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

    /// <summary>
    /// Returns true if the card can be used on the given target
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool canTarget(Pokemon target)
    {
        // Handle deck builder flow
        if (inDeckBuilderWorkflow) { return true; }

        // Handle null flow
        if (battleGameBoard.opponent == null ||
            battleGameBoard.opponent.party == null ||
            battleGameBoard.player == null ||
            battleGameBoard.player.party == null) {
            return false;
        }
        var isSelf = battleGameBoard?.activePokemon == target;
        var isOpp = battleGameBoard?.opponentActivePokemon == target;
        var onOppTeam = battleGameBoard.opponent.party.Contains(target);
        var onTeam = battleGameBoard.player.party.Contains(target);

        if (target.isFainted) {
            return false;
        }
        else if (targetType == "Self")
        {
            return isSelf;
        }
        else if (targetType == "Bench")
        {
            return !isSelf && onTeam;
        }
        else if (targetType == "Team")
        {
            return onTeam;
        }
        else if (targetType == "ActiveOpponent")
        {
            return isOpp;
        }
        else if (targetType == "BenchOpponent")
        {
            return !isOpp && onOppTeam;
        }
        else if (targetType == "AnyOpponent")
        {
            return onOppTeam;
        }

        // targetType == Any
        return true;
    }

    ///////////////////////////////////
    // Card Override Section
    ///////////////////////////////////

    public void onDraw(Pokemon activePokemon)
    {
        cardAnimator.SetTrigger("onDrawCard");
        if (overrideFunctionality) { overrideFunctionality.onDraw(this, battleGameBoard, activePokemon); }
    }

    public void onOpponentDraw(Pokemon opponentActivePokemon) {
        if (overrideFunctionality) { overrideFunctionality.onOpponentDraw(this, battleGameBoard, opponentActivePokemon); }
    }

    public void onTurnEnd() {
        if (overrideFunctionality) { overrideFunctionality.onTurnEnd(this, battleGameBoard); }
        if (flipButtonFunctionality && flipButtonFunctionality.isCardFlipped()) { flipButtonFunctionality.onUnflipEvent(this, battleGameBoard); }
    }

    public void onOpponentTurnEnd() {
        if (overrideFunctionality) { overrideFunctionality.onOpponentTurnEnd(this, battleGameBoard); }
    }

    public void onBattleEnd() {
        if (overrideFunctionality) { overrideFunctionality.onBattleEnd(this, battleGameBoard); }
    }

    public void onCardPlayed(Card move, Pokemon user, Pokemon target)
    {
        if (overrideFunctionality) { overrideFunctionality.onCardPlayed(this, battleGameBoard, move, user, target); }
    }

    public void onDiscard(bool wasPlayed) {
        if (overrideFunctionality) { overrideFunctionality.onDiscard(this, battleGameBoard, wasPlayed); }
    }


    ///////////////////////////////////
    // Flip Button Section
    ///////////////////////////////////

    public bool isFlipButtonEnabled()
    {
        if (flipButtonFunctionality == null) return false;
        return flipButtonFunctionality.isFlipButtonEnabled(this, battleGameBoard);
    }

    public void onFlipButtonPress()
    {
        if (!canBePlayed) { return; }
        cardAnimator.SetTrigger("onFlip");
        flipButtonFunctionality?.onFlipButtonPress(this, battleGameBoard);
    }

    public void onFlip()
    {
        flipButtonFunctionality?.onFlipEvent(this, battleGameBoard);
    }

    public void onUnflip()
    {
        flipButtonFunctionality?.onUnflipEvent(this, battleGameBoard);
    }


}
