using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FIMSpace.FTail;

public class wingPhysics : MonoBehaviour
{

    public List<TailAnimator2> wingCollection = new List<TailAnimator2>();
    public void enablePhysics()
    {
        enabled = true;
    }
    public void disablePhysics()
    {
        enabled = false;
    }

    public bool enabled = true;
    public float Speed = 100;

    public void Update()
    {
        var scaleDirection = enabled ? 1 : -1;

        var step = Time.deltaTime * (Speed * scaleDirection);
        foreach (var wing in wingCollection)
        {
            wing.TailAnimatorAmount = Mathf.Clamp(wing.TailAnimatorAmount + 
                step, 0.45f, 1);
        }
    }

}
