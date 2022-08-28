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

        // Play multiple cards
        numberOfCardsCanPlay = 10;

        // Remove all energy things
        energyHandSize = 0;

        // Setup deck
        mergedHand = new List<Card>();
        mergedDiscard = new List<Card>();
        deck = new List<Card>();
        player.party.ForEach(p => {
            var deckWithoutSwitches = p.initDeck.Where(c => c.cardName != "Switch").ToList();
            deck.AddRange(deckWithoutSwitches);
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

        // Remove energy slots
        player.party.ForEach(p => p.maxNumberOfAttachedEnergy = 0);

        // Change card energy to pokemon face
        deck.ForEach(c => setupCardToShowPokemonFace(c));

        // Add card can play override
        deck.ForEach(c => c.canBePlayedOverride = () => true);

    }

    protected override void onPackupPlayer()
    {
        base.onPackupPlayer();

        // Add energy slots
        player.party.ForEach(p => p.maxNumberOfAttachedEnergy = 4);

        // Change card energy to pokemon face
        deck.ForEach(c => packupCardToHidePokemonFace(c));

        // Remove card can play override
        deck.ForEach(c => c.canBePlayedOverride = null);
    }

    private void setupCardToShowPokemonFace(Card card)
    {
        // Hide energy
        card.cost.ForEach(e => e.gameObject.SetActive(false));

        // Calculate owner
        var owner = player.party.Find(pokemon => pokemon.initDeck.Contains(card));

        // Show pokemon icon
        var pokemonIconName = "icon-" + owner?.pokemonName?.ToLower();
        var pokemonIcon = card.gameObject.transform.Find(pokemonIconName);
        pokemonIcon.gameObject.SetActive(true);

        // Add custom drag
        card._onDragFunc = () => { 
            previousActivePokemon = activePokemon;
            switchPokemonAndKeepCards(activePokemon, owner);
            return true;
        };
        card._onDropFunc = () => {
            switchPokemonAndKeepCards(owner, previousActivePokemon);
            return true;
        };

    }

    private void packupCardToHidePokemonFace(Card card)
    {
        // Show energy
        card.cost.ForEach(e => e.gameObject.SetActive(true));

        // Calculate owner
        var owner = player.party.Find(pokemon => pokemon.initDeck.Contains(card));

        // Hide pokemon icon
        var pokemonIconName = "icon-" + owner?.pokemonName?.ToLower();
        var pokemonIcon = card.gameObject.transform.Find(pokemonIconName);
        pokemonIcon.gameObject.SetActive(false);

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
            switchPokemonAndKeepCards(activePokemon, owner);
        }
        

        base.onPlay(move, owner, target);
    }

    private void switchPokemonAndKeepCards(Pokemon user, Pokemon target)
    {
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
    }

    public override void onTurnEnd()
    {
        pokemonAllowedToPlayCards = null;
        base.onTurnEnd();
    }
}