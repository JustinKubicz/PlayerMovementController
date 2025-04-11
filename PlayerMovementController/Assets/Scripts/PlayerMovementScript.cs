using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Windows;

public class PlayerMovementScript : MonoBehaviour, PlayerMovement.IPlayerControlsActions
{ //CONFIGURABLES
    [Range(0f, 1f)]
    public float accelerationRate = 1f;

    [Range(0f, 1f)]
    public float stoppingPower = 1f;

    [Range(0f, 1f)]
    public float jumpForce = 1f;

    [Range(0f, 1f)]
    public float maxSpeed = 1f;
    public Rigidbody playerRb;
    public Camera cam;

    //INTERNALS
    private PlayerMovement inpt;
    private Vector3 dirVec = new Vector3();
    private State curState;
    private bool jumped = false;
    private bool coyoteTimerStarted = false;
    private bool decelerate = false;

    private enum State
    {
        INVALID = 0,
        GROUNDED = 1,
        WALKING = 2,
        AIRBORNE = 3,
        SLIDING = 4,
        COYOTETIME = 5,
    }

    public void OnEnable()
    {
        if (this.inpt == null)
        {
            this.inpt = new PlayerMovement();
            this.inpt.PlayerControls.SetCallbacks(this);
        }
        this.inpt.Enable();
    }

    public void OnDisable()
    {
        if (this.inpt != null)
        {
            this.inpt.Disable();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (this.curState != State.AIRBORNE)
            {
                this.curState = State.AIRBORNE;
                this.jumped = true;
                this.dirVec.y = 1;
                this.dirVec = cam.transform.TransformVector(dirVec).normalized;
                playerRb.AddForce(dirVec * (this.jumpForce * 800f));
            }
        }
    }

    private void StopMoving()
    {
        this.curState = State.GROUNDED;
        if (this.playerRb.velocity != Vector3.zero)
        {
            this.decelerate = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (this.curState == State.AIRBORNE)
        {
            RaycastHit hit;
            if (
                Physics.Raycast(
                    this.transform.position,
                    this.transform.TransformDirection(Vector3.down),
                    out hit,
                    Mathf.Infinity
                )
            )
            {
                if (hit.distance <= 1.5)
                {
                    this.StopMoving();
                    this.jumped = false;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start() { }

    void FixedUpdate()
    {
        if (this.curState == State.WALKING)
        {
            RaycastHit slopes;
            Vector3 rayEnd = -this.transform.up;
            if (Physics.Raycast(this.transform.position, rayEnd, out slopes))
            {
                Debug.DrawRay(slopes.point, slopes.normal * 10, Color.cyan);
                Debug.DrawRay(slopes.point, Vector3.up * 10, Color.cyan);
                //SLOPES LOGIC BEING BUILT HERE!
                Debug.Log(
                    "Angle between surface normal and Vector3.up: "
                        + Vector3.Angle(slopes.normal, Vector3.up)
                );
                if (Vector3.Angle(slopes.normal, Vector3.up) > 30)
                {
                    //Player can't walk that slope
                    this.dirVec.z = -this.dirVec.z;
                }

                Vector3 temp = this.dirVec;
                temp = cam.transform.TransformVector(temp).normalized;
                temp.y = 0;
                if (this.playerRb.velocity.magnitude <= (this.maxSpeed * 8f))
                    playerRb.AddForce((this.accelerationRate * 1000) * Time.fixedDeltaTime * temp);
            }
        }
        RaycastHit hit;
        if (
            Physics.Raycast(
                this.transform.position,
                this.transform.TransformDirection(Vector3.down),
                out hit,
                Mathf.Infinity
            )
        )
        {
            if (hit.distance >= 2 && !this.jumped && !(this.curState == State.AIRBORNE))
                this.curState = State.COYOTETIME;
        }
        if (this.curState == State.COYOTETIME && !this.coyoteTimerStarted)
        {
            StartCoroutine(CoyoteTimer());
        }
        if (
            this.decelerate
            && !(this.playerRb.velocity.x <= 0.01f && this.playerRb.velocity.z <= 0.01f)
        )
        {
            this.playerRb.AddForce(
                Time.fixedDeltaTime * (this.stoppingPower * 1000) * -this.playerRb.velocity
            );
            Debug.Log("player vel: " + this.playerRb.velocity);
            Debug.Log(this.decelerate);
        }
        else if (this.playerRb.velocity.x <= 0.01f && this.playerRb.velocity.z <= 0.01f)
            this.decelerate = false;
    }

    IEnumerator CoyoteTimer()
    { //https://docs.unity3d.com/6000.0/Documentation/ScriptReference/WaitForSeconds.html
        Debug.Log(this.curState);
        this.coyoteTimerStarted = true;
        yield return new WaitForSeconds(0.25f);
        this.curState = State.AIRBORNE;
        Debug.Log(this.curState);
        this.coyoteTimerStarted = false;
    }

    // Update is called once per frame
    void Update() { }

    public void OnWalk(InputAction.CallbackContext context)
    {
        if (
            (this.curState == State.GROUNDED || this.curState == State.WALKING)
            && !(this.curState == State.AIRBORNE)
        )
        {
            if (context.performed)
            {
                this.curState = State.WALKING;

                Vector3 temp = this.dirVec;
                temp.x = context.ReadValue<Vector2>().x;
                temp.z = context.ReadValue<Vector2>().y;
                this.dirVec = temp;
            }
            if (context.canceled)
            {
                this.StopMoving();
            }
        }
    }
}
