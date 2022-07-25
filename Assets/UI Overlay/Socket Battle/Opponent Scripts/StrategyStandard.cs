using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StrategyStandard : IOpponentStrategy
{
    public IOpponentMove firstChoice = null;
    public IOpponentMove secondChoice = null;
    public IOpponentMove thirdChoice = null;

    private List<Card> playerMovesLastTurn = new List<Card>();

    public override string computeOpponentsNextMove()
    {
        // Reset priority picks
        availableMoves.ForEach(m => { m.turnPriority = m.initialPriority; });

        // Move used last turn
        if (lastMoveUsed != null)
        {
            availableMoves.Where(m => m == lastMoveUsed)
                .ToList()
                .ForEach(m => { m.turnPriority -= 1; });
        }

        // Move is super effective against target
        availableMoves.Where(m => TypeChart.getEffectiveness(m, battleGameBoard.activePokemon) > 1).ToList().ForEach(m => { m.turnPriority += 2; });

        // Current pokemon was hit by a super effective move
        var lastMoveWasSuperEffective = playerMovesLastTurn.Any(c => TypeChart.getEffectiveness(c, battleGameBoard.opponentActivePokemon) > 1);
        if (lastMoveWasSuperEffective)
        {
            availableMoves.Where(m => m.actingPokemon == battleGameBoard.opponentActivePokemon)
                .ToList()
                .ForEach(m => { m.turnPriority -= 1; });
        }

        // Prefer active pokemon moves if they just switched in
        var isActivePokemonSecondTurn = moveUsedHistory.Count > 2 && battleGameBoard.opponentActivePokemon != moveUsedHistory[0]?.actingPokemon && moveUsedHistory[0]?.actingPokemon != moveUsedHistory[1]?.actingPokemon;
        if (isActivePokemonSecondTurn)
        {
            availableMoves.Where(m => m.actingPokemon == battleGameBoard.opponentActivePokemon)
                .ToList()
                .ForEach(m => { m.turnPriority += 1; });
        }

        // Prefer bench pokemon moves if the active pokemon has been in 4
        var isActivePokemonThirdTurn = moveUsedHistory.Count > 4 && moveUsedHistory[0]?.actingPokemon == moveUsedHistory[1]?.actingPokemon && moveUsedHistory[0]?.actingPokemon == moveUsedHistory[2]?.actingPokemon && moveUsedHistory[0]?.actingPokemon == moveUsedHistory[3]?.actingPokemon;
        if (isActivePokemonThirdTurn)
        {
            availableMoves.Where(m => m.actingPokemon == battleGameBoard.opponentActivePokemon)
                .ToList()
                .ForEach(m => { m.turnPriority -= 1; });
        }

        // Add a random float to each priority  
        availableMoves.ToList()
                .ForEach(m => { m.turnPriority += Random.Range(0f, 2f); });

        nextOpponentMove = pickMoveUsingPriority(availableMoves);
        nextOpponentMove.onNextMoveSelect();
        // Do no trigger if it was already played
        if (nextOpponentMove.playInstantly)
        {
            return nextOpponentMove.playMove()[0];
        }
        return null;
    }

    private IOpponentMove pickMoveUsingPriority(List<IOpponentMove> moveList)
    {
        var sortedMoveList = moveList.OrderByDescending(m => m.turnPriority).ToList();
        firstChoice = sortedMoveList[0];
        secondChoice = sortedMoveList.ElementAtOrDefault(1);
        thirdChoice = sortedMoveList.ElementAtOrDefault(2);

        var d20RollOnHowWellBotPerformed = Random.Range(1, 20);
        if (d20RollOnHowWellBotPerformed <= 2 && thirdChoice != null)
        {
            return thirdChoice;
        }
        else if (d20RollOnHowWellBotPerformed <= 6 && secondChoice != null)
        {
            return secondChoice;
        }

        return firstChoice;
    }
}
