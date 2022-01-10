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
        if(!Friendly )
        {
            ReplaceBall(playerBall, direction, true);

            return;
        }
    }

    public GameObject ReplaceBall(GameObject newBall, Vector3 direction, bool isDeflectalbe = false)
    {
        var spawnedBall = PoolManager.Spawn(newBall, this.transform.position, this.transform.rotation);
        var projectile = spawnedBall.GetComponent<projectile>();
        if(isDeflectalbe)
        projectile.deflect(direction);

        PoolManager.Despawn(this.gameObject);
        return spawnedBall;
    }
}