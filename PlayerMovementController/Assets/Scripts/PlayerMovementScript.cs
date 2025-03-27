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
    private float jumpForce = 500f;
    private float maxSpeed = 5f;
    private Vector3 dirVec = new Vector3();
    public Rigidbody playerRb;
    public Camera cam;
    private bool moving = false;
    private State curState;
    private enum State{
        INVALID =   0,
        GROUNDED =  1,
        WALKING =   2,
        AIRBORNE =  3,
        SLIDING =   4,
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
        if (context.started) this.curState = State.AIRBORNE;
        if (context.performed)     
        {   
            this.dirVec.y = 1;
            this.dirVec = cam.transform.TransformVector(dirVec).normalized;
            playerRb.AddForce(dirVec * this.jumpForce);      
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
            this.curState = State.GROUNDED;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.curState = State.GROUNDED;
    }
    void FixedUpdate()
    {
            if (this.curState == State.WALKING)
            {
                Vector3 temp = this.dirVec;
                temp = cam.transform.TransformVector(temp).normalized;
                temp.y = 0;
                if(this.playerRb.velocity.magnitude <= this.maxSpeed) playerRb.AddForce(temp * this.moveForce);
                Debug.DrawLine(playerRb.position, playerRb.position + temp);
            }
        
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
                this.moving = true;
                Vector3 temp = this.dirVec;
                temp.x = context.ReadValue<Vector2>().x;
                temp.z = context.ReadValue<Vector2>().y;
                this.dirVec = temp;
            }
            if (context.canceled)
            {
                
                this.moving = false;
                this.StopMoving();
            }
        }
    }
}
