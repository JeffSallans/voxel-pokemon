using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stats for the given pokemon
/// </summary>
public class Pokemon : MonoBehaviour
{
    public string pokemonName;

    public List<string> pokemonTypes;

    public int health;
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
}
