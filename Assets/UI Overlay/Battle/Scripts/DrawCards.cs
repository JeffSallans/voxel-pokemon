using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : MonoBehaviour
{
    public GameObject energy_fire;
    public GameObject energy_lightning;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClick()
    {
        GameObject card = Instantiate(energy_fire, new Vector2(0, 0), Quaternion.identity);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
