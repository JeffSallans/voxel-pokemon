using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour
{
    public string statusEffectName;

    /// <summary>
    /// Target Pokemon affected with status
    /// </summary>
    public Pokemon targetPokemon;

    /// <summary>
    /// How many turns the status has left
    /// </summary>
    public int turnsLeft;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onActivate(Pokemon targetPokemon, Card moveUsed) {
        
    }

    private void onDeactivate(Pokemon activePokemon) { }

    public void onDraw(Pokemon activePokemon) { }

    public void onOpponentDraw(Pokemon opponentActivePokemon) { }

    public void onTurnEnd(Pokemon activePokemon) { }

    public void onOpponentTurnEnd(Pokemon opponentActivePokemon) { }

    public void onBattleEnd() { }

}
