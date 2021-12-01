using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains all possible cards in the game (not including modification variations)
/// </summary>
public class CardInventory : MonoBehaviour
{
    public List<Card> cardInventory;

    public List<Energy> energyInventory;

    public List<Pokemon> pokemonInventory;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject getCard(string cardName)
    {
        var targetCard = cardInventory.Where(item => item.cardName == cardName)
            .Select(item => item.gameObject)
            .Single();
        return Instantiate(targetCard);
    }

    public GameObject getRandomCard() {
        int index = Mathf.FloorToInt(Random.value * cardInventory.Count);
        return Instantiate(cardInventory[index].gameObject);
    }

    public GameObject getEnergy(string energyName)
    {
        var targetEnergy = energyInventory.Where(item => item.energyName == energyName)
            .Select(item => item.gameObject)
            .Single();
        return Instantiate(targetEnergy);
    }

    public GameObject getRandomEnergy() {
        int index = Mathf.FloorToInt(Random.value * energyInventory.Count);
        return Instantiate(energyInventory[index].gameObject);
    }

    public GameObject getPokemon(string pokemonName)
    {
        var targetPokemon = pokemonInventory.Where(item => item.pokemonName == pokemonName)
            .Select(item => item.gameObject)
            .Single();
        return Instantiate(targetPokemon);
    }

    public GameObject getRandomPokemon() {
        int index = Mathf.FloorToInt(Random.value * pokemonInventory.Count);
        return Instantiate(pokemonInventory[index].gameObject);
    }
}
