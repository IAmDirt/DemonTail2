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

        if (collision.gameObject.layer == 12)
        {
            if (collision.transform.name.Contains("DeflectiveShield"))
            collision.gameObject.GetComponent<Block>().takeDamage(1111);
            else
            collision.gameObject.GetComponent<Block>().takeDamage(2);
        }
        // updateDeflectColor(false);
        PoolManager.Despawn(gameObject);// Destroy(gameObject);
    }
}