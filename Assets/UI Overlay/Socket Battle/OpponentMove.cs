using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentMove : MonoBehaviour
{
    public string moveName;

    /// <summary>
    /// Move description to show above the attacking pokemon head
    /// </summary>
    public string moveDescriptionWithTemplates;

    /// <summary>
    /// The move to show above the attacking pokemon
    /// </summary>
    public string moveDescription
    {
        get
        {
            var desc = moveDescriptionWithTemplates.Replace("{damage}", damage.ToString())
                    .Replace("{damageEnergy}", damageEnergy)
                    .Replace("{damageType}", isPhysicalType ? "physical" : "special");
            return desc;
        }
    }

    /// <summary>
    /// The pokemon that can use this move
    /// </summary>
    public Pokemon actingPokemon;

    /// <summary>
    /// The amount of damage to deal
    /// </summary>
    public int damage;

    /// <summary>
    /// The energy type of the damage
    /// </summary>
    public string damageEnergy;

    /// <summary>
    /// True if the damage type is physical, false if special
    /// </summary>
    public bool isPhysicalType;

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

    public void playMove()
    {
        var target = battleGameBoard.activePokemon;
        var dealtDamage = damage + actingPokemon.attackStat - target.defenseStat;
        var newHealth = target.health - dealtDamage;
        target.health = Mathf.Max(newHealth, 0);
    }

    public void onTurnEnd() { }

    public void onOpponentTurnEnd() { }

    public void onBattleEnd() { }
}
