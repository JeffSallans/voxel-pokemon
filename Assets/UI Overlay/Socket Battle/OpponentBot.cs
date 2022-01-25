using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentBot : MonoBehaviour
{
    public OpponentMove nextOpponentMove;

    private BattleGameBoard battleGameBoard;
    private OpponentDeck opponentDeck
    {
        get { return battleGameBoard.opponent; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (nextOpponentMove != null) 
        {
            nextOpponentMove.actingPokemon.nextAttackText.text = UnicodeUtil.replaceWithUnicode(nextOpponentMove?.moveDescription);
        }
    }

    public void onBattleStart(BattleGameBoard _battleGameBoard)
    {
        battleGameBoard = _battleGameBoard;
    }

    public string opponentPlay()
    {
        return nextOpponentMove.playMove();
    }

    public void computeOpponentsNextMove()
    {
        // Randomly select move
        var moveIndex = Mathf.FloorToInt(Random.value * opponentDeck.movesConfig.Count);
        nextOpponentMove = opponentDeck.movesConfig[moveIndex];
    }

    public void onTurnEnd() { }

    public void onOpponentTurnEnd() { }

    public void onBattleEnd() { }
}
