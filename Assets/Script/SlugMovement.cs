using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raycasting;
//refference for movemetn https://catlikecoding.com/unity/tutorials/movement/sliding-a-sphere/

public class SlugMovement : movement
{
    public static Vector3 DefaultInput { get { return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); } }
    public static Vector3 CameraRelativeInput;

    private Rigidbody baseRb;
    private Camera cam;

    public float dashForce = 1;

    [Header("jumping")]
    public float jumpForce = 10;
    public int MaxAllowedJumps = 2;
    public int AllowedJumps = 2;

    private float groundCheckDelay = 0.6f; //disable ground check when jumping
    private float currentDelay; //disable ground check when jumping
    private bool groundCheck = false;

    [Header("Audio")]
    public RandomAudioPlayer DashSound;
    public override void Awake()
    {
        base.Awake();
        baseRb = GetComponent<Rigidbody>();

    }
    public override void Start()
    {
        base.Start();
        cam = Camera.main;


        setSlowness(1);
    }

    public void Update()
    {
        jumpingStateLogic();
        grdInfo = GroundCheck();

        if (groundCheck)
            grounded = IsGrounded();
        else
            grounded = true;


        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Space))
        {
            if (!isDashing && canDash)
                StartCoroutine(StartDash());
        }
    }


    private bool LockRotation = true;
    private bool LockRotationHorizontal = true;
    public void lockrotation(bool isLocked, bool horizontal = false)
    {
        LockRotation = isLocked;
        LockRotationHorizontal = horizontal;

        if (LockRotation)
        {
            baseRb.constraints = RigidbodyConstraints.FreezeRotation;
            baseRb.rotation = Quaternion.LookRotation(new Vector3(0, 0, 1));
        }
       // else
           // baseRb.constraints = RigidbodyConstraints.None;
    }
    public void FixedUpdate()
    {
        if (showDebug) { drawDebug(); }

        CameraRelativeInput = DefaultInput;// InputRelativetoCamera();// turn(InputVecRelativeToCamera);
        if (LockRotationHorizontal)
        {
            faceInput();
        }
        else if (!LockRotation)
        faceCursor();
      
        //  tiltVelocityDirection();
        ArtificialGravity();
        CorrectionUp();

        if (grounded && !jumping && !isDashing)
        {
            addingMovement(CameraRelativeInput, data.walkParamaters, baseRb);   //make a list for movement (instead of hard coded)
            data.setCurrentSpeed(baseRb.velocity.magnitude, data.walkParamaters.maxSpeed );
        }


        if (!grounded || jumping)  //inputRelativeToPlayer works for movement
            return;
        ForceBodyStand();
    }

    public void faceCursor()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            var lookDirection = new Vector3(hit.point.x, transform.position.y, hit.point.z);

            lookDirection = lookDirection - transform.position;
            lookDirection = lookDirection.normalized;

            rotateBody(lookDirection, baseRb);
        }
        else
        {
            rotateBody(CameraRelativeInput, baseRb);
        }
    }
    public void faceInput()
    {

            var lookDirection = new Vector3(DefaultInput.x, 0, 1);
            lookDirection = lookDirection.normalized;

            rotateBody(lookDirection, baseRb);
    }
    [Header("Dash")]
    public ParticleSystem dashParticles;
    public float dashDuration = 1;
    public float dashSpeed = 1;
    private bool isDashing;
    private bool canDash = true;

    public GameObject Mesh;
    public IEnumerator StartDash()
    {
        isDashing = true;
        canDash = false;
        var dashProgress = 0.0f;
        var dashDirection = CameraRelativeInput.magnitude > 0 ? CameraRelativeInput : transform.forward;
        dashDirection = dashDirection.normalized;

        //visual flare
        var currentScale = Mesh.transform.localScale;
        var dashScale = new Vector3(1.8f ,  0.15f, 3f) * 1.4f; 

        LeanTween.scale(Mesh, dashScale, 0.5f)
        .setEasePunch();
        dashParticles.Play();
        DashSound.PlayRandomClip();
        while (dashProgress < dashDuration)
        {
            dashProgress += Time.deltaTime;

            baseRb.velocity = dashDirection * dashSpeed;

            yield return new WaitForFixedUpdate();
        }
        dashParticles.Stop();
        isDashing = false;

        yield return new WaitForSeconds(0.45f);
        canDash = true;
    }

    #region jumping
    public void jumpingStateLogic()
    {
        if (!jumping)
            return;

        if (currentDelay > 0)
            currentDelay -= Time.deltaTime;
        else
        {
            groundCheck = true;
            if (grounded)
            {
                jumping = false;
                AllowedJumps = MaxAllowedJumps;
            }
        }
    }
    public void jump()
    {
        if (IsGrounded() && !jumping || AllowedJumps >= 1)
        {
            if (AllowedJumps == 1)
            {
                baseRb.velocity -= new Vector3(0, baseRb.velocity.y * 0.65f, 0);
                StartCoroutine(wickedFlip(0.5f));
            }
            else
                AllowedJumps--;
            jumping = true;
            groundCheck = false;
            currentDelay = groundCheckDelay;

            var direction = grdInfo.groundNormal;//
            direction = direction * jumpForce;

            foreach (var rb in allRigidbodies)
            {
                rb.AddForce(direction, ForceMode.VelocityChange);
            }
        }
    }
    IEnumerator wickedFlip(float duration)
    {
        Vector3 startRotation = transform.eulerAngles;
        float endRotation = startRotation.x + 360.0f;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation.x, endRotation, t / duration) % 360.0f;
            transform.eulerAngles = new Vector3(yRotation, startRotation.y, startRotation.z);
            yield return null;
        }
    }
    /*public void dash()
    {
        if (!grounded)
            AllowedJumps = 1;
        var direction = InputRelativeToGround() * dashForce;
        foreach (var rb in allRigidbodies)
        {
            rb.AddForce(direction, ForceMode.VelocityChange);
        }
    }*/
    #endregion

    #region input Calculations
    private Vector3 InputRelativeToGround()
    {
        var inputRelativeCamera = InputRelativetoCamera();

        //find half way point 
        Vector3 halfWayNormal = Vector3.up * 2 + grdInfo.groundNormal;
        halfWayNormal = halfWayNormal.normalized;

        Vector3 input = Vector3.ProjectOnPlane(inputRelativeCamera, halfWayNormal); //find correct input
        input = Vector3.ProjectOnPlane(input, grdInfo.groundNormal);
        input = Vector3.ClampMagnitude(input, 1f); //normalized (smmoth)

        float magnitude = input.magnitude;
        return (magnitude <= 1) ? input : input /= magnitude;
    }


    public Vector3 InputRelativetoCamera()
    {
        var inpoutRelativeCamera = cam.transform.TransformDirection(DefaultInput);
        inpoutRelativeCamera.y = 0;

        return inpoutRelativeCamera;
    }

    #endregion
}