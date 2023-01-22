using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Card switch functionality for opponents
/// </summary>
public class CardSwitchOpponent : ICard
{
    public override bool overridesPlayFunc() { return true; }

    public override List<bool> play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon selectedTarget, List<Pokemon> targets) {
        // Switch board roles
        battleGameBoard.switchPokemon(battleGameBoard.opponentActivePokemon, selectedTarget);

        return base.play(card, battleGameBoard, user, selectedTarget, targets);
    }
}
