using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goSpawner : MonoBehaviour
{
    public GameObject GoToSpawn;
    public bool onDisable;
    public void OnDisable()
    {
        if (onDisable)
            spawnGo();
    }

    private void spawnGo()
    {
        PoolManager.Spawn(GoToSpawn.gameObject, transform.position, transform.rotation);
    }
}
