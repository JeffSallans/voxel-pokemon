using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains all possible cards in the game (not including modification variations)
/// </summary>
public class PokemonFactory : MonoBehaviour
{
    public List<Card> cardInventory;

    private List<Energy> energyInventory;

    public List<Pokemon> pokemonInventory;

    // Start is called before the first frame update
    void Start()
    {
        var loadingFactoryObject = GameObject.Find("loading-factory");
        var allPokemon = loadingFactoryObject.GetComponentsInChildren<Pokemon>(true);
        pokemonInventory = allPokemon.ToList();

        var allCards = loadingFactoryObject.transform.Find("card-encyclopedia").GetComponentsInChildren<Card>(true);
        cardInventory = allCards.ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetCard(string cardName)
    {
        var targetCard = cardInventory.Where(item => item.cardName == cardName)
            .Select(item => item.gameObject)
            .Single();
        return Instantiate(targetCard);
    }

    public GameObject GetRandomCard() {
        int index = Mathf.FloorToInt(Random.value * cardInventory.Count);
        return Instantiate(cardInventory[index].gameObject);
    }

    public GameObject GetEnergy(string energyName)
    {
        var targetEnergy = energyInventory.Where(item => item.energyName == energyName)
            .Select(item => item.gameObject)
            .Single();
        return Instantiate(targetEnergy);
    }

    public GameObject GetRandomEnergy() {
        int index = Mathf.FloorToInt(Random.value * energyInventory.Count);
        return Instantiate(energyInventory[index].gameObject);
    }

    public GameObject GetPokemon(string pokemonName, Transform parent = null)
    {
        var targetPokemon = pokemonInventory.Where(item => item.pokemonName == pokemonName)
            .Select(item => item.gameObject)
            .Single();
        return Instantiate(targetPokemon, parent);
    }

    public GameObject GetRandomPokemon(Transform parent = null) {
        int index = Mathf.FloorToInt(Random.value * pokemonInventory.Count);
        return Instantiate(pokemonInventory[index].gameObject, parent);
    }
}
