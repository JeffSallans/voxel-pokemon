using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCards : MonoBehaviour
{
    public GameObject energy_fire;
    public GameObject energy_lightning;
    public GameObject hand_area;

    // Start is called before the first frame update
    void Start()
    {
        hand_area = GameObject.Find("Player Hand");
    }

    public void OnClick()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject card = Instantiate(energy_lightning, new Vector2(0, 0), Quaternion.identity);
            card.transform.SetParent(hand_area.transform, false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
