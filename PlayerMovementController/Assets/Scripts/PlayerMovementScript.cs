using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.Windows;


//Justin Kubicz
/*
 * PlayerMovementScript handles user movement inputs from WSAD and the spacebar. It manages player's movement state with 
 * a finite state machine, which regulates when input actions are available. It manages slope detection via raycasting, and coyote time
 * via raycasting. 
 */


public class PlayerMovementScript : MonoBehaviour, PlayerMovement.IPlayerControlsActions
{   //CONFIGURABLES
    [Range(0f, 1f)]
    public float accelerationRate = 1f;

    [Range(0.0f, 1f)]
    public float stoppingPower = 1f;

    [Range(0f, 1f)]
    public float jumpForce = 1f;

    [Range(0f, 1f)]
    public float maxSpeed = 1f;
    public Rigidbody playerRb;
    public Camera cam;

    //INTERNALS
    private PlayerMovement inpt;
    private Vector3 dirVec = new Vector3();
    private State curState;
    private bool jumped;
    private bool coyoteTimerStarted;
    private bool decelerate;
    private bool onSlope;

    private enum State
    //FSM used to regulate what actions are available when
    {
        INVALID = 0,
        GROUNDED = 1,
        WALKING = 2,
        AIRBORNE = 3,
        SLIDING = 4,
        COYOTETIME = 5,
    }

    public void OnEnable()
    {//Initialize input
        if (this.inpt == null)
        {
            this.inpt = new PlayerMovement();
            this.inpt.PlayerControls.SetCallbacks(this);
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

    public void OnJump(InputAction.CallbackContext context)
    {//If not already airborne, jump! Set this.jumped to true to avoid triggering coyote time.
       
        if (context.performed)
        {  
            if (this.curState != State.AIRBORNE)
            {
                this.curState = State.AIRBORNE;
                this.playerRb.isKinematic = false;
                this.jumped = true;
                this.dirVec.y = 2;
                this.dirVec = cam.transform.TransformVector(dirVec).normalized;
                playerRb.AddForce(dirVec * (this.jumpForce * 800f));
            }
        }
    }

    private void StopMoving()
    {//StopMoving, triggers deceleration in physics loop
        
        this.curState = State.GROUNDED;
        if (this.playerRb.velocity != Vector3.zero)
        {
            this.decelerate = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {//Reset state to grounded when impacting the ground, and not with verticle walls
        
        if (this.curState == State.AIRBORNE)
        {
            RaycastHit hit;
            if (
                Physics.Raycast(
                    this.transform.position,
                    this.transform.TransformDirection(Vector3.down),
                    out hit,
                    Mathf.Infinity
                )
            )
            {
                if (hit.distance <= 1.5)
                {
                
                    
                        this.StopMoving();
                        this.jumped = false;
                    
                       
                }
            }
        }
    }

   
    void Start() {
        //Scale and transform stopping power to appropriate range
        this.stoppingPower = stoppingPower * 0.1f + 0.9f;
    }

    void FixedUpdate()
    {//This code is messy. If I have time I'd like to refactor this significantly.

        
        //Detect Slopes by Raycasting straight down and getting a normal to the 
        //surface the player is standing on. Take the angle of that normal to Vector3.up,
        //if it's above 20 degrees trigger slope handling
        RaycastHit slopes;
        Vector3 rayEnd = this.transform.TransformDirection(Vector3.down);
        Physics.Raycast(this.transform.position, rayEnd, out slopes);
        float angle = Vector3.Angle(slopes.normal, Vector3.up);
            if ( angle > 20)
            {
                this.onSlope = true;
            }
            else
            {
                this.onSlope = false;
            }
        



        //Handle Walking, transform the input direction vector from relative 
        //to camera space to world space. This allows W to be forward as if coming out of the camera, when applying forces to the player character.
        //If player is on a slope, project movement to that slopes plane with ProjectOnPlane being given the direction vector to project and the normal
        // of the plane to project on. Then acceleration forces are also scaled differently on slopes, I think this is due to the fact that 
        // when on a level surface I don't project to it, so maybe that affects how the forces apply to the player character.
        if (this.curState == State.WALKING)
        {
             Vector3 temp = this.dirVec;
             temp = cam.transform.TransformVector(temp);
             temp.y = 0;
            if (this.onSlope)
            {
                if (this.playerRb.velocity.magnitude <= (this.maxSpeed * 8f))
                    temp = Vector3.ProjectOnPlane(temp, slopes.normal);
                    playerRb.AddForce((this.accelerationRate * 1500) * Time.fixedDeltaTime * temp);
                
            }
            else
            {
                 if (this.playerRb.velocity.magnitude <= (this.maxSpeed * 8f))
                    playerRb.AddForce((this.accelerationRate * 4000) * Time.fixedDeltaTime * temp); 
            }
                                                                            
        }


        //COYOTE TIME LOGIC
        //If raycast straight down hits more than or equal to 2 meters, 
        //and it wasn't a result of a jump and we're not airborne, then we've fallen
        //and should start coyoteTimer to allow the player a brief moment to jump on fall.
        RaycastHit hit;
        if (
            Physics.Raycast(
                this.transform.position,
                this.transform.TransformDirection(Vector3.down),
                out hit,
                Mathf.Infinity
            )
        )
        {
            if (hit.distance >= 2 && !this.jumped && !(this.curState == State.AIRBORNE))
                this.curState = State.COYOTETIME;
        }
        if (this.curState == State.COYOTETIME && !this.coyoteTimerStarted)
        {
            StartCoroutine(CoyoteTimer());
        }


        //HANDLE Deceleration
        //If decelerating, apply negativized velocity * stopping power until velocity's magnitude reaches a threshold.
        //If we're on a slope when the threshold is reached and we aren't either walking, airborning or coyotetiming,
        //then make player character kinematic, to prevent sliding down slopes
        if (
            this.decelerate
        )
        {
           Vector3 projectedVelocity = Vector3.ProjectOnPlane(-this.playerRb.velocity, slopes.normal);
           projectedVelocity = projectedVelocity.normalized * this.playerRb.velocity.magnitude;
           this.playerRb.AddForce(
           Time.fixedDeltaTime * (this.stoppingPower * 2000) * projectedVelocity);
            if(this.playerRb.velocity.magnitude <= 0.5f)
                this.decelerate = false;                            
        }
        if (this.onSlope && this.curState != State.WALKING && !this.decelerate && this.curState != State.AIRBORNE && this.curState != State.COYOTETIME) {
            
            this.playerRb.isKinematic = true;
        }
     
       
        
    }

    IEnumerator CoyoteTimer()
    { //Coroutine sets state to AIRBORNE after .25 seconds, allowing player to jump for that time after falling
        //boolean coyoteTimerStarted was added to prevent infinite triggering of coyote timer
      
        this.coyoteTimerStarted = true;
        yield return new WaitForSeconds(0.25f);
        this.curState = State.AIRBORNE;
        this.coyoteTimerStarted = false;
    }
private static void Update()
    { }

    public void OnWalk(InputAction.CallbackContext context)
    {
        //If we are in a state to walk, grab the composite
        //x and y values that result from user input. 
        //and apply them to the x and z values of the direction vector
        //when canceled, call StopMoving
        if (
            (this.curState == State.GROUNDED || this.curState == State.WALKING)
            && !(this.curState == State.AIRBORNE)
        )
        {
            if (context.performed)
            {
                this.curState = State.WALKING;
                this.playerRb.isKinematic = false;
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
