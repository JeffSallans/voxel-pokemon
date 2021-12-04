using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCards : MonoBehaviour
{
    public GameObject energy_fire;
    public GameObject energy_lightning;
    public GameObject hand_area;
    public GameObject[] deck;

    // Start is called before the first frame update
    void Start()
    {
        hand_area = GameObject.Find("Player Hand");
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
            GameObject card = Instantiate(deck[Random.Range(0, deck.Length)], new Vector2(0, 0), Quaternion.identity);
            card.transform.SetParent(hand_area.transform, false);
        }
    }

}
