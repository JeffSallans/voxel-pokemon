using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the entire battle state and events
/// </summary>
public class BattleGameBoard : MonoBehaviour
{
    public PlayerDeck player;

    public OpponentDeck opponent;

    public InteractionChecker playerInteractionChecker;

    public string debugVariablesBelow;

    /// <summary>
    /// Cards to be drawn
    /// </summary>
    public virtual List<Card> deck
    {
        get {
            if (_getOverrideDeck != null) return _getOverrideDeck();
            return activePokemon.deck;
        }
        set {
            if (_setOverrideDeck != null) { _setOverrideDeck(value); return; }
            activePokemon.deck = value;
        }
    }
    protected System.Func<List<Card>> _getOverrideDeck = null;
    protected System.Func<List<Card>, bool> _setOverrideDeck = null;

    /// <summary>
    /// The hand the player has
    /// </summary>
    public virtual List<Card> hand
    {
        get
        {
            if (_getOverrideHand != null) return _getOverrideHand();
            return activePokemon.hand;
        }
        set
        {
            if (_setOverrideHand != null) { _setOverrideHand(value); return; }
            activePokemon.hand = value;
        }
    }
    protected System.Func<List<Card>> _getOverrideHand = null;
    protected System.Func<List<Card>, bool> _setOverrideHand = null;

    /// <summary>
    /// The cards the player used to be reshuffled
    /// </summary>
    public virtual List<Card> discard
    {
        get
        {
            if (_getOverrideDiscard != null) return _getOverrideDiscard();
            return activePokemon.discard;
        }
        set
        {
            if (_setOverrideDiscard != null) { _setOverrideDiscard(value); return; }
            activePokemon.discard = value;
        }
    }
    protected System.Func<List<Card>> _getOverrideDiscard = null;
    protected System.Func<List<Card>, bool> _setOverrideDiscard = null;

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
    /// The order of the party when entering
    /// </summary>
    private List<Pokemon> playerInitPartyOrder;

    /// <summary>
    /// The locations of the party
    /// </summary>
    public GameObject opponentPartyParentGameobject;

    /// <summary>
    /// How many energies to pick from
    /// </summary>
    public int energyHandSize = 2;

    /// <summary>
    /// True if the energy hand should be discarded after using one
    /// </summary>
    public bool energyHandDiscard = false;

    /// <summary>
    /// True if the energy should disable after playing one
    /// </summary>
    public bool onlyPlayOneEnergy = false;

    /// <summary>
    /// True if playing a card discards that cost by default
    /// </summary>
    public bool discardCardCost = true;

    /// <summary>
    /// How many cards to draw
    /// </summary>
    public int handSize = 4;

    /// <summary>
    /// True if the hand should be discarded at the end of the turn
    /// </summary>
    public bool handDiscard = false;

    /// <summary>
    /// True if the deck is shuffle at the beginning and when adding the discard back
    /// </summary>
    public bool shuffleDeck = true;

    /// <summary>
    /// The number of cards a user can play per turn
    /// </summary>
    public int numberOfCardsCanPlay = 1;

    /// <summary>
    /// The number of cards a user can play for this current turn
    /// </summary>
    public int remainingNumberOfCardsCanPlay = 1;

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
            player = GameObject.FindObjectOfType<PlayerDeck>();
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
    protected virtual void onSetupPlayer()
    {
        // Disabled player model
        player.gameObject.transform.Find("default").gameObject.SetActive(false);
        opponent.gameObject.transform.Find("position-offset").gameObject.SetActive(false);

        // Move party parent reference into placement
        var playerParty = player.gameObject.transform.Find("party");
        if (playerParty)
        {
            playerInitPartyOrder = player.party.ToList();
            prevPlayerPartyRotation = playerParty.gameObject.transform.rotation;
            // zero out position to avoid any issues moving the objects into the canvas
            playerParty.gameObject.transform.position = new Vector3(0f, 0f, 0f);
            // zero out rotation to remove any rotation from the player
            playerParty.gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        }

        var opponentParty = opponent.gameObject.transform.Find("party");
        if (opponentParty)
        {
            // zero out position to avoid any issues moving the objects into the canvas
            opponentParty.gameObject.transform.position = new Vector3(0f, 0f, 0f);
            // zero out rotation to remove any rotation from the player
            opponentParty.gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        }

        // Move pokemon into placement
        var i = 0;
        player.party.ForEach(p =>
        {
            p.setupPokemonPlacement(playerPokemonLocations[i], pokemonModelLocations[i]);
            p.showModels();
            i++;
        });
        var j = 0;
        opponent.party.ForEach(p =>
        {
            p.setupPokemonPlacement(opponentPokemonLocations[j], opponentPokemonModelLocations[j]);
            p.showModels();
            j++;
        });

        // Moves cards into placement
        player.party.ForEach(p => setupCardsOffPokemon(p));
    }

