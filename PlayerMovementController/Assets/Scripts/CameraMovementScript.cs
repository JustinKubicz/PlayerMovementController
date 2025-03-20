using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovementScript : MonoBehaviour, CameraMovement.ICameraControlsActions


{   
    
    public Camera cam;
    public Rigidbody target;
    private Vector3 offset = new Vector3(-10, 4, 0);
    private bool cameraTurningL;
    private bool cameraTurningR;


    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void FixedUpdate()
    {
        if (this.target)
        {
            this.cam.transform.position = this.target.transform.position + offset;
        }
        if (this.cameraTurningL) turnCamera(true);
        if (this.cameraTurningR) turnCamera(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCircleL(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.cameraTurningL = true;
            this.cameraTurningR = false; 
        }
        if (context.canceled)
        {
            this.stopTurningCamera();
        }
    }

    public void OnCircleR(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.cameraTurningR = true;
            this.cameraTurningL = false;
        }
        if (context.canceled)
        {
            this.stopTurningCamera();
        }
    }
    private void stopTurningCamera()
    {
        throw new NotImplementedException();
    }
    private void turnCamera(bool lOrR)
    {//true turns left, false turns right;
    

    }
}
