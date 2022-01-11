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
    public override void onDraw(Card card, BattleGameBoard battleGameBoard, Pokemon activePokemon) { }

    public override void onOpponentDraw(Card card, BattleGameBoard battleGameBoard, Pokemon opponentActivePokemon) { }

    public override void play(Card card, BattleGameBoard battleGameBoard, Pokemon user, Pokemon target) {
        // Switch board roles
        battleGameBoard.switchPokemon(user, target);
    }

    public override void onTurnEnd(Card card, BattleGameBoard battleGameBoard) { }

    public override void onOpponentTurnEnd(Card card, BattleGameBoard battleGameBoard) { }

    public override void onBattleEnd(Card card, BattleGameBoard battleGameBoard) { }
}
