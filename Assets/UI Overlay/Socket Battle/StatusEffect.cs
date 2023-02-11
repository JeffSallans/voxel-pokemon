using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StatusEffect
{
    public string statusEffectName;

    /// <summary>
    /// What to display on the pokemon status UI
    /// </summary>
    public string statusEffectDisplay
    {
        get
        {
            if (statType == "attackMultStat" && stackCount > 100) { return "+" + getUnicodeForStatType(statType); }
            if (statType == "attackMultStat" && stackCount < 100) { return "-" + getUnicodeForStatType(statType); }
            if (statType == "blockStat") { return getUnicodeForStatType(statType); }

            if (stackCount > 2)
            {
                return "" + stackCount + " " + getUnicodeForStatType(statType);
            }

            var message = "";
            for (var i = 0; i < stackCount; i++)
            {
                var spacer = "";
                if (i != 0) spacer = " ";
                message += spacer + getUnicodeForStatType(statType);
            }
            return message;
        }
    }

    /// <summary>
    /// The type of stat modified by the status
    /// </summary>
    public string statType;

    public enum StatusEffectType
    {
        Attack,
        Defense,
        Special,
        Evasion,
        Block,
        AttackMultStat,
        Invulnerability,
        Trap,
        Poison
    }

    /// <summary>
    /// The number of stacks of the stat modification
    /// </summary>
    public int stackCount;

    /// <summary>
    /// Target Pokemon affected with status
    /// </summary>
    public Pokemon targetPokemon;

    /// <summary>
    /// How many turns the status has left
    /// </summary>
    public int turnsLeft = 99;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetPokemon"></param>
    /// <param name="moveUsed"></param>
    /// <param name="_statusEffectName"></param>
    /// <param name="config"></param>
    public StatusEffect(Pokemon _targetPokemon, Card moveUsed, string _statusEffectName, Dictionary<string, string> config)
    {
        onActivate(_targetPokemon, moveUsed, _statusEffectName, config);
    }

    // Update is called once per frame
    public void Update()
    {
        if (stackCount <= 0)
        {
            onDeactivate();
        }

        if (statType == "attackMultStat" && stackCount == 100)
        {
            onDeactivate();
        }
    }

    private void onActivate(Pokemon _targetPokemon, Card moveUsed, string _statusEffectName, Dictionary<string, string> config) {

        statusEffectName = _statusEffectName;
        targetPokemon = _targetPokemon;

        // Check if a status effect already exists
        var existingStatusEffect = targetPokemon.attachedStatus.Where(s => s.statusEffectName == statusEffectName).FirstOrDefault();
        if (existingStatusEffect != null)
        {
            existingStatusEffect.onStackUpdate(_targetPokemon, moveUsed, config);
            stackCount = 0;
            return;
        }

        // Otherwise configure the status effect
        statType = config["statType"];
        if (int.Parse(config["stackCount"]) > 0)
        {
            stackCount = int.Parse(config["stackCount"]);
        }
        turnsLeft = int.Parse(config["turnsLeft"]);
    }
    
    /// <summary>
    /// Call to combine to combine two status effects
    /// </summary>
    /// <param name="targetPokemon"></param>
    /// <param name="moveUsed"></param>
    /// <param name="config"></param>
    public void onStackUpdate(Pokemon _targetPokemon, Card moveUsed, Dictionary<string, string> config)
    {
        stackCount += int.Parse(config["stackCount"]);

        // Check if stack reach max
        if (int.Parse(config["maxStack"]) > 0 && 
            stackCount > int.Parse(config["maxStack"]))
        {
            stackCount = int.Parse(config["maxStack"]);
        }
        // Check if min stack is reached
        if (stackCount < 0)
        {
            stackCount = 0;
        }
    }

    /// <summary>
    /// Call to clean up the StatusEffect
    /// </summary>
    private void onDeactivate() {
        Debug.Log("DEACTIVATED");
        targetPokemon.attachedStatus.Remove(this);
    }

    public void onDraw(Pokemon activePokemon) { }

    public void onOpponentDraw() { }

    public void onTurnEnd() {
        if (!targetPokemon.isPlayerPokemon) onTurnEndHelper();
    }

    public void onOpponentTurnEnd() {
        if (targetPokemon.isPlayerPokemon) onTurnEndHelper();
    }

    private void onTurnEndHelper()
    {
        turnsLeft--;
        if (statType == "poison")
        {
            targetPokemon.health -= 10;
            targetPokemon.modelAnimator.SetTrigger("onDebuff");
            targetPokemon.modelAnimator.SetTrigger("onPoison");
            stackCount--;
        }
        if (turnsLeft <= 0)
        {
            onDeactivate();
        }
    }

    public void onBattleEnd() { }

    /// <summary>
    /// Converts the stat type to unicode character for the summary status location
    /// </summary>
    /// <param name="givenStatType"></param>
    /// <returns></returns>
    private string getUnicodeForStatType(string givenStatType) {
        if (givenStatType == "attackStat") return "ZZAtk";
        if (givenStatType == "defenseStat") return "ZZDef";
        if (givenStatType == "specialStat") return "ZZSpc";
        if (givenStatType == "evasionStat") return "ZZEva";
        if (givenStatType == "blockStat") return "ZZBlock";
        if (givenStatType == "attackMultStat") return "ZZAttackMult";
        if (givenStatType == "invulnerability") return "ZZInvul";
        if (givenStatType == "trap") return "ZZTrap";
        if (givenStatType == "poison") return "ZZPoison";
        return "\uf111";
    }

}
