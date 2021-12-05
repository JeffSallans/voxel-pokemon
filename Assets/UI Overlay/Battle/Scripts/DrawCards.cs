using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCards : MonoBehaviour
{
    public GameObject energy_fire;
    public GameObject energy_lightning;
    public GameObject hand_area;
    public GameObject[] deckOptions;
    public List<GameObject> deck; //= new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        hand_area = GameObject.Find("Player Hand");
        AddMultipleGameObjects(deckOptions[0], 5, deck);
        AddMultipleGameObjects(deckOptions[1], 5, deck);
        AddMultipleGameObjects(deckOptions[2], 5, deck);
        AddMultipleGameObjects(deckOptions[3], 5, deck);
        AddMultipleGameObjects(deckOptions[4], 5, deck);
        RandomizeDeck();
        DrawFive();
    }

    public void OnClick()
    {
        DrawFive();
    }

    public void DrawFive()
    {
        for (int i = 0; i < 5; i++)
        {
            //GameObject card = Instantiate(deckOptions[Random.Range(0, deckOptions.Length)], new Vector2(0, 0), Quaternion.identity);
            //card.transform.SetParent(hand_area.transform, false);
            GameObject card = Instantiate(deck[0], new Vector2(0, 0), Quaternion.identity);
            deck.RemoveAt(0);
            card.transform.SetParent(hand_area.transform, false);
        }
    }

    void AddMultipleGameObjects(GameObject objectToAdd, int count, List<GameObject> targetList)
    {
        for (int i = 0; i < count; i++)
        {
            targetList.Add(objectToAdd);
        }
    }

    public void RandomizeDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            GameObject temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

}
