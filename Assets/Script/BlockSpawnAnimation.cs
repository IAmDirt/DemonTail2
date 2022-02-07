using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawnAnimation : MonoBehaviour
{
    public float moveDuration = 1;
    public float ArcHeight;
    public AnimationCurve ArcCurve;
    private Rigidbody rb;

    public void Start()
    {
    }

    public void spawnAnimation(Vector3 endPosition, Quaternion endRotation)
    {
        StartCoroutine(moverArc(endPosition, endRotation));

    }
    IEnumerator moverArc(Vector3 endPoint, Quaternion endrotation)
    {
        rb = GetComponent<Rigidbody>();

        moveDuration = Random.Range(moveDuration * 0.7f, moveDuration * 1.3f);
        ArcHeight = Random.Range(ArcHeight * 0.7f, ArcHeight * 1.3f);

        var startPoint = rb.transform.position;
        var timeElapsed = 0f;
        do
        {
            timeElapsed += Time.deltaTime;

            var prosentage = timeElapsed / moveDuration;
            var jumpArc = Vector3.up * ArcHeight * ArcCurve.Evaluate(prosentage);   //calculate height of jumpArc

            //move foot here
            var animationPoint = Vector3.Lerp(startPoint, endPoint, prosentage) + jumpArc;
            rb.MovePosition(animationPoint);
            yield return null;
        }
        while (timeElapsed <= moveDuration);
        //reached end location
    }
}
