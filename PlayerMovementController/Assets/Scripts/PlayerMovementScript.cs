using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovementScript : MonoBehaviour, PlayerMovement.IPlayerControlsActions


{

    private PlayerMovement inpt;
    private float moveForce = 50.0f;
    public float jumpForce = 600f;
    public float maxSpeed = 5f;
    private Vector3 dirVec = new Vector3();
    public Rigidbody playerRb;
    public Camera cam;
    private State curState;
    private bool jumped = false;
    private bool coyoteTimerStarted = false;
    private enum State{
        INVALID =   0,
        GROUNDED =  1,
        WALKING =   2,
        AIRBORNE =  3,
        SLIDING =   4,
        COYOTETIME = 5,
        }

   
    public void OnEnable()
    {
        if(this.inpt == null)
        {
            this.inpt = new PlayerMovement();
            this.inpt.PlayerControls.SetCallbacks(this);
        }
        this.inpt.Enable();
    }

    public void OnDisable()
    {
        if(this.inpt != null)
        {
            this.inpt.Disable();
        }
    }



    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)     
        {
            if (this.curState != State.AIRBORNE)
            {this.curState = State.AIRBORNE;
                this.jumped = true;
                this.dirVec.y = 1;
                this.dirVec = cam.transform.TransformVector(dirVec).normalized;
                playerRb.AddForce(dirVec * this.jumpForce);
            }
        }
    }
    

    private void StopMoving()
    {   
        this.curState = State.GROUNDED;
        this.playerRb.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(this.curState == State.AIRBORNE)
        {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, this.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
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
    void Start()
    {
    }
    void FixedUpdate()
    {
            if (this.curState == State.WALKING)
            {
                Vector3 temp = this.dirVec;
                temp = cam.transform.TransformVector(temp).normalized;
                temp.y = 0;
                if(this.playerRb.velocity.magnitude <= this.maxSpeed) playerRb.AddForce(temp * this.moveForce);
            Debug.Log(this.playerRb.velocity.magnitude);
            }
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position, this.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            if (hit.distance >= 2 && !this.jumped && !(this.curState == State.AIRBORNE)) this.curState = State.COYOTETIME;
        }
        if(this.curState == State.COYOTETIME && !this.coyoteTimerStarted)
        {
            StartCoroutine(CoyoteTimer());
        }

        
    }
    IEnumerator CoyoteTimer()
    {//https://docs.unity3d.com/6000.0/Documentation/ScriptReference/WaitForSeconds.html
        Debug.Log(this.curState);
        this.coyoteTimerStarted = true;
        yield return new WaitForSeconds(0.25f);
        this.curState = State.AIRBORNE;
        Debug.Log(this.curState);
        this.coyoteTimerStarted = false;

    }
    // Update is called once per frame
    void Update()
    {
      
    }

    public void OnWalk(InputAction.CallbackContext context)
    {
        
        if ((this.curState == State.GROUNDED || this.curState == State.WALKING) && !(this.curState == State.AIRBORNE))
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
