using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// The stats for the given pokemon
/// </summary>
[RequireComponent(typeof(HealthBar))]
public class Pokemon : MonoBehaviour
{
    public string pokemonName;

    public int level = 1;

    public List<string> pokemonTypes;

    private int _health;
    public int health {
        get { return _health; }
        set {
            if (_health != 0 && value <= 0) modelAnimator.SetTrigger("onDeath"); 
            _health = Mathf.Max(value, 0);
            healthBar.UpdateHealthBar(this);
        }
    }

    public bool isFainted
    {
        get { return _health <= 0; }
    }

    public int initHealth;
    public int initAttackStat;
    public int initDefenseStat;
    public int initSpecialStat;
    public int initEvasionStat;

    public int attackStat
    {
        get
        {
            var statusEffect = attachedStatus.Where(s => s.statType == "attackStat").FirstOrDefault();
            if (statusEffect == null) return 0;
            return statusEffect.stackCount;
        }
        set
        {
            var statusEffect = attachedStatus.Where(s => s.statType == "attackStat").FirstOrDefault();
            if (statusEffect == null) { print("SHOULD NEVER GET HERE"); }
            else { statusEffect.stackCount = value; }
        }
    }
    public int defenseStat
    {
        get
        {
            var statusEffect = attachedStatus.Where(s => s.statType == "defenseStat").FirstOrDefault();
            if (statusEffect == null) return 0;
            return statusEffect.stackCount;
        }
        set
        {
            var statusEffect = attachedStatus.Where(s => s.statType == "defenseStat").FirstOrDefault();
            if (statusEffect == null) { print("SHOUND NEVER GET HERE"); }
            else { statusEffect.stackCount = value; }
        }
    }
    public int specialStat
    {
        get
        {
            var statusEffect = attachedStatus.Where(s => s.statType == "specialStat").FirstOrDefault();
            if (statusEffect == null) return 0;
            return statusEffect.stackCount;
        }
        set
        {
            var statusEffect = attachedStatus.Where(s => s.statType == "specialStat").FirstOrDefault();
            if (statusEffect == null) { print("SHOUND NEVER GET HERE"); }
            else { statusEffect.stackCount = value; }
        }
    }
    public int evasionStat
    {
        get
        {
            var statusEffect = attachedStatus.Where(s => s.statType == "evasionStat").FirstOrDefault();
            if (statusEffect == null) return 0;
            return statusEffect.stackCount;
        }
        set
        {
            var statusEffect = attachedStatus.Where(s => s.statType == "evasionStat").FirstOrDefault();
            if (statusEffect == null) { print("SHOUND NEVER GET HERE"); }
            else { statusEffect.stackCount = value; }
        }
    }
    public int blockStat
    {
        get
        {
            var blockStatusEffect = attachedStatus.Where(s => s.statType == "blockStat").FirstOrDefault();
            if (blockStatusEffect == null) return 0;
            return blockStatusEffect.stackCount;
        }
        set
        {
            var blockStatusEffect = attachedStatus.Where(s => s.statType == "blockStat").FirstOrDefault();
            if (blockStatusEffect == null) { print("SHOUND NEVER GET HERE"); }
            else { blockStatusEffect.stackCount = value; }
        }
    }
    public int attackMultStat
    {
        get
        {
            var statusEffect = attachedStatus.Where(s => s.statType == "attackMultStat").FirstOrDefault();
            if (statusEffect == null) return 100;
            return statusEffect.stackCount;
        }
        set
        {
            var statusEffect = attachedStatus.Where(s => s.statType == "attackMultStat").FirstOrDefault();
            if (statusEffect == null) { print("SHOUND NEVER GET HERE"); }
            else { statusEffect.stackCount = value; }
        }
    }
    public bool isInvulnerable
    {
        get
        {
            var statusEffect = attachedStatus.Where(s => s.statType == "invulnerability").FirstOrDefault();
            if (statusEffect == null) return false;
            return statusEffect.stackCount > 0;
        }
    }

