using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Card;

/// <summary>
/// Add damage bonus if the user is already active
/// </summary>
public class CardAlreadyActiveBuff : ICard
{
    /// <summary>
    /// Bonus damage to be dealt if user is already out
    /// </summary>
    public int bonusDamage = 0;

    /// <summary>
    /// Base damage of card
    /// </summary>
    private int baseDamage = 0;

    /// <summary>
    /// Track if the bonus damage is applied
    /// </summary>
    private bool bonusIsApplied = false;

    /// <summary>
    /// The pokemon that was active last turn
    /// </summary>
    private Pokemon lastTurnActivePokemon = null;

    public override bool overridesPlayFunc() { return false; }

    public override void onBattleStart(Card card, BattleGameBoard battleGameBoard)
    {
        base.onBattleStart(card, battleGameBoard);
        
        // Set base stat
        baseDamage = card.damage;
    }

    public override void onAnyCardPlayed(Card card, BattleGameBoard battleGameBoard, Card move, Pokemon user, Pokemon target)
    {
        base.onAnyCardPlayed(card, battleGameBoard, move, user, target);

        // Remove bonus if last active pokemon switches out
        if (move.owner != lastTurnActivePokemon && bonusIsApplied)
        {
            card.cardAnimator.SetTrigger("onFlip");
            card.damage = baseDamage;
            bonusIsApplied = false;
            lastTurnActivePokemon = move.owner;
        }
    }

    public override void onTurnEnd(Card card, BattleGameBoard battleGameBoard)
    {
        base.onTurnEnd(card, battleGameBoard);

        // Check if the active pokemon is this card's owner
        if (card.myActivePokemon == card.owner)
        {
            card.cardAnimator.SetTrigger("onFlip");
            card.damage += bonusDamage;
            bonusIsApplied = true;
        }

        // Update last turn active pokemon tracking
        lastTurnActivePokemon = card.myActivePokemon;
    }
}