using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Card switch functionality
/// </summary>
public class CardSwitch : ICard
{
    public override bool overridesPlayFunc() { return true; }

    public override void play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon target) {
        // Switch board roles
        battleGameBoard.switchPokemon(user, target);
    }
}
