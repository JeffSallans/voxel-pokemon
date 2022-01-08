using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the entire battle state and events
/// </summary>
public class BattleGameBoard : MonoBehaviour
{
    public PlayerDeck player;

    public OpponentDeck opponent;

    public string debugVariablesBelow;

    /// <summary>
    /// Cards to be drawn
    /// </summary>
    public List<Card> deck;

    /// <summary>
    /// The hand the player has
    /// </summary>
    public List<Card> hand;

    /// <summary>
    /// The cards the player used to be reshuffled
    /// </summary>
    public List<Card> discard;

    /// <summary>
    /// The possible energies to deal a hand with
    /// </summary>
    public List<Energy> energyDeck;

    /// <summary>
    /// The possible energies to pick from
    /// </summary>
    public List<Energy> energyHand;

    /// <summary>
    /// The energies that were removed from the pokemon, to be reshuffled in when energyDeck is empty
    /// </summary>
    public List<Energy> energyDiscard;

    /// <summary>
    /// The pokemon that can use moves
    /// </summary>
    public Pokemon activePokemon;

    /// <summary>
    /// The available energy on the active pokemon
    /// </summary>
    public List<Energy> availableEnergy
    {
        get {
            return commonEnergy.Union(activePokemon.attachedEnergy).ToList();
        }
    }

    /// <summary>
    /// The usable energy
    /// </summary>
    public List<Energy> useableEnergy
    {
        get
        {
            return availableEnergy.Where(e => !e.isUsed).ToList();
        }
    }

    /// <summary>
    /// All the energy events on pokemon to trigger
    /// </summary>
    public List<Energy> allPokemonEnergy
    {
        get
        {
            var pokemonEnergy = player.party.Select(p => p.attachedEnergy).SelectMany(x => x).ToList();
            return commonEnergy.Union(pokemonEnergy).ToList();
        }
    }

    /// <summary>
    /// All the energy events to trigger
    /// </summary>
    public List<Energy> allEnergy
    {
        get
        {
            var pokemonEnergy = player.party.Select(p => p.attachedEnergy).SelectMany(x => x).ToList();
            return commonEnergy.Union(pokemonEnergy).Union(energyDeck).ToList();
        }
    }

    /// <summary>
    /// All the card events to trigger
    /// </summary>
    public List<Card> allCards
    {
        get
        {
            return deck.Union(hand).Union(discard).ToList();
        }
    }

    /// <summary>
    /// All the pokemon events to trigger
    /// </summary>
    public List<Pokemon> allPokemon
    {
        get
        {
            return player.party.Union(opponent.party).ToList();
        }
    }

    /// <summary>
    /// The pokemon that will recieve the next attack
    /// </summary>
    public Pokemon opponentActivePokemon;

    public string renderLocationsBelow;

    /// <summary>
    /// Where to render the pokemon prefabs
    /// </summary>
    public List<Pokemon> playerPokemonLocations;

    /// <summary>
    /// Where to render the pokemon prefabs
    /// </summary>
    public List<Pokemon> opponentPokemonLocations;

    /// <summary>
    /// Where to render the deck prefabs
    /// </summary>
    public GameObject deckLocation;

    /// <summary>
    /// Where to render the card prefabs
    /// </summary>
    public List<GameObject> handLocations;

    /// <summary>
    /// Where to render the discard prefabs
    /// </summary>
    public GameObject discardLocation;

    /// <summary>
    /// Where to render the energy deck prefabs
    /// </summary>
    public GameObject energyDeckLocation;

    /// <summary>
    /// Where to render possible energies
    /// </summary>
    public List<GameObject> energyHandLocations;

    /// <summary>
    /// Where to render the discard energy
    /// </summary>
    public GameObject energyDiscardLocation;

    /// <summary>
    /// The common energy to use every turn
    /// </summary>
    public List<Energy> commonEnergy;

    /// <summary>
    /// The locations of the party
    /// </summary>
    public List<GameObject> pokemonModelLocations;

    /// <summary>
    /// The locations of the opponent party
    /// </summary>
    public List<GameObject> opponentPokemonModelLocations;

    /// <summary>
    /// The UI to choose a pokemon to switch to
    /// </summary>
    public GameObject switchPokemonOverlay;

    /// <summary>
    /// How many energies to pick from
    /// </summary>
    public int energyHandSize = 3;

    /// <summary>
    /// How many cards to draw
    /// </summary>
    public int handSize = 4;

    /// <summary>
    /// Starts the battle
    /// </summary>
    public GameObject startBattleButton;

    /// <summary>
    /// Ends the turn
    /// </summary>
    public GameObject endTurnButton;

    /// <summary>
    /// Ends the turn
    /// </summary>
    public WorldDialog worldDialog;

    /// <summary>
    /// True if the game is done
    /// </summary>
    public bool gameHasEnded = false;

