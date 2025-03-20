using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour, PlayerMovement.IPlayerControlsActions


{

    private PlayerMovement inpt;
    private float moveForce = 500.0f;
    private bool movingF = false;
    private bool movingL = false;
    private bool movingR = false;
    private bool movingB = false;
    private Vector3 dirVec = new Vector3();
    public Rigidbody playerRb;
    public Camera Cam;
    


    public enum moving
    {

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
            this.dirVec.z = -inputY;


        }
        if (context.canceled)
        {
            this.movingB = false;
        }
    }

    public void OnForward(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.movingF = true;
            float inputY = context.ReadValue<float>();
            this.dirVec.z = inputY;
           

        }
        if (context.canceled)
        {
            this.movingF = false;
        }
       
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {   this.dirVec.y = 1;
            this.dirVec = Cam.transform.TransformVector(dirVec).normalized;
            playerRb.AddForce(dirVec * this.moveForce);
            
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
            this.dirVec.x = -inputX;


        }
        if (context.canceled)
        {
            this.movingL = false;
        }
    }

    public void OnRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.movingR = true;
            float inputX = context.ReadValue<float>();
            this.dirVec.x = inputX;


        }
        if (context.canceled)
        {
            this.movingR = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void FixedUpdate()
    {
        if (this.movingF || this.movingR || this.movingL || this.movingB)
        {
            this.dirVec = Cam.transform.TransformVector(dirVec).normalized;
            this.dirVec.y = 0;
            playerRb.AddForce(dirVec * this.moveForce);
        }
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
