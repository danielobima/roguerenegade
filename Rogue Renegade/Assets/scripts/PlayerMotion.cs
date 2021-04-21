using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Cinemachine;

public class PlayerMotion : MonoBehaviour {

    public bool isUsingKeyBoard = true;

    //reference variables.
    private Transform myTransform;
    private GameObject cam;
    private Target target;
    private Animator animator;
    public float motion;
    public float speed = 10;
    public float jumpSpeed = 1;
    public float landingDist = 0.5f;
    public float floorDist = 1.2f;
    private float normSpeed;
    private Rigidbody rigidBody;
    public bool justKilledEnemy = false;
    public bool isMoving = false;
    private PlayerGun playerGun;
    public float jumpHeight = 2f;
    public float jumpForce = 2f;
    public bool isOnFloor = false;
    public bool isJumping = false;
    public bool isCrouching = false;
    public bool jumpAnim = false;
    public bool isCrouchJumping = false;
    public bool isRolling = false;
    private bool diveInterruptable = false;
    public bool initializeOnStart = true;
    private GameMech gameMech;
    private FresnelEffect fresnelEffect;
   
    public static string movementType = "run";
    public static bool obstaculated = false;
    public float obstacleDistance = 0.7f;
    public bool isBeingPunched = false;
    public float turnSmoothing = 0.1f;
    private float turnSmoothRef;
    private Vector3 direction;
    private float targetAngle;
    private CapsuleCollider capsuleCollider;
    public CoverAble currentCoverAble;
    public CoverAble possibleCoverAble;
    public float rollForce = 1000f;
    private RagdollSwitch ragdoll;
    public Quaternion thisPlayersMiddlespineRotation;
    public Vector3 thisPlayersLookat;
    public GameObject virtualMiddleSpine;
    public Transform carryPos;
    public bool isCarrying = false;
    private bool jumpMotion;
    private Vector3 jumpDirection;
    private bool isLanding = false;
    private bool isEndingJump = false;



    private void Start()
    {
        gameMech = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMech>();

        
        rigidBody = GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();
        ragdoll = GetComponent<RagdollSwitch>();
        myTransform = gameObject.transform;
        cam = Camera.main.gameObject;
        target = GetComponent<Target>();
        fresnelEffect = GetComponent<FresnelEffect>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        //cylinder = GameObject.FindGameObjectWithTag("AimCylinder").transform;

        //slowMoBar = GameObject.FindGameObjectWithTag("SlowMoBar");
        //slowMoBarGlow = slowMoBar.transform.GetChild(0).GetComponent<Animator>();

        if (initializeOnStart)
        {
            UISetup();
        }
        Time.timeScale = 1;
        normSpeed = speed;
        playerGun = GetComponent<PlayerGun>();
        //setVirtualMiddleSpine();
        updateSpeed();

    }
    private void updateSpeed()
    {
        animator.speed = speed;
    }
    private void updateSpeed(float Speed)
    {
        speed = Speed;
        animator.speed = Speed;
    }
    
    private void FixedUpdate()
    {
        movementTechnologies();
        

    }
   

    private void Update()
    {
        if (!GameMech.gameIsPaused)
        {
            MoveWhenTold(isUsingKeyBoard);
            
        }

    }
    public void UISetup()
    {

#if (UNITY_IPHONE || UNITY_ANDROID)
        isUsingKeyBoard = false;
        joystick = GameObject.FindGameObjectWithTag("joystick").GetComponent<Joystick>();
#endif

#if  UNITY_STANDALONE_WIN
        isUsingKeyBoard = true;
#endif
    }

