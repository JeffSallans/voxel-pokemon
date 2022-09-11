using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMagnitude : ICard
{
    public int minDamage = 30;
    public int maxDamage = 80;
    public int stepIncrement = 10;

    public override void onDraw(Card card, BattleGameBoard battleGameBoard, Pokemon activePokemon)
    {
        card.initDamage = Mathf.RoundToInt(Random.Range(minDamage, maxDamage + 1) / stepIncrement) * stepIncrement;
    }
}
