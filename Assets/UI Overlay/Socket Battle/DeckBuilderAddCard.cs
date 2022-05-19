using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the entire battle state and events
/// </summary>
public class DeckBuilderAddCard : MonoBehaviour
{
    public PlayerDeck player;

    public InteractionChecker playerInteractionChecker;

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
    /// Where to render the card prefabs
    /// </summary>
    public List<GameObject> packLocations;

    /// <summary>
    /// The locations of the party
    /// </summary>
    public List<GameObject> pokemonModelLocations;

    /// <summary>
    /// The locations of the player party
    /// </summary>
    public GameObject playerPartyParentGameobject;

    /// <summary>
    /// The angle the party rotation was when entering
    /// </summary>
    private Quaternion prevPlayerPartyRotation;

    /// <summary>
    /// How many cards to draw
    /// </summary>
    public int packSize = 4;

    /// <summary>
    /// Ends the turn
    /// </summary>
    public WorldDialog worldDialog;


    // Triggers before start https://www.monkeykidgc.com/2020/07/unity-lifecycle-awake-vs-onenable-vs-start.html
    void Awake()
    {
        if (player == null)
        {
            player = GameObject.Find("Player Dad").GetComponent<PlayerDeck>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        onBuilderStart();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Move the players and pokemon into the right spots
    /// </summary>
    protected void onSetupPlayer()
    {
        // Disabled players

        // Move party parent reference into placement
        var playerParty = player.gameObject.transform.Find("party");
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

        // Move pokemon into placement
        var i = 0;
        player.party.ForEach(p =>
        {
            p.setPlacement(playerPokemonLocations[i], pokemonModelLocations[i]);
            p.showModels();
            i++;
        });
    }

    /// <summary>
    /// Move the pokemon back to the players
    /// </summary>
    protected void onPackupPlayer()
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

        if (playerInteractionChecker)
        {
            playerInteractionChecker.LoadPreviousScene();
            return;
        }
        player.GetComponent<InteractionChecker>().LoadPreviousScene();
    }

    /// <summary>
    /// Assumes player and opponent are set before calling this
    /// </summary>
    public virtual void onBuilderStart()
    {
        // Place pokemon
        activePokemon = player.party.First();
        onSetupPlayer();


        worldDialog.ShowMessage("Select a pokemon to level", () => {


            return true;
        });
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
