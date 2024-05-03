using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    #region Initializing
    //MOVEMENT X-AXIS
    //Sombras' old code
    //[SerializeField, Tooltip("Constant increment to player X speed, should be similar to camera speed")]
    //public float constantVelocity;
    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    float speed = 9;
    [SerializeField, Tooltip("Acceleration while grounded.")]
    float walkAcceleration = 75;
    [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
    float groundDeceleration = 70;

    //MOVEMENT Y-AXIS jump and air
    
    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    float jumpHeight = 4;
    [SerializeField, Tooltip("Acceleration while in the air.")]
    float airAcceleration = 30;
    [SerializeField] public float gravityModifierOriginalValue = 6;
    [SerializeField] public float gravityModifierSafeValue = 1;
    private float gravityModifier;



    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    [HideInInspector] public Vector2 velocity;


    public bool isGrounded = false;
    public bool groundedLastFrame;
    private bool isTouchingCeiling;
    private bool isTouchingWall;


    // Player references
    GameObject player;
    #endregion


    #region SetUP
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>(); //we need this for sprite flip
        boxCollider = GetComponent<BoxCollider2D>();
        gravityModifier = gravityModifierOriginalValue;

        // Player references
        player = GameObject.FindWithTag("Player");
    }
    #endregion


    #region Updates
    void Update()
    {
        print("1 befMov Velocity Y: " + velocity.y);
        print("1 befMov isGrounded: " + isGrounded);
        Movement();
        print("2 aftMov Velocity Y: " + velocity.y);
        print("2 aftMov isGrounded: " + isGrounded);
        Gravity();
        print("3 grav Velocity Y: " + velocity.y);
        print("3 grav isGrounded: " + isGrounded);
        //Dash();
        Collision();
        print("5 col Velocity Y: " + velocity.y);
        print("5 col isGrounded: " + isGrounded);
        ApplyMovement();
        print("4 apMov Velocity Y: " + velocity.y);
        print("4 apMov isGrounded: " + isGrounded);
        //CameraConstrain();
        //Rendering();
    }
    #endregion


    #region Functions
    void Movement() {
        
        //X-AXIS
        // Use GetAxisRaw to ensure our input is either 0, 1 or -1.
        float moveInput = Input.GetAxisRaw("Horizontal");


        //JUMP - Y-AXIS
        //Here we reset the jump, if we touch ground
        if (isGrounded) {
            //if we touch ground, y velocity goes to zero and you stop falling
            velocity.y = 0;

            //also gravity goes to a lower value to prevent collision issues
            gravityModifier = gravityModifierSafeValue;

            // Sombras' old code
            if (Input.GetButtonDown("Jump")) {
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
            }
        }
        /*else {
            gravityModifier = gravityModifierOriginalValue;
        } */   
        

        //If player hits ceiling, velocity.y needs to zero out
        if (isTouchingCeiling) {
            //Debug.Log("1: " + velocity.y);
            velocity.y *= 0.5f;
            //Debug.Log("2: " + velocity.y);
            isTouchingCeiling = false;
        }

        // Moved this component to the "isgrounded" else check above 24.04.24
        //We can't set the gravity modifier too high without RigidBodies, otherwise collision behaves badly (issues)
        if (!isGrounded) {
            gravityModifier = gravityModifierOriginalValue;
            
            //Sombras' old code
            //Moving platform glue 
            //transform.parent = null;
        }
        //Sombras' old code
        //Here we let the JUMP more dynamic creating several maximum jump heights depending on the time the jump button is pressed
        if (Input.GetButtonUp("Jump")) {
            if (velocity.y > 0) {
                velocity.y *= 0.4f;
            }
        }

        //Air Momentum (muda a aceleração que o controle aplica no jogador dependendo se ele está no ar ou não)
        float acceleration = isGrounded ? walkAcceleration : airAcceleration;
        float deceleration = isGrounded ? groundDeceleration : 0;

        if (moveInput != 0) {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, acceleration * Time.deltaTime);
        }
        else {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

        //RENDERING
        //Flip the sprite according to getAxisRaw
        if (moveInput == -1) {
            spriteRenderer.flipX = true;
        }
        if (moveInput == 1) {
            spriteRenderer.flipX = false;
        }
    } // Movement End




    public void Gravity() {
        velocity.y += Physics2D.gravity.y * gravityModifier * Time.deltaTime;
    }

    
    private void ApplyMovement() {
        //Sombras' old code
        //Aqui se pega o resultado final de todas as modificações de velocity e aplica ao objeto, 
        //ou seja, aqui é onde o movimento é executado de fato, mais a valocidade constante da camera 
        //no eixo x
        Vector2 finalVelocity = new Vector2(velocity.x, velocity.y);
        transform.Translate(finalVelocity * Time.deltaTime);
    }
    


    public void Collision() {
    
        isGrounded = false;
        //bool groundedThisFrame = false;

    
        // Test collision
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);
        //this foreach focus on handle the physics of collision
        foreach (Collider2D hit in hits) {

            // Ignore our own collider
            if (hit == boxCollider) {
                continue;
            }

            // Collision variables
            GameObject overlappedObj = hit.gameObject;
            ColliderDistance2D colliderDistance = hit.Distance(boxCollider);
            Vector2 colliderNormal = -colliderDistance.normal;
            //print("Collider Normal: " + colliderNormal);

            // Consider X degrees to each side to detect a wall/ceiling/ground
            float angleThreshold = 10;

            // Ensure that we are still overlapping this collider.
            // The overlap may no longer exist due to another intersected collider pushing us out of this one.
            if (colliderDistance.isOverlapped) {



                // The absolute angle difference between the collision normal and ceiling normal
                float upNormalDiff = HelperF.normalDiff(colliderNormal, Vector2.up);
                
                // Test if it's touching ceiling upwards
                if (upNormalDiff < angleThreshold) {
                    isTouchingCeiling = true;
                    //print("ceiling");
                }


                // The absolute angle difference between the collision normal and left wall normal
                float leftNormalDiff = HelperF.normalDiff(colliderNormal, Vector2.left);

                // Test if it's touching wall to the left
                if (leftNormalDiff < angleThreshold) {
                    isTouchingWall = true;
                    //print("leftWall");
                }

                
                // The absolute angle difference between the collision normal and right wall normal
                float rightNormalDiff = HelperF.normalDiff(colliderNormal, Vector2.right);

                // Test if it's touching wall to the right
                if (rightNormalDiff < angleThreshold) {
                    isTouchingWall = true;
                    //print("rightWall");
                }

                /*
                if(groundedThisFrame) {
                    isGrounded = true;
                }
                */


                // The absolute angle difference between the collision normal and ground normal
                float downNormalDiff = HelperF.normalDiff(colliderNormal, Vector2.down);
                //print("1 downNormalDiff: " + downNormalDiff);
                //print("2 angleThreshold: " + angleThreshold);
                //print("1 Velocity Y: " + velocity.y);
                //print("1 isGrounded: " + isGrounded);

                // Test if it's touching ground downwards AND it's not jumping, then set isGrounded to true.
                if (downNormalDiff < angleThreshold && velocity.y < 0) {
                    isGrounded = true;
                    velocity.y = 0;
                    //groundedThisFrame = true;
                    //gravityModifier = gravityModifierSafeValue;


                    /* Sombras' old code
                    //Moving platform glue  
                    if (hit.GetComponent<AttMovablePlat>()) {
                        if (transform.parent == null && velocity.y < hit.GetComponent<AttMovablePlat>().objectVelocityY) {
                            transform.parent = hit.transform;
                        }
                    } */
                }

                //here we resolve the collision foreach
                // Pushout from collider
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
                //print("2 Velocity Y: " + velocity.y);
                //print("2 downNormalDiff: " + downNormalDiff);
                //print("2 isGrounded: " + isGrounded);


            }
        }
    // Update isGrounded only if grounded in both current and previous frame
    //isGrounded = groundedThisFrame;
    }
    #endregion    
}

