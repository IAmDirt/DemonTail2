using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Health : MonoBehaviour
{
    //visuals

    //stats
    [Header("Stats")]
    public int maxHealth = 1;
    public int currentHealth = 0;

    [Header("Events")]
    public UnityEvent deathEvent;
    public UnityEvent hitEvent;

    [Header("Invulnerable")]
    public bool useInvulnerableFrames = false;
    public float invulnerableTime = 1f;
    private bool isInvulnerable;
    private bool isDead;
    public void Awake()
    {
        Init();
    }
    private void Init()
    {
        currentHealth = maxHealth;
    }

    public virtual void takeDamage(int amount)
    {
        if (isInvulnerable)
            return;
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

        updateVisuals();
        hitEvent.Invoke();

        if (currentHealth <= 0 && !isDead)
        {
            deathEvent.Invoke();
            Kill();
        }
        if (useInvulnerableFrames)
            startInvonrableFrames(invulnerableTime);
                }
    public void startInvonrableFrames(float duration)
    {
        StartCoroutine( invulnerableFrames(duration));
    }
    private IEnumerator invulnerableFrames(float duration)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
    }
    private void updateVisuals()
    {

    }
    [Header("audio")]
    public RandomAudioPlayer DamageAudio;
    public void hitAudio()
    {
        DamageAudio.PlayRandomClip();
    }


    public virtual void Kill()
    {
        isDead = true;
        Debug.Log("I died");
    }
}