using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorailTarget : MonoBehaviour
{

    Vector3 startSize;

    public void Start()
    {
        startSize = transform.localScale;
    }
    void Update()
    {


        transform.localScale = startSize * sizeModifyer;
    }


    public float sizeModifyer =1;

    public GameObject death;
    public void hover(TutorialManager manager)
    {
        sizeModifyer += Time.deltaTime / 1;

        if(sizeModifyer >= 2)
        {
            manager.aimTargetReached();
            Destroy(gameObject);
            PoolManager.Spawn(death.gameObject, transform.position, transform.rotation);
        }
    }
}
