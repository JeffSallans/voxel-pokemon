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
    public int defenceStat;
    public int specialAttackStat;
    public int specialDefenceStat;
    public int evasionStat;

    public int availableEnergySockets;
    public List<string> energyTypes;
    public List<string> possibleEnergyTypes;

    public List<Energy> attachedEnergy;
    public List<StatusEffect> attachedStatus;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
