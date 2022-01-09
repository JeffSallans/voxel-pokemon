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
            return "" + stackCount + getUnicodeForStatType(statType);
        }
    }

    /// <summary>
    /// The type of stat modified by the status
    /// </summary>
    public string statType;

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
    public int turnsLeft;

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
        if (stackCount == 0)
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
        stackCount = int.Parse(config["stackCount"]);
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

    public void onTurnEnd(Pokemon activePokemon) { }

    public void onOpponentTurnEnd() {
        turnsLeft--;
        if (turnsLeft == 0)
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
        if (givenStatType == "attackStat") return "\uf1fb";
        if (givenStatType == "defenseStat") return "\uf132";
        if (givenStatType == "specialAttackStat") return "\uf20b";
        if (givenStatType == "specialDefenseStat") return "\uf132";
        if (givenStatType == "evadeStat") return "\uf04e";
        if (givenStatType == "blockStat") return "\uf111";
        return "\uf111";
    }

}
