using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallVoid : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.layer == 8)  //enviroment
        {
            var ball = collision.transform.GetComponent<BallBehavior>();

            if (ball)
            {
                Destroy(ball.gameObject);
            }
        }

    }
}
