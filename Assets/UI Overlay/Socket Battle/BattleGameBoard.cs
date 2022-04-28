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
    public List<Card> deck
    {
        get { return activePokemon.deck; }
        set { activePokemon.deck = value; }
    }

    /// <summary>
    /// The hand the player has
    /// </summary>
    public List<Card> hand
    {
        get { return activePokemon.hand; }
        set { activePokemon.hand = value; }
    }

    /// <summary>
    /// The cards the player used to be reshuffled
    /// </summary>
    public List<Card> discard
    {
        get { return activePokemon.discard; }
        set { activePokemon.discard = value; }
    }

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
    /// All the active deck card events to trigger
    /// </summary>
    public List<Card> allCards
    {
        get
        {
            return deck.Union(hand).Union(discard).ToList();
        }
    }

    /// <summary>
    /// All the complete deck card events to trigger
    /// </summary>
    public List<Card> allPartyCards
    {
        get
        {
            var cards = player.party.SelectMany(p => p.deck.Union(p.hand).Union(p.discard));
            return cards.ToList();
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
    /// The locations of the player party
    /// </summary>
    public GameObject playerPartyParentGameobject;

    /// <summary>
    /// The angle the party rotation was when entering
    /// </summary>
    private Quaternion prevPlayerPartyRotation;

    /// <summary>
    /// The locations of the party
    /// </summary>
    public GameObject opponentPartyParentGameobject;

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


    // Triggers before start https://www.monkeykidgc.com/2020/07/unity-lifecycle-awake-vs-onenable-vs-start.html
    void Awake()
    {
        if (player == null)
        {
            player = GameObject.Find("Player Dad").GetComponent<PlayerDeck>();
        }

        if (opponent == null)
        {
            opponent = GameObject.FindObjectOfType<OpponentDeck>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        onBattleStart();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Move the players and pokemon into the right spots
    /// </summary>
    private void onSetupPlayer()
    {
        // Disabled players

        // Move party parent reference into placement
        var playerParty = GameObject.Find("Player Dad/party");
        if (playerParty)
        {
            prevPlayerPartyRotation = playerParty.gameObject.transform.rotation;
            // zero out position to avoid any issues moving the objects into the canvas
            playerParty.gameObject.transform.position = new Vector3(0f, 0f, 0f);
            // Move to new parent
            playerParty.gameObject.transform.parent = playerPartyParentGameobject.transform;
            // zero out rotation to remove any rotation from the player
            playerParty.gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        }

        var opponentParty = GameObject.Find("Opponent/party");
        if (opponentParty)
        {
            // zero out position to avoid any issues moving the objects into the canvas
            opponentParty.gameObject.transform.position = new Vector3(0f, 0f, 0f);
            // Move to new parent
            opponentParty.gameObject.transform.parent = opponentPartyParentGameobject.transform;
            // zero out rotation to remove any rotation from the player
            opponentParty.gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        }

        // Move pokemon into placement
        var i = 0;
        player.party.ForEach(p =>
        {
            p.setPlacement(playerPokemonLocations[i], pokemonModelLocations[i]);
            p.showModels();
            i++;
        });
        var j = 0;
        opponent.party.ForEach(p =>
        {
            p.setPlacement(opponentPokemonLocations[j], opponentPokemonModelLocations[j]);
            p.showModels();
            j++;
        });
    }

    /// <summary>
    /// Move the pokemon back to the players
    /// </summary>
    private void onPackupPlayer()
    {
        // Enable players

        // Move party parent reference into placement
        var playerParty = GameObject.Find("deck/party");
        if (playerParty)
        {
            playerParty.gameObject.transform.parent = player.transform;
            // set rotation back to avoid any issues in the future
            playerParty.gameObject.transform.rotation = prevPlayerPartyRotation;
        }

        var opponentParty = GameObject.Find("opp-deck/party");
        if (opponentParty)
        {
            opponentParty.gameObject.transform.parent = opponent.transform;
        }

        player.GetComponent<InteractionChecker>().LoadPreviousScene();
    }

    /// <summary>
    /// Assumes player and opponent are set before calling this
    /// </summary>
    public void onBattleStart()
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
    }

    /// <summary>
    /// On the draw step
    /// </summary>
    public void onDraw() {
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

        // Draw cards up to handSize
        i = 0;
        while (hand.Count < handSize && i < maxThreshold)
        {
            if (deck.Count < 1) { reshuffleDiscard(); }
            drawCard(activePokemon);
            i++;
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
        var cardLoc = handLocations[hand.Count - 1].transform.position + new Vector3(0, 0, -1);
        cardDrawn.Translate(cardLoc);
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
        // Check if there are energies to draw
        if (energyDeck.Count <= 0) return;

        // Draw card
        var energyDrawn = energyDeck.First();
        energyDrawn.isUsed = false;
        energyHand.Add(energyDrawn);
        energyDeck.Remove(energyDrawn);
        energyDrawn.Translate(energyHandLocations[energyHand.Count - 1].transform.position);
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
        cardDiscard(move, user, true);

        // Send card playedEvent to all cards
        allCards.ForEach(c => c.onCardPlayed(move, user, target));
        opponent?.opponentStrategyBot?.onCardPlayed(move, user, target);


        // Check if you won after a card play
        var numberOfOpponentPokeAlive = opponent.party.Where(p => p.health > 0).Count();
        if (numberOfOpponentPokeAlive == 0) { onTurnEnd(); }

    }

    /// <summary>
    /// Update the energies that are used to play the move
    /// </summary>
    /// <param name="move"></param>
    private void payMoveCost(List<Energy> cost)
    {
        var coloredCost = cost.Where(e => e.energyName != "Normal").ToList();
        coloredCost.ForEach(energy => payEnergyCost(energy));

        var colorlessCost = cost.Where(e => e.energyName == "Normal").ToList();
        colorlessCost.ForEach(energy => payEnergyCost(energy));
    }

    /// <summary>
    /// Discards the given card
    /// </summary>
    /// <param name="target">The move to discard</param>
    /// <param name="user">The pokemon that used the move</param>
    /// <param name="wasPlayed">True if the discard was after playing</param>
    public void cardDiscard(Card target, Pokemon user, bool wasPlayed)
    {
        if (!(wasPlayed && target.isSingleUse)) user.discard.Add(target);
        user.hand.Remove(target);
        target.Translate(discardLocation.transform.position);
        target.transform.rotation = discardLocation.transform.rotation;
        target.onDiscard(wasPlayed);
    }

    private void payEnergyCost(Energy energy)
    {
        // Subtract from common
        var target = commonEnergy.Where(e => !e.isUsed && e.energyName == energy.energyName).FirstOrDefault();
        if (target != null)
        {
            target.isUsed = true;
            return;
        }
        // Subtract from active
        var targetOnPokemon = activePokemon.attachedEnergy.Where(e => !e.isUsed && e.energyName == energy.energyName).FirstOrDefault();
        if (targetOnPokemon != null)
        {
            targetOnPokemon.isUsed = true;
            return;
        }
        var colorlessTargetOnPokemon = activePokemon.attachedEnergy.Where(e => !e.isUsed).FirstOrDefault();
        if (energy.energyName == "Normal" && colorlessTargetOnPokemon != null)
        {
            colorlessTargetOnPokemon.isUsed = true;
            return;
        }
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
            e.Translate(energyDiscardLocation.transform.position);
            e.transform.rotation = energyDiscardLocation.transform.rotation;
        });
        energyDiscard.AddRange(energyHand);
        energyHand.RemoveAll(card => true);
    }

    public void onTurnEnd() {
        // Discard hand
        hand.ForEach(c =>
        {
            c.transform.rotation = discardLocation.transform.rotation;
            c.Translate(discardLocation.transform.position);
        });
        discard.AddRange(hand);
        hand.RemoveAll(card => true);

        // Discard other energies
        energyHand.ForEach(e =>
        {
            e.Translate(energyDiscardLocation.transform.position);
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
        var isPlayerPokeFainted = activePokemon.isFainted;
        if (isPlayerPokeFainted && numberOfPlayerPokeAlive > 0) {
            var nextAlivePoke = player.party.Where(p => !p.isFainted).First();
            switchPokemon(activePokemon, nextAlivePoke);
        }

        // When opponent active pokemon is 0 hp - switch in
        var isOpponentPokeFainted = opponentActivePokemon.isFainted;
        if (isOpponentPokeFainted && numberOfOpponentPokeAlive > 0)
        {
            var nextOpponentAlivePoke = opponent.party.Where(p => !p.isFainted).First();
            switchOpponentPokemon(opponentActivePokemon, nextOpponentAlivePoke);
            
            // Compute next move if acting pokemon fainted
            if (opponent.opponentStrategyBot.nextOpponentMove.actingPokemon.isFainted)
            {
                opponent.opponentStrategyBot.computeOpponentsNextMove();
            }
        }
    }


    public void showSwitchPokemon()
    {
    }

    public void hideSwitchPokemon()
    {
    }

    /// <summary>
    /// Swaps two elements of a list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <see cref="https://stackoverflow.com/a/2094316"/>
    /// <param name="list"></param>
    /// <param name="indexA"></param>
    /// <param name="indexB"></param>
    private void Swap<T>(IList<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }

    /// <summary>
    /// Switches two of the player's pokemon
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    public void switchPokemon(Pokemon user, Pokemon target)
    {
        // Discard pokemon hand
        hand.ForEach(c =>
        {
            c.transform.rotation = discardLocation.transform.rotation;
            c.Translate(discardLocation.transform.position);
        });
        discard.AddRange(hand);
        hand.RemoveAll(card => true);

        // Switch board roles
        var userIndex = player.party.IndexOf(user);
        var targetIndex = player.party.IndexOf(target);
        Swap(player.party, userIndex, targetIndex);
        activePokemon = target;
        
        // Update model locations to match switch
        var i = 0;
        player.party.ForEach(p =>
        {
            p.setPlacement(playerPokemonLocations[i], pokemonModelLocations[i], true);
            i++;
        });

        // Animate swap       
        //user.GetComponent<Animator>().SetTrigger("onSwitchOut");
        //target.GetComponent<Animator>().SetTrigger("onSwitchIn");

        // Draw new pokemon hand

        // Draw cards up to handSize
        while (hand.Count < handSize)
        {
            if (deck.Count < 1) { reshuffleDiscard(); }
            drawCard(activePokemon);
        }
    }

    /// <summary>
    /// Switches two of the opponent's pokemon
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    public void switchOpponentPokemon(Pokemon user, Pokemon target)
    {
        // Switch board roles
        var userIndex = opponent.party.IndexOf(user);
        var targetIndex = opponent.party.IndexOf(target);
        Swap(opponent.party, userIndex, targetIndex);
        opponentActivePokemon = target;

        // Update model locations to match switch
        var i = 0;
        opponent.party.ForEach(p =>
        {
            p.setPlacement(opponentPokemonLocations[i], opponentPokemonModelLocations[i], true);
            i++;
        });

        // Animate swap       
        //user.GetComponent<Animator>().SetTrigger("onSwitchOut");
        //target.GetComponent<Animator>().SetTrigger("onSwitchIn");
    }

    public void onBattleEnd(bool isPlayerWinner) {
        // Set results
        gameHasEnded = true;
        playerHasWon = isPlayerWinner;

        if (isPlayerWinner)
        {
            worldDialog.ShowMessage("Red won!", () => {
                print("The player won");

                player.party.ForEach(p => { p.hideModels(); });
                opponent.party.ForEach(p => { p.hideModels(); });

                // Send event to all energy, cards, status, and pokemon
                allPokemon.ForEach(p => p.onBattleEnd());
                allCards.ForEach(c => c.onBattleEnd());
                allEnergy.ForEach(e => e.onBattleEnd());
                opponent.opponentStrategyBot.onBattleEnd();
                opponent.movesConfig.ForEach(m => m.onBattleEnd());

                onPackupPlayer();

                return true;
            });
        }
        else
        {
            worldDialog.ShowMessage(opponent.opponentName + " won.", () => {
                print("The opponent won");

                player.party.ForEach(p => { p.hideModels(); });
                opponent.party.ForEach(p => { p.hideModels(); });

                // Send event to all energy, cards, status, and pokemon
                allPokemon.ForEach(p => p.onBattleEnd());
                allCards.ForEach(c => c.onBattleEnd());
                allEnergy.ForEach(e => e.onBattleEnd());
                opponent.opponentStrategyBot.onBattleEnd();
                opponent.movesConfig.ForEach(m => m.onBattleEnd());

                onPackupPlayer();

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
