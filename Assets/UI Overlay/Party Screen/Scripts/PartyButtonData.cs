using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyButtonData : MonoBehaviour
{
    /// <summary>
    /// Index of the pokemon the switch button is for
    /// </summary>
    public int pokemonIndex;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDetails()
    {
        GameObject.FindObjectOfType<PartyMenu>().OnDetails(pokemonIndex);
    }

    public void OnSwitch()
    {
        GameObject.FindObjectOfType<PartyMenu>().OnSwitch(pokemonIndex);
    }

    public void OnSwitchSecondClick()
    {
        GameObject.FindObjectOfType<PartyMenu>().OnSwitchSecondClick(pokemonIndex);
    }

    public void OnMoves()
    {
        GameObject.FindObjectOfType<PartyMenu>().OnMoves(pokemonIndex);
    }
}
