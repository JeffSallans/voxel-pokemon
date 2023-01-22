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
    /// If cost is not setup, this will setup cost using prefabs to be instantiated
    /// </summary>
    public List<Energy> prefabCost;

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
    /// How good the card is to determine how often it should be pulled
    /// </summary>
    public CardRarity cardRarity = CardRarity.Average;

    public enum CardRarity
    {
        BelowAverage, // Below Average
        Average, // Average
        Great // Above Average
    }

    /// <summary>
    /// The theme of the card (Rush, Control, Support, Combo)
    /// </summary>
    public CardTheme theme = CardTheme.Rush;

    public enum CardTheme
    {
        Rush,
        Control,
        Support,
        Combo
    }

    public enum CardTargetType
    {
        Self,
        Bench,
        Team,
        ActiveOpponent,
        AnyOpponent,
        BenchOpponent,
        AnyPokemon,
        BenchAll,
        TeamAll,
        AnyOpponentAll,
        BenchOpponentAll,
        AnyPokemonAll,
        PokemonAllButUser,
        OverrideTarget,
    }

    /// <summary>
    /// The possible targets of this card. Self, Bench, Team, ActiveOpponent, AnyOpponent, BenchOpponent, AnyPokemon,
    /// </summary>
    public CardTargetType targetType;

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
        set { initDamage = value; }
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
    /// How much the target is healed
    /// </summary>
    public int heal;

    /// <summary>
    /// True if the user gets immunity to the next attack
    /// </summary>
    public bool grantsInvulnerability;

    /// <summary>
    /// Number of poison stacks to apply, 5 stacks does 50 dmg over 5 turns
    /// </summary>
    public int poisonStacks;

    /// <summary>
    /// True if the target cannot switch in or out next turn
    /// </summary>
    public bool applyTrap;

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
    /// True if the card will add 1 to the remainingCardsToPlay counter
    /// </summary>
    public bool playAnotherCard = false;

    /// <summary>
    /// Set to a number greater than 0 to show the number of cards to draw
    /// </summary>
    public int numberOfCardsToDraw = 0;

    /// <summary>
    /// True if the card should keep the energy cost when played
    /// </summary>
    public bool keepEnergiesOnPlay = false;

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
            // Override this check for a different one
            if (canBePlayedOverride != null) { return canBePlayedOverride(); }
            
            // Override this check if using messaging
            if (overrideDefaultDragDropFunc) { return true; }

            // Override this check if DeckBuilder is here
            if (inDeckBuilderWorkflow) { return true; }

            // Check if there are any remaining card plays this turn
            if (battleGameBoard?.remainingNumberOfCardsCanPlay == 0) return false;

            // Check if self is trapped
            if (owner != null && battleGameBoard != null && owner.isTrapped && battleGameBoard?.activePokemon != owner) return false;

            // Check if active is trapped
            if (owner != null && battleGameBoard != null && battleGameBoard.activePokemon.isTrapped && battleGameBoard.activePokemon != owner) return false;

            // Check color energy count
            var costAsString = cost?.Select(c => c?.energyName)
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
    /// Where the card name should be rendered
    /// </summary>
    public TextMeshProUGUI nameGameObject;

    /// <summary>
    /// Where the card description should be rendered
    /// </summary>
    public TextMeshProUGUI descriptionGameObject;

    /// <summary>
    /// Where the card target should be displayed
    /// </summary>
    public TextMeshProUGUI targetGameObject;

    /// <summary>
    /// Button that plays the card
    /// </summary>
    public Button playButtonGameObject;

    /// <summary>
    /// A list of locations of all the attached energy
    /// </summary>
    public List<GameObject> energyLocations;

    /// <summary>
    /// The model to show when the card is disabled
    /// </summary>
    public GameObject disabledMesh;

    /// <summary>
    /// A list of card meshes to show based on the given type. In the order of normal, fire, electric, psychic, grass, water, ground.
    /// </summary>
    public List<GameObject> energyMeshs;

    /// <summary>
    /// The order of energyMeshs
    /// </summary>
    private List<string> energyMeshKeys = new List<string> { "Normal", "Fire", "Electric", "Psychic", "Grass", "Water", "Ground" };

    /// <summary>
    /// Set to true when we don't want
    /// </summary>
    private bool overrideDefaultDragDropFunc = false;

    /// <summary>
    /// Used to enchance the default game onDrag functionality
    /// </summary>
    public Func<bool> _onDragFunc = null;

    /// <summary>
    /// Used to enchance the default game onDrop functionality
    /// </summary>
    public Func<bool> _onDropFunc = null;

    public string lastTranslate = "";

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
    /// Set to true for card hover and drag to work.
    /// </summary>
    public bool cardInteractEnabled = true;

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
    private DeckBuilderAddCard deckBuilderAddCard;

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

    private GameOptions gameOptions
    {
        get
        {
            if (battleGameBoard != null) return battleGameBoard.player.gameOptions;
            if (deckBuilderAddCard != null) return deckBuilderAddCard.player.gameOptions;
            return null;
        }
    }

    private void Awake()
    {
        battleGameBoard = GameObject.FindObjectOfType<BattleGameBoard>();
        deckBuilderAddCard = GameObject.FindObjectOfType<DeckBuilderAddCard>();

        overrideFunctionality = GetComponent<ICard>();
        flipButtonFunctionality = GetComponent<IFlipButton>();

        // Create card cost energies
        if (cost.Count == 0)
        {
            for (var i = 0; i < prefabCost.Count; i++)
            {
                var energy = Instantiate(prefabCost[i], transform);
                energy.initCanBeDragged = false;
                cost.Add(energy);
            }
        }

        UpdateCardMesh();
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
        UpdateCardMesh();

        descriptionGameObject.text = UnicodeUtil.replaceWithUnicode(cardDesc);
        targetGameObject.text = GetTargetText();
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

        // Check for drag & drop
        if (gameOptions && gameOptions.useCardDragControls)
        {
            // Check for drop
            if (cardInteractEnabled && isDragging && Input.GetKeyUp(KeyCode.Mouse0))
            {
                onDrop();
            }
        }
        // use two click controls
        else
        {
            // Check for drop
            if (cardInteractEnabled && isDragging && Input.GetKeyDown(KeyCode.Mouse0))
            {
                onDrop();
            }

            // Check for "drag"
            if (cardInteractEnabled && isSelected && !isDragging && Input.GetKeyDown(KeyCode.Mouse0))
            {
                onDragHelper();
            }
        }

        // Update flip button activeness
        if (cardInteractEnabled && flipButtonFunctionality != null)
        {
            var flipButtonEnabled = isFlipButtonEnabled();
            if (flipButtonEnabled != cardAnimator.GetBool("isFlipButtonEnabled"))
            {
                cardAnimator.SetBool("isFlipButtonEnabled", flipButtonEnabled);
            }
        }

        if (cardInteractEnabled)
        {
            var newDropEvent = GetDropInteraction();
            OnHoverInteraction(newDropEvent);
        }
    }

    /// <summary>
    /// Returns the card target in text form
    /// </summary>
    /// <returns></returns>
    private string GetTargetText()
    {
        if (targetType == CardTargetType.Self)
        {
            return "Self";
        }
        else if (targetType == CardTargetType.Bench)
        {
            return "My Bench";
        }
        else if (targetType == CardTargetType.Team)
        {
            return "My Team";
        }
        else if (targetType == CardTargetType.ActiveOpponent)
        {
            return "Active Opp";
        }
        else if (targetType == CardTargetType.AnyOpponent)
        {
            return "Any Opp";
        }
        else if (targetType == CardTargetType.BenchOpponent)
        {
            return "Opp Bench";
        }
        else if (targetType == CardTargetType.BenchAll)
        {
            return "Both Bench";
        }
        else if (targetType == CardTargetType.TeamAll)
        {
            return "Entire Team";
        }
        else if (targetType == CardTargetType.BenchOpponentAll)
        {
            return "Both Opp Bench";
        }
        else if (targetType == CardTargetType.AnyOpponentAll)
        {
            return "All Opps";
        }
        else if (targetType == CardTargetType.AnyPokemonAll)
        {
            return "All";
        }
        else if (targetType == CardTargetType.PokemonAllButUser)
        {
            return "All But Self";
        }
        else
        {
            return "Any";
        }
    }


    /// <summary>
    /// Updates the card mesh display based on card state
    /// </summary>
    private void UpdateCardMesh()
    {
        var meshIndex = energyMeshKeys.IndexOf(cardType);

        if (canBePlayed && meshIndex >= 0)
        {
            disabledMesh.SetActive(false);
            energyMeshs[meshIndex].SetActive(true);
        }
        else if (meshIndex >= 0)
        {
            disabledMesh.SetActive(true);
            energyMeshs[meshIndex].SetActive(false);
        }
        else
        {
            disabledMesh.SetActive(true);
            Debug.LogWarning("Invalid card type " + cardType + ". Not in the expect list " + energyMeshKeys);
        }
    }


    public void OnHoverEnter()
    {
        if (!isSelected && !isDragging && cardInteractEnabled)
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
        if (!isDragging && cardInteractEnabled)
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
        if (isDragging && overrideDefaultDragDropFunc && newDropEvent?.eventType != null)
        {
            dropEvent = newDropEvent;
            FindObjectOfType<HoverAndDragMessageTarget>().OnHoverEnter(new HoverAndDragEvent()
            {
                eventType = dropEvent.eventType,
                dropEvent = dropEvent,
                targetCard = this
            });
        }
        // When we hover over deck builder
        else if (inDeckBuilderWorkflow && newDropEvent?.eventType == "TargetPokemon")
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
            dropEvent.targetPokemon.hudAnimator.SetBool("isSuperEffective", isSuperEffective);
            dropEvent.targetPokemon.hudAnimator.SetBool("isNotVeryEffective", isNotVeryEffective);
            dropEvent.targetPokemon.hudAnimator.SetTrigger("onHoverEnter");

        }
        // Discard hover
        else if (isDragging && newDropEvent?.eventType == "Discard")
        {
            dropEvent = newDropEvent;
            dropEvent.targetGameObject.GetComponent<Animator>().SetTrigger("onHoverEnter");
        }

        // When the hover event is gone
        if (newDropEvent == null && dropEvent != null && overrideDefaultDragDropFunc && dropEvent?.eventType != null)
        {
            FindObjectOfType<HoverAndDragMessageTarget>().OnHoverExit(new HoverAndDragEvent()
            {
                eventType = dropEvent.eventType,
                dropEvent = dropEvent,
                targetCard = this
            });
            dropEvent = null;
        }
        else if (newDropEvent == null && dropEvent != null && inDeckBuilderWorkflow && dropEvent?.eventType == "TargetPokemon")
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
            dropEvent.targetPokemon.hudAnimator.SetTrigger("onHoverExit");
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
        if (gameOptions && gameOptions.useCardDragControls) onDragHelper();
    }

    private void onDragHelper()
    {
        if (!cardInteractEnabled) return;

        isDragging = true;
        if (_onDragFunc != null) { _onDragFunc(); }
        
        if (overrideDefaultDragDropFunc)
        {
            FindObjectOfType<HoverAndDragMessageTarget>().OnDragHelper(this);
        }
    }

    public void onDrop()
    {
        if (!cardInteractEnabled) return;

        if (overrideDefaultDragDropFunc && dropEvent?.eventType != null)
        {
            isDragging = false;
            isSelected = false;
            FindObjectOfType<HoverAndDragMessageTarget>().OnDropHelper(new HoverAndDragEvent()
            {
                eventType = dropEvent.eventType,
                dropEvent = dropEvent,
                targetCard = this
            });
            OnHoverExit();
        }
        else if (overrideDefaultDragDropFunc && isDragging)
        {
            isDragging = false;
            isSelected = false;
            FindObjectOfType<HoverAndDragMessageTarget>().OnDragStopHelper(this);
            OnHoverExit();
        }
        // When you drop on a pokemon
        else if (inDeckBuilderWorkflow && dropEvent?.eventType == "TargetPokemon")
        {
            isDragging = false;
            isSelected = false;
            FindObjectOfType<HoverAndDragMessageTarget>().OnDropHelper(new HoverAndDragEvent()
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
            battleGameBoard.cardDiscard(this, owner, false);
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

    public void play(Pokemon user, Pokemon selectedTarget) {

        // Get all targets
        var targets = getTarget(targetType, selectedTarget);
        var attackMissed = targets.Select(t => false).ToList();

        // Run override if applicable
        if (overrideFunctionality) {
            attackMissed = overrideFunctionality.play(this, battleGameBoard, user, selectedTarget, targets);
        }
        // Don't use default functionality if specified
        var useCommonCardEffect = overrideFunctionality?.overridesPlayFunc() != true;
        if (useCommonCardEffect) {
            attackMissed = targets.Select(t =>
            {
                return commonCardPlay(user, t);
            }).ToList();
        }

        if (userAnimationType != "") user.modelAnimator.SetTrigger(userAnimationType);
        targets.ForEach(t =>
        {
            var index = targets.IndexOf(t);
            if (!attackMissed[index]) {
                if (targetAnimationType != "") t.modelAnimator.SetTrigger(targetAnimationType);
                if (targetAnimationType2 != "") t.modelAnimator.SetTrigger(targetAnimationType2);
            } 
        });

        // Trigger flip functionality if applicable
        if (flipButtonFunctionality != null && flipButtonFunctionality.isFlipButtonEnabled(this, battleGameBoard))
        {
            flipButtonFunctionality.onPlay(this, battleGameBoard, selectedTarget);
        }
    }

    private bool commonCardPlay(Pokemon user, Pokemon target)
    {
        var attackMissed = false;

        // Deal Damage if applicable
        if (damage > 0 && !target.isInvulnerable)
        {
            // Determine damage
            var dealtDamage = Mathf.RoundToInt(damage * user.attackMultStat / 100f * TypeChart.getEffectiveness(this, target)) 
                + (user.attackStat - target.defenseStat) * 10
                - target.blockStat;
            target.blockStat = Mathf.Max(-dealtDamage, 0);
            user.attackMultStat = 100;

            // Hit target
            var newHealth = target.health - Mathf.Max(dealtDamage, 0);
            target.health = Mathf.Min(Mathf.Max(newHealth, 0), target.initHealth);
        }
        if (damage > 0 && target.isInvulnerable)
        {
            attackMissed = true;
        }

        // Add status effects if applicable
        var statusTarget = (statusAffectsUser) ? user : target;
        addStatHelper(statusTarget, "attackStat", attackStat, 6);
        addStatHelper(statusTarget, "defenseStat", defenseStat, 6);
        addStatHelper(statusTarget, "specialStat", specialStat, 1);
        addStatHelper(statusTarget, "evasionStat", evasionStat, 6);
        addStatHelper(statusTarget, "blockStat", blockStat, statusTarget.initHealth);
        addStatHelper(statusTarget, "attackMultStat", attackMultStat);
        addStatHelper(statusTarget, "poison", poisonStacks, 15);
        if (grantsInvulnerability)
        {
            statusTarget.attachedStatus.Add(new StatusEffect(statusTarget, this, "invulnerabilityEffect", new Dictionary<string, string>() {
                { "statType", "invulnerability" },
                { "stackCount", "1" },
                { "turnsLeft", "1" }
            }));
        }
        if (applyTrap)
        {
            statusTarget.attachedStatus.Add(new StatusEffect(statusTarget, this, "trapEffect", new Dictionary<string, string>() {
                { "statType", "trap" },
                { "stackCount", "1" },
                { "turnsLeft", "1" }
            }));
        }

        // Heal if possible
        if (!attackMissed && heal > 0)
        {
            var newHealth = target.health + heal;
            target.health = Mathf.Min(newHealth, target.initHealth);
        }

        // Heal/dmg user if possible
        if (!attackMissed && userHeal > 0)
        {
            var newHealth = user.health + userHeal;
            user.health = Mathf.Min(newHealth, user.initHealth);
        }

        // Draw cards
        for (var i = 0; i < numberOfCardsToDraw; i++)
        {
            if (battleGameBoard.deck.Count < 1) { battleGameBoard.reshuffleDiscard(); }
            battleGameBoard.drawCard(battleGameBoard.activePokemon);
        }

        // Allow another card to be played
        if (playAnotherCard)
        {
            battleGameBoard.remainingNumberOfCardsCanPlay++;
        }

        return attackMissed;
    }

    /// <summary>
    /// Help set the stats
    /// </summary>
    /// <param name="target"></param>
    /// <param name="statName"></param>
    /// <param name="statValue"></param>
    /// <param name="maxStack">set to -1 to have no max stack</param>
    private void addStatHelper(Pokemon target, string statName, int statValue, int maxStack = -1)
    {
        if (statValue > 0)
        {
            target.attachedStatus.Add(new StatusEffect(target, null, statName + "Effect", new Dictionary<string, string>() {
                { "statType", statName.ToString() },
                { "stackCount", statValue.ToString() },
                { "maxStack", maxStack.ToString() },
                { "turnsLeft", turn.ToString() }
            }));
        }
    }

    /// <summary>
    /// Animate the card to a new location
    /// </summary>
    /// <param name="_targetPosition"></param>
    /// <param name="_distancePerSecond"></param>
    public void Translate(Vector3 _targetPosition, string callLocation, float _distancePerSecond = 150.0f)
    {
        if (_targetPosition == transform.position) return;
        lastTranslate = callLocation;
        gameObject.GetComponent<TranslationAnimation>().Translate(_targetPosition, _distancePerSecond,
            () => { cardInteractEnabled = false; return true; },
            () => { cardInteractEnabled = true; return true; }
        );
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
        if (overrideDefaultDragDropFunc) { return; }
        if (inDeckBuilderWorkflow) { return; }
        if (!cardInteractEnabled) { return; }

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
        // Handle custom flow
        if (overrideDefaultDragDropFunc) { return true; }

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
        else if (targetType == CardTargetType.Self)
        {
            return isSelf;
        }
        else if (targetType == CardTargetType.Bench || targetType == CardTargetType.BenchAll)
        {
            return !isSelf && onTeam;
        }
        else if (targetType == CardTargetType.Team || targetType == CardTargetType.TeamAll)
        {
            return onTeam;
        }
        else if (targetType == CardTargetType.ActiveOpponent || targetType == CardTargetType.PokemonAllButUser)
        {
            return isOpp;
        }
        else if (targetType == CardTargetType.BenchOpponent || targetType == CardTargetType.BenchOpponentAll)
        {
            return !isOpp && onOppTeam;
        }
        else if (targetType == CardTargetType.AnyOpponent || targetType == CardTargetType.AnyOpponentAll)
        {
            return onOppTeam;
        }

        // targetType == Any OR targetType == AnyAll
        return true;
    }

    /// <summary>
    /// Returns the target of the move
    /// </summary>
    /// <returns></returns>
    public List<Pokemon> getTarget(CardTargetType givenTargetType, Pokemon selectedTarget)
    {
        var bench = battleGameBoard.opponent.party.Where(p => !p.isFainted && p != battleGameBoard.activePokemon).ToList();
        var team = battleGameBoard.opponent.party.Where(p => !p.isFainted).ToList();

        var opponentBench = battleGameBoard.player.party.Where(p => !p.isFainted && p != battleGameBoard.activePokemon).ToList();
        var opponentTeam = battleGameBoard.player.party.Where(p => !p.isFainted).ToList();
        if (givenTargetType == CardTargetType.ActiveOpponent)
        {
            return new List<Pokemon>() { selectedTarget };
        }
        else if (givenTargetType == CardTargetType.Bench)
        {
            return new List<Pokemon>() { selectedTarget };
        }
        else if (givenTargetType == CardTargetType.Team)
        {
            return new List<Pokemon>() { selectedTarget };
        }
        else if (givenTargetType == CardTargetType.BenchOpponent)
        {
            return new List<Pokemon>() { selectedTarget };
        }
        else if (givenTargetType == CardTargetType.AnyOpponent)
        {
            return new List<Pokemon>() { selectedTarget };
        }
        else if (givenTargetType == CardTargetType.Self)
        {
            return new List<Pokemon>() { selectedTarget };
        }
        else if (givenTargetType == CardTargetType.TeamAll)
        {
            return team;
        }
        else if (givenTargetType == CardTargetType.BenchAll)
        {
            return bench;
        }
        else if (givenTargetType == CardTargetType.AnyOpponentAll)
        {
            return opponentTeam;
        }
        else if (givenTargetType == CardTargetType.BenchOpponentAll)
        {
            return opponentBench;
        }
        else if (givenTargetType == CardTargetType.AnyPokemonAll)
        {
            return team.Union(opponentTeam).ToList();
        }
        else  if (givenTargetType == CardTargetType.PokemonAllButUser)
        {
            return bench.Union(opponentTeam).ToList();
        }

        return new List<Pokemon>() { selectedTarget };
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

    /// <summary>
    /// Call to setup different card functionality
    /// </summary>
    public void setupDragDropOverride()
    {
        // Check if a component is setup correctly to receive messages
        if (GameObject.FindObjectOfType<HoverAndDragMessageTarget>() == null)
        {
            throw new Exception("HoverAndDragMessageTarget component doesn't exist in current scene");
        }

        overrideDefaultDragDropFunc = true;

    }

    /// <summary>
    /// Call to clean up different card functionality
    /// </summary>
    public void packupDragDropOverride()
    {
        overrideDefaultDragDropFunc = false;
    }
}
