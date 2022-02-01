using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raycasting;

public class movement : MonoBehaviour
{
    [System.Serializable]
    public class LimbData
    {
        public Rigidbody rb;

        [Header("values")]
        public float rideHeight;
        public float rideSpringStrenght;
        public float RideSpringDamper;

        [Header("Correctional Forces")]
        public bool CorrectionUp;   //aply forces so rb so it rotes so up faces up
        public float correctionForce = 8;

        public Vector3 totalError = new Vector3(0, 0, 0);
        public Vector3 lastError = new Vector3(0, 0, 0);
    }

    public LayerMask groundLayer;
    public MovementDataCollection data;
    public LimbData[] Limbs;    //limbs that you add force to keep of ground

    [HideInInspector] public Rigidbody[] allRigidbodies;    //limbs that you add force to keep of ground
    private Vector3 m_goalVel;

    [Header("Rotation")]
    public float RotateSpeed = 0.5f;    //time it takes to rotate
    public float rotationTorque = 2;

    [Header("Grounding")]
    public bool useArtificialGravity;
    [Range(0, 10)]
    public float gravityMultiplier;
    [HideInInspector] public float rideHeadBob = 1;

    public bool jumping;
    public bool grounded;
    public bool stopStand;
    public bool showDebug;

    public Vector3 CurrentGravity { get { return /*(IsGrounded() ? -grdInfo.groundNormal : -Vector3.up)*/-Vector3.up * gravityMultiplier * 9.81f; } }

    public virtual void Awake()
    {
        downRayRadius = downRaySize;// * getColliderRadius();
        float forwardRayRadius = forwardRaySize;// * getColliderRadius();

        downRay = new SphereCast(transform.position + transform.up * 0.4f, -transform.up, downRayLength, downRayRadius, transform, transform);
        forwardRay = new SphereCast(transform.position - transform.forward * 0.4f, transform.forward, forwardRayLength, forwardRayRadius, transform, transform);
    }