    private void setupCardsOffPokemon(Pokemon pokemon)
    {
        var cardsObject = pokemon.initDeck[0].gameObject.transform.parent;
        cardsObject.parent = playerPartyParentGameobject.transform;
    }

    private void packCardsOnPokemon(Pokemon pokemon)
    {
        var cardsObject = pokemon.initDeck[0].gameObject.transform.parent;
        cardsObject.parent = pokemon.transform;
    }

    /// <summary>
    /// Move the pokemon back to the players
    /// </summary>
    protected virtual void onPackupPlayer()
    {
        // Enable player model
        player.gameObject.transform.Find("default").gameObject.SetActive(true);
        opponent.gameObject.transform.Find("position-offset").gameObject.SetActive(true);

        // Move energies into discard
        allEnergy.ForEach(e => e.transform.position = discardLocation.transform.position);

        player.party.ForEach(p => packCardsOnPokemon(p));
        player.party.ForEach(p => p.packupPokemonPlacement());

        // Move party parent reference into placement
        var playerParty = player.gameObject.transform.Find("party");
        if (playerParty)
        {
            // set rotation back to avoid any issues in the future
            playerParty.gameObject.transform.rotation = prevPlayerPartyRotation;
            player.party = playerInitPartyOrder;
        }


        if (playerInteractionChecker)
        {
            playerInteractionChecker.LoadPreviousScene();
            return;
        }
    }

    /// <summary>
    /// Assumes player and opponent are set before calling this
    /// </summary>
    public virtual void onBattleStart()
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
        opponent.movesConfig.ForEach(m => m.onBattleStart(this));