    /// <summary>
    /// Max number of energy a pokemon can hold
    /// </summary>
    public int maxNumberOfAttachedEnergy = 2;
    public List<string> energyTypes;
    public List<string> possibleEnergyTypes;

    public List<Energy> attachedEnergy;
    public List<StatusEffect> attachedStatus;

    /// <summary>
    /// Cards for the pokemon
    /// </summary>
    public List<Card> initDeck;

    /// <summary>
    /// Cards to be drawn
    /// </summary>
    public List<Card> deck;

    /// <summary>
    /// Reference to switch card
    /// </summary>
    public Card initSwitchCard;

    /// <summary>
    /// Cards to aways show to switch out
    /// </summary>
    public Card switchCard;

    /// <summary>
    /// The hand the player has
    /// </summary>
    public List<Card> hand;

    /// <summary>
    /// The cards the player used to be reshuffled
    /// </summary>
    public List<Card> discard;

    /// <summary>
    /// Shows the name
    /// </summary>
    public TextMeshProUGUI nameText;
    /// <summary>
    /// Shows the current health
    /// </summary>
    public TextMeshProUGUI healthText;
    /// <summary>
    /// Textbox for opponent to show next attack
    /// </summary>
    public TextMeshProUGUI nextAttackText;

    public List<GameObject> energyLocations;

    public List<TextMeshProUGUI> statusLocations;

    /// <summary>
    /// The pokemon to play should be "model-script-target"
    /// </summary>
    public GameObject pokemonModel;

    /// <summary>
    /// The pokemon position root should be "model"
    /// </summary>
    public GameObject pokemonRootModel;

    /// <summary>
    /// The select indicator
    /// </summary>
    public GameObject pokemonSelectModel;

    /// <summary>
    /// The card parent
    /// </summary>
    public GameObject cardsParent;

    /// <summary>
    /// The overlay parent
    /// </summary>
    public GameObject overlayParent;

    /// <summary>
    /// The object containing the 2D box collider to move into position
    /// </summary>
    public DropEvent onHoverWrapper;

    /// <summary>
    /// The animator for pokemon hud and select movements
    /// </summary>
    public Animator hudAnimator;

    /// <summary>
    /// The animator for pokemon model movements
    /// </summary>
    public Animator modelAnimator;

    private BattleGameBoard battleGameBoard;
    private HealthBar healthBar;

    // Start is called before the first frame update
    void Awake()
    {
        if (nameText) nameText.text = pokemonName;
        attachedStatus = new List<StatusEffect>();
        modelAnimator = transform.Find("model").GetComponent<Animator>();
        modelAnimator.SetBool("hasSpawned", false);
        hudAnimator = gameObject.GetComponent<Animator>();
        healthBar = gameObject.GetComponent<HealthBar>();
        health = initHealth;

        if (initDeck.Count == 0)
        {
            initDeck = gameObject.GetComponentsInChildren<Card>().ToList();
        }
        if (initSwitchCard == null)
        {
            initSwitchCard = initDeck.Find(c => c.cardName == "Switch");
        }
        hideModels();
    }

    // Update is called once per frame
    void Update()
    {
        var healthDesc = ("{health}/{initHealth}")
            .Replace("{health}", health.ToString())
            .Replace("{initHealth}", initHealth.ToString());
        if (healthText) healthText.text = healthDesc;

        healthBar.UpdateHealthBar(this);

        // Update attached Energies to placeholder locations
        var i = 0;
        attachedEnergy.ForEach(e =>
        {
            e.transform.localScale = energyLocations[i].transform.localScale * 0.75f;
            e.transform.localRotation = energyLocations[i].transform.localRotation;
            //e.transform.position = energyLocations[i].transform.position;
            e.Translate(energyLocations[i].transform.position + new Vector3(0,0,-0.1f));

            i++;
        });

        // Update status effect
        var j = 0;
        statusLocations.ForEach(statusLocation =>
        {
            if (attachedStatus.Count > j)
            {
                var statusEffect = attachedStatus[j];
                statusEffect.Update();
                statusLocation.text = UnicodeUtil.replaceWithUnicode(statusEffect.statusEffectDisplay);
            }
            else {
                statusLocation.text = "";
            }

            j++;
        });
    }

