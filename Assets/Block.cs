using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Health
{

    private Vector3 startScale;
    public void Start()
    {
        if (!ScaleOject) ScaleOject = gameObject;
        startScale = ScaleOject .transform.localScale;

    }

    public bool invonrable;

    public bool playerBLock;

    public override void takeDamage(int amount)
    {
        base.takeDamage(amount);
    }
    public float scaleAmount = 1.3f;
    public GameObject ScaleOject;
    public void collide(BallBehavior ball)
    {
        Debug.Log(startScale * scaleAmount);
        LeanTween.scale(ScaleOject,  startScale * scaleAmount, 0.5f)
            .setEasePunch()
            .setOnComplete(ResetScale);

        if (!invonrable)
            takeDamage(1);
        if (playerBLock)
            ball.powerBounce();
    }

    private void ResetScale()
    {
        LeanTween.scale(ScaleOject, startScale, 0.3f)
            .setEaseInOutSine();
    }

    public override void Kill()
    {
        Destroy(gameObject);
    }
    public GameObject DeathParticleGO;
    public void DeathParticle()
    {
        PoolManager.Spawn(DeathParticleGO, transform.position, transform.rotation);
    }
}

