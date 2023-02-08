using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Card;

/// <summary>
/// Add damage bonus if opponent is switching out
/// </summary>
public class CardOpponentSwitchingBuff : ICard
{
    /// <summary>
    /// Bonus damage to be dealt if opponent is switching out
    /// </summary>
    public int bonusDamage = 0;

    /// <summary>
    /// Base damage of card
    /// </summary>
    private int baseDamage = 0;

    public override bool overridesPlayFunc() { return false; }

    public override void onBattleStart(Card card, BattleGameBoard battleGameBoard)
    {
        base.onBattleStart(card, battleGameBoard);

        // Set base stat
        if (usesThisOverrideInstance(card))
        {
            baseDamage = card.damage;
        }
    }

    public override void onTurnEnd(Card card, BattleGameBoard battleGameBoard)
    {
        base.onTurnEnd(card, battleGameBoard);

        if (usesThisOverrideInstance(card))
        {
            card.damage = baseDamage;
        }
    }

    public override List<bool> play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets)
    {
        var isOppSwitching = card?.otherActivePokemon != battleGameBoard.opponent.opponentStrategyBot.nextOpponentMove.card.owner;
        if (isOppSwitching)
        {
            card.damage = baseDamage + bonusDamage;
        }
        else
        {
            card.damage = baseDamage;
        }

        return base.play(card, battleGameBoard, user, selectedTarget, targets);
    }
}