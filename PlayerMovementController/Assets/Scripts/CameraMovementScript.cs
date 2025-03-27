using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovementScript : MonoBehaviour, CameraMovement.ICameraControlsActions


{
    public float Hsens = 0.50f;
    public float Vsens = 0.05f;
    private CameraMovement inpt;
    public GameObject camAnchor;
    private Vector3 offset = new Vector3(-10, 4, 0);
    private Vector2 rotateTransform;

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
    }
   
    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 mouseDelta = context.ReadValue<Vector2>();
            this.rotateTransform.x += (mouseDelta.x * this.Hsens);
            this.rotateTransform.y += (mouseDelta.y * this.Vsens);
            this.rotateTransform.y = Mathf.Clamp(this.rotateTransform.y, -80f, 0f);
            this.Rotate();
        }
 
    }

    private void Rotate()
    {
        Quaternion quat = Quaternion.Euler(0f, this.rotateTransform.x, this.rotateTransform.y);
        this.camAnchor.transform.rotation = quat;
    }
}
