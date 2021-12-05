using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ptSpirit : MonoBehaviour
{
    ptTurn ptTurnScript;
    private GameObject ptTurnObject;

    public int spiritAmount;
    public int maxSpiritAmount;

    // Start is called before the first frame update
    void Start()
    {
        ptTurnObject = GameObject.Find("pt_turn_indicator");
        ptTurnScript = ptTurnObject.GetComponent<ptTurn>();
        spiritAmount = 1;
        maxSpiritAmount = 1;
        UpdateText();
    }

    public void UpdateText()
    {
        maxSpiritAmount = ptTurnScript.turnNumber;
        if(maxSpiritAmount == 0) { maxSpiritAmount = 1; }
        Text uiText = GetComponent<Text>();
        uiText.text = "Spirit: " + spiritAmount + " / " + maxSpiritAmount + "";
    }

    public void AddSpirit(int amount)
    {
        spiritAmount = spiritAmount + amount;
        UpdateText();
    }

    public void SubtractSpirit(int amount)
    {
        spiritAmount = spiritAmount - amount;
        if (spiritAmount < 0)
        {
            spiritAmount = 0;
        }
        UpdateText();
    }

    public void IncreaseMaxSpirit()
    {
        maxSpiritAmount++;
    }
}
