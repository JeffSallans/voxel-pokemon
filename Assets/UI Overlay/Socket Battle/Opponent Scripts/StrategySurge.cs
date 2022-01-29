using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategySurge : IOpponentStrategy
{
    private IOpponentMove firstChoice = null;
    private IOpponentMove secondChoice = null;
    private IOpponentMove thirdChoice = null;

    public override void computeOpponentsNextMove()
    {
        // Move used last turn

        // Move is super effective against target

        // Current pokemon was hit by a super effective move

        // Opponent is blind

        // Raichu SPC < 3, use more SPC moves

        // Raichu SPC >= 3, use thunder

        // Ivysaur SPC >= 1, prefer giga drain over mega drain

        // Add a random float to each priority




        // Randomly select move
        var moveIndex = Mathf.FloorToInt(Random.value * opponentDeck.movesConfig.Count);
        nextOpponentMove = opponentDeck.movesConfig[moveIndex];
    }

    private IOpponentMove pickMoveUsingPriority()
    {
        return null;
    }
}
