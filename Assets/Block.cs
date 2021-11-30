using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Health
{

    private Vector3 startScale;
    public void Start()
    {
        startScale = transform.localScale;
    }

    public bool invonrable;

    public bool playerBLock;

    public override void takeDamage(int amount)
    {
        base.takeDamage(amount);
    }
    public void collide(BallBehavior ball)
    {
        LeanTween.scale(gameObject, transform.localScale + Vector3.one *0.5f, 0.5f)
            .setEasePunch()
            .setOnComplete(ResetScale);

        if (!invonrable)
            takeDamage(1);
        if (playerBLock)
            ball.powerBounce();
    }

    private void ResetScale()
    {
        LeanTween.scale(gameObject, startScale, 0.3f)
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

