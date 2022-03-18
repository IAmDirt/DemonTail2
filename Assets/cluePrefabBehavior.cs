using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class cluePrefabBehavior : MonoBehaviour
{

    private clueVisualization visualisation;

    public Image sprite;
    public TextMeshProUGUI text;
    public void setVisuals(clueVisualization newVisual)
    {
        visualisation = newVisual;

        if (visualisation.sprite1)
            sprite.sprite = visualisation.sprite1;
        else
            sprite.enabled = false;
        text.text = visualisation.clueDescription;
    }
}