        worldDialog.ShowMessage(opponent.opponentName + " wants to battle.", () => {

            // Deal 1 energy to each pokemon
            player.party.ForEach(p =>
            {
                var energy = energyDeck[0];
                energyDeck.Remove(energy);
                energy.playEnergy(p);
            });

            // Trigger opponent first move
            opponent.opponentStrategyBot.computeOpponentsNextMove();

            // Trigger draw
            onDraw();

            endTurnButton.SetActive(true);
            endTurnButton.GetComponent<Button>().interactable = false;

            return true;
        });
    }

    /// <summary>
    /// On the draw step
    /// </summary>
    public virtual void onDraw() {
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
    protected void drawCard(Pokemon activePokemon) {
        // Don't draw if the deck doesn't have cards (assuming reshuffle was used before this) 
        if (deck.Count == 0) return;

        // Draw card
        var cardDrawn = deck.First();
        hand.Add(cardDrawn);
        deck.Remove(cardDrawn);
        var cardLoc = handLocations[hand.Count - 1].transform.position + new Vector3(0, 0, -1);
        cardDrawn.Translate(cardLoc, "drawCard");
        cardDrawn.transform.rotation = handLocations[hand.Count - 1].transform.rotation;

        // Trigger event
        cardDrawn.onDraw(activePokemon);
    }

    /// <summary>
    /// Modifies discard and deck
    /// </summary>
    protected void reshuffleDiscard()
    {
        deck.AddRange(discard);
        discard.RemoveAll(card => true);
        if (shuffleDeck)
        {
            Shuffle(deck);
        }
        deck.ForEach(c =>
        {
            c.transform.position = deckLocation.transform.position;
            c.transform.rotation = deckLocation.transform.rotation;
        });
    }

    /// <summary>
    /// Draw the top energy from the deck
    /// </summary>
    protected void drawEnergy()
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
    protected void reshuffleEnergyDiscard()
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

    public async void onOpponentDraw() {
        // Take action on queued move
        await opponent.opponentStrategyBot.opponentPlay();
        onOpponentTurnEnd();
    }

    /// <summary>
    /// The Event when a card is played
    /// </summary>
    /// <param name="move"></param>
    /// <param name="user"></param>
    /// <param name="target"></param>
    public virtual void onPlay(Card move, Pokemon user, Pokemon target) {
        // Decrement counter
        if (remainingNumberOfCardsCanPlay == 0) return;
        remainingNumberOfCardsCanPlay--;
       
        // Enable end turn button
        endTurnButton.GetComponent<Button>().interactable = true;

        // Pay cost
        payMoveCost(move.cost, user, discardCardCost);
        
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

        // Check if you can't play any more cards or energy
        // if (remainingNumberOfCardsCanPlay == 0 && energyHand.Count == 0) { onTurnEnd(); }
    }

    /// <summary>
    /// Update the energies that are used to play the move
    /// </summary>
    /// <param name="move"></param>
    /// <param name="discardCardCost">Set to true if the energy should be discarded instead of disabled</param>
    protected void payMoveCost(List<Energy> cost, Pokemon user, bool discardCardCost)
    {
        var coloredCost = cost.Where(e => e.energyName != "Normal").ToList();
        coloredCost.ForEach(energy => payEnergyCost(energy, user, discardCardCost));

        var colorlessCost = cost.Where(e => e.energyName == "Normal").ToList();
        colorlessCost.ForEach(energy => payEnergyCost(energy, user, discardCardCost));
    }

    /// <summary>
    /// Discards the given card
    /// </summary>
    /// <param name="target">The move to discard</param>
    /// <param name="user">The pokemon that used the move</param>
    /// <param name="wasPlayed">True if the discard was after playing</param>
    public void cardDiscard(Card target, Pokemon user, bool wasPlayed)
    {
        if (!(wasPlayed && target.isSingleUse)) discard.Add(target);
        hand.Remove(target);
        target.Translate(discardLocation.transform.position, "cardDiscard");
        target.transform.rotation = discardLocation.transform.rotation;
        target.onDiscard(wasPlayed);
    }

    /// <summary>
    /// Modifies the given energy used for the card cost
    /// </summary>
    /// <param name="energy">energy to remove</param>
    /// <param name="discardCardCost">Set to true if the energy should be discarded instead of disabled</param>
    protected void payEnergyCost(Energy energy, Pokemon pokemon, bool discardCardCost)
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
            if (discardCardCost)
            {
                pokemon.DiscardEnergy(targetOnPokemon);
            }
            else
            {
                targetOnPokemon.isUsed = true;
            }
            return;
        }
        var colorlessTargetOnPokemon = activePokemon.attachedEnergy.Where(e => !e.isUsed).FirstOrDefault();
        if (energy.energyName == "Normal" && colorlessTargetOnPokemon != null)
        {
            if (discardCardCost)
            {
                pokemon.DiscardEnergy(colorlessTargetOnPokemon);
            }
            else
            {
                colorlessTargetOnPokemon.isUsed = true;
            }
            return;
        }
    }

    public void onEnergyEvent(int index)
    {
        onEnergyPlay(energyHand[index], activePokemon);
    }


    public virtual void onEnergyPlay(Energy source, Pokemon target)
    {
        // Enable end turn button
        endTurnButton.GetComponent<Button>().interactable = true;

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
        } else if (onlyPlayOneEnergy)
        {
            energyHand.ForEach(e =>
            {
                e.SetCanBeDragged(false);
            });
        }

        // Check if you can't play any more cards or energy
        // if (!ignoreNextTurnCheck && remainingNumberOfCardsCanPlay == 0 && energyHand.Count == 0) { onTurnEnd(); }
    }

    public virtual void onTurnEnd() {
        // Disable end turn button
        endTurnButton.GetComponent<Button>().interactable = false;

        // Discard hand
        if (handDiscard)
        {
            hand.ForEach(c =>
            {
                c.transform.rotation = discardLocation.transform.rotation;
                c.Translate(discardLocation.transform.position, "onTurnEndA");
            });
            discard.AddRange(hand);
            hand.RemoveAll(card => true);
        }
        // Remove blank spots
        else
        {
            for (var i = 0; i < hand.Count; i++)
            {
                var cardLoc = handLocations[i].transform.position + new Vector3(0, 0, -1);
                hand[i].Translate(cardLoc, "onTurnEndB");
                hand[i].transform.rotation = handLocations[i].transform.rotation;
            }
        }

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
        // Remove blank spots
        else
        {
            for (var i = 0; i < energyHand.Count; i++)
            {
                var cardLoc = energyHandLocations[i].transform.position;
                energyHand[i].Translate(cardLoc);
                energyHand[i].transform.rotation = energyHandLocations[i].transform.rotation;
            }
        }


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

    public virtual void onOpponentTurnEnd() {
        // Compute next move
        var message = opponent.opponentStrategyBot.computeOpponentsNextMove();

        System.Func<bool> callback = () =>
        {
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
            return true;
        };

        // Display dialog if message exists
        if (message == null)
        {
            callback();
            return;
        }
        worldDialog.ShowMessage(message, callback);
    }

    public virtual void onEitherTurnEnd()
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
    protected void Swap<T>(IList<T> list, int indexA, int indexB)
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
            c.Translate(discardLocation.transform.position, "switchPokemon");
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
            p.updatePlacement(playerPokemonLocations[i], pokemonModelLocations[i], true);
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
            p.updatePlacement(opponentPokemonLocations[i], opponentPokemonModelLocations[i], true);
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
            worldDialog.ShowMessage("You won!", () => {
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
                player.GetComponent<InteractionChecker>().LoadPreviousScene();

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
                player.GetComponent<InteractionChecker>().LoadTitleScreen();

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
    protected void Shuffle<T>(IList<T> list)
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
