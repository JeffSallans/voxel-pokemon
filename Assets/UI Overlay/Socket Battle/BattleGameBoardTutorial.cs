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
    public bool showedEnergyInstruction = false;

    public bool canShowCards;

    public string cardCostInstruction;
    public string cardDragInstruction;
    public bool showedCardInstruction = false;

    public string endTurnInstruction;
    public bool showedEndTurnInstruction = false;

    public string typeInstruction;
    public bool showedTypeInstruction = false;

    public string fourthEnergyInstruction;
    public bool showedFourthEnergyInstruction = false;

    /// <summary>
    /// Assumes player and opponent are set before calling this
    /// </summary>
    public override void onBattleStart()
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
            if (shuffleDeck)
            {
                Shuffle(p.deck);
            }
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

        // Send event to all energy, cards, and pokemon
        allPokemon.ForEach(p => p.onBattleStart(this));
        allPartyCards.ForEach(c => c.onBattleStart(this));
        allEnergy.ForEach(e => e.onBattleStart(this));
        opponent.opponentStrategyBot.onBattleStart(this);
        opponent.initDeck.ForEach(m => m.onBattleStart(this));

        worldDialog.ShowMessage(opponent.opponentName + " wants to battle.", (t) => {

            worldDialog.ShowMessage(gameStartInstruction, (t) => {

                // Trigger opponent first move
                opponent.opponentStrategyBot.opponentPrep();

                // Trigger draw
                onDraw(true);

                endTurnButton.SetActive(true);

                return true;
            });
            
            return true;
        });
    }

    /// <summary>
    /// On the draw step
    /// </summary>
    public override void onDraw(bool initialDraw = false)
    {
        // Each turn share some instructions
        if (!showedEnergyInstruction)
        {
            worldDialog.ShowMessage(energyInstruction, (t) => { showedEnergyInstruction = true; onDrawHelper(initialDraw); return true; });
        }
        else if (!showedCardInstruction)
        {
            worldDialog.ShowMessage(cardCostInstruction, (t) =>
            {
                worldDialog.ShowMessage(cardDragInstruction, (t) =>
                {
                    showedCardInstruction = true;
                    onDrawHelper(initialDraw);
                    return true;
                });
                return true;
            });
        }
        else if (!showedTypeInstruction)
        {
            worldDialog.ShowMessage(typeInstruction, (t) => { showedTypeInstruction = true; onDrawHelper(initialDraw); return true; });
        }
        else if (!showedFourthEnergyInstruction)
        {
            worldDialog.ShowMessage(fourthEnergyInstruction, (t) => { showedFourthEnergyInstruction = true; onDrawHelper(initialDraw); return true; });
        }
        else
        {
            onDrawHelper(initialDraw);
        }
    }

    private void onDrawHelper(bool initialDraw = false)
    {
        // Refresh card count
        remainingNumberOfCardsCanPlay = numberOfCardsCanPlay;

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

        // Draw cards up to handSize
        if (canShowCards)
        {
            // Draw one card a turn
            if (!initialDraw && drawOncePerTurn)
            {
                if (hand.Count < maxHandSize)
                {
                    if (deck.Count < 1) { reshuffleDiscard(); }
                    drawCard(activePokemon);
                    i++;
                }
            }
            // Draw cards up to handSize
            else
            {
                i = 0;
                while (hand.Count < handSize && i < maxThreshold)
                {
                    if (deck.Count < 1) { reshuffleDiscard(); }
                    drawCard(activePokemon);
                    i++;
                }
            }
        }
    }

    public override void onEnergyPlay(Energy source, Pokemon target)
    {
        base.onEnergyPlay(source, target);

        // Trigger next instruction if applicable
        if (!canShowCards)
        {
            // Shift energy
            for (var i = 0; i < energyHand.Count; i++)
            {
                var cardLoc = energyHandLocations[i].transform.position;
                energyHand[i].Translate(cardLoc);
                energyHand[i].transform.rotation = energyHandLocations[i].transform.rotation;
            }

            canShowCards = true;
            onDraw(true);
        }
    }

    public override void onPlay(Card move, Pokemon user, Pokemon target)
    {
        base.onPlay(move, user, target);

        // Trigger next instruction if applicable
        if (!showedEndTurnInstruction)
        {
            worldDialog.ShowMessage(endTurnInstruction, (t) => { showedEndTurnInstruction = true; return true; });
        }
    }

    public override void onBattleEnd(bool isPlayerWinner)
    {
        // Set results
        gameHasEnded = true;
        playerHasWon = isPlayerWinner;

        if (isPlayerWinner)
        {
            worldDialog.ShowMessage("You won!", (t) => {
                print("The player won");

                player.party.ForEach(p => { p.hideModels(); });
                opponent.party.ForEach(p => { p.hideModels(); });

                // Send event to all energy, cards, status, and pokemon
                allPokemon.ForEach(p => p.onBattleEnd());
                allEnergy.ForEach(e => e.onBattleEnd());
                opponent.opponentStrategyBot.onBattleEnd();
                cardEventService.onBattleEnd();

                onPackupPlayer();
                player.GetComponent<InteractionChecker>().LoadPreviousScene();

                return true;
            });
        }
        else
        {
            worldDialog.ShowMessage(opponent.opponentName + " won.", (t) => {
                print("The opponent won");

                player.party.ForEach(p => { p.hideModels(); });
                opponent.party.ForEach(p => { p.hideModels(); });

                // Send event to all energy, cards, status, and pokemon
                allPokemon.ForEach(p => p.onBattleEnd());
                allEnergy.ForEach(e => e.onBattleEnd());
                opponent.opponentStrategyBot.onBattleEnd();
                cardEventService.onBattleEnd();

                onPackupPlayer();
                player.GetComponent<InteractionChecker>().LoadPreviousScene(); // <- diff

                return true;
            });
        }
    }
}
