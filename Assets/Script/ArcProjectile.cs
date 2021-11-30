using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class ArcProjectile : MonoBehaviour
{
    public void OnEnable()
    {
        active = false;
    }
    Vector3 ArtificialGravity;
    public void shoot(Vector3 target, float initialAngle, Vector3 velPredict)
    {
        collided = false;
        rigid = GetComponent<Rigidbody>();
        Vector3 p = target + velPredict;

        ArtificialGravity = -Vector3.up * 9.81f *4;
        float gravity = Physics.gravity.magnitude + ArtificialGravity.magnitude;

        // Selected angle in radians
        float angle = initialAngle * Mathf.Deg2Rad;

        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(p.x, 0, p.z);
        Vector3 planarPostion = new Vector3(transform.position.x, 0, transform.position.z);

        // Planar distance between objects
        float distance = Vector3.Distance(planarTarget, planarPostion);
        // Distance along the y axis between objects
        float yOffset = transform.position.y - p.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion) * (p.x > transform.position.x ? 1 : -1);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        // Fire!
        rigid.velocity = finalVelocity;

        // Alternative way:
        // rigid.AddForce(finalVelocity * rigid.mass, ForceMode.Impulse);
    
        Invoke("activateCollider", 0.5f);
    }

    public GameObject explotion;
    private void  activateCollider()
    {
        active = true;
    }
    Rigidbody rigid;
    private bool active = false;
    public void FixedUpdate()
    {
        if (rigid != null )
            rigid.AddForce(ArtificialGravity, ForceMode.Acceleration);

    }
    public void OnCollisionEnter(Collision collision)
    {
        if (!collided && active)
            explode();
    }
    private bool collided;
    public bulletSpawner spawner;
    public void explode()
    {
        spawner.FirePatternCircle();

        PoolManager.Spawn(explotion, transform.position, explotion.transform.rotation);
        PoolManager.Despawn(gameObject);
    }
}
