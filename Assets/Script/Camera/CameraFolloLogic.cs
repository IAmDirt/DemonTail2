using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFolloLogic : MonoBehaviour
{
    public GameManager manager;

    public float speed = 1;
    private Vector3 anchor;
    private Transform player;

    public Transform testDummy;

    public float baseLerp;
    public float baseZoomOutMax = 2;
    public float baseZoominMax = 0.5f;

    public float aimLerp;
    public Transform Camerashake_parent;
    public void Start()
    {
        player = manager.player;
        anchor = transform.position;
    }
    public Transform ClosestEnemy;
    public void Update()
    {
        var step = Time.deltaTime * speed;
        var playerPos = player.position;
        var targetPos = Vector3.zero;

        var modefiedAimLerp = aimLerp;

        var diffX = 0.5f;//change base based on distance in x to "ClosestEnemyPos"
        var diffZ = 0.5f;//change base based on distance in x to "ClosestEnemyPos"
        var modefiedBaseLerp = 0.5f;

        /*  if (testDummy != null)
          {

              diffX = Mathf.Abs(player.position.x - ClosestEnemyPos.x);
              diffX = Mathf.Clamp(diffX / 3, 0, 1);

              modefiedBaseLerp = Mathf.Lerp(baseLerp * 2, baseLerp, diffX);
          }
          else
          {*/


    if (testDummy != null)
            ClosestEnemy = testDummy;
        else
            ClosestEnemy = null;


       // if (ClosestEnemy != null)
            targetPos = ClosestEnemy.position;
        //else
        //    targetPos = manager.currentRoom.worldPos + new Vector3(0, 0, -1.5f);


        diffX = Mathf.Abs(player.position.x - targetPos.x);
        diffX = Mathf.Clamp(diffX / 4, 0, 1);

        diffZ = Mathf.Abs(playerPos.z - targetPos.z);
        diffZ = Mathf.Clamp(diffZ / 3, 0, 1);

        var zoomOffset = Vector3.Distance(playerPos, targetPos);
        zoomOffset = Mathf.Clamp(zoomOffset, 1.2f, 3f);


        if (ClosestEnemy != null)
        {
            modefiedBaseLerp = Mathf.Lerp(baseLerp * 2, baseLerp, diffX);
            modefiedAimLerp = Mathf.Lerp(0.3f, aimLerp , diffX-0.1f);
        }
        else
        {
            modefiedAimLerp = Mathf.Lerp(0.5f, 0.15f, diffZ);
            modefiedBaseLerp = Mathf.Lerp(0.5f, 0.8f, diffX);
            zoomOffset = 2.5f;
        }

        //set base
        var lerpbetween_base = Vector3.Lerp(playerPos, targetPos, modefiedBaseLerp);
        lerpbetween_base.z = Mathf.Clamp(lerpbetween_base.z, anchor.z - baseZoomOutMax, anchor.z + baseZoominMax);

        var baseDestination = new Vector3(lerpbetween_base.x, anchor.y + (zoomOffset * 0.5f) - 2.3f, lerpbetween_base.z - zoomOffset * 0.5f + 1.3f);
        transform.position = Vector3.MoveTowards(transform.position, baseDestination, step);


        //set aim
        var lerpbetween_aim = Vector3.Lerp(playerPos, targetPos, modefiedAimLerp);
        var aimDestination = new Vector3(lerpbetween_aim.x, anchor.y - zoomOffset * 0.35f + 0.2f, lerpbetween_aim.z);
        Camerashake_parent.position = Vector3.MoveTowards(Camerashake_parent.position, aimDestination, step);
    }

    public void setAnchor(Vector3 newAnchor, CinemachineFreeLook freeLook)
    {
        var lastPosition = transform.position;
        this.anchor = newAnchor + new Vector3(0, 0.8f, 0);
        transform.position = manager.player.position;
        Camerashake_parent.position = manager.player.position;

        freeLook.OnTargetObjectWarped(transform, transform.position - lastPosition - new Vector3(0, 1, 0));

    }


    public Transform colsestGo(List<Transform> Go, Vector3 from)
    {
        Transform closest;
        float distance = 1;
        closest = null;

        distance = Mathf.Infinity;
        Vector3 position = from;
        foreach (Transform tran in Go)
        {
            if (tran != null)
            {
                float curDistance = Vector3.Distance(tran.transform.position, position);
                if (curDistance < distance)
                {
                    closest = tran;
                    distance = curDistance;
                }
            }
        }
        return closest;
    }
}
