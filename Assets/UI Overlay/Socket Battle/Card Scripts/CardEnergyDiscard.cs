using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Card;

/// <summary>
/// Discard energy from target
/// </summary>
public class CardEnergyDiscard : ICard
{
    /// <summary>
    /// Set to true if the override should also use the common card effect
    /// </summary>
    public bool useCommonCardEffect = false;

    /// <summary>
    /// Number of energies to remove, if the target has less it will remove them all
    /// </summary>
    public int numberOfEnergyToDiscard = 1;

    /// <summary>
    /// Target to remove energies from, used to be more flexible if we want to remove energies from the user after an attack
    /// </summary>
    public CardTargetType targetType = CardTargetType.ActiveOpponent;

    public override bool overridesPlayFunc() { return !useCommonCardEffect; }

    public override List<bool> play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets)
    {
        var discardTargets = card.getTarget(targetType, selectedTarget);

        discardTargets.ForEach(t => discardEneriesForTarget(t));

        return targets.Select(t => false).ToList();
    }

    /// <summary>
    /// Removes the energies from the given target.
    /// </summary>
    /// <param name="target"></param>
    private void discardEneriesForTarget(Pokemon target)
    {
        var maxEnergyIndex = Mathf.Min(numberOfEnergyToDiscard, target.attachedEnergy.Count);
        for (var i = maxEnergyIndex; i > 0; i--)
        {
            target.DiscardEnergy(i - 1);
        }
    }
}

