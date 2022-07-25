using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyRandom : IOpponentStrategy
{
    public override string computeOpponentsNextMove()
    {
        // Randomly select move
        var moveIndex = Mathf.FloorToInt(Random.value * availableMoves.Count);
        nextOpponentMove = availableMoves[moveIndex];
        nextOpponentMove.onNextMoveSelect();
        // Do no trigger if it was already played
        if (nextOpponentMove.playInstantly)
        {
            return nextOpponentMove.playMove()[0];
        }
        return null;
    }
}
