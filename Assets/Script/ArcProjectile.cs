using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class ArcProjectile : projectile
{
    public override void OnEnable()
    {
        active = false;
        rigid.isKinematic = false;
    }
    public void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        _startScale =transform.localScale  ;

    }
    Vector3 ArtificialGravity;
    public ParticleSystem landParticle;
    public Transform predictShadow;
    public bulletSpawner spawner;
    public GameObject explotion;
    Rigidbody rigid;

    private GameObject Spawned;
    private bool active = false;
    private bool collided;

    public bool isDud;
    private float fallPredict = 1.2f;
    public void PhysicsShoot(Vector3 target, float initialAngle, Vector3 velPredict)
    {

        StartCoroutine(SpawnAnimation(target));
        return;
        collided = false;
        Vector3 p = target + velPredict;
        setPredictShadow(p);

        ArtificialGravity = -Vector3.up * 9.81f * 3;
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

    public IEnumerator SpawnAnimation(Vector3 target)
    {
        rigid.isKinematic = true;
        LeanTween.move(gameObject, transform.position + Vector3.up * 55, 0.8f)
            .setEaseOutElastic();

        yield return new WaitForSeconds(0.3f);

        gameObject.transform.position = target + Vector3.up * 55;
        //  rigid.isKinematic = false;
        setPredictShadow(target);
        yield return new WaitForSeconds(fallPredict);

        if (isDud)
            LeanTween.move(gameObject, target - Vector3.up * 1, 0.6f).setOnComplete(dud);
        else
            LeanTween.move(gameObject, target - Vector3.up * 1, 0.6f).setOnComplete(explode);


        //go up from 
    }
    public void setPredictShadow(Vector3 Position)
    {
        Position = Position - Vector3.up * 1.5f;
        Spawned = Instantiate(predictShadow.gameObject, Position, predictShadow.rotation);
        Spawned.LeanScale(Spawned.transform.lossyScale * 6, fallPredict)
            .setEaseOutBack();
    }

    private void activateCollider()
    {
        active = true;
    }
    public override void FixedUpdate()
    {
        if (rigid != null)
            rigid.AddForce(ArtificialGravity, ForceMode.Acceleration);
    }
    public override void OnCollisionEnter(Collision collision)
    {
        if (!collided && active)
            explode();
    }
    public void explode()
    {
        spawner.FirePatternCircle();
        Destroy(Spawned);

        PoolManager.Spawn(explotion, transform.position, explotion.transform.rotation);
        PoolManager.Despawn(gameObject);
    }

    public void dud()
    {
        landParticle.Play();
        rigid.isKinematic = true;
        spawner.idleAnim = false;
        StartCoroutine(dudLateExplode());
    }

    private IEnumerator dudLateExplode()
    {
        var startScale = transform.localScale;
        yield return new WaitForSeconds(4f);
        pulsate();
        yield return new WaitForSeconds(3f);
        LeanTween.scale(gameObject, startScale * 1.5f, 0.5f).setEaseInOutBack();
        yield return new WaitForSeconds(0.5f);
        explode();
    }

   private void pulsate()
    {
        var startScale = transform.localScale;
        LeanTween.scale(gameObject, startScale * 1.15f, 0.5f).setEasePunch().setOnComplete(pulsate);
       // LeanTween.scale(gameObject, startScale * 1.2f, 1).setEasePunch().onCompleteOnRepeat();
    }
    private Vector3 _startScale;
    public override void DespawnSelf()
    {
        transform.localScale = _startScale;
        Spawned.gameObject.SetActive(false);
        base.DespawnSelf();
    }
}