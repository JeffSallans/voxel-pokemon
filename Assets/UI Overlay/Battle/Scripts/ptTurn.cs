using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ptTurn : MonoBehaviour
{
    public int turnNumber;
    public bool isPlayerTurn;
    // Start is called before the first frame update
    void Start()
    {
        isPlayerTurn = true;
        turnNumber = 1;
        UpdateText();
    }

    public void NextTurn()
    {
        if (isPlayerTurn)
        {
            isPlayerTurn = false;
        }
        else
        {
            isPlayerTurn = true;
            turnNumber = turnNumber + 1;
        }
        UpdateText();
    }
    void UpdateText()
    {
        if (isPlayerTurn == true)
        {
            Text uiText = GetComponent<Text>();
            uiText.text = "Your turn " + turnNumber + "";
        }
        else
        {
            Text uiText = GetComponent<Text>();
            uiText.text = "Enemy turn " + turnNumber + "";
        }
            
    }
}
