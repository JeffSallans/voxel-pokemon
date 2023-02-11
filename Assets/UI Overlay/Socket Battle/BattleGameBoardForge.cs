using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleGameBoardForge : BattleGameBoard
{


    public Pokemon pokemonAllowedToPlayCards = null;

    /// <summary>
    /// The combined deck of all cards
    /// </summary>
    public List<Card> mergedDeck = null;

    /// <summary>
    /// The combined hand of all cards
    /// </summary>
    public List<Card> mergedHand = null;

    /// <summary>
    /// The combined discard of all cards
    /// </summary>
    public List<Card> mergedDiscard = null;

    private Pokemon previousActivePokemon = null;

    /// <summary>
    /// Assumes player and opponent are set before calling this
    /// </summary>
    public override void onBattleStart()
    {
        _getOverrideDeck = () => { return mergedDeck; };
        _setOverrideDeck = (d) => { mergedDeck = d; return true; };
        _getOverrideHand = () => { return mergedHand; };
        _setOverrideHand = (h) => { mergedHand = h; return true; };
        _getOverrideDiscard = () => { return mergedDiscard; };
        _setOverrideDiscard = (d) => { mergedDiscard = d; return true; };

        // Set hand size based on party count
        handSize = 2 + player.party.Count;

        // Play single card
        numberOfCardsCanPlay = 1;

        // Remove all energy things
        energyHandSize = 2;

        // Setup deck
        mergedHand = new List<Card>();
        mergedDiscard = new List<Card>();
        mergedDeck = new List<Card>();
        player.party.ForEach(p => {
            mergedDeck.AddRange(p.initDeck);
        });
        if (shuffleDeck)
        {
            Shuffle(deck);
        }

        // Set first previous active pokemon
        previousActivePokemon = player.party[0];

        base.onBattleStart();
    }

    protected override void onSetupPlayer()
    {
        base.onSetupPlayer();

        // Change card energy to pokemon face
        deck.ForEach(c => setupCardToShowPokemonFace(c));
        opponent.initDeck.ForEach(c => setupCardToShowPokemonFace(c));
    }

    protected override void onPackupPlayer()
    {
        base.onPackupPlayer();

        // Change card energy to pokemon face
        deck.ForEach(c => packupCardToHidePokemonFace(c));
        opponent.initDeck.ForEach(c => packupCardToHidePokemonFace(c));
    }

    private void setupCardToShowPokemonFace(Card card)
    {
        // Calculate owner before it is setup
        var owner = allPokemon.Find(pokemon => pokemon.initDeck.Contains(card));

        // Show pokemon icon
        var pokemonIconName = "icon-" + owner?.pokemonName?.ToLower();
        var pokemonIcon = card.gameObject.transform.Find(pokemonIconName);
        if (pokemonIcon != null) pokemonIcon.gameObject.SetActive(true);

        // Add custom drag
        card._onDragFunc = (c) => { 
            if (c.switchInOnUse)
            {
                previousActivePokemon = activePokemon;
                switchPokemon(activePokemon, card.owner);
            }
            return true;
        };
        card._onDropFunc = (c) => {
            if (c.switchInOnUse)
            {
                switchPokemon(card.owner, previousActivePokemon);
            }
            return true;
        };

    }

    private void packupCardToHidePokemonFace(Card card)
    {
        // Calculate owner before it is setup
        var owner = allPokemon.Find(pokemon => pokemon.initDeck.Contains(card));

        // Hide pokemon icon
        var pokemonIconName = "icon-" + owner?.pokemonName?.ToLower();
        var pokemonIcon = card.gameObject.transform.Find(pokemonIconName);
        if (pokemonIcon != null) pokemonIcon.gameObject.SetActive(false);

        // Remove custom drag
        card._onDragFunc = null;
        card._onDropFunc = null;
    }

    public override void onPlay(Card move, Pokemon user, Pokemon target)
    {
        // Calculate owner
        var owner = player.party.Find(pokemon => pokemon.initDeck.Contains(move));

        // Only play cards of that user
        if (pokemonAllowedToPlayCards != null && pokemonAllowedToPlayCards != owner) return;
        pokemonAllowedToPlayCards = owner;

        if (activePokemon != owner)
        {
            // Switch user in
            switchPokemon(activePokemon, owner);
        }
        

        base.onPlay(move, owner, target);
    }

    public override void onTurnEnd()
    {
        pokemonAllowedToPlayCards = null;
        base.onTurnEnd();
    }

    public override void onEitherTurnEnd()
    {
        // When player active pokemon is 0 hp - switch in and discard cards
        // (since the active pokemon will not be fainted after this switch, this is will avoid the base case happening which discards the hand)
        var numberOfPlayerPokeAlive = player.party.Where(p => p.health > 0).Count();
        var isPlayerPokeFainted = activePokemon.isFainted;
        if (isPlayerPokeFainted && numberOfPlayerPokeAlive > 0)
        {
            var nextAlivePoke = player.party.Where(p => !p.isFainted).First();
            switchPokemon(activePokemon, nextAlivePoke);
        }

        // Remove all deck and hand cards with fainted pokemon
        for (int i = hand.Count - 1; i >= 0; i--)
        {
            if (hand[i].owner.isFainted)
            {
                cardDiscard(hand[i], hand[i].owner, false);
            }
        }
        for (int i = deck.Count - 1; i >= 0; i--)
        {
            if (deck[i].owner.isFainted)
            {
                cardDiscard(deck[i], deck[i].owner, false, true);
            }
        }

        // When player active pokemon is 0 hp - switch in and discard cards
        // (since the active pokemon will not be fainted after this switch, this is will avoid the base case happening which discards the hand)
        var numberOfOpponentPokeAlive = opponent.party.Where(p => p.health > 0).Count();
        var isOpponentPokeFainted = opponentActivePokemon.isFainted;
        if (isOpponentPokeFainted && numberOfOpponentPokeAlive > 0)
        {
            var nextAlivePoke = opponent.party.Where(p => !p.isFainted).First();
            switchPokemon(opponentActivePokemon, nextAlivePoke);
        }

        // Remove all deck and hand cards with fainted OPPONENT pokemon
        var oppHand = opponent.opponentStrategyBot.hand;
        var oppDeck = opponent.opponentStrategyBot.deck;
        for (int i = oppHand.Count - 1; i >= 0; i--)
        {
            if (oppHand[i].card.owner.isFainted)
            {
                opponent.opponentStrategyBot.cardDiscard(oppHand[i], oppHand[i].card.owner, false);
            }
        }
        for (int i = oppDeck.Count - 1; i >= 0; i--)
        {
            if (oppDeck[i].card.owner.isFainted)
            {
                opponent.opponentStrategyBot.cardDiscard(oppDeck[i], oppDeck[i].card.owner, false, true);
            }
        }

        base.onEitherTurnEnd();
    }   

    /// <summary>
    /// Returns true if the game being played at the moment is a forge game.
    /// </summary>
    /// <returns></returns>
    public static bool IsAForgeGame()
    {
        return GameObject.FindObjectOfType<BattleGameBoardForge>() != null;
    }
}