using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMenu : MonoBehaviour
{
    PlayerDeck player;

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
    /// The card locations to populate cards into
    /// </summary>
    public List<GameObject> cardPlaceholders;

    /// <summary>
    /// The selected first switch pokemon
    /// </summary>
    public int firstSwitchIndex;

    private List<Transform> previousHudParent = new List<Transform> { null, null, null };

    private List<RectTransform> prevPokemonPlaceholders = new List<RectTransform> { null, null, null };

    private List<Transform> previousModelParent = new List<Transform> { null, null, null };

    private List<Transform> prevPokemonModelPlaceholders = new List<Transform> { null, null, null };

    private List<List<Transform>> prevCardPlaceholders = new List<List<Transform>> { null, null, null };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Opens the pause menu
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);
        GameObject.FindObjectOfType<OverlayCursor>().showCursor = true;
        GameObject.FindObjectOfType<CinemachineFreeLook>().enabled = false;
        var playerObject = GameObject.FindObjectOfType<CharacterController>().gameObject;
        playerObject.GetComponent<InteractionChecker>().enabled = false;
        playerObject.GetComponent<FallToGround>().enabled = false;
        playerObject.GetComponent<CharacterController>().enabled = false;
        playerObject.GetComponent<ThirdPersonMovement>().enabled = false;
        playerObject.GetComponent<AnimationExample>().enabled = false;

        player = playerObject.GetComponent<PlayerDeck>();

        SetupPlayer();
    }

    /// <summary>
    /// Closes the pause menu
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
        GameObject.FindObjectOfType<OverlayCursor>().showCursor = false;
        GameObject.FindObjectOfType<CinemachineFreeLook>().enabled = true;
        var playerObject = GameObject.FindObjectOfType<CharacterController>().gameObject;
        playerObject.GetComponent<InteractionChecker>().enabled = true;
        playerObject.GetComponent<FallToGround>().enabled = true;
        playerObject.GetComponent<CharacterController>().enabled = true;
        playerObject.GetComponent<ThirdPersonMovement>().enabled = true;
        playerObject.GetComponent<AnimationExample>().enabled = true;

        PackupPlayer();
    }

    private void SetupPlayer()
    {
        menues.ForEach(m => m.SetActive(false));
        secondSwitchMenues.ForEach(m => m.SetActive(false));
        movesMenues.SetActive(false);
        for (var i = 0; i < player.party.Count; i++)
        {
            menues[i].SetActive(true);
            player.party[i].onMenuStart();
            // Set HUD Parent
            prevPokemonPlaceholders[i] = player.party[i].GetComponent<RectTransform>();
            previousHudParent[i] = player.party[i].transform.parent;
            // Set HUD Position
            player.party[i].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            player.party[i].transform.SetParent(pokemonPlaceholders[i].transform.parent, false);
            player.party[i].transform.rotation = pokemonPlaceholders[i].transform.rotation;
            player.party[i].transform.position = pokemonPlaceholders[i].transform.position;
            // Show HUD
            player.party[i].transform.Find("pokemon-overlay").gameObject.SetActive(true);

            // Set Model Parent
            prevPokemonModelPlaceholders[i] = player.party[i].pokemonModel.GetComponent<Transform>();
            previousModelParent[i] = player.party[i].pokemonModel.transform.parent;
            // Set Model Position
            player.party[i].pokemonModel.transform.SetParent(pokemonModelPlaceholders[i].transform.parent, false);
            player.party[i].pokemonModel.transform.localScale = pokemonModelPlaceholders[i].transform.localScale;
            player.party[i].pokemonModel.transform.rotation = pokemonModelPlaceholders[i].transform.rotation;
            player.party[i].pokemonModel.transform.position = pokemonModelPlaceholders[i].transform.position;
            // Show Model
            SetLayerOnChildren(player.party[i].pokemonModel, LayerMask.NameToLayer("UI"));
        }
    }

    private void PackupPlayer()
    {
        for (var i = 0; i < player.party.Count; i++)
        {
            // Set HUD Parent
            player.party[i].transform.SetParent(previousHudParent[i], false);
            // Set HUD Position
            player.party[i].transform.rotation = prevPokemonPlaceholders[i].rotation;
            player.party[i].transform.position = prevPokemonPlaceholders[i].position;
            // Hide HUD
            player.party[i].transform.Find("pokemon-overlay").gameObject.SetActive(false);

            // Set Model Parent
            player.party[i].pokemonModel.transform.SetParent(previousModelParent[i], false);
            // Set Model Position
            player.party[i].pokemonModel.transform.localScale = prevPokemonModelPlaceholders[i].localScale;
            player.party[i].pokemonModel.transform.rotation = prevPokemonModelPlaceholders[i].rotation;
            player.party[i].pokemonModel.transform.position = prevPokemonModelPlaceholders[i].position;
            // Hide Model
            SetLayerOnChildren(player.party[i].pokemonModel, LayerMask.NameToLayer("Default"));

            // Move cards into the slots
            player.party[i].transform.Find("cards").gameObject.SetActive(false);
            for (var j = 0; j < player.party[i].initDeck.Count; j++)
            {
                if (prevCardPlaceholders[i] != null && prevCardPlaceholders[i][j] != null)
                {
                    player.party[i].initDeck[j].cardInteractEnabled = true;
                    player.party[i].initDeck[j].transform.rotation = prevCardPlaceholders[i][j].transform.rotation;
                    player.party[i].initDeck[j].transform.position = prevCardPlaceholders[i][j].transform.position;
                }
            }
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

    }

    public void OnSwitch(int pokemonIndex)
    {
        firstSwitchIndex = pokemonIndex;

        menues.ForEach(m => m.SetActive(false));
        for (var i = 0; i < secondSwitchMenues.Count && i < player.party.Count; i++)
        {
            if (i != firstSwitchIndex)
            {
                secondSwitchMenues[i].SetActive(true);
            }
        }
    }

    public void OnSwitchSecondClick(int pokemonIndex)
    {
        SwitchPokemon(player.party[firstSwitchIndex], player.party[pokemonIndex]);

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
        var userIndex = player.party.IndexOf(user);
        var targetIndex = player.party.IndexOf(target);
        Swap(player.party, userIndex, targetIndex);

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
    protected void Swap<T>(IList<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }

    public void OnMoves(int pokemonIndex)
    {
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
            player.party[i].onMenuStart();
            // Set HUD Parent
            prevPokemonPlaceholders[i] = player.party[i].GetComponent<RectTransform>();
            previousHudParent[i] = player.party[i].transform.parent;
            // Set HUD Position
            player.party[i].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            player.party[i].transform.SetParent(pokemonPlaceholders[2].transform.parent, false);
            player.party[i].transform.rotation = pokemonPlaceholders[2].transform.rotation;
            player.party[i].transform.position = pokemonPlaceholders[2].transform.position;
            // Show HUD
            player.party[i].transform.Find("pokemon-overlay").gameObject.SetActive(true);

            // Set Model Parent
            prevPokemonModelPlaceholders[i] = player.party[i].pokemonModel.GetComponent<Transform>();
            previousModelParent[i] = player.party[i].pokemonModel.transform.parent;
            // Set Model Position
            player.party[i].pokemonModel.transform.SetParent(pokemonModelPlaceholders[2].transform.parent, false);
            player.party[i].pokemonModel.transform.rotation = pokemonModelPlaceholders[2].transform.rotation;
            player.party[i].pokemonModel.transform.position = pokemonModelPlaceholders[2].transform.position;
            // Show Model
            SetLayerOnChildren(player.party[i].pokemonModel, LayerMask.NameToLayer("UI"));
        }

        // Move cards into the slots
        player.party[i].transform.Find("cards").gameObject.SetActive(true);
        prevCardPlaceholders[i] = new List<Transform>();
        for (var j = 0; j < player.party[i].initDeck.Count; j++)
        {
            prevCardPlaceholders[i].Add(player.party[i].initDeck[j].transform);

            player.party[i].initDeck[j].cardInteractEnabled = false;
            player.party[i].initDeck[j].transform.rotation = cardPlaceholders[j].transform.rotation;
            player.party[i].initDeck[j].transform.position = cardPlaceholders[j].transform.position;
        }
    }

    public void OnReturn()
    {
        Close();
    }
}
