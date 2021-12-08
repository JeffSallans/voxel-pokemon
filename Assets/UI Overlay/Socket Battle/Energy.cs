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
        
    }

    public void onBattleStart(BattleGameBoard _battleGameBoard)
    {
        battleGameBoard = _battleGameBoard;
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

        isUsed = true;
        attachedToPokemon = target;
    }

    public void onTurnEnd() { }

    public void onOpponentTurnEnd() { }

    public void onBattleEnd() { }
}
