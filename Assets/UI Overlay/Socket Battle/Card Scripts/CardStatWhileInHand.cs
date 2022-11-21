using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStatWhileInHand : ICard
{
    /// <summary>
    /// The attack stat the card will change
    /// </summary>
    public int attackStat;

    /// <summary>
    /// The special stat the card will change
    /// </summary>
    public int specialStat;

    /// <summary>
    /// The defense stat the card will change
    /// </summary>
    public int defenseStat;

    public override void onCardPlayed(Card card, BattleGameBoard battleGameBoard, Card move, Pokemon user, Pokemon target)
    {
        // Remove the stat if the card is played
        if (card == move)
        {
            card.owner.attackStat -= attackStat;
            card.owner.defenseStat -= defenseStat;
            card.owner.specialStat -= specialStat;
        }
    }

    public override void onDiscard(Card card, BattleGameBoard battleGameBoard, bool wasPlayed)
    {
        // Remove the stat if the card is discarded
        card.owner.attackStat -= attackStat;
        card.owner.defenseStat -= defenseStat;
        card.owner.specialStat -= specialStat;
    }

    public override void onDraw(Card card, BattleGameBoard battleGameBoard, Pokemon activePokemon)
    {
        // Reset count when drawn
        if (card == GetComponent<Card>())
        {
            addStatHelper(card.owner, "attackStat", attackStat, 6);
            addStatHelper(card.owner, "defenseStat", defenseStat, 6);
            addStatHelper(card.owner, "specialStat", specialStat, 1);
        }
    }

    /// <summary>
    /// Help set the stats
    /// </summary>
    /// <param name="target"></param>
    /// <param name="statName"></param>
    /// <param name="statValue"></param>
    /// <param name="maxStack">set to -1 to have no max stack</param>
    private void addStatHelper(Pokemon target, string statName, int statValue, int maxStack = -1)
    {
        if (statValue > 0)
        {
            target.attachedStatus.Add(new StatusEffect(target, null, statName + "Effect", new Dictionary<string, string>() {
                { "statType", statName.ToString() },
                { "stackCount", statValue.ToString() },
                { "maxStack", maxStack.ToString() },
                { "turnsLeft", turn.ToString() }
            }));
        }
    }
}
