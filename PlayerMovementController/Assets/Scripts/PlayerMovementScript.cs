using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour, PlayerMovement.IPlayerControlsActions


{

    private PlayerMovement inpt;
    private float moveForce = 75.0f;
    private Vector3 dirVec = new Vector3();
    public Rigidbody playerRb;
    public Camera Cam;
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
        Debug.Log(context.phase);
    }

    public void OnForward(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float inputY = context.ReadValue<float>();
            this.dirVec.z = inputY;
            this.dirVec = Cam.transform.TransformVector(dirVec).normalized;
            this.dirVec.y = 0;
            playerRb.AddForce(dirVec * this.moveForce);

        }
       
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log(context.phase);
    }

    public void OnLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float inputX = context.ReadValue<float>();
            this.dirVec.x = -inputX;
            this.dirVec = Cam.transform.TransformVector(dirVec).normalized;
            this.dirVec.y = 0;
            playerRb.AddForce(dirVec * this.moveForce);

        }
    }

    public void OnRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float inputX = context.ReadValue<float>();
            this.dirVec.x = inputX;
            this.dirVec = Cam.transform.TransformVector(dirVec).normalized;
            this.dirVec.y = 0;
            playerRb.AddForce(dirVec * this.moveForce);

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
