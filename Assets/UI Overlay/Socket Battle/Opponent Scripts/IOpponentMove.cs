using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOpponentMove : MonoBehaviour
{
    public string moveName;

    /// <summary>
    /// Move description to show above the attacking pokemon head
    /// </summary>
    public string moveDescriptionWithTemplates;

    /// <summary>
    /// The move to show above the attacking pokemon
    /// </summary>
    virtual public string moveDescription
    {
        get
        {
            var desc = moveDescriptionWithTemplates
                .Replace("{userName}", actingPokemon.pokemonName)
                .Replace("{damage}", damage.ToString());

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
    public float initialPriority;

    /// <summary>
    /// The amount of damage to deal
    /// </summary>
    public float turnPriority;

    /// <summary>
    /// The amount of damage to deal
    /// </summary>
    public int damage;

    /// <summary>
    /// The energy type of the damage
    /// </summary>
    public string damageEnergy;

    /// <summary>
    /// True if using the move will switch in the user
    /// </summary>
    public bool switchInOnUse = true;

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
    /// True if the attack should happen before the player's turn
    /// </summary>
    public bool playInstantly;

    protected BattleGameBoard battleGameBoard;

    public virtual bool canUseMove
    {
        get { return true; }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void onBattleStart(BattleGameBoard _battleGameBoard)
    {
        battleGameBoard = _battleGameBoard;
    }

    public virtual string playMove() { return ""; }

    public virtual void onTurnEnd() { }

    public virtual void onOpponentTurnEnd() { }

    public virtual void onBattleEnd() { }

    /// <summary>
    /// Runs when the move is selected from the strategy script
    /// </summary>
    public virtual void onNextMoveSelect() { }
}
