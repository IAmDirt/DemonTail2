using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class BallBehavior : projectile
{
    public Transform animationTarget;
    public Transform scaleTarget;
    public ParticleSystem particleTrail;
    public ParticleSystem particleHit;
    public MeshRenderer renderer;
    public Material normalMat;
    public Material hitMat;

    [Header("PowerLevel")]
    [Range (0, 1)]
    public float PowerProsentage = 0.5f; 

    public float powerIncreseOnHit = 0.25f; 
    public float DecaySpeed = 0.1f;     //this amount per second

    public float decayWait = 5;        //time to decay after  hit
    public float timeToDecay = 5;       //time to decay after  hit

    private float minScale =1f;
    private float maxScale=2.3f;

    public Gradient PoweColor;
    public Light glowLight;
    public TrailRenderer trail;

    [Header("Audio")]

    public RandomAudioPlayer DeflectSound;
    public RandomAudioPlayer ImpactSound;

    [Header("misc")]
    public UnityEvent deflectEvent;
    public int bounceAmount = 0;
    public int maxBounceAmount = 5;
    public float powerSpeed = 1;

    private float squishTime = 0.3f;
    public AnimationCurve SquishCurveZ;

    public override void Start()
    {
        base.Start();
        m_velocity = transform.forward.normalized;

        var meshRenderer = animationTarget.GetComponent<MeshRenderer>();
        meshRenderer.material = Instantiate<Material>(meshRenderer.material);
        normalMat = meshRenderer.material;

    }
    public override void Update()
    {
        UpdateVisuals();
    }
    public override void FixedUpdate()
    {
        rb.velocity = m_velocity * ballSpeed * powerSpeed;
    }

    public void powerBounce()
    {
        bounceAmount = maxBounceAmount;
    }
    public void DecayPower()
    {
        PowerProsentage = Mathf.Clamp(PowerProsentage * 0.80f - 0.05f, 0, 1);

        if (PowerProsentage <= 0)
        {
            StartCoroutine(ChangeBall());
        }
    }
    public void UpdateVisuals()
    {
        if (PowerProsentage > 0.5f)
            glowLight.intensity = Mathf.Lerp(0f, 2.5f, PowerProsentage);
        else
            glowLight.intensity = 0;

        var scale = Mathf.Lerp(minScale, maxScale, PowerProsentage);

        scaleTarget.localScale = Vector3.one * scale;
        trail.widthMultiplier = scale;

        normalMat.SetColor("_Color", PoweColor.Evaluate(PowerProsentage));
        normalMat.SetColor("_EmissionColor", PoweColor.Evaluate(PowerProsentage));
        trail.startColor = PoweColor.Evaluate(PowerProsentage);
    }

    //bounce of wall
    public void ricochet(Vector3 hitNormal)
    {
        ImpactSound.PlayRandomClip();
        checkBounce();
        var reflectDirection = Vector3.Reflect(m_velocity, hitNormal);

        reflectDirection.y = 0;
        reflectDirection = reflectDirection.normalized;

        m_velocity = reflectDirection;
        transform.rotation = Quaternion.LookRotation(reflectDirection);

        //juice
        particleHit.Play();
        StartCoroutine(ImpactSquish());
    }

    //deflected by player
    public override void deflect(Vector3 direction)
    {
        DeflectSound.PlayRandomClip();
        powerBounce();
        checkBounce();
        m_velocity = direction;
        transform.rotation = Quaternion.LookRotation(direction);

        //juice
        particleHit.Play();
        StartCoroutine(ImpactSquish());

        //power
        PowerProsentage = Mathf.Clamp(PowerProsentage + powerIncreseOnHit, 0f, 1f);
        timeToDecay = decayWait;

        deflectEvent.Invoke();
    }

    private bool checkBounce()
    {
        if (bounceAmount <= 0)
        {
            powerSpeed = 1;
            particleTrail.Stop();
            DecayPower();
            return false;
        }
        else
        {
            particleTrail.Play();
            powerSpeed = 2f;
            bounceAmount--;
            return true;
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 9|| collision.gameObject.layer == 12)  //enviroment || spawners
        {
            var block = collision.transform.GetComponent<Block>();

            if (block)
            {
                block.collide(this);
            }
        }
        ricochet(collision.contacts[0].normal);
    }

    //animate ball
    private IEnumerator ImpactSquish()
    {
            var stretchAmount = (ballSpeed * 0.03f);
        var timeElapsed = 0f;
        do
        {
            timeElapsed += Time.deltaTime;
            var squishCurve = SquishCurveZ.Evaluate(timeElapsed);

           var stretchBase = new Vector3( - (stretchAmount / 2),  - (stretchAmount / 2),  + stretchAmount);
            var squishTarget = new Vector3(1 - squishCurve / 2, 1 - squishCurve / 2, 1 + squishCurve);
            animationTarget.localScale = squishTarget;

            yield return new WaitForFixedUpdate();
        }
        while (timeElapsed <= squishTime);
    }

    public GameObject BlackBall;
    private IEnumerator ChangeBall()
    {
        var blinkAmount = 4;
        var finalBlinkAmount = 18;

        //first blink
        for (int i = 0; i < blinkAmount; i++)
        {
            renderer.material = hitMat;
            yield return new WaitForSeconds(0.075f);
            renderer.material = normalMat;
            yield return new WaitForSeconds(0.04f);
            renderer.material = hitMat;
            yield return new WaitForSeconds(0.075f);
            renderer.material = normalMat;
            yield return new WaitForSeconds(1f);
            if (PowerProsentage > 0) yield break;

        }
        //final blink
        for (int i = 0; i < finalBlinkAmount; i++)
        {
            renderer.material = hitMat;
            yield return new WaitForSeconds(0.04f);
            renderer.material = normalMat;
            yield return new WaitForSeconds(0.075f);
            if (PowerProsentage > 0) yield break;
        }

        if (PowerProsentage > 0) yield break;
        //turn to black ball
        var spawnedBall = ReplaceBall(BlackBall, m_velocity.normalized);
        spawnedBall.GetComponent<EnemyBullet>().updateDeflectColor(true);
        spawnedBall.GetComponent<EnemyBullet>().m_velocity = m_velocity;
    }

    /*   public void StretchBall()
      {
          var stretchAmount = (ballSpeed * 0.01f);

          animationTarget.localScale = new Vector3(1 - (stretchAmount / 2), 1 - (stretchAmount / 2), 1 + stretchAmount) * powerSpeed;
      }

       public void move()
        {
            transform.Translate(transform.forward * Time.deltaTime);
        }*/
}
