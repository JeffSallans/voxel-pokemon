using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains the meta game configurations the player brings into a battle.  Includes current deck, base pokemon stats/state, items, and others.
/// </summary>
public class PlayerDeck : MonoBehaviour
{
    /// <summary>
    /// The pokemon to bring into battle, the first one in the list is the active one
    /// </summary>
    public List<Pokemon> party;

    /// <summary>
    /// The energies to bring into battle. (Should probably be computed)
    /// </summary>
    public List<Energy> energies;

    /// <summary>
    /// The amount of money the player has
    /// </summary>
    public int money = 0;

    /// <summary>
    /// Set to true to skip all battles for testing the rest of the game
    /// </summary>
    public bool skipBattles = false;

    /// <summary>
    /// The options the users has for this game
    /// </summary>
    public GameOptions gameOptions;

    private PokemonFactory pokemonFactory;

    // Start is called before the first frame update
    void Awake()
    {
        if (party.Count == 0)
        {
            party = gameObject.GetComponentsInChildren<Pokemon>().ToList();
        }
        if (energies.Count == 0)
        {
            var allEnergies = gameObject.GetComponentsInChildren<Energy>().ToList();
            energies = allEnergies.Where(e => e.initCanBeDragged).ToList();
        }
        if (gameOptions == null)
        {
            gameOptions = gameObject.GetComponent<GameOptions>();
        }
        if (pokemonFactory == null)
        {
            pokemonFactory = gameObject.GetComponent<PokemonFactory>();
        }
    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Returns if it creates a pokemon from the given list and add to the player's party.
    /// </summary>
    /// <param name="pokemonName">The name of the pokemon prefab to add to the party</param>
    public bool AddPokemon(string pokemonName)
    {
        // Check if player has 3 pokemon, if so abort
        if (party.Count >= 3) return false;

        // Select current player and parent
        var partyParent = gameObject.transform.Find("party");

        // Instantiate pokemon at given parent
        var addedPokemonObject = pokemonFactory.GetPokemon(pokemonName, partyParent);
        var addedPokemon = addedPokemonObject.GetComponent<Pokemon>();

        // Add pokemon to party list
        party.Add(addedPokemon);

        // Add pokemon energies to deck
        var newEnergies = addedPokemonObject.GetComponentsInChildren<Energy>();
        energies.AddRange(newEnergies);

        return true;
    }

    /// <summary>
    /// Returns the active player
    /// </summary>
    /// <returns></returns>
    public static PlayerDeck GetPlayer()
    {
        var playerObject = GameObject.FindObjectOfType<CharacterController>().gameObject;
        var player = playerObject.GetComponent<PlayerDeck>();
        return player;
    }
}
