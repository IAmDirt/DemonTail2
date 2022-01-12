using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletSpawner : MonoBehaviour
{
    public GameObject Bullet;
    public bool isenabled;
    public void Start()
    {
        if(isenabled)
            StartCoroutine(firePatternSpiral());
    }
    void Update()
    {
      /*  if(Input.GetKeyDown(KeyCode.Alpha1))
        {
         //   explotion();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
        //    FirePatternCircle();
        }*/
        if (obj2 != null)
            idleAnimation();
    }
    public IEnumerator StartBulletHell( float waitTime = 0.4f)
    {
        var waveAmount = 2;
        for (int i = 0; i < waveAmount; i++)
        {
            explotion();

            yield return new WaitForSeconds(0.8f);
        }

        yield return new WaitForSeconds(Random.Range(0.05f, waitTime));
        StartCoroutine(StartBulletHell(Random.Range(8, 8)));
    }

    public void explotion()
    {
        SpikeAnim();
        var deflectabelAmount = Random.Range(-1, 2);
        for (int i = 0; i < BulletAmount; i++)
        {
            var canBeDeflected = deflectabelAmount >= i+1 ? true : false;

            var direction = GetDirectionCircle(Random.Range(minAngle, maxAngle));
            //put offset for start rotation here
            spawnBullet(direction, canBeDeflected);
        }
    }
    [Header ("BulletCircle")]
    public int BulletAmount;
    [SerializeField]
    private float minAngle= 45f, maxAngle = -45f;
    public Transform BulletSpawner;

    public void spiralStart()
    {
        StartCoroutine(firePatternSpiral());
    }
    private IEnumerator firePatternSpiral()
    {

        float angleStep = (minAngle - maxAngle) / BulletAmount;
        var angle = minAngle;


        var deflectableStart = Random.Range(0, BulletAmount-1);

        for (int i = 0; i <= BulletAmount; i++)
        {
            var direction = GetDirectionCircle(angle);


            var canBeDeflected = deflectableStart == i ? true : false; 
            spawnBullet(direction, canBeDeflected);

            angle += angleStep;
            yield return new WaitForSeconds(0.045f);
        }

            yield return new WaitForSeconds(Random.Range(4, 8));
      //  StartCoroutine(firePatternSpiral());
    }

    public void FirePatternCircle()
    {
        SpikeAnim();
        float angleStep = (minAngle - maxAngle) / BulletAmount;
        var angle = minAngle;

        for (int i = 0; i <= BulletAmount; i++)
        {
           var direction= GetDirectionCircle(angle);

            spawnBullet(direction, false);

            angle += angleStep;
        }
    }

    public void spawnBullet( Vector3 direction, bool canBeDeflected)
    {
        var spawnPosition = new Vector3(BulletSpawner.position.x, 1.5f, BulletSpawner.position.z);
        var spawned = PoolManager.Spawn(Bullet, spawnPosition, Quaternion.LookRotation(direction));
        spawned.GetComponent<EnemyBullet>().updateDeflectColor(canBeDeflected);
    }

    public Vector3 GetDirectionCircle(float angle)
    {
        var bulletDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
        var bulletDirZ = transform.position.z + Mathf.Cos((angle * Mathf.PI) / 180f);

        var bulletDir = new Vector3(bulletDirX, transform.position.y, bulletDirZ);
        return bulletDir = (bulletDir - transform.position).normalized;
    }

    //test animations
    //--------------------------------------------------------------
    public Vector3 rotationAxis1;
    public Vector3 rotationAxis2;
    public float rotationSpeed;

    public Transform obj1;
    public Transform obj2;
    public GameObject animationObj;
    public GameObject shadow;
    public void idleAnimation()
    {
        obj1.Rotate(rotationAxis1 * rotationSpeed * Time.deltaTime);
        obj2.Rotate(rotationAxis2 * rotationSpeed * Time.deltaTime);
    }

    public void SpikeAnim()
    {
        if(obj2 != null)
        {

        LeanTween.scale(obj2.gameObject, Vector3.one *2.8f, Random.Range(0.5f, 0.8f))
           .setEasePunch()
           .setDelay(0.2f);

             LeanTween.scale(obj1.gameObject, Vector3.one *2.5f, Random.Range(0.4f, 0.6f))
           .setEasePunch();
        }
    }

    private Vector3 Shadow_startScale = new Vector3(1, 0.001f, 1);
    private Vector3 Shadow_endScale = new Vector3(2f, 0.001f, 2f);

    public void Animate(Vector3 spawnPos, Vector3 target)
    {

        StartCoroutine(spawnAnimation(spawnPos, target));
    }
    private IEnumerator spawnAnimation(Vector3 spawnPos, Vector3 target)
    {
        shadow.transform.position = target - Vector3.up*0.9f;


        isenabled = false;
        LeanTween.move(animationObj, spawnPos + Vector3.up * 6, 0.5f)
        .setEaseOutElastic();

        LeanTween.scale(shadow, Shadow_startScale , 0.3f).setEaseOutBounce();
        yield return new WaitForSeconds(0.5f);

        LeanTween.scale(shadow, Shadow_endScale , 0.3f).setEaseOutBounce();
        LeanTween.move(animationObj, target, 0.4f)
        .setEaseOutBounce();
        isenabled = true;

        yield return new WaitForSeconds(0.8f);
        StartCoroutine(firePatternSpiral());

    }
}