    public void onBattleStart(BattleGameBoard _battleGameBoard)
    {
        modelAnimator.SetBool("hasSpawned", true);

        battleGameBoard = _battleGameBoard;
        health = initHealth;
        attachedStatus = new List<StatusEffect>();
        attachedEnergy = new List<Energy>();

        // Set initial stats
        addInitialStatHelper("attackStat", initAttackStat);
        addInitialStatHelper("defenseStat", initDefenseStat);
        addInitialStatHelper("specialStat", initSpecialStat);
        addInitialStatHelper("evasionStat", initEvasionStat);

        // Hide locked energies
        energyLocations.ForEach(e =>
        {
            var index = energyLocations.IndexOf(e);
            e.SetActive(index < maxNumberOfAttachedEnergy);
        });

        // Remove attached energies
        attachedEnergy = new List<Energy>();
    }

    public void onDeckBuildStart(DeckBuilderAddCard _deckBuilderAddCard)
    {
        modelAnimator.SetBool("hasSpawned", true);

        health = initHealth;
        attachedStatus = new List<StatusEffect>();
        attachedEnergy = new List<Energy>();

        // Set initial stats
        addInitialStatHelper("attackStat", initAttackStat);
        addInitialStatHelper("defenseStat", initDefenseStat);
        addInitialStatHelper("specialStat", initSpecialStat);
        addInitialStatHelper("evasionStat", initEvasionStat);

        // Hide locked energies
        energyLocations.ForEach(e =>
        {
            var index = energyLocations.IndexOf(e);
            e.SetActive(index < maxNumberOfAttachedEnergy);
        });

        // Remove attached energies
        attachedEnergy = new List<Energy>();
    }

    public void onMenuStart()
    {
        modelAnimator.SetBool("hasSpawned", true);

        health = initHealth;
        attachedStatus = new List<StatusEffect>();
        attachedEnergy = new List<Energy>();

        // Set initial stats
        addInitialStatHelper("attackStat", initAttackStat);
        addInitialStatHelper("defenseStat", initDefenseStat);
        addInitialStatHelper("specialStat", initSpecialStat);
        addInitialStatHelper("evasionStat", initEvasionStat);

        // Hide locked energies
        energyLocations.ForEach(e =>
        {
            var index = energyLocations.IndexOf(e);
            e.SetActive(index < maxNumberOfAttachedEnergy);
        });

        // Remove attached energies
        attachedEnergy = new List<Energy>();
    }

    /// <summary>
    /// Show all visible models
    /// </summary>
    public void showModels()
    {
        cardsParent.SetActive(true);
        pokemonRootModel.SetActive(true);
        overlayParent.SetActive(true);

        // For each card
        var cards = gameObject.GetComponentsInChildren<Card>();
        cards.ToList().ForEach(
            e => e.GetComponentsInChildren<MeshRenderer>()
                .ToList()
                .ForEach(m => m.enabled = true)
        );

        // For each energy hide mesh render
        var energies = gameObject.GetComponentsInChildren<Energy>();
        energies.ToList().ForEach(e => e.GetComponentInChildren<MeshRenderer>().enabled = true);
    }

