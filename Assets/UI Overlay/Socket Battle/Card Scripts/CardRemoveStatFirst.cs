using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardRemoveStatFirst : ICard
{
    /// <summary>
    /// stat effect name to remove (def
    /// </summary>
    public List<StatusEffect.StatusEffectType> statEffectNames = new List<StatusEffect.StatusEffectType>();

    public override List<bool> play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets)
    {
        targets.ForEach(t => removeStat(card, user, t));

        return targets.Select(t => false).ToList();
    }

    /// <summary>
    /// deals direct damage to target
    /// </summary>
    /// <param name="target"></param>
    private void removeStat(Card card, Pokemon user, Pokemon target)
    {
        statEffectNames.ForEach(statNameType =>
        {
            var statName = GetNameFromStatusEffect(statNameType);
            var stat = target.attachedStatus.Find(s => s.statusEffectName == statName);
            if (stat != null)
            {
                target.attachedStatus.Remove(stat);
            }
        });
    }

    /// <summary>
    /// Returns the string form of stat type
    /// </summary>
    /// <param name="statusEffectType"></param>
    /// <returns></returns>
    private string GetNameFromStatusEffect(StatusEffect.StatusEffectType statusEffectType)
    {
        if (statusEffectType == StatusEffect.StatusEffectType.Attack) return "attackStat";
        if (statusEffectType == StatusEffect.StatusEffectType.Defense) return "defenseStat";
        if (statusEffectType == StatusEffect.StatusEffectType.Special) return "specialStat";
        if (statusEffectType == StatusEffect.StatusEffectType.Evasion) return "evasionStat";
        if (statusEffectType == StatusEffect.StatusEffectType.Block) return "blockStat";
        if (statusEffectType == StatusEffect.StatusEffectType.AttackMultStat) return "attackMultStat";
        if (statusEffectType == StatusEffect.StatusEffectType.Invulnerability) return "invulnerability";
        if (statusEffectType == StatusEffect.StatusEffectType.Trap) return "trap";
        if (statusEffectType == StatusEffect.StatusEffectType.Poison) return "poison";

        return null;
    }
}
