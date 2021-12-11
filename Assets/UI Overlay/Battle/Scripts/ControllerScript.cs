using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerScript : MonoBehaviour
{
    public int lightningEnergy = 0;
    public int normalEnergy = 0;
    public int spiritAmount;
    public int maxSpiritAmount;
    public int turnNumber;
    public bool isPlayerTurn;
    public bool discardUsed;
    public int cardsInHand = 0;

    //private int actHealth;
    //private int actLevel;
    //private int actAttack;
    //private int actDefense;
    //private int actSAttack;
    //private int actSDefense;
    //private int actSpeed;
    //public int eneHealth;
    //public int eneLevel;
    private int totalEnergy = 0;
    private string enemyIntent;

    public GameObject slotLightning;
    public GameObject slotNormal;
    public GameObject uiTurnIndicator;
    public GameObject uiSpiritIndicator;
    public GameObject uiAnnouncer;
    public GameObject hand_area;
    public GameObject energyArea;
    public GameObject ZoomPlaceholderCard;
    public GameObject[] deckOptions;
    public List<GameObject> deck;

    private GameObject placeholder;
    private GameObject pt_intent;
    private GameObject pt_active_energy_lightning;
    private GameObject pt_active_energy_normal;
    private GameObject pt_active_health;
    private GameObject pt_enemy_health;
    private GameObject img_highlight_active;
    private GameObject img_highlight_enemy;
    private GameObject inp_pokemon1GO;
    private GameObject inp_pokemon2GO;
    //private InputField inp_pokemon1CO;
    //private InputField inp_pokemon2CO;

    private List<int> actBS;
    private List<int> eneBS;
    private List<int> actMS;
    private List<int> eneMS;
    private List<int> actCS;
    private List<int> eneCS;

    // Start is called before the first frame update
    void Start()
    {
        pt_active_energy_lightning = GameObject.Find("pt_active_energy_lightning");
        pt_active_energy_normal = GameObject.Find("pt_active_energy_normal");
        pt_active_health = GameObject.Find("pt_active_health");
        pt_enemy_health = GameObject.Find("pt_enemy_health");
        energyArea = GameObject.Find("Energy Display");
        hand_area = GameObject.Find("Player Hand");
        pt_intent = GameObject.Find("pt_intent");
        img_highlight_active = GameObject.Find("img_highlight_active");
        img_highlight_enemy = GameObject.Find("img_highlight_enemy");
        inp_pokemon1GO = GameObject.Find("inp_pokemon1");
        inp_pokemon1GO = inp_pokemon1GO.transform.GetChild(1).gameObject;
        inp_pokemon2GO = GameObject.Find("inp_pokemon2");
        inp_pokemon2GO = inp_pokemon2GO.transform.GetChild(1).gameObject;
        actBS = new List<int> { 35, 55, 40, 50, 50, 90, 5 };
        eneBS = new List<int> { 40, 45, 40, 35, 35, 56, 5 };
        actMS = new List<int> { 0, 0, 0, 0, 0, 0 };
        eneMS = new List<int> { 0, 0, 0, 0, 0, 0 };
        actCS = new List<int> { 0, 0, 0, 0, 0, 0 };
        eneCS = new List<int> { 0, 0, 0, 0, 0, 0 };
        StartBattle();
    }

    public void StartBattle()
    {
        CalculateStats();
        DiscardHand();
        DiscardAllEnergy();
        spiritAmount = 2;
        maxSpiritAmount = 2;
        isPlayerTurn = true;
        turnNumber = 1;
        lightningEnergy = 0;
        normalEnergy = 0;

        AddMultipleGameObjects(deckOptions[0], 3, deck);
        AddMultipleGameObjects(deckOptions[1], 3, deck);
        AddMultipleGameObjects(deckOptions[2], 6, deck);
        AddMultipleGameObjects(deckOptions[3], 6, deck);
        AddMultipleGameObjects(deckOptions[4], 3, deck);
        AddMultipleGameObjects(deckOptions[5], 3, deck);
        RandomizeDeck();
        DrawFive();

        img_highlight_enemy.gameObject.SetActive(false);
        img_highlight_active.gameObject.SetActive(true);

        UpdateTextTurn();
        UpdateTextSpirit();
        UpdateTextHealth();
        Announcer("A wild Pidgey appeared!", false);
        NewEnemyIntent();
    }

    private void CalculateStats()
    {
        actBS[6] = int.Parse(inp_pokemon1GO.GetComponent<Text>().text);
        eneBS[6] = int.Parse(inp_pokemon2GO.GetComponent<Text>().text);

        actMS[0] = ((2 * actBS[0] + 31) * actBS[6])/100 + actBS[6] + 10;
        actMS[1] = ((2 * actBS[1] + 31) * actBS[6])/ 100 + 5;
        actMS[2] = ((2 * actBS[2] + 31) * actBS[6]) / 100 + 5;
        actMS[3] = ((2 * actBS[3] + 31) * actBS[6]) / 100 + 5;
        actMS[4] = ((2 * actBS[4] + 31) * actBS[6]) / 100 + 5;
        actMS[5] = ((2 * actBS[5] + 31) * actBS[6]) / 100 + 5;

        eneMS[0] = ((((2 * eneBS[0] + 31) * eneBS[6]) / 100) + eneBS[6] + 10);
        eneMS[1] = ((2 * eneBS[1] + 31) * eneBS[6]) / 100 + 5;
        eneMS[2] = ((2 * eneBS[2] + 31) * eneBS[6]) / 100 + 5;
        eneMS[3] = ((2 * eneBS[3] + 31) * eneBS[6]) / 100 + 5;
        eneMS[4] = ((2 * eneBS[4] + 31) * eneBS[6]) / 100 + 5;
        eneMS[5] = ((2 * eneBS[5] + 31) * eneBS[6]) / 100 + 5;

        actCS[0] = ((2 * actBS[0] + 31) * actBS[6]) / 100 + actBS[6] + 10;
        actCS[1] = ((2 * actBS[1] + 31) * actBS[6]) / 100 + 5;
        actCS[2] = ((2 * actBS[2] + 31) * actBS[6]) / 100 + 5;
        actCS[3] = ((2 * actBS[3] + 31) * actBS[6]) / 100 + 5;
        actCS[4] = ((2 * actBS[4] + 31) * actBS[6]) / 100 + 5;
        actCS[5] = ((2 * actBS[5] + 31) * actBS[6]) / 100 + 5;

        eneCS[0] = ((((2 * eneBS[0] + 31) * eneBS[6]) / 100) + eneBS[6] + 10);
        eneCS[1] = ((2 * eneBS[1] + 31) * eneBS[6]) / 100 + 5;
        eneCS[2] = ((2 * eneBS[2] + 31) * eneBS[6]) / 100 + 5;
        eneCS[3] = ((2 * eneBS[3] + 31) * eneBS[6]) / 100 + 5;
        eneCS[4] = ((2 * eneBS[4] + 31) * eneBS[6]) / 100 + 5;
        eneCS[5] = ((2 * eneBS[5] + 31) * eneBS[6]) / 100 + 5;
    }

    // Turn functions
    public void NextTurn()
    {
        if (isPlayerTurn)
        {
            isPlayerTurn = false;
            img_highlight_enemy.gameObject.SetActive(true);
            img_highlight_active.gameObject.SetActive(false);
            EnemyTurn();
        }
        else
        {
            isPlayerTurn = true;
            discardUsed = false;
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

    public void Attack(string name, int power, string type)
    {
        if (isPlayerTurn)
        {
            int damage = 0;
            if (type == "Attack - Physical")
            {
                float random = Mathf.Round(Random.Range(.85f, 1.0f));
                damage = Mathf.RoundToInt(((((2 * actBS[6] / 5) + 2) * power * actCS[1] / eneCS[2] / 50) + 2) * random);
            }
            else if (type == "Attack - Special")
            {
                float random = Mathf.Round(Random.Range(.85f, 1.0f));
                damage = Mathf.RoundToInt(((((2 * actBS[6] / 5) + 2) * power * actCS[3] / eneCS[4] / 50) + 2) * random);
            }
            eneCS[0] = eneCS[0] - damage;
            Announcer("Pikachu used " + name + "! \n The enemy Pidgey took " + damage +  " damage!", false);
        }
        else if (isPlayerTurn == false)
        {
            float random = Mathf.Round(Random.Range(.85f, 1.0f));
            int damage = Mathf.RoundToInt(((((2 * eneBS[6] / 5) + 2) * power * eneCS[1] / actCS[2] / 50) + 2) * random);
            actCS[0] = actCS[0] - damage;
            Announcer("Pikachu took " + damage + " damage!", true);
        }
        UpdateTextHealth();
    }

    void UpdateTextHealth()
    {
        Debug.Log("Active: " + actCS[0] + ", " + actCS[1] + ", " + actCS[2] + ", " + actCS[3] + ", " + actCS[4] + ", " + actCS[5]);
        Debug.Log("Enemy: " + eneCS[0] + ", " + eneCS[1] + ", " + eneCS[2] + ", " + eneCS[3] + ", " + eneCS[4] + ", " + eneCS[5]);
        Text uiText = pt_enemy_health.GetComponent<Text>();
        uiText.text = "Health: " + eneCS[0] + " / " + eneMS[0];
        uiText = pt_active_health.GetComponent<Text>();
        uiText.text = "Health: " + actCS[0] + " / " + actMS[0];
    }

    public void Announcer(string announcement, bool addToExisting)
    {
        Text uiText = uiAnnouncer.GetComponent<Text>();
        if (!addToExisting)
        {
            uiText.text = announcement;
        }
        else if (addToExisting)
        {
            string text = uiAnnouncer.GetComponent<Text>().text;
            uiText.text = text + "\n" + announcement;
        }
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

    public void DiscardHand()
    {
        while (cardsInHand != 0)
        {
            Destroy(hand_area.transform.GetChild(cardsInHand - 1).gameObject);
            cardsInHand = cardsInHand - 1;
        }
    }

    public void AddPlaceHolder(int indexno)
    {
        placeholder = Instantiate(ZoomPlaceholderCard, new Vector2(Input.mousePosition.x, Input.mousePosition.y), Quaternion.identity);
        placeholder.transform.SetParent(hand_area.transform, false);
        placeholder.transform.SetSiblingIndex(indexno);
    }

    public void RemovePlaceHolder()
    {
        Destroy(placeholder);
    }

    // Energy functions
    public void AddEnergy(string energy, int amount)
    {
        if (energy == "Lightning")
        {
            lightningEnergy = lightningEnergy + amount;
            totalEnergy = totalEnergy + amount;
            GameObject energySlot = Instantiate(slotLightning, new Vector2(0, 0), Quaternion.identity);
            energySlot.transform.SetParent(energyArea.transform, false);
            energySlot.transform.SetSiblingIndex(10);
        }
        else if (energy == "Normal")
        {
            normalEnergy = normalEnergy + amount;
            totalEnergy = totalEnergy + amount;
            GameObject energySlot = Instantiate(slotNormal, new Vector2(0, 0), Quaternion.identity);
            energySlot.transform.SetParent(energyArea.transform, false);
            energySlot.transform.SetSiblingIndex(0);
        }
        Announcer("Pikachu is charging up! (+1 " + energy + " energy)", false);
        UpdateTextEnergy(energy);
    }

    public void SubtractEnergy(string energy, int amount)
    {
        if (energy == "Lightning")
        {
            for (int i = 0; i < amount; i++)
            {
                lightningEnergy = lightningEnergy - 1;
                totalEnergy = totalEnergy - amount;
                GameObject obj = energyArea.transform.Find("slot_energy_lightning(Clone)").gameObject;
                Destroy(obj);
            }
        }
        else if (energy == "Normal")
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
        if (energy == "Lightning")
        {
            Text uiText = pt_active_energy_lightning.GetComponent<Text>();
            uiText.text = "Energy: " + lightningEnergy + "";
        }
        else if (energy == "Normal")
        {
            Text uiText = pt_active_energy_normal.GetComponent<Text>();
            uiText.text = "Energy: " + normalEnergy + "";
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

    //Enemy Turn
    private void EnemyTurn()
    {
        if (enemyIntent == "Attacking")
        {
            EnemyAttackSelection();
        }
        else if (enemyIntent == "Charging")
        {
            StartCoroutine(EnemyChargingLogic());
        }

    }

    private void EnemyAttackSelection()
    {
        if (Random.Range(0, 2) == 0)
        {
            StartCoroutine(EnemyAttackLogic("Tackle", 25));
        }
        else
        {
            StartCoroutine(EnemyAttackLogic("Gust", 40));
        }
    }

    IEnumerator EnemyAttackLogic(string attackName, int attackDamage)
    {
        //thinking...
        Announcer("Enemy Pidgey is thinking...", false);
        //Wait for 2 seconds
        yield return new WaitForSeconds(2);
        Announcer("Enemy Pidgey used " + attackName + "!", false);
        //Wait for 1 seconds
        yield return new WaitForSeconds(1);
        Attack(attackName, attackDamage, "Attack - Physical");
        UpdateTextHealth();
        //Wait for 1 seconds
        yield return new WaitForSeconds(1);
        NewEnemyIntent();
        NextTurn();
    }

    IEnumerator EnemyChargingLogic()
    {
        //thinking...
        Announcer("Enemy Pidgey is thinking...", false);
        //Wait for 2 seconds
        yield return new WaitForSeconds(2);
        Announcer("Enemy Pidgey is building energy!", false);
        //Wait for 1 seconds
        yield return new WaitForSeconds(1);
        NewEnemyIntent();
        NextTurn();
    }

    private void NewEnemyIntent()
    {
        if (Random.Range(0, 2) == 0)
        {
            enemyIntent = "Charging";
        }
        else
        {
            enemyIntent = "Attacking";
        }
        SetTextEnemyIntent(enemyIntent);
    }

    private void SetTextEnemyIntent(string intent)
    {
        Text uiText = pt_intent.GetComponent<Text>();
        uiText.text = intent;
    }

}
