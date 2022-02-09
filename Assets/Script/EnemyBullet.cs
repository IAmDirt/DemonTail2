using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : projectile
{
  //  public Material deflectableColor;
   // public Material NotDeflectableColor;
  //  public MeshRenderer renderer;
    public bool startBlack = false;


    public override void OnEnable()
    {
   
        CanBeDeflected = false;
        base.OnEnable();
      //  if (startBlack) { updateDeflectColor(true); }
    }
    public void updateDeflectColor(bool canBeDeflected)
    {
        CanBeDeflected = canBeDeflected;
        //renderer.material = CanBeDeflected ? deflectableColor : NotDeflectableColor;

            transform.GetChild(0).gameObject.SetActive(!canBeDeflected);
            transform.GetChild(1).gameObject.SetActive(canBeDeflected);
    }
    public override void Start()
    {
        base.Start();
        Wobble();
    }
    public override void Collide(Collision collision)
    {
        base.Collide(collision);

        if (collision.gameObject.layer == 10)                               
        {
            collision.gameObject.GetComponent<Health>().takeDamage(1);
        }
       // updateDeflectColor(false);
        PoolManager.Despawn(gameObject);// Destroy(gameObject);
    }
    public void Wobble()
    {
        LeanTween.scale( gameObject, Vector3.one * Random.Range(1, 1.5f), Random.Range(0.4f, 1.5f))
            .setEasePunch()
            .setOnComplete(Wobble);
    }
}