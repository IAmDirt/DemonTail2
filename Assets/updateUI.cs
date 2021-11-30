using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class updateUI : MonoBehaviour
{
    [Header("fill")]
    public Image fillImage;

    private float maxFill;
    public void setMaxFill(int max)
    {
        maxFill = max;
    }

    public void setFill(int current)
    {
        fillImage.fillAmount = current / maxFill;
    }
}
