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

        if (currentHealth <= 0)
        {
            deathEvent.Invoke();
            Kill();
        }
        if(useInvulnerableFrames)
        StartCoroutine( invulnerableFrames());
    }

    private IEnumerator invulnerableFrames()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerableTime);
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
        Debug.Log("I died");
    }
}