    /// <summary>
    /// Hide all visible models
    /// </summary>
    public void hideModels()
    {
        cardsParent.SetActive(false);
        pokemonRootModel.SetActive(false);
        overlayParent.SetActive(false);

        // For each card
        var cards = gameObject.GetComponentsInChildren<Card>();
        cards.ToList().ForEach(
            e => e.GetComponentsInChildren<MeshRenderer>()
                .ToList()
                .ForEach(m => m.enabled = false)
        );

        // For each energy hide mesh render
        var energies = gameObject.GetComponentsInChildren<Energy>();
        energies.ToList().ForEach(e => e.GetComponentInChildren<MeshRenderer>().enabled = false);
    }

    private void addInitialStatHelper(string statName, int statValue)
    {
        if (statValue > 0)
        {
            attachedStatus.Add(new StatusEffect(this, null, statName + "Effect", new Dictionary<string, string>() {
                { "statType", statName.ToString() },
                { "stackCount", statValue.ToString() },
                { "turnsLeft", "99" }
            }));
        }
    }

    private Transform previousHudParent = null;

    private RectTransform prevPokemonPlaceholder = null;

    private Transform previousModelParent = null;

    private Transform prevPokemonModelPlaceholder = null;

    public void setupPokemonPlacement(Pokemon placeholder, GameObject modelPlaceholder)
    {
        // Set HUD Parent
        prevPokemonPlaceholder = GetComponent<RectTransform>();
        previousHudParent = transform.parent;
        // Set HUD Position
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        transform.SetParent(placeholder.transform.parent, false);
        transform.rotation = placeholder.transform.rotation;
        transform.position = placeholder.transform.position;
        // Show HUD
        transform.Find("pokemon-overlay").gameObject.SetActive(true);
        // Set HUD Text position because sometimes it shifts between scenes
        // TODO: Review why this is happening and remove this code
        transform.Find("pokemon-overlay").transform.position = placeholder.transform.Find("pokemon-overlay").transform.position;

        // Set Model Parent
        prevPokemonModelPlaceholder = pokemonRootModel.GetComponent<Transform>();
        previousModelParent = pokemonRootModel.transform.parent;
        // Set Model Position
        // ASSUMING structure is model -> model-animation-target -> <actual model>
        pokemonRootModel.transform.SetParent(modelPlaceholder.transform.parent, false);
        pokemonRootModel.transform.localScale = modelPlaceholder.transform.localScale;
        pokemonRootModel.transform.rotation = modelPlaceholder.transform.rotation;
        if (isFainted) {
            var deadPos = new Vector3(modelPlaceholder.transform.position.x, pokemonRootModel.gameObject.transform.position.y, modelPlaceholder.transform.position.z);
            pokemonRootModel.gameObject.transform.position = deadPos;
        } else
        {
            pokemonRootModel.transform.position = modelPlaceholder.transform.position;
        }

        // Set box collider position
        if (onHoverWrapper) onHoverWrapper.gameObject.transform.position = placeholder.onHoverWrapper.gameObject.transform.position;

        // Set select position
        pokemonSelectModel.gameObject.transform.position = modelPlaceholder.transform.position;
    }

    public void packupPokemonPlacement()
    {
        // Set HUD Parent
        transform.SetParent(previousHudParent, false);
        // Set HUD Position
        transform.rotation = prevPokemonPlaceholder.rotation;
        transform.position = prevPokemonPlaceholder.position;
        // Hide HUD
        transform.Find("pokemon-overlay").gameObject.SetActive(false);

        // Set Model Parent
        pokemonRootModel.transform.SetParent(previousModelParent, false);
        // Set Model Position
        // ASSUMING structure is model -> model-animation-target -> <actual model>
        pokemonRootModel.transform.localScale = prevPokemonModelPlaceholder.transform.localScale;
        pokemonRootModel.transform.rotation = prevPokemonModelPlaceholder.rotation;
        pokemonRootModel.transform.position = prevPokemonModelPlaceholder.position;
    }

