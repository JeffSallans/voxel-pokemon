using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class PartyMenu : HoverAndDragMessageTarget
{
    PlayerDeck deck;

    /// <summary>
    /// All the menues for the party pokemon
    /// </summary>
    public List<GameObject> menues;

    /// <summary>
    /// All the switch to buttons for each pokemon
    /// </summary>
    public List<GameObject> secondSwitchMenues;

    /// <summary>
    /// The extra overlay for the moves cards
    /// </summary>
    public GameObject movesMenues;

    /// <summary>
    /// 
    /// </summary>
    public List<Pokemon> pokemonPlaceholders;

    /// <summary>
    /// The location to copy the "model-script-target" to
    /// </summary>
    public List<GameObject> pokemonModelPlaceholders;

    /// <summary>
    /// The party index of the pokemon to show cards for
    /// </summary>
    public int movesSelectedPokemonIndex;

    /// <summary>
    /// The selected pokemon to show cards for
    /// </summary>
    private Pokemon movesSelectedPokemon
    {
        get
        {
            return deck.party[movesSelectedPokemonIndex];
        }
    }

    /// <summary>
    /// The card locations to populate cards into
    /// </summary>
    public List<GameObject> cardPlaceholders;

    /// <summary>
    /// The button that saves to update the pokemon deck with newDeckList
    /// </summary>
    public GameObject movesSaveButton;

    /// <summary> 
    /// The background for the new deck card names
    /// </summary>
    public List<GameObject> newDeckCardObjects;
    
    /// <summary>
    /// The text to display the selected cards
    /// </summary>
    private List<TextMeshProUGUI> newDeckCardNames;

    /// <summary>
    /// The selected first switch pokemon
    /// </summary>
    public int firstSwitchIndex;

    private List<Transform> previousHudParent = new List<Transform> { null, null, null };

    private List<RectTransform> prevPokemonPlaceholders = new List<RectTransform> { null, null, null };

    private List<Transform> previousModelParent = new List<Transform> { null, null, null };

    private List<Transform> prevPokemonModelPlaceholders = new List<Transform> { null, null, null };

    private List<List<Transform>> prevCardPlaceholders = new List<List<Transform>> { null, null, null };

    private MenuCommon menuCommon;

    /// <summary>
    /// A list of prefabs to instantiate to make the new player deck
    /// </summary>
    private List<Card> newDeckList;

    /// <summary>
    /// A list of instantiated card objects to display
    /// </summary>
    private List<Card> unlockedCards;

    public void Awake()
    {
        menuCommon = gameObject.AddComponent<MenuCommon>();
        menuCommon.Initialize(Close);

        newDeckList = new List<Card>();

        // init card text
        newDeckCardNames = new List<TextMeshProUGUI>();
        newDeckCardObjects.ForEach(o =>
        {
            var textObject = o.transform.Find("return-button/Text (TMP)");
            newDeckCardNames.Add(textObject.GetComponent<TextMeshProUGUI>());
        });
    }

    public void Update()
    {
        // update new deck state
        for (var i = 0; i < newDeckCardObjects.Count; i++)
        {
            var newCard = (i < newDeckList.Count) ? newDeckList[i] : null;
            newDeckCardObjects[i].SetActive(newCard != null);
            newDeckCardNames[i].SetText(newCard != null ? newCard.cardName : "");
        }

        // check save
        movesSaveButton.GetComponent<Button>().interactable = newDeckList.Count == movesSelectedPokemon.GetComponent<PokemonDeckBuildSettings>().deckSize;
    }

    /// <summary>
    /// Opens the party menu
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);

        menuCommon.EnableDialogMode();

        deck = menuCommon.Player.GetComponent<PlayerDeck>();

        SetupPlayer();
    }

    /// <summary>
    /// Closes the party menu
    /// </summary>
    private bool Close()
    {
        // Delete unlocked cards
        for (var i = 0; i < unlockedCards.Count; i++)
        {
            Destroy(unlockedCards[i].gameObject);
        }
        unlockedCards.RemoveAll((c) => { return true; });

        gameObject.SetActive(false);

        menuCommon.PlayCloseSound();
        menuCommon.DisableDialogMode();

        PackupPlayer();

        return false;
    }

    private void SetupPlayer()
    {
        menues.ForEach(m => m.SetActive(false));
        secondSwitchMenues.ForEach(m => m.SetActive(false));
        movesMenues.SetActive(false);
        for (var i = 0; i < deck.party.Count; i++)
        {
            menues[i].SetActive(true);
            deck.party[i].onMenuStart();
            // Set HUD Parent
            prevPokemonPlaceholders[i] = deck.party[i].GetComponent<RectTransform>();
            previousHudParent[i] = deck.party[i].transform.parent;
            // Set HUD Position
            deck.party[i].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            deck.party[i].transform.SetParent(pokemonPlaceholders[i].transform.parent, false);
            deck.party[i].transform.rotation = pokemonPlaceholders[i].transform.rotation;
            deck.party[i].transform.position = pokemonPlaceholders[i].transform.position;
            // Show HUD
            deck.party[i].transform.Find("pokemon-overlay").gameObject.SetActive(true);

            // Set Model Parent
            prevPokemonModelPlaceholders[i] = deck.party[i].pokemonModel.GetComponent<Transform>();
            previousModelParent[i] = deck.party[i].pokemonModel.transform.parent;
            // Set Model Position
            deck.party[i].pokemonModel.transform.SetParent(pokemonModelPlaceholders[i].transform.parent, false);
            deck.party[i].pokemonModel.transform.localScale = pokemonModelPlaceholders[i].transform.localScale;
            deck.party[i].pokemonModel.transform.rotation = pokemonModelPlaceholders[i].transform.rotation;
            deck.party[i].pokemonModel.transform.position = pokemonModelPlaceholders[i].transform.position;
            // Show Model
            SetLayerOnChildren(deck.party[i].pokemonModel, LayerMask.NameToLayer("UI"));
        }
    }

    private void PackupPlayer()
    {
        for (var i = 0; i < deck.party.Count; i++)
        {
            // Set HUD Parent
            deck.party[i].transform.SetParent(previousHudParent[i], false);
            // Set HUD Position
            deck.party[i].transform.rotation = prevPokemonPlaceholders[i].rotation;
            deck.party[i].transform.position = prevPokemonPlaceholders[i].position;
            // Hide HUD
            deck.party[i].transform.Find("pokemon-overlay").gameObject.SetActive(false);

            // Set Model Parent
            deck.party[i].pokemonModel.transform.SetParent(previousModelParent[i], false);
            // Set Model Position
            deck.party[i].pokemonModel.transform.localScale = prevPokemonModelPlaceholders[i].localScale;
            deck.party[i].pokemonModel.transform.rotation = prevPokemonModelPlaceholders[i].rotation;
            deck.party[i].pokemonModel.transform.position = prevPokemonModelPlaceholders[i].position;
            // Hide Model
            SetLayerOnChildren(deck.party[i].pokemonModel, LayerMask.NameToLayer("Default"));

            // Move cards into the slots
            deck.party[i].transform.Find("cards").gameObject.SetActive(false);
            /*
            for (var j = 0; j < deck.party[i].initDeck.Count; j++)
            {
                if (prevCardPlaceholders[i] != null && prevCardPlaceholders[i][j] != null)
                {
                    deck.party[i].initDeck[j].cardInteractEnabled = true;
                    deck.party[i].initDeck[j].transform.rotation = prevCardPlaceholders[i][j].transform.rotation;
                    deck.party[i].initDeck[j].transform.position = prevCardPlaceholders[i][j].transform.position;
                }
            }
            */
        }
    }

    /// <summary>
    /// @see https://forum.unity.com/threads/change-gameobject-layer-at-run-time-wont-apply-to-child.10091/
    /// </summary>
    /// <param name="go"></param>
    /// <param name="layerNumber"></param>
    private void SetLayerOnChildren(GameObject go, int layerNumber)
    {
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    public void OnDetails(int pokemonIndex)
    {
        menuCommon.PlaySelectSound();
    }

    public void OnSwitch(int pokemonIndex)
    {
        menuCommon.PlaySelectSound();

        firstSwitchIndex = pokemonIndex;

        menues.ForEach(m => m.SetActive(false));
        for (var i = 0; i < secondSwitchMenues.Count && i < deck.party.Count; i++)
        {
            if (i != firstSwitchIndex)
            {
                secondSwitchMenues[i].SetActive(true);
            }
        }
    }

    public void OnSwitchSecondClick(int pokemonIndex)
    {
        menuCommon.PlaySelectSound();

        SwitchPokemon(deck.party[firstSwitchIndex], deck.party[pokemonIndex]);

        menues.ForEach(m => m.SetActive(true));
        secondSwitchMenues.ForEach(m => m.SetActive(false));
    }

    /// <summary>
    /// Switches two of the player's pokemon
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    private void SwitchPokemon(Pokemon user, Pokemon target)
    {
        // Switch board roles
        var userIndex = deck.party.IndexOf(user);
        var targetIndex = deck.party.IndexOf(target);
        Swap(deck.party, userIndex, targetIndex);

        // Swap previous data
        Swap(previousHudParent, userIndex, targetIndex);
        Swap(previousModelParent, userIndex, targetIndex);

        // Update model locations to match switch
        Close();
        Open();
    }

    /// <summary>
    /// Swaps two elements of a list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <see cref="https://stackoverflow.com/a/2094316"/>
    /// <param name="list"></param>
    /// <param name="indexA"></param>
    /// <param name="indexB"></param>
    private static void Swap<T>(IList<T> list, int indexA, int indexB)
    {
        (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
    }

    public void OnMoves(int pokemonIndex)
    {
        menuCommon.PlaySelectSound();

        movesSelectedPokemonIndex = pokemonIndex;

        // Remove current menues
        menues.ForEach(m => m.SetActive(false));

        // Show new menu
        movesMenues.SetActive(true);

        // Hide other pokemon
        PackupPlayer();

        // Move this pokemon into the first slot
        var i = movesSelectedPokemonIndex;
        {
            deck.party[i].onMenuStart();
            // Set HUD Parent
            prevPokemonPlaceholders[i] = deck.party[i].GetComponent<RectTransform>();
            previousHudParent[i] = deck.party[i].transform.parent;
            // Set HUD Position
            deck.party[i].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            deck.party[i].transform.SetParent(pokemonPlaceholders[2].transform.parent, false);
            deck.party[i].transform.rotation = pokemonPlaceholders[2].transform.rotation;
            deck.party[i].transform.position = pokemonPlaceholders[2].transform.position;
            // Show HUD
            deck.party[i].transform.Find("pokemon-overlay").gameObject.SetActive(true);

            // Set Model Parent
            prevPokemonModelPlaceholders[i] = deck.party[i].pokemonModel.GetComponent<Transform>();
            previousModelParent[i] = deck.party[i].pokemonModel.transform.parent;
            // Set Model Position
            deck.party[i].pokemonModel.transform.SetParent(pokemonModelPlaceholders[2].transform.parent, false);
            deck.party[i].pokemonModel.transform.rotation = pokemonModelPlaceholders[2].transform.rotation;
            deck.party[i].pokemonModel.transform.position = pokemonModelPlaceholders[2].transform.position;
            // Show Model
            SetLayerOnChildren(deck.party[i].pokemonModel, LayerMask.NameToLayer("UI"));
        }

        // Move cards into the slots
        /*
        deck.party[i].transform.Find("cards").gameObject.SetActive(true);
        prevCardPlaceholders[i] = new List<Transform>();
        for (var j = 0; j < deck.party[i].initDeck.Count; j++)
        {
            prevCardPlaceholders[i].Add(deck.party[i].initDeck[j].transform);

            deck.party[i].initDeck[j].cardInteractEnabled = false;
            deck.party[i].initDeck[j].transform.rotation = cardPlaceholders[j].transform.rotation;
            deck.party[i].initDeck[j].transform.position = cardPlaceholders[j].transform.position;
        }
        */

        // Create unlock cards and move into slots
        deck.party[i].transform.Find("cards").gameObject.SetActive(true);
        var unlockedCardParent = deck.party[i].transform.Find("cards").transform;
        unlockedCards = deck.party[i].GetComponent<PokemonDeckBuildSettings>().InstantiateUnlockedCards(unlockedCardParent, cardPlaceholders[0].transform.position);
        prevCardPlaceholders[i] = new List<Transform>();
        for (var j = 0; j < unlockedCards.Count; j++)
        {
            prevCardPlaceholders[i].Add(unlockedCards[j].transform);

            unlockedCards[j].setupDragDropOverride();
            unlockedCards[j].transform.rotation = cardPlaceholders[j].transform.rotation;
            unlockedCards[j].transform.position = cardPlaceholders[j].transform.position;
        }

        // Init new deck
        newDeckList = deck.party[i].initDeck.Select(c =>
        {
            /*
            var cardPrefab = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(c);
            return cardPrefab;
            */
            return c;
        })
        .ToList();
    }

    /// <summary>
    /// Removes the given card from the list
    /// </summary>
    public void OnCardRemove(int cardIndex)
    {
        // Check if index is valid
        if (cardIndex >= newDeckList.Count) { return; }

        // Remove card
        newDeckList.RemoveAt(cardIndex);
    }

    /// <summary>
    /// Updates the pokemon deck to the new selection
    /// </summary>
    public void OnSave()
    {
        // Check if deck is valid
        if (newDeckList.Count != movesSelectedPokemon.GetComponent<PokemonDeckBuildSettings>().deckSize) { return; }

        // Delete init deck cards
        movesSelectedPokemon.DeleteDeck();

        // For each prefab in new deck list
        for (var i = 0; i < newDeckList.Count; i++)
        {
            // Instatiate object
            movesSelectedPokemon.AddCardToDeck(newDeckList[i]);
        }

        OnReturn();
    }

    public void OnReturn()
    {
        // Close menu
        menuCommon.PlayCloseSound();
        Close();
    }

    /// <summary>
    /// When an object is hovering over a custom target
    /// </summary>
    protected override void OnDrag(Card card, List<DropEvent> _events)
    {
        _events.Where(e => e.eventType == "Deck")
            .ToList()
            .ForEach(e =>
            {
                e.targetAnimator.SetTrigger("onDrag");
            });
    }

    /// <summary>
    /// When an object is hovering over a custom target
    /// </summary>
    public override void OnHoverEnter(HoverAndDragEvent _event)
    {
        if (_event.eventType == "Deck")
        {
            _event.dropEvent.targetAnimator.SetTrigger("onHoverEnter");
            //_event.dropEvent.targetGameObject.GetComponent<Animator>().SetTrigger("onHoverEnter");
        }
    }

    /// <summary>
    /// When an object is exited hovering over a custom target
    /// </summary>
    public override void OnHoverExit(HoverAndDragEvent _event)
    {
        if (_event.eventType == "Deck")
        {
            _event.dropEvent.targetAnimator.SetTrigger("onHoverExit");
            //_event.dropEvent.targetGameObject.GetComponent<Animator>().SetTrigger("onHoverExit");
        }
    }

    /// <summary>
    /// When an object is dropped on a custom target
    /// </summary>
    protected override void OnDrop(HoverAndDragEvent _event, List<DropEvent> _events)
    {
        if (_event.eventType == "Deck" && newDeckList.Count < movesSelectedPokemon.GetComponent<PokemonDeckBuildSettings>().deckSize)
        {
            _event.dropEvent.targetAnimator.SetTrigger("onDropSuccess");
            //var cardPrefab = movesSelectedPokemon.GetComponent<PokemonDeckBuildSettings>().GetPrefabFromCard(_event.targetCard);
            //newDeckList.Add(cardPrefab);
            newDeckList.Add(_event.targetCard);
        }

        // Remove all drop areas
        if (_events != null)
        {
            _events.Where(e => e.eventType == "Deck" && e != _event.dropEvent)
                .ToList()
                .ForEach(e =>
                {
                    e.targetAnimator.SetTrigger("onDropStop");
                });
        }
    }

    protected override void OnDragStop(Card card, List<DropEvent> _events)
    {
        // Remove all drop areas
        if (_events != null)
        {
            _events.Where(e => e.eventType == "Deck")
                .ToList()
                .ForEach(e =>
                {
                    e.targetAnimator.SetTrigger("onDropStop");
                });
        }
    }
}
