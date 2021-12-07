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

    private int totalEnergy = 0;

    public GameObject slotLightning;
    public GameObject slotNormal;
    public GameObject uiTurnIndicator;
    public GameObject uiSpiritIndicator;
    public GameObject uiAnnouncer;
    public GameObject hand_area;
    public GameObject energyArea;
    public GameObject[] deckOptions;
    public List<GameObject> deck;

    private GameObject pt_active_energy_lightning;
    private GameObject pt_active_energy_normal;
    private GameObject pt_active_health;
    private GameObject pt_enemy_health;
    private GameObject img_highlight_active;
    private GameObject img_highlight_enemy;

    // Start is called before the first frame update
    void Start()
    {
        pt_active_energy_lightning = GameObject.Find("pt_active_energy_lightning");
        pt_active_energy_normal = GameObject.Find("pt_active_energy_normal");
        pt_active_health = GameObject.Find("pt_active_health");
        pt_enemy_health = GameObject.Find("pt_enemy_health");
        energyArea = GameObject.Find("Energy Display");
        hand_area = GameObject.Find("Player Hand");
        img_highlight_active = GameObject.Find("img_highlight_active");
        img_highlight_enemy = GameObject.Find("img_highlight_enemy");

        StartBattle();
    }

    // Turn functions
    public void NextTurn()
    {
        if (isPlayerTurn)
        {
            isPlayerTurn = false;
            img_highlight_enemy.gameObject.SetActive(true);
            img_highlight_active.gameObject.SetActive(false);
        }
        else
        {
            isPlayerTurn = true;
            img_highlight_enemy.gameObject.SetActive(false);
            img_highlight_active.gameObject.SetActive(true);
            DrawFive();
            turnNumber++;
            if (maxSpiritAmount == 0) { maxSpiritAmount = 1; }
            IncreaseMaxSpirit(1);
            MaximizeSpirit();
        }
        UpdateTextTurn();
        UpdateTextSpirit();
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
            Announcer("The enemy Pidgey took " + damage +  " damage!");
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

    public void Announcer(string announcement)
    {
        Text uiText = uiAnnouncer.GetComponent<Text>();
        uiText.text = announcement;
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
    public void StartBattle()
    {
        DiscardHand();
        DiscardAllEnergy();
        spiritAmount = 2;
        maxSpiritAmount = 2;
        activeHealth = 40;
        enemyHealth = 40;
        isPlayerTurn = true;
        turnNumber = 1;
        lightningEnergy = 0;
        normalEnergy = 0;

        AddMultipleGameObjects(deckOptions[0], 5, deck);
        AddMultipleGameObjects(deckOptions[1], 5, deck);
        AddMultipleGameObjects(deckOptions[2], 5, deck);
        AddMultipleGameObjects(deckOptions[3], 5, deck);
        AddMultipleGameObjects(deckOptions[4], 2, deck);
        RandomizeDeck();
        DrawFive();

        img_highlight_enemy.gameObject.SetActive(false);
        img_highlight_active.gameObject.SetActive(true);

        UpdateTextTurn();
        UpdateTextSpirit();
        UpdateTextHealth();
        Announcer("A wild Pidgey appeared!");
    }
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

    public void DiscardHand()
    {
        while (cardsInHand != 0)
        {
            Destroy(hand_area.transform.GetChild(cardsInHand - 1).gameObject);
            cardsInHand = cardsInHand - 1;
        }
    }

    public void DiscardAllEnergy()
    {
        while (totalEnergy != 0)
        {
            Destroy(energyArea.transform.GetChild(totalEnergy - 1).gameObject);
            totalEnergy = totalEnergy - 1;
        }
        normalEnergy = 0;
        lightningEnergy = 0;
    }

    // Energy functions
    public void AddEnergy(string energy, int amount)
    {
        if (energy == "lightning")
        {
            lightningEnergy = lightningEnergy + amount;
            totalEnergy = totalEnergy + amount;
            GameObject energySlot = Instantiate(slotLightning, new Vector2(0, 0), Quaternion.identity);
            energySlot.transform.SetParent(energyArea.transform, false);
            energySlot.transform.SetSiblingIndex(10);
        }
        else if (energy == "normal")
        {
            normalEnergy = normalEnergy + amount;
            totalEnergy = totalEnergy + amount;
            GameObject energySlot = Instantiate(slotNormal, new Vector2(0, 0), Quaternion.identity);
            energySlot.transform.SetParent(energyArea.transform, false);
            energySlot.transform.SetSiblingIndex(0);
        }
        Announcer("Pikachu is charging up! (+1 " + energy + " energy)");
        UpdateTextEnergy(energy);
    }

    public void SubtractEnergy(string energy, int amount)
    {
        if (energy == "lightning")
        {
            for (int i = 0; i < amount; i++)
            {
                lightningEnergy = lightningEnergy - 1;
                totalEnergy = totalEnergy - amount;
                GameObject obj = energyArea.transform.Find("slot_energy_lightning(Clone)").gameObject;
                Destroy(obj);
            }
        }
        else if (energy == "normal")
        {
            for (int i = 0; i < amount; i++)
            {
                normalEnergy = normalEnergy - 1;
                totalEnergy = totalEnergy - amount;
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
