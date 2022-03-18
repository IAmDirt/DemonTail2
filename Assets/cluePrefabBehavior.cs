using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class cluePrefabBehavior : MonoBehaviour
{

    private clueVisualization visualisation;

    public Image sprite;
    public Sprite _sprite1;
    public Sprite _sprite2;
    public TextMeshProUGUI text;
    public void setVisuals(clueVisualization newVisual, Sprite sprite1, Sprite sprite2)
    {
            sprite.enabled = false;
        visualisation = newVisual;

        if (sprite1 != null)
        {
            sprite.sprite = visualisation.sprite1;
            sprite.enabled = true;
            animate = true;

            _sprite1 = sprite1;
            _sprite2 = sprite2;
        }
        text.text = visualisation.clueDescription;
    }
    public bool animate;
    private bool firstSprite;

    public void OnEnable()
    {
        if(animate)
            StartCoroutine(spriteAnim());
    }
    public void OnDisable()
    {
        StopAllCoroutines();
        animate = false;
    }
    public IEnumerator spriteAnim()
    {
        var newSprite = firstSprite ? _sprite1 : _sprite2;
        sprite.sprite = newSprite;
        firstSprite = !firstSprite;
       yield return new WaitForSeconds(1);
        StartCoroutine(spriteAnim());
    }
}