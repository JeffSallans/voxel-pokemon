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
            var desc = moveDescriptionWithTemplates.Replace("{damage}", damage.ToString());
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

    public string playMove()
    {
        var target = battleGameBoard.activePokemon;
        var dealtDamage = damage;
        var newHealth = target.health - dealtDamage;
        target.health = newHealth;
        if (userAnimationType != "") actingPokemon.GetComponent<Animator>().SetTrigger(userAnimationType);
        if (targetAnimationType != "") target.GetComponent<Animator>().SetTrigger(targetAnimationType);
        if (targetAnimationType2 != "") target.GetComponent<Animator>().SetTrigger(targetAnimationType2);
        return moveDescription;
    }

    public void onTurnEnd() { }

    public void onOpponentTurnEnd() { }

    public void onBattleEnd() { }
}
