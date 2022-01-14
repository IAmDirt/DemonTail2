using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceDeflectProjectile : projectile
{

    public override void OnEnable()
    {
        base.OnEnable();
    }
    public override void Start()
    {
        base.Start();
    }
    public override void Collide(Collision collision)
    {
        base.Collide(collision);

        if (collision.gameObject.layer == 10)
        {
            collision.gameObject.GetComponent<Block>().takeDamage(1111);
        }
        // updateDeflectColor(false);
        PoolManager.Despawn(gameObject);// Destroy(gameObject);
    }
}