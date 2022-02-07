using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public AudioClip bossIntro;
    public AudioClip BossLoop1;
    public AudioClip BossBuildUp2;
    public AudioClip BossLoop2;
    public AudioClip bossOutro;


    [Header("Sources")]
    public AudioSource Current;
    public AudioSource Fade;
    void Start()
    {
        Intro();
      //  waitTime -= 1414;
       // Current.time = 14;
        NextSwitch = waitTime;


    }

        private float waitTime = 17f;
        private float NextSwitch;

    void Update()
    {

        if (NextSwitch < 0)
        {
            fadeLoop1();
            NextSwitch = waitTime +1000;
        }
        else
            NextSwitch -= Time.unscaledDeltaTime ;
       // Debug.Log(NextSwitch - 14);
    }

    public void Intro()
    {

        Current.clip = bossIntro;
        StartCoroutine(fadeInNew(Current, 1));
    }
    private Task fade1;
    private Task fade2;

    public void fadeLoop1()
    {
        Debug.Log("change");
        Fade.clip = BossLoop1;
        StartCoroutine(fadeInNew(Fade, 0.6f, Current));

    }
    public void fadeLoop2()
    {
        Fade.clip = BossLoop2;
        StartCoroutine(fadeInNew(Fade, _fadeTime, Current));
    }
    public void fadeOutro()
    {
        Fade.clip = bossOutro;
        StartCoroutine(fadeInNew(Fade, _fadeTime, Current));
    }

    public IEnumerator fadeInNew(AudioSource newAudio,  float fadeTime, AudioSource currentAudio = null)
    {
      if(fade1 != null)  fade1.Stop();
        if (fade2 != null) fade2.Stop();

        fade1 = new Task(FadeAudio(newAudio, fadeTime, true));                      //fadeIn
       if(currentAudio) fade2 = new Task(FadeAudio(currentAudio, fadeTime, false)); //fadeOut

        var newCurrent = newAudio;
        var newFade = currentAudio;
        if (newCurrent) Current = newCurrent;
        if (newFade) Fade = newFade;
        yield return null;
    }

    private float _fadeTime =2;
    public IEnumerator FadeAudio(AudioSource audio, float fadeTime, bool fadeIn = true)
    {
        var fadeGhoal = fadeIn ? 1f : 0f;
        var fadeCurrent = fadeIn ? 0f : 1f;
        var timeElapsed = 0.0f;
        if(fadeTime <=0.001f)
        {
            fadeGhoal = fadeIn ? 1f : 0f;
            fadeCurrent = fadeIn ? 0.999f : 0.001f;
        }
            Debug.Log(fadeCurrent + " "+ fadeGhoal);
        if (!audio.isPlaying) audio.Play();
        while (ReachedGhoal(fadeIn, fadeCurrent, fadeGhoal))
        {
            timeElapsed += Time.deltaTime;
            fadeCurrent = Mathf.Lerp(fadeCurrent, fadeGhoal, timeElapsed / fadeTime);
            audio.volume = fadeCurrent;

            yield return null;
        }
    }
    private bool ReachedGhoal(bool fadeIn, float current, float ghoal)
    {
        return !fadeIn ? current > ghoal : current < ghoal;
    }

}
