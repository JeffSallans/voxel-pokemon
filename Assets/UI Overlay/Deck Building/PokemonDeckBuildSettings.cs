using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all pokemon settings of possible cards to get from deck builder event
/// </summary>
public class PokemonDeckBuildSettings : MonoBehaviour
{
    /// <summary>
    /// Possible cards to receive in deck builder
    /// </summary>
    public List<Card> possibleCards = new List<Card>();

    /// <summary>
    /// The parent to create cards under
    /// </summary>
    private Transform canvasParent;

    // Start is called before the first frame update
    void Start()
    {
        canvasParent = gameObject.transform.Find("cards");
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
