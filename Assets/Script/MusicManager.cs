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
       // waitTime -= 90;
        Current.time = 10;
        NextSwitch = waitTime ;


    }

        private float waitTime = 16f;
        private float NextSwitch;

    void FixedUpdate()
    {
        
        if (NextSwitch <= Current.time)
        {
            fadeLoop1();
            NextSwitch = waitTime +1000;
        }
       // Debug.Log(NextSwitch - 14);
    }

    public void Intro()
    {
        Current.clip = bossIntro;

     

        fadeInNew(Current, 0.01f, 0.01f);
    }
    private Task fade1;
    private Task fade2;

    public void fadeLoop1()
    {
        Fade.clip = BossLoop1;
        Fade.Play();
        fadeInNew(Fade, 0.01f, 4.5f, Current);
    }
    public void fadeLoop2()
    {
        Fade.clip = BossLoop2;
        fadeInNew(Fade, _fadeTime, _fadeTime, Current);
    }
    public void fadeOutro()
    {
        Fade.clip = bossOutro;
        fadeInNew(Fade, _fadeTime, _fadeTime, Current);
    }

    public void fadeInNew(AudioSource newAudio,  float fadeTime, float fadeTimeCurrent, AudioSource currentAudio = null)
    {
      if(fade1 != null)  fade1.Stop();
        if (fade2 != null) fade2.Stop();

        fade1 = new Task(FadeAudio(newAudio, fadeTime, true));                      //fadeIn
       if(currentAudio) fade2 = new Task(FadeAudio(currentAudio, fadeTimeCurrent, false)); //fadeOut

        var newCurrent = newAudio;
        var newFade = currentAudio;
        if (newCurrent) Current = newCurrent;
        if (newFade) Fade = newFade;
    }

    private float _fadeTime =1f;
    public IEnumerator FadeAudio(AudioSource audio, float fadeTime, bool fadeIn = true)
    {
        var fadeGhoal = fadeIn ? 1f : 0f;
        var fadeCurrent = fadeIn ? 0f : 1f;
        var timeElapsed = 0.0f;

        if (fadeTime <= 0.01f)
        {
            fadeGhoal = fadeIn ? 1f : 0f;
            fadeCurrent = fadeIn ? 0.999f : 0.001f;
        }
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
