using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        set { _health = Mathf.Max(value, 0); }
    }

    public int initHealth;

    public int attackStat;
    public int defenseStat;
    public int specialAttackStat;
    public int specialdefenseStat;
    public int evasionStat;

    public int availableEnergySockets;
    public List<string> energyTypes;
    public List<string> possibleEnergyTypes;

    public List<Energy> attachedEnergy;
    public List<StatusEffect> attachedStatus;

    public Text healthText;

    private BattleGameBoard battleGameBoard;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var healthDesc = ("{health} / {initHealth}")
            .Replace("{health}", health.ToString())
            .Replace("{initHealth}", initHealth.ToString());
        healthText.text = healthDesc;
    }

    public void onBattleStart(BattleGameBoard _battleGameBoard)
    {
        battleGameBoard = _battleGameBoard;
        health = initHealth;
    }

    public void onTurnEnd() { }

    public void onOpponentTurnEnd() { }

    public void onBattleEnd() { }
}
