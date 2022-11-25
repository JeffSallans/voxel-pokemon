using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains all pokemon settings of possible cards to get from deck builder event
/// </summary>
public class PokemonDeckBuildSettings : MonoBehaviour
{
    /// <summary>
    /// Size of the pokemon's deck
    /// </summary>
    public int deckSize = 6;

    /// <summary>
    /// Where all the possible cards are stored. Must be an inactive GameObject or the cards will be picked up in other places.
    /// </summary>
    public GameObject cardEncyclopedia = null;

    /// <summary>
    /// The card types that this pokemon can support
    /// </summary>
    public List<string> possibleCardTypes = new List<string>();

    /// <summary>
    /// Possible cards to receive in deck builder
    /// </summary>
    public List<Card> possibleCards = new List<Card>();

    /// <summary>
    /// Unlocked cards to build with
    /// </summary>
    public List<Card> unlockedCards = new List<Card>();

    /// <summary>
    /// The parent to create deck cards under
    /// </summary>
    private Transform canvasParent;

    // Start is called before the first frame update
    void Start()
    {
        canvasParent = gameObject.transform.Find("cards");

        var allCards = GameObject.FindObjectOfType<PokemonFactory>().cardInventory;
        //var allCards = GameObject.Find("loading-factory").GetComponentsInChildren<Card>(true);
        var allowedCards = allCards.Where(c => possibleCardTypes.Any(cardType => cardType == c.cardType));
        possibleCards = allowedCards.ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Creates random cards from the possible list and moves them to drawPos
    /// </summary>
    /// <param name="canvasParent"></param>
    /// <param name="drawPos"></param>
    /// <param name="packSize"></param>
    /// <returns></returns>
    public List<Card> InstantiateRandomPack(Vector3 drawPos, int packSize)
    {
        var result = new List<Card>();

        // Determines the card to instantiate
        Shuffle(possibleCards);

        for (var i = 0; i < packSize && i < possibleCards.Count; i++)
        {
            // Create Card
            var card = Instantiate(possibleCards[i], canvasParent);
            result.Add(card);

            // Position Card
            card.transform.position = drawPos;
        }

        return result;
    }

    /// <summary>
    /// Returns a prefab instance for a given object
    /// </summary>
    /// <param name="cardInstance"></param>
    /// <returns></returns>
    public Card GetPrefabFromCard(Card cardInstance)
    {
        var cardPrefab = possibleCards.Find(c => c.cardName == cardInstance.cardName);
        return cardPrefab;
    }

    /// <summary>
    /// Add a given card
    /// </summary>
    /// <param name="card"></param>
    public void UnlockCard(Card card)
    {
        var cardPrefab = GetPrefabFromCard(card);
        unlockedCards.Add(cardPrefab);
    }

    /// <summary>
    /// Instantiates the unlocked card list to help see them for deck building
    /// </summary>
    /// <param name="parentTransform"></param>
    /// <param name="drawPos"></param>
    /// <returns></returns>
    public List<Card> InstantiateUnlockedCards(Transform parentTransform, Vector3 drawPos)
    {
        var result = new List<Card>();

        for (var i = 0; i < unlockedCards.Count; i++)
        {
            // Create Card
            var card = Instantiate(unlockedCards[i], parentTransform);
            result.Add(card);

            // Position Card
            card.transform.position = drawPos;
        }

        return result;
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
