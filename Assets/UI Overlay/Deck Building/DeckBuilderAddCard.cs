using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the entire deck building state and events
/// </summary>
public class DeckBuilderAddCard : HoverAndDragMessageTarget
{
    public PlayerDeck player;

    /// <summary>
    /// Used to determine what next song to load
    /// </summary>
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
    /// The pack the player has drawn
    /// </summary>
    public List<Card> pack;

    /// <summary>
    /// The cards the player did not select
    /// </summary>
    public List<Card> packDiscard;

    /// <summary>
    /// The pokemon that can use moves
    /// </summary>
    public Pokemon activePokemon;

    /// <summary>
    /// The target card for building
    /// </summary>
    public Card targetCard;

    /// <summary>
    /// The candy to trigger the level up
    /// </summary>
    public RareCandy candy;

    public string renderLocationsBelow;

    /// <summary>
    /// Where to render the pokemon prefabs
    /// </summary>
    public List<Pokemon> playerPokemonLocations;

    /// <summary>
    /// Where to render the card prefabs
    /// </summary>
    public List<GameObject> packLocations;

    /// <summary>
    /// Where the cards and items are discarded to off screen
    /// </summary>
    public GameObject discardLocations;

    /// <summary>
    /// Where the cards and item are drawn from off screen
    /// </summary>
    public GameObject drawLocations;

    /// <summary>
    /// The locations of the party
    /// </summary>
    public List<GameObject> pokemonModelLocations;

    /// <summary>
    /// The locations of the party cameras
    /// </summary>
    public List<GameObject> pokemonCameraLocations;

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
    public int packSize = 3;

    /// <summary>
    /// Dialog helping to instruct the user
    /// </summary>
    public WorldDialog worldDialog;

    // Triggers before start https://www.monkeykidgc.com/2020/07/unity-lifecycle-awake-vs-onenable-vs-start.html
    void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindObjectOfType<PlayerDeck>();
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

        // Move cards into placement
        player.party.ForEach(p =>
        {
            p.initDeck.ForEach(c => c.Translate(drawLocations.transform.position, "deck"));
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
            // Rotate active pokemon 90 degrees
            player.party.ForEach(p => p.pokemonRootModel.transform.rotation = Quaternion.Euler(0f, 0f, 0f));

            playerParty.gameObject.transform.parent = player.transform;
            // set rotation back to avoid any issues in the future
            playerParty.gameObject.transform.rotation = prevPlayerPartyRotation;
        }

        // Hide card and modal assets
        player.party.ForEach(p => p.hideModels());

        // Load previous screen
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
    public async virtual void onBuilderStart()
    {
        // Place pokemon
        activePokemon = null;
        onSetupPlayer();
        player.party.ForEach(p => p.onDeckBuildStart(this));

        // Remove energies
        player.energies.ToList().ForEach(e =>
        {
            e.transform.position = drawLocations.transform.position;
        });

        await worldDialog.ShowMessageAsync("Select a pokemon to level");

        // Move Candy
        drawCandy();
    }

    /// <summary>
    /// When the candy is placed on a pokemon
    /// </summary>
    /// <param name="candy"></param>
    /// <param name="targetPokemon"></param>
    public async virtual void onCandyPlay(RareCandy candy, Pokemon targetPokemon)
    {
        // Set active pokemon
        activePokemon = targetPokemon;

        // Discard candy
        candy.Translate(discardLocations.transform.position);

        // Focus active pokemon
        focusActivePokemon(activePokemon);

        // Level up pokemon
        activePokemon.level++;
        await worldDialog.ShowMessageAsync(activePokemon.pokemonName + " grew to level " + activePokemon.level + "!");

        // Draw pack
        drawPack();
    }

    public async virtual void onCardClick()
    {
        // Flip card up

        // Play card sound

        // Disable click button

        // Enable drag
    }

    public async virtual void onCardSelect(Card _targetCard)
    {
        // Set
        targetCard = _targetCard;
        activePokemon.initDeck.Add(targetCard);
        pack.Remove(targetCard);

        // Discard pack
        pack.ForEach(p => p.Translate(discardLocations.transform.position, "onCardSelect"));

        // Move your card
        targetCard.Translate(drawLocations.transform.position, "onCardSelect");

        // Comment on card
        await worldDialog.ShowMessageAsync("Added " + targetCard.cardName + " to " + activePokemon.pokemonName + "'s deck!");

        // Delete other pack cards from pokemon
        pack.ForEach(p => Destroy(p.gameObject));

        // Clean up card state
        targetCard.onDeckBuilderAddCardEnd();

        // Clean up pokemon state from the focus
        player.party.ForEach(p => {
            p.GetComponent<Animator>().SetBool("hasSpawned", false);
            p.transform.Find("onHover").gameObject.SetActive(true);
        });

        // Transition back
        onPackupPlayer();
    }


    /// <summary>
    /// Moves candy into position
    /// </summary>
    private async void drawCandy()
    {
        candy.Translate(packLocations[1].transform.position);
    }

    /// <summary>
    /// Moves camera into position
    /// </summary>
    private async void focusActivePokemon(Pokemon targetPokemon)
    {
        var targetIndex = player.party.FindIndex(p => p == targetPokemon);

        // Remove other pokemon
        player.party.Where(p => p != targetPokemon).ToList().ForEach(p => p.GetComponent<Animator>().SetTrigger("onDeath"));

        // Remove other pokemon onHovers
        player.party.Where(p => p != targetPokemon).ToList().ForEach(p => p.transform.Find("onHover").gameObject.SetActive(false));

        // Move camera to focus on selected pokemon
        //var targetPokemonCamera = pokemonCameraLocations[targetIndex];
        //var _distancePerSecond = 150.0f;
        //gameObject.GetComponent<TranslationAnimation>().Translate(targetPokemonCamera.transform.position, _distancePerSecond);
    }

    /// <summary>
    /// Generates cards and moves them into position
    /// </summary>
    private async void drawPack()
    {
        pack = activePokemon.GetComponent<PokemonDeckBuildSettings>().InstantiateRandomPack(drawLocations.transform.position, packSize);

        await worldDialog.ShowMessageAsync("Select a card to add to " + activePokemon.pokemonName + "'s deck");

        // For each pack card
        var i = 0;
        pack.ForEach(c =>
        {
            c.Translate(packLocations[i].transform.position, "drawPack");
            i++;
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

    /// <summary>
    /// When an object is hovering over a custom target
    /// </summary>
    public override void OnHoverEnter(HoverAndDragEvent _event)
    {
        _event.dropEvent.targetGameObject.GetComponent<Animator>().SetTrigger("onHoverEnter");
    }

    /// <summary>
    /// When an object is exited hovering over a custom target
    /// </summary>
    public override void OnHoverExit(HoverAndDragEvent _event)
    {
        _event.dropEvent.targetGameObject.GetComponent<Animator>().SetTrigger("onHoverExit");
    }

    /// <summary>
    /// When an object is dropped on a custom target
    /// </summary>
    public override void OnDrop(HoverAndDragEvent _event)
    {
        onCardSelect(_event.targetCard);
    }
}