    /// <summary>
    /// True if the game has ended. Don't use this value if gameHasEnded is false.
    /// </summary>
    public bool playerHasWon = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Assumes player and opponent are set before calling this
    /// </summary>
    public void onBattleStart()
    {
        gameHasEnded = false;

        // Place pokemon
        activePokemon = player.party.First();
        opponentActivePokemon = opponent.party.First();
        var i = 0;
        player.party.ForEach(p =>
        {
            p.setPlacement(playerPokemonLocations[i], pokemonModelLocations[i]);
            i++;
        });
        var j = 0;
        opponent.party.ForEach(p =>
        {
            p.setPlacement(opponentPokemonLocations[j], opponentPokemonModelLocations[j]);
            j++;
        });


        // Shuffle player deck
        deck = player.deck.ToList();
        Shuffle(deck);
        deck.ForEach(c =>
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
        allCards.ForEach(c => c.onBattleStart(this));
        allEnergy.ForEach(e => e.onBattleStart(this));
        opponent.opponentStrategyBot.onBattleStart(this);
        opponent.movesConfig.ForEach(m => m.onBattleStart(this));

        // Trigger opponent first move
        opponent.opponentStrategyBot.computeOpponentsNextMove();

        // Trigger draw
        onDraw();

        startBattleButton.SetActive(false);
        endTurnButton.SetActive(true);
    }

    /// <summary>
    /// On the draw step
    /// </summary>
    public void onDraw() {
        // Refresh energy
        availableEnergy.ForEach(e => { e.isUsed = false; }); 

        // Pick 3 energies
        while (energyHand.Count < energyHandSize)
        {
            if (energyDeck.Count < 1) { reshuffleEnergyDiscard(); }
            drawEnergy();
        }

        // Draw cards up to handSize
        while (hand.Count < handSize)
        {
            if (deck.Count < 1) { reshuffleDiscard(); }
            drawCard(activePokemon);
        }
    }

    /// <summary>
    /// Draw the top card from the deck
    /// </summary>
    /// <param name="activePokemon"></param>
    private void drawCard(Pokemon activePokemon) {
        // Draw card
        var cardDrawn = deck.First();
        hand.Add(cardDrawn);
        deck.Remove(cardDrawn);
        cardDrawn.transform.position = handLocations[hand.Count - 1].transform.position + new Vector3(0, 0, -1);
        cardDrawn.transform.rotation = handLocations[hand.Count - 1].transform.rotation;

        // Trigger event
        cardDrawn.onDraw(activePokemon);
    }

    /// <summary>
    /// Modifies discard and deck
    /// </summary>
    private void reshuffleDiscard()
    {
        deck.AddRange(discard);
        discard.RemoveAll(card => true);
        Shuffle(deck);
        deck.ForEach(c =>
        {
            c.transform.position = deckLocation.transform.position;
            c.transform.rotation = deckLocation.transform.rotation;
        });
    }

    /// <summary>
    /// Draw the top energy from the deck
    /// </summary>
    private void drawEnergy()
    {
        // Draw card
        var energyDrawn = energyDeck.First();
        energyDrawn.isUsed = false;
        energyHand.Add(energyDrawn);
        energyDeck.Remove(energyDrawn);
        energyDrawn.transform.position = energyHandLocations[energyHand.Count - 1].transform.position;
        energyDrawn.transform.rotation = energyHandLocations[energyHand.Count - 1].transform.rotation;
        energyDrawn.transform.localScale = energyHandLocations[energyHand.Count - 1].transform.localScale;
    }

    /// <summary>
    /// Modifies energy discard and energy deck
    /// </summary>
    private void reshuffleEnergyDiscard()
    {
        energyDeck.AddRange(energyDiscard);
        energyDiscard.RemoveAll(card => true);
        Shuffle(energyDeck);
        energyDeck.ForEach(c =>
        {
            c.transform.position = energyDeckLocation.transform.position;
            c.transform.rotation = energyDeckLocation.transform.rotation;
        });
    }

    public void onOpponentDraw() {
        // Take action on queued move
        var message = opponent.opponentStrategyBot.opponentPlay();
        worldDialog.ShowMessage(message, () => {
            // Trigger right away for now
            onOpponentTurnEnd();
            return true;
        });
    }

    /// <summary>
    /// The Event when a card is played
    /// </summary>
    /// <param name="move"></param>
    /// <param name="user"></param>
    /// <param name="target"></param>
    public void onPlay(Card move, Pokemon user, Pokemon target) {
        // Pay cost
        payMoveCost(move.cost);
        
        // Trigger move action
        move.play(user, target);

        // Discard card
        discard.Add(move);
        hand.Remove(move);
        move.transform.position = discardLocation.transform.position;
        move.transform.rotation = discardLocation.transform.rotation;
    }

    /// <summary>
    /// Update the energies that are used to play the move
    /// </summary>
    /// <param name="move"></param>
    private void payMoveCost(List<Energy> cost)
    {
        cost.ForEach(energy =>
        {
            // Subtract from common
            var target = commonEnergy.Where(e => !e.isUsed && e.energyName == energy.energyName).FirstOrDefault();
            if (target != null)
            {
                target.isUsed = true;
            }
            // Subtract from active
            else
            {
                var targetOnPokemon = activePokemon.attachedEnergy.Where(e => !e.isUsed && e.energyName == energy.energyName).FirstOrDefault();
                if (targetOnPokemon != null)
                {
                    targetOnPokemon.isUsed = true;
                }
            }
        });
    }

    public void onEnergyEvent(int index)
    {
        onEnergyPlay(energyHand[index], activePokemon);
    }


    public void onEnergyPlay(Energy source, Pokemon target)
    {
        // Remove energy
        energyHand.Remove(source);
        
        // Trigger energy action
        source.playEnergy(target);

        // Discard other energies
        energyHand.ForEach(e =>
        {
            e.transform.position = energyDiscardLocation.transform.position;
            e.transform.rotation = energyDiscardLocation.transform.rotation;
        });
        energyDiscard.AddRange(energyHand);
        energyHand.RemoveAll(card => true);
    }

    public void onTurnEnd() {
        // Discard hand
        hand.ForEach(c =>
        {
            c.transform.position = discardLocation.transform.position;
            c.transform.rotation = discardLocation.transform.rotation;
        });
        discard.AddRange(hand);
        hand.RemoveAll(card => true);

        // Discard other energies
        energyHand.ForEach(e =>
        {
            e.transform.position = energyDiscardLocation.transform.position;
            e.transform.rotation = energyDiscardLocation.transform.rotation;
        });
        energyDiscard.AddRange(energyHand);
        energyHand.RemoveAll(card => true);

        // Send event to all energy, cards, status, and pokemon
        allPokemon.ForEach(p => p.onTurnEnd());
        allCards.ForEach(c => c.onTurnEnd());
        allEnergy.ForEach(e => e.onTurnEnd());
        opponent.opponentStrategyBot.onTurnEnd();
        opponent.movesConfig.ForEach(m => m.onTurnEnd());

        // Check game end conditions
        onEitherTurnEnd();

        // Trigger next step
        if (!gameHasEnded)
        {
            onOpponentDraw();
        }
    }

    public void onOpponentTurnEnd() {
        // Compute next move
        opponent.opponentStrategyBot.computeOpponentsNextMove();

        // Send event to all energy, cards, status, and pokemon
        allPokemon.ForEach(p => p.onOpponentTurnEnd());
        allCards.ForEach(c => c.onOpponentTurnEnd());
        allEnergy.ForEach(e => e.onOpponentTurnEnd());
        opponent.opponentStrategyBot.onOpponentTurnEnd();
        opponent.movesConfig.ForEach(m => m.onOpponentTurnEnd());

        // Check game end conditions
        onEitherTurnEnd();

        // Trigger draw
        if (!gameHasEnded)
        {
            onDraw();
        }
    }

    public void onEitherTurnEnd()
    {

        // When all opponents pokemon are 0 hp
        var numberOfOpponentPokeAlive = opponent.party.Where(p => p.health > 0).Count();
        if (numberOfOpponentPokeAlive == 0) { onBattleEnd(true); }

        // When all player pokemon are 0 hp
        var numberOfPlayerPokeAlive = player.party.Where(p => p.health > 0).Count();
        if (numberOfPlayerPokeAlive == 0) { onBattleEnd(false); }

        // When player active pokemon is 0 hp - switch in

        // When opponent active pokemon is 0 hp - switch in

    }


    public void showSwitchPokemon()
    {
        switchPokemonOverlay.SetActive(true);
    }

    public void hideSwitchPokemon()
    {
        switchPokemonOverlay.SetActive(false);
    }

    public void switchPokemon(Pokemon selectedPokemon)
    {
        activePokemon = selectedPokemon;
        switchPokemonOverlay.SetActive(false);
    }

    public void onBattleEnd(bool isPlayerWinner) {
        // Send event to all energy, cards, status, and pokemon
        allPokemon.ForEach(p => p.onBattleEnd());
        allCards.ForEach(c => c.onBattleEnd());
        allEnergy.ForEach(e => e.onBattleEnd());
        opponent.opponentStrategyBot.onBattleEnd();
        opponent.movesConfig.ForEach(m => m.onBattleEnd());

        // Set results
        gameHasEnded = true;
        playerHasWon = isPlayerWinner;

        if (isPlayerWinner)
        {
            worldDialog.ShowMessage("Red won!", () => {
                print("The player won");
                return true;
            });
        }
        else
        {
            worldDialog.ShowMessage("Youngster Joey won.", () => {
                print("The opponent won");
                return true;
            });
        }
    }

    /// <summary>
    /// Fisher-Yates Shuffle on the list
    /// https://stackoverflow.com/a/1262619
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    private void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Mathf.FloorToInt(Random.value * n);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
