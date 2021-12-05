using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            _isUsed = value;
        }
    }

    public Card attachedToCard;

    public Pokemon attachedToPokemon;

    /// <summary>
    /// Reference to the default object
    /// </summary>
    public GameObject defaultObject;

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

    public void onTurnEnd() { }

    public void onOpponentTurnEnd() { }

    public void onBattleEnd() { }
}
