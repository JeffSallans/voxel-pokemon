using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerScript : MonoBehaviour
{
    public int activeHealth;
    public int enemyHealth;
    public int lightningEnergy = 0;
    public int normalEnergy = 0;
    public int spiritAmount;
    public int maxSpiritAmount;
    public int turnNumber;
    public bool isPlayerTurn;
    public int cardsInHand = 0;

    public GameObject slotLightning;
    public GameObject slotNormal;
    public GameObject uiTurnIndicator;
    public GameObject uiSpiritIndicator;
    public GameObject hand_area;
    public GameObject energyArea;
    public GameObject[] deckOptions;
    public List<GameObject> deck;

    private GameObject pt_active_energy_lightning;
    private GameObject pt_active_energy_normal;
    private GameObject pt_active_health;
    private GameObject pt_enemy_health;

    // Start is called before the first frame update
    void Start()
    {
        spiritAmount = 2;
        maxSpiritAmount = 2;
        activeHealth = 40;
        enemyHealth = 40;
        isPlayerTurn = true;
        turnNumber = 1;

        AddMultipleGameObjects(deckOptions[0], 5, deck);
        AddMultipleGameObjects(deckOptions[1], 5, deck);
        AddMultipleGameObjects(deckOptions[2], 5, deck);
        AddMultipleGameObjects(deckOptions[3], 5, deck);
        AddMultipleGameObjects(deckOptions[4], 2, deck);
        RandomizeDeck();
        DrawFive();

        pt_active_energy_lightning = GameObject.Find("pt_active_energy_lightning");
        pt_active_energy_normal = GameObject.Find("pt_active_energy_normal");
        pt_active_health = GameObject.Find("pt_active_health");
        pt_enemy_health = GameObject.Find("pt_enemy_health");
        energyArea = GameObject.Find("Energy Display");

        UpdateTextTurn();
        UpdateTextSpirit();
        UpdateTextHealth();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Turn functions
    public void NextTurn()
    {
        if (isPlayerTurn)
        {
            isPlayerTurn = false;
        }
        else
        {
            isPlayerTurn = true;
            turnNumber++;
            if (maxSpiritAmount == 0) { maxSpiritAmount = 1; }
            IncreaseMaxSpirit(1);
            MaximizeSpirit();
        }
        UpdateTextTurn();
    }

    void UpdateTextTurn()
    {
        if (isPlayerTurn == true)
        {
            Text uiText = uiTurnIndicator.GetComponent<Text>();
            uiText.text = "Your turn " + turnNumber + "";
        }
        else
        {
            Text uiText = uiTurnIndicator.GetComponent<Text>();
            uiText.text = "Enemy turn " + turnNumber + "";
        }

    }

    // Battle functions

    public void Attack(int damage)
    {
        if (isPlayerTurn)
        {
            enemyHealth = enemyHealth - damage;
        }
        else if (isPlayerTurn == false)
        {
            activeHealth = activeHealth - damage;
        }
        UpdateTextHealth();
    }

    void UpdateTextHealth()
    {
        Text uiText = pt_enemy_health.GetComponent<Text>();
        uiText.text = "Health: " + enemyHealth + "";
        uiText = pt_active_health.GetComponent<Text>();
        uiText.text = "Health: " + activeHealth + "";
    }

    // Spirit functions
    public void UpdateTextSpirit()
    {
        Text uiText = uiSpiritIndicator.GetComponent<Text>();
        uiText.text = "Spirit: " + spiritAmount + " / " + maxSpiritAmount + "";
    }

    public void AddSpirit(int amount)
    {
        spiritAmount = spiritAmount + amount;
        UpdateTextSpirit();
    }

    public void SubtractSpirit(int amount)
    {
        spiritAmount = spiritAmount - amount;
        if (spiritAmount < 0)
        {
            spiritAmount = 0;
        }
        UpdateTextSpirit();
    }

    public void IncreaseMaxSpirit(int amount)
    {
        maxSpiritAmount = maxSpiritAmount + amount;
    }

    public void MaximizeSpirit()
    {
        spiritAmount = maxSpiritAmount;
    }

    // Deck and hand functions
    public void DrawFive()
    {
        for (int i = cardsInHand; i < 5; i++)
        {
            GameObject card = Instantiate(deck[0], new Vector2(0, 0), Quaternion.identity);
            deck.RemoveAt(0);
            card.transform.SetParent(hand_area.transform, false);
            cardsInHand++;
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

    // Energy functions
    public void AddEnergy(string energy, int amount)
    {
        if (energy == "lightning")
        {
            lightningEnergy = lightningEnergy + amount;
            GameObject energySlot = Instantiate(slotLightning, new Vector2(0, 0), Quaternion.identity);
            energySlot.transform.SetParent(energyArea.transform, false);
            energySlot.transform.SetSiblingIndex(10);
        }
        else if (energy == "normal")
        {
            normalEnergy = normalEnergy + amount;
            GameObject energySlot = Instantiate(slotNormal, new Vector2(0, 0), Quaternion.identity);
            energySlot.transform.SetParent(energyArea.transform, false);
            energySlot.transform.SetSiblingIndex(0);
        }
        UpdateTextEnergy(energy);
    }

    public void SubtractEnergy(string energy, int amount)
    {
        if (energy == "lightning")
        {
            for (int i = 0; i < amount; i++)
            {
                lightningEnergy = lightningEnergy - 1;
                GameObject obj = energyArea.transform.Find("slot_energy_lightning(Clone)").gameObject;
                Destroy(obj);
            }
        }
        else if (energy == "normal")
        {
            for (int i = 0; i < amount; i++)
            {
                normalEnergy = normalEnergy - 1;
                GameObject obj = energyArea.transform.Find("slot_energy_normal(Clone)").gameObject;
                Destroy(obj);
            }
        }
        UpdateTextEnergy(energy);
    }

    void UpdateTextEnergy(string energy)
    {
        if (energy == "lightning")
        {
            Text uiText = pt_active_energy_lightning.GetComponent<Text>();
            uiText.text = "Energy: " + lightningEnergy + "";
        }
        else if (energy == "normal")
        {
            Text uiText = pt_active_energy_normal.GetComponent<Text>();
            uiText.text = "Energy: " + normalEnergy + "";
        }
    }
}
