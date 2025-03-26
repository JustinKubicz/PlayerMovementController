using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovementScript : MonoBehaviour, CameraMovement.ICameraControlsActions


{
    private float sens = 100f;
    private CameraMovement inpt;
    public Camera cam;
    public Rigidbody target;
    private Vector3 offset = new Vector3(-10, 4, 0);
    //private bool rotating = false;
    private float rotateTransform;

    private void OnEnable()
    {
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
       
    }

    private void Update()
    {
        if (this.target)
        {
            this.cam.transform.position = this.target.transform.position + offset;
            ////this.cam.transform.rotation.SetLookRotation(this.target.transform.position, Vector3.up);
            //Debug.Log("this.cam.transform.rotation.z + this.rotateTransform: " + (this.cam.transform.rotation.z + this.rotateTransform));
            //this.cam.transform.Rotate(new Vector3(this.cam.transform.rotation.x , this.cam.transform.rotation.y, this.cam.transform.rotation.z+ this.rotateTransform));

        }
    }
   
    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.rotateTransform = context.ReadValue<Vector2>().x;                        
        }
 
    }

}