    /// <summary>
    /// Set the pokemon in the spot according to the hud and model placeholder positions. Assumes animateModelTranslation is only false once.
    /// </summary>
    /// <param name="placeholder"></param>
    /// <param name="modelPlaceholder"></param>
    public void updatePlacement(Pokemon placeholder, GameObject modelPlaceholder, bool animateModelTranslation = false)
    {
        // Check assumptions
        if (placeholder.transform.parent != transform.parent) Debug.LogError("Updating placement of " + pokemonName + " before setupPokemonPlacement is called. Check placeholder.");
        if (modelPlaceholder.transform.parent != pokemonRootModel.transform.parent) Debug.LogError("Updating placement of " + pokemonName + " before setupPokemonPlacement is called. Check modelPlaceholder.");

        // Set HUD position
        transform.rotation = placeholder.transform.rotation;
        transform.position = placeholder.transform.position;

        // Set HUD Text position because sometimes it shifts between scenes
        // TODO: Review why this is happening and remove this code
        transform.Find("pokemon-overlay").transform.position = placeholder.transform.Find("pokemon-overlay").transform.position;

        // Set Model Position
        // ASSUMING structure is model -> model-animation-target -> <actual model>
        pokemonModel.transform.rotation = modelPlaceholder.transform.rotation;
        if (isFainted)
        {
            var deadPos = new Vector3(modelPlaceholder.transform.position.x, pokemonRootModel.gameObject.transform.position.y, modelPlaceholder.transform.position.z);
            pokemonRootModel.gameObject.transform.position = deadPos;
        }
        else
        {
            pokemonRootModel.transform.position = modelPlaceholder.transform.position;
        }

        // Set box collider position
        if (onHoverWrapper) onHoverWrapper.gameObject.transform.position = placeholder.onHoverWrapper.gameObject.transform.position;

        // Set select position
        pokemonSelectModel.gameObject.transform.position = modelPlaceholder.transform.position;
    }

    public void onTurnEnd() { }

    public void onOpponentTurnEnd() {
        // Need to use this method because this function might shrink attachedStatus function
        var statusCount = attachedStatus.Count;
        for (var i = statusCount - 1; i >= 0; i--)
        {
            attachedStatus[i].onOpponentTurnEnd();
        }
    }

    public void onBattleEnd() {

        // Display
        modelAnimator.SetBool("hasSpawned", false);

        // Remove hand
        battleGameBoard.discard.AddRange(hand);
        hand.RemoveAll(c => true);

        // Remove attached energies
        battleGameBoard.energyDiscard.AddRange(attachedEnergy);
        attachedEnergy.RemoveAll(e => true);
        Update();
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

    /// <summary>
    /// Removes the energy at the given index from the pokemon
    /// </summary>
    /// <param name="energyToRemoveIndex"></param>
    public void DiscardEnergy(int energyToRemoveIndex)
    {
        if (energyToRemoveIndex >= attachedEnergy.Count)
        {
            Debug.LogError("Trying to remove energy at index " + energyToRemoveIndex + " but attached energy is only " + attachedEnergy.Count);
            return;
        }

        var energyToRemove = attachedEnergy[energyToRemoveIndex];
        
        // Reset animation state
        energyToRemove.animator.SetTrigger("onToBeUsedHoverLeave");
        energyToRemove.animator.SetBool("isUsed", false);
        energyToRemove.SetCanBeDragged(true);
        
        // Move energy to discard
        energyToRemove.transform.localScale = battleGameBoard.energyDiscardLocation.transform.localScale;
        energyToRemove.transform.localRotation = battleGameBoard.energyDiscardLocation.transform.localRotation;
        energyToRemove.Translate(battleGameBoard.energyDiscardLocation.transform.position);
        
        battleGameBoard.energyDiscard.Add(energyToRemove);
        attachedEnergy.Remove(energyToRemove);
    }

    /// <summary>
    /// Removes the energy at the given index from the pokemon
    /// </summary>
    public void DiscardEnergy(Energy energyToRemove)
    {
        DiscardEnergy(attachedEnergy.IndexOf(energyToRemove));
    }
}
