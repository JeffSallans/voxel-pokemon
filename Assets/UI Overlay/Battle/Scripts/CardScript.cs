using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{
    ptDisplayElectricEnergy ptDisplay;
    ControllerScript controllerScript;

    public string cardName;
    public string cardType;
    public string energyType;
    public int attackPower;
    public string specialEffect;
    public int spareVariable1;
    public int spareVariable2;
    public int costSpiritAmount;
    public int costEnergy1Amount;
    public string costEnergy1Type;
    public int costEnergy2Amount;
    public string costEnergy2Type;

    private bool isZoomed = false;
    private bool isPlayable = true;
    private bool isDragging = false;
    private GameObject startParent;
    private GameObject canvas;
    private GameObject controller;
    private GameObject dropzone;
    private GameObject txtName;
    private GameObject txtCardType;
    private GameObject txtSpirit;
    private GameObject txtEnergyCost;
    private GameObject txtDamage;
    private Vector2 startPos;
    private bool isOverDropZone = false;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Main Canvas");
        controller = GameObject.Find("Controller");
        controllerScript = controller.GetComponent<ControllerScript>();
        if (cardType == "Attack")
        {
            txtName = transform.Find("txtName").gameObject;
            txtCardType = transform.Find("txtCardType").gameObject;
            txtSpirit = transform.Find("txtSpirit").gameObject;
            txtEnergyCost = transform.Find("txtEnergyCost").gameObject;
            txtDamage = transform.Find("txtDamage").gameObject;
            SetCardText();
        }
    }

    void PlayCard()
    {
        if (cardType == "Energy")
        {
            controllerScript.AddEnergy(energyType, 1);
        }
        else if (cardType == "Attack")
        {
            controllerScript.Attack(attackPower);
        }

        // Spend resources
        if (costEnergy1Type != null)
        {
            controllerScript.SubtractEnergy(costEnergy1Type, costEnergy1Amount);
        }
        if (costEnergy2Type != null)
        {
            controllerScript.SubtractEnergy(costEnergy2Type, costEnergy2Amount);
        }
        controllerScript.SubtractSpirit(costSpiritAmount);
        controllerScript.cardsInHand = controllerScript.cardsInHand - 1;
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        isOverDropZone = true;
        dropzone = collision.gameObject;

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        isOverDropZone = false;
        dropzone = null;
    }
    public void StartDrag()
    {
        isDragging = true;
        startParent = transform.parent.gameObject;
        startPos = transform.position;
        isPlayable = true;
        ResourceCheck();
    }

    public void EndDrag()
    {
        isDragging = false;
        if (isOverDropZone && isPlayable)
        {
            PlayCard();
        }
        else
        {
            transform.position = startPos;
            transform.SetParent(startParent.transform, false);
        }
    }

    void ResourceCheck()
    {
        if (costSpiritAmount > controllerScript.spiritAmount)
        {
            isPlayable = false;
        }

        if (costEnergy1Type != null)
        {
            if (costEnergy1Type == "Normal")
            {
                if (costEnergy1Amount > controllerScript.normalEnergy) { isPlayable = false; }
            }
            else if (costEnergy1Type == "Lightning")
            {
                if (costEnergy1Amount > controllerScript.lightningEnergy) { isPlayable = false; }
            }
        }

        if (costEnergy2Type != null)
        {
            if (costEnergy2Type == "Normal")
            {
                if (costEnergy2Amount > controllerScript.normalEnergy) { isPlayable = false; }
            }
            else if (costEnergy2Type == "Lightning")
            {
                if (costEnergy2Amount > controllerScript.lightningEnergy) { isPlayable = false; }
            }
        }
    }

    void SetCardText()
    {
        Text uiText = txtName.GetComponent<Text>();
        uiText.text = cardName;
        uiText = txtCardType.GetComponent<Text>();
        uiText.text = cardType;
        uiText = txtSpirit.GetComponent<Text>();
        uiText.text = costSpiritAmount.ToString();
        uiText = txtEnergyCost.GetComponent<Text>();
        uiText.text = costEnergy1Type + ": " + costEnergy1Amount;
        if (costEnergy2Type != "") { string text = txtEnergyCost.GetComponent<Text>().text; ; uiText.text = text + "\n" + costEnergy2Type + ": " + costEnergy2Amount; }
        uiText = txtDamage.GetComponent<Text>();
        uiText.text = attackPower.ToString();
    }
    
    public void OnHoverEnter()
    {
        if (!isZoomed)
        {
            transform.localScale *= 2f;
            isZoomed = true;
        }
    }

    public void OnHeaverExit()
    {
        if (isZoomed)
        {
            transform.localScale *= .5f;
            isZoomed = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(canvas.transform, true);
        }
        
    }
}
