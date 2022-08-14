using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMenu : MonoBehaviour
{
    PlayerDeck player;

    public List<GameObject> menues;

    public List<GameObject> secondSwitchMenues;

    public List<Pokemon> pokemonPlaceholders;

    public List<GameObject> pokemonModelPlaceholders;

    public int firstSwitchIndex;

    private List<Transform> previousHudParent = new List<Transform> { null, null, null };

    private List<Transform> prevPokemonPlaceholders = new List<Transform> { null, null, null };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        for (var i = 0; i < player.party.Count; i++)
        {
            menues[i].SetActive(true);
            player.party[i].onMenuStart();

            prevPokemonPlaceholders[i] = player.party[i].transform;
            previousHudParent[i] = player.party[i].transform.parent;

            player.party[i].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            player.party[i].transform.parent = pokemonPlaceholders[i].transform.parent;

            player.party[i].transform.rotation = pokemonPlaceholders[i].transform.rotation;
            player.party[i].transform.position = pokemonPlaceholders[i].transform.position;

            player.party[i].pokemonRootModel.transform.rotation = pokemonModelPlaceholders[i].transform.rotation;
            player.party[i].pokemonRootModel.transform.position = pokemonModelPlaceholders[i].transform.position;

            player.party[i].gameObject.transform.Find("pokemon-overlay").gameObject.SetActive(true);
        }
    }

    private void PackupPlayer()
    {
        for (var i = 0; i < player.party.Count; i++)
        {
            player.party[i].transform.parent = previousHudParent[i];
            player.party[i].transform.localScale = prevPokemonPlaceholders[i].localScale;
            player.party[i].transform.rotation = prevPokemonPlaceholders[i].rotation;
            player.party[i].transform.position = prevPokemonPlaceholders[i].position;

            player.party[i].gameObject.transform.Find("pokemon-overlay").gameObject.SetActive(false);
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

        // Update model locations to match switch
        var i = 0;
        player.party.ForEach(p =>
        {
            p.setPlacement(pokemonPlaceholders[i], pokemonModelPlaceholders[i]);
            i++;
        });
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

    }

    public void OnReturn()
    {
        Close();
    }
}
