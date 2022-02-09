using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class deflector : MonoBehaviour
{
    public GameManager manager;
    public LayerMask BallLayer;

    [Header("shield")]
    public Transform shield;

    public DialogueManager dialogueManager;
    public void Start()
    {
        manager.player = transform;
        movement = GetComponent<SlugMovement>();
        trail.emitting = false;
        deflectRadius = minDeflectRadius;

        deflectType = 2;
        shield.gameObject.SetActive(false);
        //movement.lockrotation(false);

      playerMaterial = new Material(playerMaterial);
   flashMaterial= new Material(flashMaterial);
    Cursor.lockState = CursorLockMode.Confined;
    }
    public int deflectType = 1;
    public void Update()
    {
       /* if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            deflectType = 2;
            shield.gameObject.SetActive(false);
            movement.lockrotation(false);
        }*/
        /* 
             if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            deflectType = 1;
            shield.gameObject.SetActive(true);
            movement.lockrotation(true);
        }
      if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            deflectType = 3;
            shield.gameObject.SetActive(false);
            movement.lockrotation(false, true);
        }*/


        if (deflectType == 2 || deflectType == 3)
        {
            // instant
            if (nextReflect <= 0)
            {

              /*  if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Comma))
                {
                    deflectRelease();
                    nextReflect = reflectFireRate;
                }*/
            }
            else
            {
                nextReflect = Mathf.Clamp(nextReflect - Time.deltaTime, 0, 10);
            }

          /*  if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Comma))
            {
                // SpawnBall();
            }*/

            //charge
            /* if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Period))
             {
                 chargeProgress = 0;
                 ChargingDeflect = true;
                 //StartCoroutine(SlowDownPlayer());
             }
             else if (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Period))
             {
                 deflectCharge();
             }
             else if (Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.Period))
             {
                 deflectRelease(true);
                 ChargingDeflect = false;
             }*/
        }


        updateAmmoUI();
    }

    public float nextReflect;
    private float reflectFireRate = 0.3f;
    [Header("ballRecovery")]
    public GameObject Ball;

    public int BallAmmo = 1;
    public float ballRechargeTime = 5;
    public float CurrentBallRecharge;

    public Transform AmmoImage;
    public Image AmmofillImage;

    public void updateAmmoUI()
    {
        if (CurrentBallRecharge < ballRechargeTime)
        {
            CurrentBallRecharge += Time.deltaTime;
            AmmofillImage.color = Color.white;
        }
        else
            AmmofillImage.color = Color.red * Color.white;

        var prosentageDone = CurrentBallRecharge / ballRechargeTime;
        AmmoImage.localScale = Vector3.one * Mathf.Lerp(0, 1, prosentageDone);

    }
    public void SpawnBall()
    {
        if (CurrentBallRecharge >= ballRechargeTime)
        {
            CurrentBallRecharge = 0;

            var spawned = Instantiate(Ball, transform.position, transform.rotation);
        }
    }
  
    public float minDeflectRadius = 2;
    public float maxDeflectRadius = 4;
    private float deflectRadius;
    // private float chargeProgress;
    //  private float chargeTime = 0.6f;

    //  private bool ChargingDeflect;
    public Transform DeflectVisual;
    private SlugMovement movement;
    /*  private void deflectCharge()
      {
          //scale ring
          //slow down player

          chargeProgress = Mathf.Clamp(chargeProgress + Time.deltaTime / chargeTime, 0, 1);
          deflectRadius = Mathf.Lerp(minDeflectRadius, maxDeflectRadius, chargeProgress);

          DeflectVisual.localScale = new Vector3(deflectRadius*2, 0.01f, deflectRadius*2);

          movement.setSlowness(Mathf.Lerp(1, maxSlow,  chargeProgress));

          DeflectVisual.gameObject.SetActive(true);
      }*/

    public void deflectRelease()
    {
        if (nextReflect <= 0)
        {
          //  SpawnBall();
            nextReflect = reflectFireRate;
            StartCoroutine(openHitBox());
            StartCoroutine(wickedFlip(0.13f));

            DeflectVisual.gameObject.SetActive(false);

            rumbler.RumbleConstant(1, 1, 0.06f);
        }
    }
    //open hitbox more frames when hit
    public IEnumerator openHitBox()
    {
        var deflectDir = transform.forward;
        var colliders = new List<Collider>();
        var CheckIntervals = 0.05f;
        var lastChecked = 0f;
        var timeElapsed = 0f;
        while (timeElapsed < 0.15f)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed >= (lastChecked + CheckIntervals))
            {
                lastDeflect = transform.position;
                lastChecked += CheckIntervals;
                //check collison
                var ballsCollider = Physics.OverlapSphere(transform.position, deflectRadius, BallLayer);
                foreach (var collider in ballsCollider)
                {
                    if (!colliders.Contains(collider))
                    {
                        var projectille = collider.GetComponent<projectile>();
                        if (projectille)

                            if (projectille.CanBeDeflected)
                            {
                                projectille.deflect(deflectDir);
                                colliders.Add(collider);
                                // DoSlowmotion(ball.PowerProsentage, slowDownRecoverTime, slowdownFactor);
                                yield break;
                            }

                        var dialogue = collider.GetComponent<dialogueContainer>();
                        if (dialogue)
                        {
                            dialogue.triggerDialogue(dialogueManager);
                            yield break;
                        }

                    }
                }
            }
            yield return null;
        }
    }

    public TrailRenderer trail;
    public Transform turningParent;
    IEnumerator wickedFlip(float duration)
    {
        movement.setSlowness(0.3f);


        trail.emitting = true;
        Vector3 startRotation = Vector3.zero;
        float endRotation = startRotation.y - 360.0f;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation.y, endRotation, t / duration) % 360.0f;
            turningParent.localEulerAngles = new Vector3(startRotation.x, yRotation, startRotation.z);
            yield return null;
        }
        trail.emitting = false;
        movement.setSlowness(1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, deflectRadius);

        Gizmos.color = Color.black;

        Gizmos.DrawWireSphere(lastDeflect, lastRadius);
        Gizmos.DrawSphere(lastDeflect, 0.5f);
    }

    private Vector3 lastDeflect;
    private float lastRadius = 4;

    public Material playerMaterial;
    public Material flashMaterial;
    public GameObject[] body;

    [Header("Damage Rumble")]
    public float rumbleTime =0.5f;
    public float low = 0.5f;
    public float high = 1f;
    public bool Rumble = true;
    [SerializeField] Rumbler rumbler;
    public void DamageFlash()
    {
        StartCoroutine(Flash());
        if(Rumble)
        rumbler.RumbleConstant(low, high, rumbleTime);
        DoSlowmotion(slowDownProsentage, slowDownRecoverTime, slowdownFactor);
    }
    public IEnumerator Flash()
    {
        playerMaterial.SetFloat("_ToonRimPower", 0.0f);
        yield return new WaitForSeconds(0.08f);


        var blinkAmount = 2;
        for (int i = 0; i < blinkAmount; i++)
        {

        playerMaterial.SetFloat("_ToonRimPower", 1.1f);
            yield return new WaitForSeconds(0.03f);
        playerMaterial.SetFloat("_ToonRimPower", 0.0f);

            yield return new WaitForSeconds(0.1f);
        playerMaterial.SetFloat("_ToonRimPower", 1.1f);
        }
    }

    
  [Header("slowDown")]

  public float slowDownProsentage = 0.05f;
  public float slowdownFactor = 0.05f;
  public float slowDownStayTime = 0.05f;   //time stay at slow factor
  public float slowDownRecoverTime = 1;    //time to smoothe back to normal time

  #region slowDown
  public void DoSlowmotion(float prosentage, float slowDownLength, float slowDownFactor)
  {
      Time.timeScale = slowDownFactor;
      Time.fixedDeltaTime = Time.timeScale * 0.01333f;
      StartCoroutine(resetTime(prosentage, slowDownLength));
  }

  IEnumerator resetTime(float prosentage, float slowDownLength)
  {
      yield return new WaitForSecondsRealtime(Mathf.Lerp(0, slowDownStayTime, prosentage));
      while (Time.timeScale < 1)
      {
          Time.timeScale += (1f / slowDownLength) * Time.deltaTime;
          Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1f);

          Time.fixedDeltaTime = Time.timeScale * 0.01333f;
          yield return null;
      }
  }
  #endregion
  
    /*
    private float waitBeforeSlow = 0.4f;
    private float maxSlow = 0.4f;


 public IEnumerator SlowDownPlayer()
    {
        var TimePassed = 0.0f;

        while (true)
        {
            TimePassed += Time.deltaTime;

            if(waitBeforeSlow < TimePassed)
            {
                //apply Slow
            }
            yield return null;
        }
    }*/
}