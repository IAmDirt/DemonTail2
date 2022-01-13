using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    public float ballSpeed = 1;
    [HideInInspector] public Rigidbody rb;
     public Vector3 m_velocity;

    public GameObject playerBall;
    public bool Friendly;
    public bool CanBeDeflected = true;

    public virtual void OnEnable()
    {
        m_velocity = transform.forward;
        m_velocity = m_velocity.normalized;

    }
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
        rb.velocity = m_velocity * ballSpeed;
    }

    public LayerMask CollisionLayer;

    public virtual void OnCollisionEnter(Collision collision)
    {
        Collide(collision);
    }
    public virtual void OnCollisionStay(Collision collision)
    {
        Collide(collision);
    }

    public virtual void Collide(Collision collision)
    {

    }

    public virtual void deflect(Vector3 direction)
    {
        if(!Friendly && CanBeDeflected)
        {
            ReplaceBall(playerBall, direction);

            return;
        }
        else
            m_velocity = direction;
    }

    public GameObject ReplaceBall(GameObject newBall, Vector3 direction)
    {
        var position = new Vector3(transform.position.x, 1.5f, transform.position.z);
        var spawnedBall = PoolManager.Spawn(newBall, position, this.transform.rotation);
        var projectile = spawnedBall.GetComponent<projectile>();
        projectile.deflect(direction);

        DespawnSelf();
        return spawnedBall;
    }
    public virtual void DespawnSelf()
    {
        PoolManager.Despawn(this.gameObject);
    }
}