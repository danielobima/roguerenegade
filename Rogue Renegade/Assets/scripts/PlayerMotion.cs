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
    private Vector3 motionDirection;
    private Vector3 cameraBackForth;
    public float motion;
    public float speed = 10;
    private float initSpeed;
    private float halfSpeed;
    private float slowMoSpeed;
    private float startSpeed;
    private Vector3 difference;
    private Rigidbody rigidBody;
    public bool justKilledEnemy = false;
    public bool isMoving = false;
    private PlayerGun playerGun;
    public float jumpHeight = 2f;
    public float jumpForce = 2f;
    //private float currentHeight = 0;
    private float lastMagnitude = 0;
    public bool isOnFloor = false;
    public bool isJumping = false;
    public bool isCrouching = false;
    public bool jumpAnim = false;
    public bool isCrouchJumping = false;
    public bool isRolling = false;
    private bool diveInterruptable = false;
    public bool initializeOnStart = true;
    private Volume volume;
    private WhiteBalance whiteBalance;
    private Joystick joystick;
    private GameMech gameMech;
    public static bool isSlowMo = false;
    private float s = 0;
    private float ss = 0;
    private float sss = 0;
    public float slowMoTime = 5;
    private FresnelEffect fresnelEffect;
    private Color defaultFresnelColor;
    private float defaultFresnelDuration;
    private Color slowMoFresnel = new Color(0, 0.274f, 1, 1);
    private bool gotVolume = false;
    public Material trailMat;
    public int abilityType = 0;
    public float slowMoMeter = 1;
    private GameObject slowMoBar;
    public float slowMoCoolDown = 2;
    public float slowMoBarRatio = 0;
    private Animator slowMoBarGlow;
    private float defaultWhiteBalanceVal;
    //private float afb = 0;
    public static string movementType = "run";
    public static bool obstaculated = false;
    public float obstacleDistance = 0.7f;
    public bool isBeingPunched = false;
    private float motionDecay;
    public float turnSmoothing = 0.1f;
    private float turnSmoothRef;
    private Vector3 direction;
    private float targetAngle;
    private CapsuleCollider capsuleCollider;
    public bool isTakingCover = false;
    private bool shouldStandCover = false;
    public CoverAble currentCoverAble;
    public CoverAble possibleCoverAble;
    public float rollForce = 1000f;
    public bool joked = true;
    private RagdollSwitch ragdoll;
    private Vector3 coverPoint;
    private Vector2 transformToWall;
    private bool isCoverMovingRight = false;
    private bool isGoingBack = false;
    public Transform cylinder;
    private Vector3 screenPoint;
    private Vector3 offset;


    private void Start()
    {
        
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        rigidBody = GetComponent<Rigidbody>();
        playerGun = GetComponent<PlayerGun>();
        animator = GetComponent<Animator>();
        ragdoll = GetComponent<RagdollSwitch>();
        myTransform = gameObject.transform;
        cam = Camera.main.gameObject;
        target = GetComponent<Target>();
        fresnelEffect = GetComponent<FresnelEffect>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        defaultFresnelColor = fresnelEffect.fresnelColor;
        defaultFresnelDuration = fresnelEffect.duration;
        initSpeed = speed;
        startSpeed = speed;
        halfSpeed = speed;
        slowMoSpeed = speed * 2.5f;
        difference = cam.transform.position - transform.position;
        slowMoBar = GameObject.FindGameObjectWithTag("SlowMoBar");
        slowMoBarGlow = slowMoBar.transform.GetChild(0).GetComponent<Animator>();
        
        if (initializeOnStart)
        {
            UISetup();
        }
        Time.timeScale = 1;
        slowMoMeter = 2;

         
    }
    private void FixedUpdate()
    {
        movementTechnologies();
        slowMoTechnologies();
        Vector3 yBounds = new Vector3(0, 0.6f,0);
        cylinder.position = MoveCylinder() + yBounds;
        cylinder.LookAt(new Vector3(transform.position.x, cylinder.position.y, transform.position.z));
    }
    private void Update()
    {
        MoveWhenTold(isUsingKeyBoard);
        
    }
    private void CoverTechnologies()
    {
        if (!isTakingCover)
        {
            if (possibleCoverAble != null)
            {
                currentCoverAble = possibleCoverAble;
                isTakingCover = true;
                shouldStandCover = currentCoverAble.shouldStand;
                if (!shouldStandCover)
                {
                    capsuleCollider.center = new Vector3(0, -1.573208f, 0.2924273f);
                    capsuleCollider.height = 3.448039f;
                }
            }
        }
        else
        {
            isTakingCover = false;
            currentCoverAble = null;
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
        direction = new Vector3(horizontal, 0, vertical).normalized;
        targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        if (!obstaculated && isOnFloor && !isJumping && !isTakingCover && !isRolling && !playerGun.isPunching)
        {
            if (!isBeingPunched)
            {
                if (direction.magnitude >= .1f)
                {
                    float target = targetAngle + cam.transform.eulerAngles.y;

                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, target, ref turnSmoothRef, turnSmoothing);
                   
                    
                    if (!PlayerGun.isFineAim)
                    {
                        transform.rotation = Quaternion.Euler(0, angle, 0);
                    }
                    if (!playerGun.meleeMode)
                    {
                        if (!PlayerGun.isFineAim )
                        {
                            if (!isCrouching)
                            {
                                if (!isGoingBack)
                                {
                                    myTransform.Translate(Vector3.forward * Time.deltaTime * speed);
                                }
                                else
                                {
                                    myTransform.Translate(Vector3.back * Time.deltaTime * speed);
                                }

                                //rigidBody.velocity = transform.forward * speed;
                            }
                            else
                            {
                                myTransform.Translate(Vector3.forward * Time.deltaTime * speed * 0.5f);
                                
                            }
                            lastMagnitude = direction.magnitude;
                        }
                        else
                        {
                            myTransform.Translate(Vector3.right * Time.deltaTime * direction.x * speed * 0.25f);
                            myTransform.Translate(Vector3.forward * Time.deltaTime * direction.z * speed * 0.25f);
                            

                        }
                        isMoving = true;

                    }
                    else
                    {
                        if (!playerGun.isPunching)
                        {
                            myTransform.Translate(Vector3.forward * Time.deltaTime * speed * 0.5f);
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
                   
                    isMoving = false;
                    lastMagnitude = 0;
                }
            }

        }
        else
        {
            
            lastMagnitude = 0;
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hits, obstacleDistance))
            {

                if (hits.collider.GetComponent<Obstacles>())
                {
                    obstaculated = true;
                    if (animator.GetInteger("motion") == 5 || animator.GetInteger("motion") == 2)
                    {
                        animator.SetInteger("motion", 0);
                    }
                }
                else
                {
                    if (!isBeingPunched)
                    {
                        obstaculated = false;
                    }
                }
                if(targetAngle >= 90 || targetAngle <= -90)
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
            myTransform.Translate(Vector3.forward * rollForce * Time.deltaTime);
        }
        if (isTakingCover)
        {
            if (!PlayerGun.isFineAim)
            {
                if (transformToWall == Vector2.zero)
                    transformToWall = new Vector2(coverPoint.x - transform.position.x, coverPoint.z - transform.position.z);
                Vector2 perp = Vector2.Perpendicular(transformToWall);
                Vector3 movementDir = new Vector3(transformToWall.normalized.x < 0 ? -transformToWall.normalized.x : transformToWall.normalized.x,
                    0, transformToWall.normalized.y);
                //Debug.Log(movementDir);
                myTransform.Translate(movementDir * horizontal * speed * 0.25f * Time.deltaTime);
                if (horizontal >= 1 || horizontal <= -1)
                {
                    isMoving = true;
                    if (horizontal >= 1)
                    {
                        isCoverMovingRight = true;
                    }
                    else
                    {
                        isCoverMovingRight = false;
                    }
                }
                else
                {
                    isMoving = false;
                }
            }
        }
        
        

    }
    


    private void movementTechnologies()
    {
        if (!target.isDead)
        {
            if (isUsingKeyBoard)
            {
                Move();

                if (isMoving)
                {

                    if (isOnFloor && !isTakingCover)
                    {
                        if (!obstaculated )
                        {
                            if (movementType.Equals("run"))
                            {
                                if (!isCrouching)
                                {
                                    if (!PlayerGun.isFineAim)
                                    {
                                        if (Quaternion.Angle(playerGun.playerMiddleSpine.rotation,transform.rotation) < 120)
                                        {
                                            
                                            animator.SetInteger("motion", 2);
                                        }
                                        else
                                        {
                                            
                                            animator.SetInteger("motion", 16);
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
                            if (playerGun.meleeMode)
                            {
                                if (!playerGun.isPunching)
                                {
                                    animator.SetInteger("punch", 1);
                                }
                            }
                        }
                        else
                        {
                            animator.SetInteger("motion", 0);
                        }
                    }
                    else
                    {
                        if (isTakingCover)
                        {
                            if (isCoverMovingRight)
                            {
                                if (!shouldStandCover)
                                {
                                    animator.SetInteger("motion", 13);
                                }
                                else
                                {
                                    animator.SetInteger("motion", 15);
                                }
                            }
                            else
                            {
                                if (!shouldStandCover)
                                {
                                    animator.SetInteger("motion", 12);
                                }
                                else
                                {
                                    animator.SetInteger("motion", 14);
                                }
                            }
                        }
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
                        if (!isTakingCover)
                        {
                            if (!isCrouching)
                            {
                                animator.SetInteger("motion", 0);
                            }
                            else
                            {
                                animator.SetInteger("motion", 6);
                            }
                        }
                        else
                        {
                            coverAnim();
                        }
                        
                    }
                    if (playerGun.meleeMode)
                    {
                        if (!playerGun.isPunching)
                        {
                            animator.SetInteger("punch", 0);
                        }
                    }

                }

            }
            else
            {



                myTransform.Translate(Vector3.forward * 1 * Time.deltaTime * motion);


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
                            animator.SetInteger("motion", 2);

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
                        animator.SetInteger("motion", 0);
                    }

                }
            }
            if (!isOnFloor)
            {
                if (!jumpAnim)
                {
                    animator.SetInteger("motion", 4);
                }
            }
            if (jumpAnim)
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
            if (isRolling)
            {
                animator.SetInteger("motion", 9);
            }


        }
        
        if (Physics.Raycast(transform.position, transform.up * -1, out RaycastHit hit))
        {
            if (hit.distance >= 1.2f)
            {
                isOnFloor = false;
            }

        }
    }
    private void coverAnim()
    {
        if (!shouldStandCover)
        {
            if (!PlayerGun.isFineAim)
            {
                animator.SetInteger("motion", 10);
            }
            else
            {
                if (!isCrouching)
                {
                    animator.SetInteger("motion", 0);
                }
                else
                {
                    animator.SetInteger("motion", 6);
                }
            }
        }
        else
        {
            animator.SetInteger("motion", 11);
        }
    }
    private Vector3 MoveCylinder()
    {

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        string[] layers = { "Player", "Enemies","Armature bones" ,"bullets"};
        int mask = LayerMask.GetMask(layers);
        mask = ~mask;
        if (Physics.Raycast(ray, out RaycastHit hit,1000,mask))
        {
            Debug.Log(hit.collider.name);
            return hit.point;
        }
        else
        {
            return curPosition;
        }
        
    }
    private void lookAtMouse()
    {
        //Vector3 mousePos = Input.mousePosition;
        //mousePos.x -= Screen.width / 2;
        //mousePos.y -= Screen.height / 2;

       


        //float targetAngle = Mathf.Atan2(mousePos.normalized.x, mousePos.normalized.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;


       // Vector3 lookat = new Vector3(playerGun.playerMiddleSpine.position.x + mousePos.x, playerGun.playerMiddleSpine.position.y,
        //    playerGun.playerMiddleSpine.position.z + mousePos.y);

       // Vector3 lookat2 = new Vector3(transform.position.x + mousePos.normalized.x, transform.position.y,
       //     transform.position.z + mousePos.normalized.y);



        if(!target.isDead && !isRolling && joked)
        {
            
            if (playerGun.meleeMode && !isMoving)
            {
                transform.LookAt(new Vector3(cylinder.position.x, transform.position.y, cylinder.position.z));
            }
            else
            {
                playerGun.playerMiddleSpine.LookAt(cylinder);
            }
        }
       
        //playerGun.playerMiddleSpine.eulerAngles = new Vector3(0, playerGun.playerMiddleSpine.eulerAngles.y + cam.transform.eulerAngles.y, 0);
        //playerGun.playerMiddleSpine.eulerAngles = new Vector3(0, targetAngle, 0);
        /*if (!isMoving)
        {
            transform.LookAt(cylinder);
            //playerGun.playerMiddleSpine.eulerAngles = new Vector3(0, transform.eulerAngles.y + cam.transform.eulerAngles.y, 0);
        }*/
       
       
       
    }
    private void LateUpdate()
    {
        lookAtMouse();
    }


    private void MoveWhenTold(bool isUsingKeys = true)
    {
        
        if (isUsingKeys)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                startJump();
            }
            if (Input.GetKeyDown("f"))
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
            }
            /*if (Input.GetKey(KeyCode.LeftControl))
            {
                isCrouching = true;
                if (!isTakingCover)
                {
                    capsuleCollider.center = new Vector3(0, -0.82f, 0.54f);
                    capsuleCollider.height = 4.6f;
                }

            }
            else
            {
                if(!isCrouchJumping && !isRolling)
                {
                    isCrouching = false;
                    if (!isTakingCover)
                    {
                        capsuleCollider.center = new Vector3(0, 0, 0);
                        capsuleCollider.height = 6;
                    }
                }
            }*/
            if (Input.GetKeyDown("g"))
            {
                //CoverTechnologies();
            }
            if (Input.GetKeyDown("u"))
            {
                joked = ragdoll.ragdollJokes(joked);
            }
        }
        if (isTakingCover)
        {
            transform.LookAt(coverPoint);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(coverPoint), 5 * Time.deltaTime);
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(coverPoint - transform.position), 5*Time.deltaTime);
        }
    }
   
    public static Vector3 checkAxisOfObjRelativeToTarget(Collider obj,Transform target)
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
    }


    private void startJump()
    {
        if (!target.isDead && !playerGun.meleeMode)
        {
            if (isOnFloor && !PlayerGun.isFineAim && !isJumping && !isRolling && !isCrouchJumping)
            {
                
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
    }


    public void jump()
    {
        if (!isCrouchJumping && !isJumping)
        {
            isJumping = true;

            if (!isCrouching)
            {
                rigidBody.AddForce(transform.up * jumpHeight);
                if (isMoving)
                {
                    rigidBody.AddForce(transform.forward * jumpForce);
                }
                
            }
            else
            {
                isCrouchJumping = true;
                rigidBody.AddForce(transform.up * jumpHeight * 0.5f);
                rigidBody.AddForce(transform.forward * jumpForce );
            }
        }
        
        //Debug.Log(direction.magnitude);
       
    }
   

    public void endJump()
    {
        if (!isCrouchJumping)
        {
            jumpAnim = false;
        }
        //isRolling = false;
        diveInterruptable = true;
    }
    public void JokeTrue()
    {
        if (joked)
        {
            joked = ragdoll.ragdollJokes(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isCrouchJumping)
        {
            isRolling = true;
            isCrouchJumping = false;
            isJumping = false;
            if (!collision.collider.CompareTag("floor"))
            {
                JokeTrue();
            }
        }
        else
        {
            isRolling = false;
        }
        if (!isOnFloor)
        {
            isOnFloor = true;
            isJumping = false;
            if (Input.GetKey(KeyCode.LeftControl)){
                isRolling = true;
                isCrouchJumping = false;
                isJumping = false;
            }
            
        }
        else
        {
            isJumping = false;
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
        }
        if (isTakingCover)
        {
            if (collision.collider.GetComponent<CoverAble>())
            {
                coverPoint = collision.GetContact(0).point;
                coverPoint.y = transform.position.y;
                transformToWall = collision.GetContact(0).point;
            }
        }

        if(isCrouchJumping && diveInterruptable)
        {
            jumpAnim = false;
            isCrouchJumping = false;
            isRolling = true;
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
    
    
    public void stopPunchImpact()
    {
        isBeingPunched = false;
    }


    public void setAnimatorWeight(float weight)
    {
        animator.SetLayerWeight(2, weight);
    }
    

    public void slowMo()
    {
        if (!isSlowMo && slowMoMeter >= 2 )
        {
            if (!gotVolume)
            {
                gameMech = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMech>();
                volume = gameMech.volume;
                WhiteBalance wb;
                if (volume.profile.TryGet(out wb))
                {
                    whiteBalance = wb;
                    defaultWhiteBalanceVal = whiteBalance.temperature.value;
                }
                gotVolume = true;
            }
            fresnelEffect.fresnelColor = slowMoFresnel;
            fresnelEffect.fresnate();
            isSlowMo = true;
            s = 0;
            ss = 0;
            slowMoBarGlow.SetTrigger("glow-up");
            jumpHeight = jumpHeight * 5;
            Time.timeScale = 0.2f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            
        }

    }


    private void slowMoTechnologies()
    {
        if (isSlowMo)
        {
            s += 1 * Time.deltaTime;
            ss += 1 * Time.deltaTime;
            slowMoBarRatio = (slowMoTime - s) / slowMoTime;
            slowMoBar.transform.localScale = new Vector3(slowMoBarRatio, 1, 1);

            if (ss <= .5f)
            {
                whiteBalance.temperature.value -= 140 * Time.deltaTime;

            }
            if (ss >= slowMoTime - 0.5f)
            {
                whiteBalance.temperature.value += 140 * Time.deltaTime;
            }
            if (s >= slowMoTime)
            {
                whiteBalance.temperature.value = defaultWhiteBalanceVal;
                fresnelEffect.fresnelColor = defaultFresnelColor;
                speed = startSpeed;
                initSpeed = startSpeed;
                halfSpeed = startSpeed;
                jumpHeight = jumpHeight / 5;
                Time.timeScale = 1;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                animator.speed = 1f;
                sss = s;
                slowMoBarGlow.SetTrigger("glow-down");
                slowMoMeter = Mathf.Clamp(slowMoTime - sss, 0, slowMoTime) / slowMoTime * slowMoCoolDown;
                isSlowMo = false;
                
                
            }

        }


        if (slowMoMeter < slowMoCoolDown)
        {
            slowMoMeter += 0.1f * Time.deltaTime;
            if (slowMoMeter >= slowMoCoolDown - .05f)
            {
            }
            slowMoBarRatio = slowMoMeter / slowMoCoolDown;
            slowMoBar.transform.localScale = new Vector3(slowMoBarRatio, 1, 1);
        }
    }



}
