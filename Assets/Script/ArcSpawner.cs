using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcSpawner : MonoBehaviour
{
    [Header("projectiles")]
    public GameObject bigProjectile;
    public Transform spawnTrans;
    public float shootAngle = 25;
    public float predictMultiplyer = 1;

    public Transform player;

    public bool fire;

    private float firerate = 1;
    private float fireprogress = 1;
    public void Update()
    {
        if (fire || fireprogress <= 0)
        {
            fire = false;
            fireprogress = firerate;
            arcProjectile();
        }
        else
            fireprogress -= Time.deltaTime;
    }

    public void arcProjectile()
    {

        var target = Vector3.zero;

        var velocityPredicition = player.GetComponent<Rigidbody>().velocity;

        velocityPredicition *= Random.Range(predictMultiplyer - 0.1f, predictMultiplyer + 0.2f);


        var spawned = PoolManager.Spawn(bigProjectile.gameObject, spawnTrans.position, spawnTrans.rotation);
        var ball = spawned.GetComponent<ArcProjectile>();
        ball.PhysicsShoot(player.position, Random.Range(shootAngle - 5, shootAngle + 5), velocityPredicition);
    }
}
