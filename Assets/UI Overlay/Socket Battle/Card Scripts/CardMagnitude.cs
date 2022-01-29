using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMagnitude : ICard
{
    public int minDamage = 5;
    public int maxDamage = 35;

    public override void onDraw(Card card, BattleGameBoard battleGameBoard, Pokemon activePokemon)
    {
        card.initDamage = Random.Range(minDamage, maxDamage);
    }
}
