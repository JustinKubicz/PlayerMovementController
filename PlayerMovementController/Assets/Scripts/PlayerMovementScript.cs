using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour, PlayerMovement.IPlayerControlsActions


{

    private PlayerMovement inpt;
    private float moveForce = 25.0f; 
    
    private float curInput;
    public Rigidbody rb;
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
            Vector2 in = context.ReadValue<Vector2>();
            this.curInput = context.ReadValue<float>();
            
            
       
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log(context.phase);
    }

    public void OnLeft(InputAction.CallbackContext context)
    {
        Debug.Log(context.phase);
    }

    public void OnRight(InputAction.CallbackContext context)
    {
        Debug.Log(context.phase);
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
