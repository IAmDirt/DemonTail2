using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHealth : MonoBehaviour
{
    public List<GameObject> HealthUi = new List<GameObject>();
    public GameObject healthPrefab;
    public Health health;


    public void Start()
    {
        for (int i = 0; i < health.maxHealth; i++)
        {
            var spawned = Instantiate(healthPrefab, transform);
            HealthUi.Add(spawned);
        }
    }
    public void updateHealth()
    {
        foreach (var item in HealthUi)
        {
            item.SetActive(true);
        }

        for (int i = health.currentHealth; i < health.maxHealth; i++)
        {
            HealthUi[i].SetActive(false);
        }
    }

    public void DeathEvent()
    {
        gameManager manager = gameManager.Instance;

        manager.GameOver();
    }

}