using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Card;

/// <summary>
/// Takes all status from target and copies them over to user
/// </summary>
public class CardStealStats : ICard
{
    public override bool overridesPlayFunc() { return true; }

    public override List<bool> play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets)
    {

        var attackMissed = targets.Select(t =>
        {
            user.attachedStatus.AddRange(t.attachedStatus);
            t.attachedStatus.RemoveAll(status => true);
            return true;
        }).ToList();

        return attackMissed;
    }
}