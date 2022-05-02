using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the entire battle state and events
/// </summary>
public class BattleGameBoardTutorial : BattleGameBoard
{
    public string gameStartInstruction;

    public string energyInstruction;
    public bool showedEnergyInstruction;

    public bool canShowCards;

    public string cardCostInstruction;
    public string cardDragInstruction;
    public bool showedCardInstruction;

    public string endTurnInstruction;
    public bool showedTurnInstruction;

    public string cardButtonInstruction;
    public bool showedCardButtonInstruction;

    public string typeInstruction;
    public bool showedTypeInstruction;

    public string oppAttackInstruction;
    public bool showedOppAttackInstruction;


    /// <summary>
    /// Assumes player and opponent are set before calling this
    /// </summary>
    public new void onBattleStart()
    {
        startBattleButton.SetActive(false);
        gameHasEnded = false;

        // Place pokemon
        activePokemon = player.party.First();
        opponentActivePokemon = opponent.party.First();
        onSetupPlayer();

        // Shuffle player deck
        allPokemon.ForEach(p =>
        {
            // Create a copy of initial deck to use for the game
            p.deck = p.initDeck.ToList();
            Shuffle(p.deck);
        });
        allPartyCards.ForEach(c =>
        {
            c.transform.position = deckLocation.transform.position;
            c.transform.rotation = deckLocation.transform.rotation;
        });

        energyDeck = player.energies.ToList();
        Shuffle(energyDeck);
        energyDeck.ForEach(e =>
        {
            e.transform.position = deckLocation.transform.position;
            e.transform.rotation = deckLocation.transform.rotation;
        });

        worldDialog.ShowMessage(opponent.opponentName + " wants to battle.", () => {

            worldDialog.ShowMessage(gameStartInstruction, () => {

                // Send event to all energy, cards, and pokemon
                allPokemon.ForEach(p => p.onBattleStart(this));
                allPartyCards.ForEach(c => c.onBattleStart(this));
                allEnergy.ForEach(e => e.onBattleStart(this));
                opponent.opponentStrategyBot.onBattleStart(this);
                opponent.movesConfig.ForEach(m => m.onBattleStart(this));

                // Trigger opponent first move
                opponent.opponentStrategyBot.computeOpponentsNextMove();

                // Trigger draw
                onDraw();

                endTurnButton.SetActive(true);

                return true;
            });
            
            return true;
        });
    }

    /// <summary>
    /// On the draw step
    /// </summary>
    public new void onDraw()
    {
        // Each turn share some instructions
        if (!showedEnergyInstruction)
        {
            worldDialog.ShowMessage(energyInstruction, () => { onDrawHelper(); return true; });
        }
        else if (!showedCardInstruction)
        {
            worldDialog.ShowMessage(cardCostInstruction, () =>
            {
                worldDialog.ShowMessage(cardDragInstruction, () =>
                {
                    showedCardInstruction = true;
                    onDrawHelper();
                    return true;
                });
                return true;
            });
        }
        else if (!showedCardButtonInstruction)
        {
            worldDialog.ShowMessage(cardButtonInstruction, () => { showedCardButtonInstruction = true; onDrawHelper(); return true; });
        }
        else if (!showedTypeInstruction)
        {
            worldDialog.ShowMessage(typeInstruction, () => { showedTypeInstruction = true; onDrawHelper(); return true; });
        }
        else if (!showedOppAttackInstruction)
        {
            worldDialog.ShowMessage(oppAttackInstruction, () => { showedOppAttackInstruction= true; onDrawHelper(); return true; });
        }
        else
        {
            onDrawHelper();
        }
    }

    private void onDrawHelper()
    {
        if (showedCardInstruction && !showedCardButtonInstruction)
        {
            // Refresh energy
            availableEnergy.ForEach(e => { e.isUsed = false; });

            // Pick 3 energies
            var maxThreshold = 10;
            var i = 0;
            while (energyHand.Count < energyHandSize && i < maxThreshold)
            {
                if (energyDeck.Count < 1) { reshuffleEnergyDiscard(); }
                drawEnergy();
                i++;
            }

            // Re-activate any lingering energies
            if (!energyHandDiscard)
            {
                energyHand.ForEach(e =>
                {
                    e.SetCanBeDragged(true);
                });
            }
        }

        // Draw cards up to handSize
        if (canShowCards)
        {
            var maxThreshold = 10;
            var i = 0;
            while (hand.Count < handSize && i < maxThreshold)
            {
                if (deck.Count < 1) { reshuffleDiscard(); }
                drawCard(activePokemon);
                i++;
            }
        }
    }


    public new void onEnergyPlay(Energy source, Pokemon target)
    {
        // Remove energy
        energyHand.Remove(source);

        // Trigger energy action
        source.playEnergy(target);

        // Discard other energies
        if (energyHandDiscard)
        {
            energyHand.ForEach(e =>
            {
                e.Translate(energyDiscardLocation.transform.position);
                e.transform.rotation = energyDiscardLocation.transform.rotation;
            });
            energyDiscard.AddRange(energyHand);
            energyHand.RemoveAll(card => true);
        }
        else
        {
            energyHand.ForEach(e =>
            {
                e.SetCanBeDragged(false);
            });
        }

        // Trigger next instruction if applicable
        if (!canShowCards)
        {
            canShowCards = true;
            onDraw();
        }
    }
}
