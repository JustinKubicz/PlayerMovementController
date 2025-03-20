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
    private bool movingF = false;
    private bool movingL = false;
    private bool movingR = false;
    private bool movingB = false;
    private float maxSpeed = 5f;
    private Vector3 dirVec = new Vector3();
    public Rigidbody playerRb;
    public Camera Cam;
    

    private bool CheckMovementFlags()
    {//if we're moving return false, else return true;
        if(this.movingL || this.movingR || this.movingF || this.movingB)
        {
            return false;
        }
        else
        {
            return true;
        }
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
    public void OnBack(InputAction.CallbackContext context)
    {
          if (context.performed)
        {
            this.movingB = true;
            float inputY = context.ReadValue<float>();
            Vector3 temp = this.dirVec;
            temp.z = -inputY;
            this.dirVec = temp;


        }
        if (context.canceled)
        {
            this.movingB = false;
            if (this.CheckMovementFlags())
            {
                this.StopMoving();
            }
            
        }
    }

    public void OnForward(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.movingF = true;
            float inputY = context.ReadValue<float>();
            Vector3 temp = this.dirVec;
            temp.z = inputY;
            this.dirVec = temp;

        }
        if (context.canceled)
        {
            this.movingF = false;
            if (this.CheckMovementFlags())
            {
                this.StopMoving();
            }
        }
       
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {   this.dirVec.y = 1;
            this.dirVec = Cam.transform.TransformVector(dirVec).normalized;
            playerRb.AddForce(dirVec * this.jumpForce);
            
        }
        if (context.canceled)
        {
            this.dirVec.y = 0;

        }
    }

    public void OnLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.movingL = true;
            float inputX = context.ReadValue<float>();
       
            Vector3 temp = this.dirVec;
            temp.x = -inputX;
            this.dirVec = temp;

        }
        if (context.canceled)
        {
            this.movingL = false;
            if (this.CheckMovementFlags())
            {
                this.StopMoving();
            }
        }
    }

    private void StopMoving()
    {
        this.playerRb.velocity = Vector3.zero;
    }

    public void OnRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.movingR = true;
            float inputX = context.ReadValue<float>();
            Vector3 temp = this.dirVec;
            temp.x = inputX;
            this.dirVec = temp;

        }
        if (context.canceled)
        {
            this.movingR = false;
            if (this.CheckMovementFlags())
            {
                this.StopMoving();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void FixedUpdate()
    {
        if (this.playerRb.velocity.magnitude < this.maxSpeed)
        {
            if (this.movingF || this.movingR || this.movingL || this.movingB)
            {
                Vector3 temp = this.dirVec;
                temp = Cam.transform.TransformVector(temp).normalized;
                temp.y = 0;
                playerRb.AddForce(temp * this.moveForce);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
