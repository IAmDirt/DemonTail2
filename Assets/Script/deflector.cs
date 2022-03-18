using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Cinemachine;
public class deflector : MonoBehaviour
{
    public LayerMask BallLayer;
    public DialogueManager dialogueManager;
    public GameObject combatUI;
    public void Start()
    {
        movement = GetComponent<SlugMovement>();
        trail.emitting = false;
        deflectRadius = minDeflectRadius;

        playerMaterial = new Material(playerMaterial);
        flashMaterial = new Material(flashMaterial);
        Cursor.lockState = CursorLockMode.Confined;

        if(movement.inOverworld)
        {
            combatUI.SetActive(false);
        }
    }
    public void Update()
    {
        if (nextReflect > 0)
        {
            nextReflect = Mathf.Clamp(nextReflect - Time.deltaTime, 0, 10);
        }
    }


    #region ball deflect
    [Header("ballRecovery")]
    public float minDeflectRadius = 2;

    private float reflectFireRate = 0.3f;
    private float deflectRadius;
    private float nextReflect;
    private float lastRadius = 4;

    public Transform turningParent;
    private SlugMovement movement;
    public TrailRenderer trail;
    private Vector3 lastDeflect;

    public void deflectRelease()
    {
        if (nextReflect <= 0)
        {
            //  SpawnBall();
            nextReflect = reflectFireRate;
            StartCoroutine(openHitBox());
            StartCoroutine(wickedFlip(0.13f));


            rumbler.RumbleConstant(1, 1, 0.06f);
        }
    }
    //open hitbox more frames when hit

    public void OverwordlInteract()
    {
        if (!movement.inOverworld)
            return;
        if (nextReflect <= 0)
        {
            //  SpawnBall();
            nextReflect = reflectFireRate;
            StartCoroutine(openHitBox());
            rumbler.RumbleConstant(1, 1, 0.06f);
        }
    }
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

    public void deflectSuper()
    {
        Debug.Log("super");
    }
    #endregion

    #region damageSignals
    [Header("Damage Rumble")]
    public float rumbleTime = 0.5f;
    public float low = 0.5f;
    public float high = 1f;
    public bool Rumble = true;
    [SerializeField] Rumbler rumbler;
    public CinemachineImpulseSource impulse;
    public void DamageFlash()
    {
        impulse.GenerateImpulse(5);
        StartCoroutine(Flash());

        //StartCoroutine(Flash());
        if (Rumble)
            rumbler.RumbleConstant(low, high, rumbleTime);
        DoSlowmotion(slowDownProsentage, slowDownRecoverTime, slowdownFactor);
    }

    public Material playerMaterial;
    public Material flashMaterial;
    public MeshRenderer[] MeshRenderers;
    public SkinnedMeshRenderer skineedRenderer;
    public IEnumerator Flash()
    {
        SetEmissionMaterial(true);
        yield return new WaitForSeconds(0.08f);


        var blinkAmount = 4;
        for (int i = 0; i < blinkAmount; i++)
        {

            SetEmissionMaterial(false);
            yield return new WaitForSeconds(0.03f);
            SetEmissionMaterial(true);

            yield return new WaitForSeconds(0.08f);
            SetEmissionMaterial(false);
        }
    }

    private void SetEmissionMaterial(bool bo)
    {
        Material mat = bo ? flashMaterial : playerMaterial;

        skineedRenderer.materials = new Material[2] { mat, skineedRenderer.materials[1] };
        foreach (var renderer in MeshRenderers)
        {
            renderer.materials = new Material[2] { mat, renderer.materials[1] };
        }
    }

    #endregion

    #region slowDown
    [Header("slowDown")]

    public float slowDownProsentage = 0.05f;
    public float slowdownFactor = 0.05f;
    public float slowDownStayTime = 0.05f;   //time stay at slow factor
    public float slowDownRecoverTime = 1;    //time to smoothe back to normal time
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, deflectRadius);

        Gizmos.color = Color.black;

        Gizmos.DrawWireSphere(lastDeflect, lastRadius);
        Gizmos.DrawSphere(lastDeflect, 0.5f);
    }







    //public GameObject Ball;
    // public int BallAmmo = 1;
    // public float ballRechargeTime = 5;
    // public float CurrentBallRecharge;
    /*
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
    */
    //  public float maxDeflectRadius = 4;
    // private float chargeProgress;
    //  private float chargeTime = 0.6f;

    //  private bool ChargingDeflect;
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