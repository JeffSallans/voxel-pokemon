using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyRandom : IOpponentStrategy
{
    public override void computeOpponentsNextMove()
    {
        // Randomly select move
        var moveIndex = Mathf.FloorToInt(Random.value * availableMoves.Count);
        nextOpponentMove = availableMoves[moveIndex];
    }
}
