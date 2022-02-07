using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class homingProjectile : projectile
{

    public override void Update()
    {
        base.Update();
        Rotate();
    }
    public float RotateSpeed;
    public Transform target;

    public override void FixedUpdate()
    {
        m_velocity = transform.forward.normalized;
        rb.velocity = m_velocity * ballSpeed;
    }

    public override void Collide(Collision collision)
    {
        base.Collide(collision);

        if (collision.gameObject.layer == 10)
        {
            collision.gameObject.GetComponent<Health>().takeDamage(1);
        }
        PoolManager.Despawn(gameObject);// Destroy(gameObject);
    }

    public void Rotate()
    {
        var direction = target.position-transform.position ;
        direction.y = 0;
        float singleStep = RotateSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction.normalized, singleStep, 0.0f);

        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}
