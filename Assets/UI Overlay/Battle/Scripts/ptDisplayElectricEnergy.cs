using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ptDisplayElectricEnergy : MonoBehaviour
{
    public int energyAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void UpdateText()
    {
        Text uiText = GetComponent<Text>();
        uiText.text = "Energy: " + energyAmount + "";
    }
    public void AddEnergy()
    {
        energyAmount++;
        UpdateText();
    }

    public void SubtractEnergy()
    {
        energyAmount = energyAmount - 1;
        if (energyAmount < 0)
        {
            energyAmount = 0;
        }
        UpdateText();
    }
}
