using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRotate : MonoBehaviour
{
    public float speed = 1;
    public float RotAngleY = 45;

    private Transform RotationParent;

    private float startOffset;
    void Start()
    {
        RotationParent = transform.GetChild(0).GetComponent<Transform>();
        startOffset= Random.Range(0, 0.3f);
    }
    void Update()
    {
        float rY = Mathf.SmoothStep(-RotAngleY, RotAngleY, Mathf.PingPong(Time.time * (speed+ startOffset), 1));
        RotationParent.localRotation = Quaternion.Euler(rY, 0, 0);
    }
}
