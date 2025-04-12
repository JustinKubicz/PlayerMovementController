using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
//Justin Kubicz


/*
 * CameraMovementScript handles user input from mouse delta. It applies x and y values from mouse 
 * delta to the quaternion that handles the rotation of the camAnchor object which is located inside the player character.
 * The camera itself is a child of that anchor, just positioned in the scene view and it stays in the same position relative to 
 * the anchor object, basically like the camera is on the end of a long stick attached to a rotating ball inside the player.
 */
public class CameraMovementScript : MonoBehaviour, CameraMovement.ICameraControlsActions


{
    //CONFIGURABLES
    [Range(0,1)]
    public float Hsens = 1f;
    [Range(0,1)]
    public float Vsens = 1f;
    public GameObject camAnchor;

    //INTERNALS
    private CameraMovement inpt;    
    private Vector2 rotateTransform;

    private void OnEnable()
    {
        //Initialize Input
        if (this.inpt == null)
        {
            this.inpt = new CameraMovement();
            this.inpt.CameraControls.SetCallbacks(this);
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
    // Start is called before the first frame update
    void Start()
    {
        //Scale Hsens and Vsens user settings to appropriate ranges
        this.Hsens = this.Hsens * 0.5f;
        
        this.Vsens = this.Vsens * 0.05f;
    }

    private void Update()
    {
    }
   
    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Grab quaternion values
            Vector2 mouseDelta = context.ReadValue<Vector2>();
            this.rotateTransform.x += (mouseDelta.x * this.Hsens);
            this.rotateTransform.y += (mouseDelta.y * this.Vsens);
            this.rotateTransform.y = Mathf.Clamp(this.rotateTransform.y, -75f, 20f);
            this.Rotate();
        }
 
    }

    private void Rotate()
    {
        //using Euler angles, rotate the camera
        Quaternion quat = Quaternion.Euler(0f, this.rotateTransform.x, this.rotateTransform.y);
        this.camAnchor.transform.rotation = quat;
    }
}
