using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PMovement : MonoBehaviour
{
    /*```````````````````````````````````````````
     * https://www.youtube.com/watch?v=xCxSjgYTw9c
     * https://youtu.be/QRYGrCWumFw?t=290
     * Fix dash not going up
     * add momentum
     * fix floaty jump
     * tweak dash dur
     * make run better
     * only 1 dash in air
     * sliding
     * wall stuff later
     ``````````````````````````````````````````*/
    [Header("Movement Script")]
    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode dashKey = KeyCode.Q;

    [Header("Assignables")]
    [SerializeField] Transform pos;
    [SerializeField] Rigidbody rb;
    [SerializeField] Camera cam;

    [Header("Movement Variables")]
    [SerializeField] MoveState state;

    [Header("FOV Variables")]
    [SerializeField] float pFOVWalk;
    [SerializeField] float pFOVRun;
    [SerializeField] float pFOVAir;
    [SerializeField] float pFOVDash;
    [SerializeField] float pFOVSmoothing = 0.5f;

    [Header("Ground Movement Variables")]
    [SerializeField] float pSpeedWalk;
    [SerializeField] float pSpeedRun;
    private float pSpeed;

    [Header("Air Movement Variables")]
    [SerializeField] float pJumpForce;
    [SerializeField] float pJumpCooldown;
    [SerializeField] float pAirMultiplier;
    bool canJump = true;

    [Header("Dash Variables")]
    [SerializeField] float pDashForce;
    [SerializeField] float pDashSpeed;
    [SerializeField] float pDashForceUp;
    [SerializeField] float pDashDuration;
    [SerializeField] float pDashCooldown;
    bool dashing = false;   

    [Header("Drag Variables")]
    [SerializeField] float groundDrag;
    [SerializeField] float airDrag;

    [Header("Ground Check Variables")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float playerHeight;
    bool grounded;

    Vector3 moveDir;
    float xInput;
    float yInput;

    private void Start()
    {
        rb.GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        GetInput();
        SpeedController();
        StateHandler();

        if (grounded)
            rb.drag = groundDrag;
        else if (!grounded)
            rb.drag = airDrag;

        if (pDashCooldown > 0)
        {
            pDashCooldown -= Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        Movement();
    }


    public enum MoveState
    {
        walking,
        sprinting,
        air,
        dashing
    }
    void StateHandler()
    {
        if (dashing)
        {
            state = MoveState.dashing;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, pFOVDash, pFOVSmoothing);
            pSpeed = pDashSpeed;
        }
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MoveState.sprinting;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, pFOVRun, pFOVSmoothing);
            pSpeed = pSpeedRun;
        }
        else if (grounded)
        {
            state = MoveState.walking;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, pFOVWalk, pFOVSmoothing);
            pSpeed = pSpeedWalk;
        }
        else if (!grounded && Input.GetKey(sprintKey))
        {
            state = MoveState.air;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, pFOVRun, pFOVSmoothing);
        }
        else if (!grounded && dashing)
        {
            state = MoveState.dashing;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, pFOVDash, pFOVSmoothing);
            pSpeed = pDashSpeed;
        }
        else
        {
            state = MoveState.air;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, pFOVWalk, pFOVSmoothing);
        }
    }

    void GetInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && canJump && grounded)
        {
            canJump = false;
            Jump();
            Invoke(nameof(JumpReset), pJumpCooldown);
        }
        else if (Input.GetKey(dashKey))
        {
            Dash();
        }
        
    }

    void Movement()
    {
        moveDir = pos.forward * yInput + pos.right * xInput;

        if (grounded)
            rb.AddForce(moveDir.normalized * pSpeed * 10f, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(moveDir.normalized * pSpeed * 10f * pAirMultiplier, ForceMode.Force);
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * pJumpForce, ForceMode.Impulse);
    }

    void JumpReset()
    {
        canJump = true;
    }

    void Dash()
    {
        if (pDashCooldown > 0) return;
        else pDashCooldown = 1;

        dashing = true;
        Vector3 force = pos.forward * pDashForce + pos.up * pDashForceUp;
        forceapplydelay = force;
        Invoke(nameof(DashForceDelay), 0.025f);
        Invoke(nameof(DashReset), pDashDuration);
    }

    private Vector3 forceapplydelay;
    void DashForceDelay()
    {
        rb.AddForce(forceapplydelay, ForceMode.Impulse);
    }

    void DashReset()
    {
        dashing = false;
    }

    void SpeedController()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > pSpeed)
        {
            Vector3 limit = flatVel.normalized * pSpeed;
            rb.velocity = new Vector3(limit.x, rb.velocity.y, limit.z);
        }
    }

}
