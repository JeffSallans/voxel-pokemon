using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{
    ptDisplayElectricEnergy ptDisplay;
    ControllerScript controllerScript;

    public GameObject canvas;
    public GameObject controller;
    public GameObject handArea;

    public int costSpiritAmount;
    public int costEnergy1Amount;
    public string costEnergy1Type;
    public int costEnergy2Amount;
    public string costEnergy2Type;

    private bool isPlayable = true;
    private bool isDragging = false;
    private GameObject dropzone;
    private GameObject startParent;
    private Vector2 startPos;
    private bool isOverDropZone = false;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Main Canvas");
        controller = GameObject.Find("Controller");
        controllerScript = controller.GetComponent<ControllerScript>();
    }

    void PlayCard()
    {
        if (this.gameObject.name == "card_energy_normal(Clone)")
        {
            controllerScript.AddEnergy("normal", 1);
        }
        else if (this.gameObject.name == "card_energy_lightning(Clone)")
        {
            controllerScript.AddEnergy("lightning", 1);
        }
        else if (this.gameObject.name == "card_attack_gnaw(Clone)")
        {
            controllerScript.Attack(10);
        }
        else if (this.gameObject.name == "card_attack_thunderjolt(Clone)")
        {
            controllerScript.Attack(20);
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
            if (costEnergy1Type == "normal")
            {
                if (costEnergy1Amount > controllerScript.normalEnergy) { isPlayable = false; }
            }
            else if (costEnergy1Type == "lightning")
            {
                if (costEnergy1Amount > controllerScript.lightningEnergy) { isPlayable = false; }
            }
        }

        if (costEnergy2Type != null)
        {
            if (costEnergy2Type == "normal")
            {
                if (costEnergy2Amount > controllerScript.normalEnergy) { isPlayable = false; }
            }
            else if (costEnergy2Type == "lightning")
            {
                if (costEnergy2Amount > controllerScript.lightningEnergy) { isPlayable = false; }
            }
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