    public virtual void Start()
    {

        if (useArtificialGravity)
        {
            allRigidbodies = transform.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in allRigidbodies)
            {
                rb.useGravity = false;
            }

            if (transform.TryGetComponent<Rigidbody>(out Rigidbody myRb))
                myRb.useGravity = false;
        }
    }
    private float slownessFactor =1;
    public void setSlowness(float f)
    {
        slownessFactor = Mathf.Clamp(f, 0.1f, 1f);
    }
    public bool IsGrounded()
    {
        return grdInfo.isGrounded;
    }

    #region ApplyingForces
    public void ArtificialGravity()
    {
        foreach (var rb in allRigidbodies)
        {
            if (!rb.useGravity)
                rb.AddForce(CurrentGravity, ForceMode.Acceleration); //Important using the groundnormal and not the lerping normal here!
        }
    }
    public void ForceBodyStand()
    {
        foreach (var limbData in Limbs)
        {
            if (!stopStand)
                ForceRbAtHeight(limbData);
        }
    }
    public void CorrectionUp()
    {
        foreach (var limbData in Limbs)
        {
            if (limbData.CorrectionUp)   //force rb to rotate so transform.up faces vector3.up
            {
                var rb = limbData.rb;
                rb.AddForceAtPosition(-CurrentGravity.normalized * limbData.correctionForce, rb.position + rb.transform.up);
                rb.AddForceAtPosition(CurrentGravity.normalized * limbData.correctionForce, rb.position - rb.transform.up);
            }
        }
    }
    public void ForceRbAtHeight(LimbData data) //reference https://youtu.be/qdskE8PJy6Q?t=133
    {
        Vector3 velocity = data.rb.velocity;
        Vector3 rayDir = CurrentGravity.normalized;

        Vector3 otherVelocity = Vector3.zero;

        float rayDirVel = Vector3.Dot(rayDir, velocity);
        float otherDirVel = Vector3.Dot(rayDir, otherVelocity);

        float relVelocity = rayDirVel - otherDirVel;
        float x = grdInfo.distanceToGround - (data.rideHeight + rideHeadBob);

        float springForce = (x * data.rideSpringStrenght) - (relVelocity * data.RideSpringDamper);

        if (x == Mathf.Infinity)
            return;
        data.rb.AddForce(rayDir * springForce - CurrentGravity);
    }
    public void addingMovement(Vector3 newInput, SpeedParamaters move, Rigidbody rb) //https://www.youtube.com/watch?v=qdskE8PJy6Q
    {
        if (newInput.magnitude < 0.01f && !grounded) //for external forces dampen (can apply force when not moving)
            move.maxSpeed = 0;

        var velDot = Vector3.Dot(newInput, m_goalVel.normalized);
        var accel = move.Acceleration * move.AccelerationFactorFromDot.Evaluate(velDot);
        var goalVel = newInput * move.maxSpeed * slownessFactor;

        
        var maxAccel = move.MaxAccelForce * move.MaxAccelerationForceFactorFromDot.Evaluate(velDot) * move.MaxAccelForce;

        m_goalVel = Vector3.MoveTowards(m_goalVel, goalVel , accel * Time.deltaTime);



        Vector3 neededAccel = ((m_goalVel) - rb.velocity) / Time.deltaTime;//actual force...
        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);

        rb.AddForce(Vector3.Scale(neededAccel * rb.mass, move.forceScale));  //now dont add force in y axis
    }
    public void addingAirMovement(Vector3 newInput, SpeedParamaters move, Rigidbody rb)
    {
        var maxSpeed = move.maxSpeed;
        if (newInput.magnitude < 0.01f && !grounded) //for external forces dampen (can apply force when not moving)
            maxSpeed = 0;

        Vector3 ForceScale = move.forceScale;
        Vector3 goalVel = newInput * maxSpeed;

        m_goalVel = Vector3.MoveTowards(m_goalVel, goalVel, move.Acceleration * Time.deltaTime);

        //actual force...
        Vector3 neededAccel = ((m_goalVel) - Vector3.ClampMagnitude(rb.velocity, maxSpeed * move.movementDampen)) / Time.deltaTime;
        neededAccel = Vector3.ClampMagnitude(neededAccel, move.MaxAccelForce);

        Debug.DrawLine(transform.position, transform.position + goalVel * 2, Color.white * Color.green);
        Debug.DrawLine(transform.position, transform.position + neededAccel * rb.mass, Color.white * Color.yellow);

        rb.AddForce(Vector3.Scale(neededAccel * rb.mass, ForceScale));  //now dont add force in y axis
    }
    public void rotateBody(Vector3 direction, Rigidbody rb, bool debug = false)
    {
        //physics
        /*
        var forward = rb.transform.forward;
        var normalizedDirection = direction.normalized;

        var dirToRotate = Vector3.RotateTowards(forward, normalizedDirection, Time.deltaTime * rotateSmoothSpeed, 0.0f);  //slow down turning

        rotateRBInDirection(rb, dirToRotate * rotationTorque);
        */

        //non physics
        float singleStep = RotateSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, singleStep, 0.0f);
        Debug.DrawRay(transform.position, newDirection, Color.red);

        transform.rotation = Quaternion.LookRotation(newDirection);
   
    }
    private void rotateRBInDirection(Rigidbody rigidbody, Vector3 force, float upcorrection = 1)
    {
        //tug on the front
        rigidbody.AddForceAtPosition(force, rigidbody.position + rigidbody.transform.forward, ForceMode.VelocityChange);
        //tug on the back
        rigidbody.AddForceAtPosition(-force, rigidbody.position - rigidbody.transform.forward, ForceMode.VelocityChange);
    }
    #endregion

    #region SphereCast SpiderImport

    public void drawDebug()
    {
        //Draw the two Sphere Rays
        downRay.draw(Color.green);
        forwardRay.draw(Color.blue);
    }

    public enum RayType { None, ForwardRay, DownRay };
    public struct groundInfo
    {
        public bool isGrounded;
        public Vector3 groundNormal;
        public float distanceToGround;
        public RayType rayType;

        public groundInfo(bool isGrd, Vector3 normal, float dist, RayType m_rayType)
        {
            isGrounded = isGrd;
            groundNormal = normal;
            distanceToGround = dist;
            rayType = m_rayType;
        }
    }

    [Header("Ray Adjustments")]
    [Range(0.0f, 1.0f)]
    public float forwardRayLength;
    [Range(0.0f, 2.0f)]
    public float downRayLength;
    [Range(0.1f, 2.0f)]
    public float forwardRaySize = 0.66f;
    [Range(0.1f, 1.0f)]
    public float downRaySize = 0.9f;
    private float downRayRadius;

    public groundInfo grdInfo;
    // public Vector3 SmoothedgroundNormal;

    private bool groundCheckForward = false;
    private bool groundCheckDown = true;
    private bool groundCheckOn = true;
    private SphereCast downRay, forwardRay;
    private RaycastHit hitInfo;

    public groundInfo GroundCheck()
    {
        if (groundCheckOn)
        {
            if (forwardRay.castRay(out hitInfo, groundLayer) && groundCheckForward)
            {
                //  Debug.DrawLine(transform.position, hitInfo.point, Color.yellow);
                return new groundInfo(true, hitInfo.normal.normalized, Vector3.Distance(transform.position, hitInfo.point), RayType.ForwardRay);
            }

            if (downRay.castRay(out hitInfo, groundLayer) && groundCheckDown)
            {
                //  Debug.DrawLine(transform.position, hitInfo.point, Color.yellow);
                return new groundInfo(true, hitInfo.normal.normalized, Vector3.Distance(transform.position, hitInfo.point), RayType.DownRay);
            }
        }
        return new groundInfo(false, Vector3.up, float.PositiveInfinity, RayType.None);
    }
    #endregion
}