    private void Move()
    {

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        animator.SetFloat("InputX", Mathf.MoveTowards(animator.GetFloat("InputX"), horizontal, Time.deltaTime * 5));
        animator.SetFloat("InputY", Mathf.MoveTowards(animator.GetFloat("InputY"), vertical, Time.deltaTime * 5));
        direction = new Vector3(horizontal, 0, vertical).normalized;
        targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        if (!obstaculated  && !isRolling && !isLanding && !isCrouchJumping)
        {
            if (!isBeingPunched)
            {
                float target =
                    //targetAngle + 
                    cam.transform.eulerAngles.y;

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, target, ref turnSmoothRef, turnSmoothing);


                transform.rotation = Quaternion.Euler(0, target, 0);
                if (direction.magnitude >= .1f && !GameMech.gameIsPaused )
                {

                    if (!isOnFloor || isJumping)
                    {

                    }

                    isMoving = true;

                }
                else
                {

                    isMoving = false;
                }
            }

        }
        else
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hits, obstacleDistance))
            {

                if (hits.collider.GetComponent<Obstacles>())
                {
                    obstaculated = true;
                    if (animator.GetInteger("motion") == 5 || animator.GetInteger("motion") == 2)
                    {
                        if (playerGun.gun)
                        {
                            animator.SetInteger("motion", 2);
                        }
                        else
                        {
                            animator.SetInteger("motion", 0);
                        }
                    }
                }
                else
                {
                    if (!isBeingPunched)
                    {
                        obstaculated = false;
                    }
                }
                if (targetAngle >= 90 || targetAngle <= -90)
                {
                    obstaculated = false;
                }
            }
            else
            {
                if (!isBeingPunched)
                {
                    obstaculated = false;
                }
            }
        }
        if (isRolling)
        {
            //myTransform.Translate(Vector3.forward * rollForce * Time.deltaTime);
        }
        



    }

    
    public void Setspeed()
    {
       
        if (playerGun.gun == null)
        {
            speed = normSpeed;
        }
        else
        {
            if (playerGun.hasHandgun)
            {
                //speed = handGunSpeed;

            }
            else
            {
                //speed = gunSpeed;
            }
        }
    }
    private void movementTechnologies()
    {
        Setspeed();
        if (!target.isDead)
        {
            if (isUsingKeyBoard)
            {
                Move();

                if (isMoving)
                {

                    if (isOnFloor && !isEndingJump)
                    {
                        if (!obstaculated)
                        {
                            if (movementType.Equals("run"))
                            {
                                if (!isCrouching)
                                {
                                    if (!PlayerGun.isFineAim)
                                    {
                                        /*if (Quaternion.Angle(virtualMiddleSpine.transform.rotation, transform.rotation) < 120)
                                        {

                                            animator.SetInteger("motion", 2);
                                        }
                                        else
                                        {

                                            animator.SetInteger("motion", 16);
                                        }*/
                                        if (playerGun.gun)
                                        {
                                            animator.SetInteger("motion", 2);
                                        }
                                        else
                                        {
                                            animator.SetInteger("motion", 0);
                                        }
                                    }
                                    else
                                    {
                                        animator.SetInteger("motion", 1);
                                    }
                                }
                                else
                                {
                                    animator.SetInteger("motion", 7);
                                }
                            }
                            if (movementType.Equals("stairs"))
                            {
                                animator.SetInteger("motion", 5);
                            }

                        }
                        else
                        {
                            if (playerGun.gun)
                            {
                                animator.SetInteger("motion", 2);
                            }
                            else
                            {
                                animator.SetInteger("motion", 0);
                            }
                        }
                    }
                    else
                    {
                        
                    }
                    if (justKilledEnemy)
                    {
                        target.isAiming = false;
                        justKilledEnemy = false;
                    }






                }
                else
                {


                    if (isOnFloor)
                    {
                        if (  !jumpAnim && !isEndingJump && !isCrouchJumping && !isLanding)
                        {
                            if (!isCrouching)
                            {
                                if (playerGun.gun)
                                {
                                    animator.SetInteger("motion", 2);
                                }
                                else
                                {
                                    animator.SetInteger("motion", 0);
                                }
                            }
                            else
                            {
                                animator.SetInteger("motion", 6);
                            }
                        }
                        else
                        {
                            //coverAnim();
                        }

                    }


                }

            }
            else
            {



                //myTransform.Translate(Vector3.forward * 1 * Time.deltaTime * motion);


                if (motion > 0)
                {
                    //Debug.Log(motion);
                    if (justKilledEnemy)
                    {
                        target.isAiming = false;
                        justKilledEnemy = false;
                    }
                    if (isOnFloor && !obstaculated)
                    {
                        if (motion > 0.04f)
                        {
                            if (playerGun.gun)
                            {
                                animator.SetInteger("motion", 2);
                            }
                            else
                            {
                                animator.SetInteger("motion", 0);
                            }

                        }
                        else
                        {
                            animator.SetInteger("motion", 1);
                        }

                    }

                }
                else
                {
                    if (isOnFloor)
                    {
                        if (playerGun.gun)
                        {
                            animator.SetInteger("motion", 2);
                        }
                        else
                        {
                            animator.SetInteger("motion", 0);
                        }
                    }

                }
            }
            if (!isOnFloor)
            {
                if (!jumpAnim && !isEndingJump)
                {
                    animator.SetInteger("motion", 4);
                }
            }
            if (jumpAnim)
            {

                if (jumpDiveConditions())
                {
                    if (!isCrouching && !isCrouchJumping && !animator.GetCurrentAnimatorStateInfo(0).IsName("dive"))
                    {
                        animator.SetInteger("motion", 3);
                    }
                    else
                    {
                        animator.SetInteger("motion", 8);
                    }
                }
            }
            if (isRolling)
            {
                animator.SetInteger("motion", 9);
            }
            if (isLanding)
            {
                animator.SetInteger("motion", 17);
            }
            if (jumpMotion)
            {
                myTransform.Translate(jumpDirection * Time.deltaTime * jumpSpeed);
            }

        }

        FloorDetection();
    }
    private bool jumpDiveConditions()
    {


        if (!animator.GetInteger("motion").Equals(3))
            if (!animator.GetInteger("motion").Equals(17))
                if (!animator.GetInteger("motion").Equals(8))
                    if (!animator.GetInteger("motion").Equals(4))
                    {
                        //Debug.Log(animator.GetInteger("motion"));
                        return true;
                    }
                        
        return false;
    }

    public void FloorDetection()
    {
        if (Physics.Raycast(transform.position, transform.up * -1, out RaycastHit hit))
        {
            if (hit.distance >= floorDist)
            {

                isOnFloor = false;
            }
            if(hit.distance <= landingDist  && !isOnFloor)
            {

                if(!Input.GetKey(KeyCode.LeftControl))
                    isLanding = true;

            }

        }
    }
    
    

    


   
    
    
    
    


    private void MoveWhenTold(bool isUsingKeys = true)
    {

        if (isUsingKeys)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                startJump();
                //Debug.Log("WOOOWW!!");
            }
            /*if (Input.GetKeyDown("f"))
            {
                if (!isSlowMo)
                {
                    ability(0);
                }
                else
                {
                    sss = s;
                    s = slowMoTime;
                }
            }*/

            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (!isCarrying)
                {
                    isCrouching = true;
                    
                    capsuleCollider.center = new Vector3(0.0008849353f, 0.773929f, 0.01664619f);
                    capsuleCollider.height = 1.569248f;
                }

            }
            else
            {
                if (!isCrouchJumping && !isRolling)
                {
                    isCrouching = false;
                    
                    capsuleCollider.center = new Vector3(0.0008849353f, 0.773929f, 0.01664619f);
                    capsuleCollider.height = 1.569248f;
                }
            }
            if (Input.GetKeyDown("g"))
            {
                //CoverTechnologies();
            }
            /*if (Input.GetKeyDown("u"))
            {
                joked = ragdoll.ragdollJokes(joked);
            }*/
        }
      
    }

    /*public static Vector3 checkAxisOfObjRelativeToTarget(Collider obj, Transform target)
    {
        Vector3 closestPos = obj.ClosestPointOnBounds(target.position);
        Vector3 difference = closestPos - target.position;
        Vector3 difference2 = difference;
        difference2.y = target.position.y;
        float angle = Mathf.Atan2(difference2.normalized.z, difference2.normalized.x) * Mathf.Rad2Deg;
        Debug.Log(angle);

        //90 is back
        //-90 is forward
        //180 is right
        //0 is left

        return new Vector3();
    }


    public void ability(int type)
    {
        switch (type)
        {
            case 0:
                slowMo();
                break;

        }
    }*/


    private void startJump()
    {
        if (!target.isDead)
        {
            if (isOnFloor && !PlayerGun.isFineAim && !isJumping && !isRolling && !isCrouchJumping && !isLanding)
            {
                if (isCrouching)
                {
                    isCrouchJumping = true;
                }
                jumpAnim = true;
                //jump();
                diveInterruptable = false;


            }
        }

    }


    public void endRoll()
    {
        isRolling = false;
        jumpAnim = false;
        isCrouchJumping = false;
    }
    public void endLand()
    {
        isLanding = false;
        jumpAnim = false;
        isCrouchJumping = false;
        isEndingJump = false;
    }


    public void jump()
    {
        if (!isCrouchJumping && !isJumping)
        {

            if (!isCrouching)
            {
                isJumping = true;
                rigidBody.AddForce(transform.up * jumpHeight);

            }

        }

        //Debug.Log(direction.magnitude);

    }


    public void endJump()
    {
        if (!isCrouchJumping)
        {
            jumpAnim = false;
            isEndingJump = true;
        }
        //isRolling = false;
        diveInterruptable = true;
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (isCrouchJumping)
        {
            if (!isCarrying && !jumpAnim)
            {
                isRolling = true;
            }
            isCrouchJumping = false;
            isJumping = false;
            jumpMotion = false;
            isEndingJump = false;
            
        }
        else
        {
            isRolling = false;
        }
        if (!isOnFloor)
        {
            isOnFloor = true;
            jumpMotion = false;
            isJumping = false;
            isEndingJump = false;

            if (!isCarrying && !jumpAnim)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    isRolling = true;
                    isCrouchJumping = false;
                    isJumping = false;
                    isEndingJump = false;
                }
                else
                {
                    //isLanding = true;
                    isCrouchJumping = false;
                    isJumping = false;
                    isEndingJump = false;
                }

            }


        }
        else
        {
            jumpMotion = false;
            isJumping = false;
            isEndingJump = false;
        }



        /*if (Physics.Raycast(transform.position,transform.forward,out RaycastHit hit, obstacleDistance))
        {

            if (hit.collider.GetComponent<Obstacles>())
            {
                obstaculated = true;
                if(animator.GetInteger("motion") == 5 || animator.GetInteger("motion") == 2)
                {
                    animator.SetInteger("motion", 0);
                }
            }
        }*/
        if (collision.collider.GetComponent<CoverAble>())
        {
            possibleCoverAble = collision.collider.GetComponent<CoverAble>();
        }
        if (animator.GetInteger("motion") == 8)
        {
            animator.SetInteger("motion", 9);
            isJumping = false;
            isCrouchJumping = false;
            jumpAnim = false;
            jumpMotion = false;
            isEndingJump = false;
        }
    }


    private void OnCollisionStay(Collision collision)
    {


        if (!isOnFloor || isJumping)
        {

            if (!obstaculated)
                isOnFloor = true;

        }
        if (!jumpAnim)
        {
            isJumping = false;
            jumpMotion = false;
            isEndingJump = false;
        }
        

        if (isCrouchJumping && diveInterruptable)
        {
            jumpAnim = false;
            isEndingJump = false;
            isCrouchJumping = false;
            jumpMotion = false;
            //isRolling = true;
            isJumping = false;
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.GetComponent<CoverAble>())
        {
            possibleCoverAble = null;
        }
    }
}
