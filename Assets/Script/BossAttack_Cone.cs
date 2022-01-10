using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack_Cone : MonoBehaviour
{
    public void Animate(Vector3 spawnPos, Vector3 target)
    {

        StartCoroutine(animatie(spawnPos,target));
    }

        

    private IEnumerator animatie(Vector3 spawnPos, Vector3 target)
    {
        LeanTween.move(gameObject, spawnPos + Vector3.up * 6, 0.5f)
        .setEaseOutElastic();
        yield return new WaitForSeconds(0.5f);

        LeanTween.move(gameObject, target, 0.4f)
        .setEaseOutBounce();
    }
}
