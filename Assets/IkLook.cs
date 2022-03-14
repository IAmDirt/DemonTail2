using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkLook : MonoBehaviour
{
    [System.Serializable]
    public class IkTarget
    {
        //public ParticleSystem smashParticle;
        public bool isRight;
        public bool isIdle = true;
        public Transform target;
        [HideInInspector] public Vector3 StartOffset;

        [Header("Wiggle")]

        public Vector3 WiggleMagnitude = Vector3.one;
        public Vector3 magnitudeOffset = Vector3.zero;
        public float scaleModifyer = 1;
        public float nextChangeTimer = 0.2f;
        public float wiggleSpeed = 10;

        [HideInInspector] public Vector3 wiggleOffset;
        [HideInInspector] public Vector3 vel;
        [HideInInspector] public float directionChangeTimer;
    }
    public IkTarget head;
    //  public IkTarget rightHand_IK;
    //  public IkTarget Root_IK;

    public void Start()
    {
        head.StartOffset = head.target.transform.position;
    }
    public void Update()
    {
        WiggleAnim(head);
    }
  

    public void WiggleAnim(IkTarget IKData)
    {
        if (!IKData.isIdle)
            return;

        if (IKData.directionChangeTimer <= 0)
        {
            changeDirectionWiggle(IKData);
        }
        else
            IKData.directionChangeTimer -= Time.deltaTime;

        var targetPosition = IKData.StartOffset + transform.InverseTransformPoint(transform.position + IKData.wiggleOffset);

        IKData.target.localPosition = Vector3.SmoothDamp(IKData.target.localPosition, targetPosition, ref IKData.vel, Time.deltaTime * IKData.wiggleSpeed);
    }

    public void changeDirectionWiggle(IkTarget IKData)
    {
        var offsetMagnitude = IKData.WiggleMagnitude;
        var xPos = Random.Range(-offsetMagnitude.x, offsetMagnitude.x) + IKData.magnitudeOffset.x;
        var yPos = Random.Range(-offsetMagnitude.y, offsetMagnitude.y) + IKData.magnitudeOffset.y;
        var zPos = Random.Range(-offsetMagnitude.z, offsetMagnitude.z) + IKData.magnitudeOffset.z;

        IKData.wiggleOffset = new Vector3(xPos, yPos, zPos);
        IKData.wiggleOffset *= IKData.scaleModifyer;

        IKData.directionChangeTimer = IKData.nextChangeTimer * Random.Range(0.4f, 1.7f);
    }
}