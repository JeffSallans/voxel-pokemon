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
    protected override List<Card> _getDeck()
    {
        return mergedDeck;
    }

    protected override void _setDeck(List<Card> d)
    {
        mergedDeck = d;
    }

    /// <summary>
    /// The combined hand of all cards
    /// </summary>
    public List<Card> mergedHand = null;
    protected override List<Card> _getHand()
    {
        return mergedHand;
    }

    protected override void _setHand(List<Card> h)
    {
        mergedHand = h;
    }

    /// <summary>
    /// The combined discard of all cards
    /// </summary>
    public List<Card> mergedDiscard = null;
    protected override List<Card> _getDiscard()
    {
        return mergedDiscard;
    }

    protected override void _setDiscard(List<Card> d)
    {
        mergedDiscard = d;
    }



    /// <summary>
    /// Assumes player and opponent are set before calling this
    /// </summary>
    public override void onBattleStart()
    {
        /*
        _getDeck = () =>
        {

        }
        */

        handSize = 5;

        // Remove all energy things
        energyHandSize = 0;

        // Setup deck
        mergedHand = new List<Card>();
        mergedDiscard = new List<Card>();
        mergedDeck = new List<Card>();
        player.party.ForEach(p => {
            mergedDeck.AddRange(p.initDeck);
        });
        
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
    }

    public override void onPlay(Card move, Pokemon user, Pokemon target)
    {
        // Only play cards of that user
        if (pokemonAllowedToPlayCards != null && pokemonAllowedToPlayCards != user) return;
        pokemonAllowedToPlayCards = user;

        if (activePokemon == user)
        {
            // Switch user in
            switchPokemonAndKeepCards(activePokemon, user);
        }
        

        base.onPlay(move, user, target);
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
            p.setPlacement(playerPokemonLocations[i], pokemonModelLocations[i], true);
            i++;
        });
    }

    public override void onTurnEnd()
    {
        pokemonAllowedToPlayCards = null;
        base.onTurnEnd();
    }
}