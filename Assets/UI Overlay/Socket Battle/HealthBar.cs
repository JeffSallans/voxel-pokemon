using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject healthBarRightTip;
    public GameObject healthBarMiddle;
    public GameObject healthBarLeftTip;

    private Vector3 initialHealthBarMiddlePosition;
    private Vector3 initialHealthBarMiddleScale;

    public GameObject healthBarEffectRightTip;
    public GameObject healthBarEffectMiddle;
    public GameObject healthBarEffectLeftTip;

    private Vector3 initialHealthBarEffectMiddlePosition;
    private Vector3 initialHealthBarEffectMiddleScale;

    public GameObject blockBarRightTip;
    public GameObject blockBarMiddle;
    public GameObject blockBarLeftTip;

    private Vector3 initialBlockBarMiddlePosition;
    private Vector3 initialBlockBarMiddleScale;

    // Start is called before the first frame update
    void Awake()
    {
        initialHealthBarMiddlePosition = healthBarMiddle.transform.localPosition;
        initialHealthBarMiddleScale = healthBarMiddle.transform.localScale;

        initialHealthBarEffectMiddlePosition = healthBarEffectMiddle.transform.localPosition;
        initialHealthBarEffectMiddleScale = healthBarEffectMiddle.transform.localScale;

        initialBlockBarMiddlePosition = blockBarMiddle.transform.localPosition;
        initialBlockBarMiddleScale = blockBarMiddle.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealthBar(Pokemon pokemon)
    {
        UpdateHealthBar(pokemon.health, pokemon.initHealth);
        UpdateBlockBar(pokemon.blockStat, pokemon.initHealth);
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthBarRightTip.SetActive(currentHealth == maxHealth);
        healthBarLeftTip.SetActive(currentHealth > 0);

        var healthRatio = currentHealth / (float)maxHealth;
        var newScale = initialHealthBarEffectMiddleScale.x * healthRatio;
        var newX = (initialHealthBarEffectMiddleScale.x - newScale) / 2f * 1.4f;
        healthBarMiddle.transform.localPosition = new Vector3(initialHealthBarEffectMiddlePosition.x - newX, initialHealthBarEffectMiddlePosition.y, initialHealthBarEffectMiddlePosition.z);
        healthBarMiddle.transform.localScale = new Vector3(newScale, initialHealthBarEffectMiddleScale.y, initialHealthBarEffectMiddleScale.z);
    }

    public void UpdateBlockBar(int currentBlock, int maxBlock)
    {
        blockBarRightTip.SetActive(currentBlock >= maxBlock);
        blockBarLeftTip.SetActive(currentBlock > 0);

        var blockRatio = currentBlock / (float)maxBlock;
        var newScale = initialBlockBarMiddleScale.x * blockRatio;
        var newX = (initialBlockBarMiddleScale.x - newScale) / 2f * 1.4f;
        blockBarMiddle.transform.localPosition = new Vector3(initialBlockBarMiddlePosition.x - newX, initialBlockBarMiddlePosition.y, initialBlockBarMiddlePosition.z);
        blockBarMiddle.transform.localScale = new Vector3(newScale, initialBlockBarMiddleScale.y, initialBlockBarMiddleScale.z);
    }
}
