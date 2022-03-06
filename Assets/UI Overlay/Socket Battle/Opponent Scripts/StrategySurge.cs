using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StrategySurge : IOpponentStrategy
{
    private IOpponentMove firstChoice = null;
    private IOpponentMove secondChoice = null;
    private IOpponentMove thirdChoice = null;

    private List<Card> playerMovesLastTurn = new List<Card>();

    public override void computeOpponentsNextMove()
    {
        // Move used last turn
        if (lastMoveUsed != null)
        {
            availableMoves.Where(m => m == lastMoveUsed)
                .ToList()
                .ForEach(m => m.turnPriority -= 1);
        }

        // Move is super effective against target
        availableMoves.Where(m => TypeChart.getEffectiveness(m , battleGameBoard.activePokemon) > 1).ToList().ForEach(m => m.turnPriority += 2);

        // Current pokemon was hit by a super effective move
        var lastMoveWasSuperEffective = playerMovesLastTurn.Any(c => TypeChart.getEffectiveness(c, battleGameBoard.opponentActivePokemon) > 1);
        if (lastMoveWasSuperEffective)
        {
            availableMoves.Where(m => m.actingPokemon == battleGameBoard.opponentActivePokemon)
                .ToList()
                .ForEach(m => m.turnPriority -= 2);
        }

        // Opponent is blind
        /*
        if (battleGameBoard.activePokemon.isBlind)
        {
            av
        }
        */

        // Raichu SPC < 3, use more SPC moves

        // Raichu SPC >= 3, use thunder

        // Ivysaur SPC >= 1, prefer giga drain over mega drain

        // Add a random float to each priority
        
        nextOpponentMove = pickMoveUsingPriority(availableMoves);
    }

    private IOpponentMove pickMoveUsingPriority(List<IOpponentMove> moveList)
    {
        var sortedMoveList = moveList.OrderBy(m => m.turnPriority).ToList();
        firstChoice = moveList[0];
        secondChoice = moveList[1];
        thirdChoice = moveList[2];

        var d20RollOnHowWellBotPerformed = Random.Range(1, 20);
        if (d20RollOnHowWellBotPerformed <= 2)
        {
            return thirdChoice;
        }
        else if (d20RollOnHowWellBotPerformed <= 6)
        {
            return secondChoice;
        }

        return firstChoice;
    }
}
