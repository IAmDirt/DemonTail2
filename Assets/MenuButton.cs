using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class MenuButton : MonoBehaviour
{
    public bool startdeselect= true;
    public float delayFadeIn;
    public EventSystem eventSystem;
    public void OnEnable()
    {
        if (!startdeselect)
            eventSystem.SetSelectedGameObject(gameObject);
        transform.localScale = Vector3.zero;
        Invoke("starteScaleIn", delayFadeIn * 0.8f);
    }
    public void Start()
    {
        if (startdeselect)
        {
            StartCoroutine(fadecolor(deselected, 0.1f));
        }

        transform.localScale = Vector3.zero;
        Invoke("starteScaleIn", delayFadeIn *0.8f);

    }

    public void starteScaleIn()
    {

        LeanTween.scale(gameObject, Vector3.one , 0.8f).setEaseOutQuart();
    }
    public void selelected()
    {
        LeanTween.scale(gameObject, Vector3.one * 0.6f, 0.4f).setEasePunch();
        StartCoroutine(fadecolor(select, 0.1f));
    }
    public void deselect()
    {
        LeanTween.scale(gameObject, Vector3.one , 0.4f).setEaseInOutCirc();
        StopAllCoroutines();

        backPanel.color = deselected;
        //StartCoroutine(fadecolor(deselected, 0.1f));
    }

    public TextMeshProUGUI text;
    public Image backPanel;

    public Color deselected;
    public Color select;
    public Color click;
    public void pressed()
    {
        LeanTween.scale(gameObject, Vector3.one *1.4f, 0.3f).setEasePunch();
        backPanel.color = click;
        StartCoroutine(pressedColor());
    }
    private IEnumerator pressedColor()
    {
        yield return new WaitForSeconds(0.2f);
        LeanTween.scale(gameObject, Vector3.one *0.9f, 0.2f).setEasePunch();
        StartCoroutine(fadecolor(select, 0.1f));
    }

    public IEnumerator fadecolor( Color wanted, float fadeTime)
    {
        if (!backPanel)
            yield break;
        var currentTime = 0f;
        while (currentTime <= fadeTime)
        {
            currentTime += Time.deltaTime;

            var prosentage = currentTime / fadeTime;
            Debug.Log(prosentage);
            backPanel.color = Color.Lerp(backPanel.color, wanted, prosentage);
            yield return null;
        }
    }
}
