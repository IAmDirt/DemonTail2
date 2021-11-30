using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoDeswpaner : MonoBehaviour
{
    public float despwanDelay = 2;
    void OnEnable()
    {
        Invoke("despawn", despwanDelay);
    }
    private void despawn()
    {
        PoolManager.Despawn(this.gameObject);
    }
}
