using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDeckCreator : MonoBehaviour
{
    ControllerScript controllerScript;

    public GameObject cardAttackGnaw;
    public GameObject cardAttackTackle;
    public GameObject cardAttackGust;
    public GameObject cardStatusSandAttack;

    private List<GameObject> possibleDeck;

    // Start is called before the first frame update
    void Start()
    {
        controllerScript = gameObject.GetComponent<ControllerScript>();
    }

    public void BuildEnemyDeck(string name, int level)
    {
        if (name == "Pidgey") { if (level >= 0) { possibleDeck.Add(cardAttackTackle); } }
        if (name == "Pidgey") { if (level >= 5) { possibleDeck.Add(cardAttackGnaw); } }
    }

    void AddMultipleGameObjects(GameObject objectToAdd, int count, List<GameObject> targetList)
    {
        for (int i = 0; i < count; i++)
        {
            targetList.Add(objectToAdd);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
