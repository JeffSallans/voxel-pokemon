using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Card;

/// <summary>
/// Reveal what opponent is going to attack next
/// </summary>
public class CardShowAttack : ICard
{

    public override bool overridesPlayFunc() { return false; }

    /// <summary>
    /// Hide revealed attack at the end of your turn
    /// </summary>
    /// <param name="card"></param>
    /// <param name="battleGameBoard"></param>
    public override void onTurnEnd(Card card, BattleGameBoard battleGameBoard)
    {
        base.onTurnEnd(card, battleGameBoard);
        battleGameBoard.opponent.party.ForEach(p =>
        {
            p.nextAttackText.text = "";
        });
    }

    public override List<bool> play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets)
    {
        // Show "attacking" above the opponent that is switching in
        battleGameBoard.opponent.opponentStrategyBot.nextOpponentMove.actingPokemon.nextAttackText.text = "Attacking";

        var attackMissed = targets.Select(t =>
        {
            return true;
        }).ToList();

        return attackMissed;
    }
}