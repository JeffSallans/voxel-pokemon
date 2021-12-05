using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the entire battle state and events
/// </summary>
public class BattleGameBoard : MonoBehaviour
{
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

    public PlayerDeck player;

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
            var cardEnergy = allCards.Select(p => p.attachedEnergies).SelectMany(x => x).ToList();
            return commonEnergy.Union(pokemonEnergy).Union(cardEnergy).ToList();
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


    public OpponentDeck opponent;

    /// <summary>
    /// The pokemon that will recieve the next attack
    /// </summary>
    public Pokemon opponentActivePokemon;

    /// <summary>
    /// Where to render the pokemon prefabs
    /// </summary>
    public List<GameObject> playerPokemonLocations;

    /// <summary>
    /// Where to render the pokemon prefabs
    /// </summary>
    public List<GameObject> opponentPokemonLocations;

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
    /// The common energy to use every turn
    /// </summary>
    public List<Energy> commonEnergy;

    /// <summary>
    /// The UI to choose a pokemon to switch to
    /// </summary>
    public GameObject switchPokemonOverlay;

    /// <summary>
    /// How many cards to draw
    /// </summary>
    public int handSize = 5;

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
        // Place pokemon
        activePokemon = player.party.First();
        opponentActivePokemon = opponent.party.First();
        var i = 0;
        player.party.ForEach(p =>
        {
            p.transform.position = playerPokemonLocations[i].transform.position;
            p.transform.rotation = playerPokemonLocations[i].transform.rotation;
            i++;
        });
        var j = 0;
        opponent.party.ForEach(p =>
        {
            p.transform.position = opponentPokemonLocations[j].transform.position;
            p.transform.rotation = opponentPokemonLocations[j].transform.rotation;
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

        // Send event to all energy, cards, and pokemon
        allPokemon.ForEach(p => p.onBattleStart(this));
        allCards.ForEach(c => c.onBattleStart(this));
        allEnergy.ForEach(e => e.onBattleStart(this));

        // Trigger opponent first move
    }

    /// <summary>
    /// On the draw step
    /// </summary>
    public void onDraw() {
        // Draw cards up to handSize
        while (hand.Count < handSize)
        {
            if (deck.Count < 1)
            {
                reshuffleDiscard();
            }
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
        cardDrawn.transform.position = handLocations[hand.Count - 1].transform.position;
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

    public void onOpponentDraw() {
        // Take action on queued move

        // Trigger right away for now
        onOpponentTurnEnd();
    }

    public void onPlay(Card move, Pokemon user, Pokemon target) {
        // Pay cost
        payMoveCost(move);
        
        // Trigger move action
        move.onPlay(user, target);

        // Discard card
        discard.Add(move);
        hand.Remove(move);
        move.transform.position = discardLocation.transform.position;
        move.transform.rotation = discardLocation.transform.rotation;
    }

    private void payMoveCost(Card move)
    {
        move.cost.ForEach(energy =>
        {
            // Subtract from common

            // Subtract from active

        });
    }

    public void onEnergyPlay(Card move, Energy source, Pokemon target)
    {
        move.onEnergyPlay(source, target);

        discard.Add(move);
        hand.Remove(move);
        move.transform.position = discardLocation.transform.position;
        move.transform.rotation = discardLocation.transform.rotation;
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

        // Send event to all energy, cards, status, and pokemon
        allPokemon.ForEach(p => p.onTurnEnd());
        allCards.ForEach(c => c.onTurnEnd());
        allEnergy.ForEach(e => e.onTurnEnd());

        // Check game end conditions
        onEitherTurnEnd();

        // Trigger next step
        onOpponentDraw();
    }

    public void onOpponentTurnEnd() {
        // Compute next move

        // Send event to all energy, cards, status, and pokemon
        allPokemon.ForEach(p => p.onOpponentTurnEnd());
        allCards.ForEach(c => c.onOpponentTurnEnd());
        allEnergy.ForEach(e => e.onOpponentTurnEnd());

        // Check game end conditions
        onEitherTurnEnd();
    }

    public void onEitherTurnEnd()
    {

        // When all opponents pokemon are 0 hp
        var numberOfPlayeredKOed = player.party.Where(p => p.health <= 0).Count();
        if (numberOfPlayeredKOed == 0) { onBattleEnd(true); }

        // When all player pokemon are 0 hp
        var numberOfOpponentKOed = opponent.party.Where(p => p.health <= 0).Count();
        if (numberOfOpponentKOed == 0) { onBattleEnd(false); }

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

        if (isPlayerWinner)
        {
            print("The player won");
        }
        else
        {
            print("The opponent won");
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
