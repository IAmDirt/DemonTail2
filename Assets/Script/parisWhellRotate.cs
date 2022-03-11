using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parisWhellRotate : MonoBehaviour
{
    public float speed = 1;
    public float RotAngleY = 45;

    public Transform RotationParent;

    public GameObject[] carts;
    private Quaternion startRotation;
    private void Start()
    {
        startRotation = carts[0].transform.rotation;
    }

    void Update()
    {
        foreach (var item in carts)
        {
            item.transform.rotation= startRotation;
        }
        float rY = Time.time * speed ;
        RotationParent.localRotation = Quaternion.Euler(rY, 0, 0);
    }
}
