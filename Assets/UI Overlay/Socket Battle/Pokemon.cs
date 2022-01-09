using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// The stats for the given pokemon
/// </summary>
public class Pokemon : MonoBehaviour
{
    public string pokemonName;

    public List<string> pokemonTypes;

    private int _health;
    public int health {
        get { return _health; }
        set {
            if (_health != 0 && value <= 0) gameObject.GetComponent<Animator>().SetTrigger("onDeath"); 
            _health = Mathf.Max(value, 0);
        }
    }

    public int initHealth;

    public int attackStat;
    public int defenseStat;
    public int specialAttackStat;
    public int specialDefenseStat;
    public int evasionStat;
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

    /// <summary>
    /// Max number of energy a pokemon can hold
    /// </summary>
    public int maxNumberOfAttachedEnergy = 2;
    public List<string> energyTypes;
    public List<string> possibleEnergyTypes;

    public List<Energy> attachedEnergy;
    public List<StatusEffect> attachedStatus;

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
    /// The pokemon to play
    /// </summary>
    public GameObject pokemonModel;

    /// <summary>
    /// The select indicator
    /// </summary>
    public GameObject pokemonSelectModel;

    /// <summary>
    /// The object containing the 2D box collider to move into position
    /// </summary>
    public DropEvent onHoverWrapper;

    private BattleGameBoard battleGameBoard;

    // Start is called before the first frame update
    void Start()
    {
        if (nameText) nameText.text = pokemonName;
        attachedStatus = new List<StatusEffect>();
        gameObject.GetComponent<Animator>().SetBool("hasSpawned", false);
    }

    // Update is called once per frame
    void Update()
    {
        var healthDesc = ("{health} / {initHealth}")
            .Replace("{health}", health.ToString())
            .Replace("{initHealth}", initHealth.ToString());
        if (healthText) healthText.text = healthDesc;

        // Update attached Energies to placeholder locations
        var i = 0;
        attachedEnergy.ForEach(e =>
        {
            e.transform.localScale = energyLocations[i].transform.localScale;
            e.transform.localRotation = energyLocations[i].transform.localRotation;
            //e.transform.position = energyLocations[i].transform.position;
            e.Translate(energyLocations[i].transform.position);

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
                statusLocation.text = statusEffect.statusEffectDisplay;
            }
            else {
                statusLocation.text = "";
            }

            j++;
        });
    }

    public void onBattleStart(BattleGameBoard _battleGameBoard)
    {
        gameObject.GetComponent<Animator>().SetBool("hasSpawned", true);

        battleGameBoard = _battleGameBoard;
        health = initHealth;
        attachedStatus = new List<StatusEffect>();

        // Hide locked energies
        energyLocations.ForEach(e =>
        {
            var index = energyLocations.IndexOf(e);
            e.SetActive(index < maxNumberOfAttachedEnergy);
        });
    }

    /// <summary>
    /// Set the pokemon in the spot according to the hud and model placeholder positions
    /// </summary>
    /// <param name="placeholder"></param>
    /// <param name="modelPlaceholder"></param>
    public void setPlacement(Pokemon placeholder, GameObject modelPlaceholder)
    {
        // Set HUD position
        transform.position = placeholder.transform.position;

        // Set box collider position
        if (onHoverWrapper) onHoverWrapper.gameObject.transform.position = placeholder.onHoverWrapper.gameObject.transform.position;

        // Set model position
        // ASSUMING structure is model -> model-animation-target -> <actual model>
        pokemonModel.gameObject.transform.parent.parent.rotation *= modelPlaceholder.transform.localRotation;
        pokemonModel.gameObject.transform.parent.parent.rotation *= Quaternion.Euler(0, 180f, 0);
        pokemonModel.gameObject.transform.position = modelPlaceholder.transform.position;

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
