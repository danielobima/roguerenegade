using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Cinemachine;
using Mirror;
using Mirror.Experimental;

public class PlayerMotion : NetworkBehaviour {

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
    //private GameObject slowMoBar;
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
    public PlayerMultiDetails playerMultiDetails;
    public Quaternion thisPlayersMiddlespineRotation;
    public Vector3 thisPlayersLookat;
    private Vector3 yBounds = new Vector3(0, 0.6f, 0);
    public GameObject virtualMiddleSpine;
    private NetworkRigidbody networkRigidbody;
    private GameObject possibleCarryAble;
    private GameObject currentCarryAble;
    private CarryAble currCarryAble;
    public Transform carryPos;
    private Material cylinderMat;
    [SyncVar]
    public bool isCarrying = false;
    private float yCarryLock = 0;
    private GameObject handContainerReal;
    private float handContainerSmoothRef;
    public EnemyManager currentEnemyManager;
    private GameObject currentTarget;
    private bool jumpMotion;
    private Vector3 jumpDirection;
    private bool isLanding = false;
    private bool isEndingJump = false;



    private void Start()
    {
        gameMech = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMech>();
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {

            playerMultiDetails = GetComponent<PlayerMultiDetails>();
           
            if (playerMultiDetails.isMultiPlayer)
            {
                
                GameMechMulti gameMechMulti = GameObject.FindGameObjectWithTag("GameMechMulti").GetComponent<GameMechMulti>();
               
                cylinder = Instantiate(gameMechMulti.aimCylinder).transform;
                cylinder.gameObject.SetActive(true);
               
            }
            else
            {
                cylinder = gameMech.cylinder;
            }
            cylinderMat = cylinder.GetChild(1).GetComponent<Renderer>().material;
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            rigidBody = GetComponent<Rigidbody>();
            
            animator = GetComponent<Animator>();
            ragdoll = GetComponent<RagdollSwitch>();
            myTransform = gameObject.transform;
            cam = Camera.main.gameObject;
            target = GetComponent<Target>();
            fresnelEffect = GetComponent<FresnelEffect>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            defaultFresnelColor = fresnelEffect.fresnelColor;
            //cylinder = GameObject.FindGameObjectWithTag("AimCylinder").transform;
           
            //slowMoBar = GameObject.FindGameObjectWithTag("SlowMoBar");
            //slowMoBarGlow = slowMoBar.transform.GetChild(0).GetComponent<Animator>();

            if (initializeOnStart)
            {
                UISetup();
            }
            Time.timeScale = 1;
            slowMoMeter = 2;
            
        }
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
    private void setVirtualMiddleSpine()
    {

        virtualMiddleSpine.transform.SetParent(playerGun.playerMiddleSpine.parent);
        virtualMiddleSpine.transform.position = playerGun.playerMiddleSpine.position;
        virtualMiddleSpine.transform.rotation = playerGun.playerMiddleSpine.rotation;
        playerGun.playerMiddleSpine.SetParent(virtualMiddleSpine.transform);

        handContainerReal = Instantiate(new GameObject("handContainer"), playerGun.playerMiddleSpine);
        handContainerReal.transform.localPosition = new Vector3();
        handContainerReal.transform.localEulerAngles = new Vector3();
        playerGun.playerMiddleSpine.GetChild(2).SetParent(handContainerReal.transform);
        playerGun.playerMiddleSpine.GetChild(1).SetParent(handContainerReal.transform);
        //carryPos.SetParent(virtualMiddleSpine.transform);

    }
    private void FixedUpdate()
    {
        if ((isLocalPlayer || !playerMultiDetails.isMultiPlayer)  )
        {

            movementTechnologies();
            if (!GameMech.gameIsPaused && !playerMultiDetails.isTyping) {
                //slowMoTechnologies();
                //cylinder.position = MoveCylinder() + yBounds;

                //cylinder.LookAt(new Vector3(transform.position.x, cylinder.position.y, transform.position.z));
                
                
                if (!isRolling && joked)
                {
                    //virtualMiddleSpine.transform.LookAt(cylinder);
                    aimTechnologies();

                }
            }
            
           
        }
       
    }
    private void aimTechnologies()
    {
        if (!target.isDead)
        {
            
            Vector3 lTargetDir = cylinder.position - transform.position;
            //lTargetDir.y = 0.0f;
            Quaternion targetRotation = Quaternion.LookRotation(lTargetDir);
            Vector3 lTargetDirNoY = lTargetDir;
            lTargetDirNoY.y = 0.0f;
            Quaternion targetRotationNoY = Quaternion.LookRotation(lTargetDirNoY);


            /*if (!isCarrying)
            {
                virtualMiddleSpine.transform.rotation = Quaternion.Slerp(virtualMiddleSpine.transform.rotation, targetRotation, 10 * Time.deltaTime);
            }
            else
            {
                virtualMiddleSpine.transform.rotation = Quaternion.Slerp(virtualMiddleSpine.transform.rotation, targetRotationNoY, 10 * Time.deltaTime);
                
            }
            if (!isMoving)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationNoY, 10 * Time.deltaTime);

            }*/
            //carryPos.eulerAngles = new Vector3(0, virtualMiddleSpine.transform.eulerAngles.y, 0);

            if (playerGun.gun != null)
            {
                if (isCrouching && !isJumping && isOnFloor)
                {
                    //float x = Mathf.SmoothDampAngle(handContainerReal.transform.localEulerAngles.x, isMoving ? -55 : -26, ref handContainerSmoothRef, 0.1f);
                    //handContainerReal.transform.localEulerAngles = new Vector3(x, 0, 0);
                }
                else
                {
                    //float x = Mathf.SmoothDampAngle(handContainerReal.transform.localEulerAngles.x, 0, ref handContainerSmoothRef, 0.1f);
                    //handContainerReal.transform.localEulerAngles = new Vector3(x, 0, 0);
                }
            }
        }
    }


    private void Update()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer  )
        {
            if (!GameMech.gameIsPaused && !playerMultiDetails.isTyping)
            {
                MoveWhenTold(isUsingKeyBoard);
                CarryTechnologies();
                if (Input.GetMouseButtonDown(1))
                {
                    SetTarget();
                }
            }
               
        }

    }
    private void CoverTechnologies()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
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

    float rotAngle;
    private void Move()
    {

        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            animator.SetFloat("InputX", Mathf.MoveTowards(animator.GetFloat("InputX"),horizontal,Time.deltaTime * 5));
            animator.SetFloat("InputY", Mathf.MoveTowards(animator.GetFloat("InputY"), vertical, Time.deltaTime * 5));
            direction = new Vector3(horizontal, 0, vertical).normalized;
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            if (!obstaculated   && !isTakingCover && !isRolling && !isLanding  && !isCrouchJumping)
            {
                if (!isBeingPunched)
                {
                    float target = 
                        //targetAngle + 
                        cam.transform.eulerAngles.y;

                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, target, ref turnSmoothRef, turnSmoothing);


                    transform.rotation = Quaternion.Euler(0, target, 0);
                    if (direction.magnitude >= .1f && !GameMech.gameIsPaused && !playerMultiDetails.isTyping)
                    {

                        if (!PlayerGun.isFineAim)
                        {
                            if (isCrouching && isOnFloor && !isJumping)
                            {


                                //rigidBody.velocity = transform.forward * speed;
                                //myTransform.Translate(Vector3.forward * Time.deltaTime * speed * 0.5f);
                            }
                            else
                            {

                                if (!isGoingBack)
                                {
                                    //myTransform.Translate(Vector3.forward * Time.deltaTime * speed);
                                }
                                else
                                {
                                    //myTransform.Translate(Vector3.back * Time.deltaTime * speed);
                                }

                            }
                        }
                        else
                        {
                            //myTransform.Translate(Vector3.right * Time.deltaTime * direction.x * speed * 0.25f);
                            //myTransform.Translate(Vector3.forward * Time.deltaTime * direction.z * speed * 0.25f);


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
                    //myTransform.Translate(movementDir * horizontal * speed * 0.25f * Time.deltaTime);
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
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            Setspeed();
            if (!target.isDead)
            {
                if (isUsingKeyBoard)
                {
                    Move();

                    if (isMoving)
                    {

                        if (isOnFloor && !isTakingCover && !isEndingJump)
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
                            if (!isTakingCover && !jumpAnim && !isEndingJump && !isCrouchJumping &&!isLanding)
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
    private void coverAnim()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
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
            }
            else
            {
                animator.SetInteger("motion", 11);
            }
        }
    }
    [Command]
    private void CmdCarry(GameObject CarryAble,NetworkConnectionToClient conn = null)
    {
        CarryAble carryAble = CarryAble.GetComponent<CarryAble>();
        if (!carryAble.isBeingCarried)
        {
            carryAble.isBeingCarried = true;
            carryAble.hasSlerped = false;
            carryAble.carryPos = carryPos;
            TargetCarry(conn, CarryAble);
            isCarrying = true;
        }
    }
    [TargetRpc]
    private void TargetCarry(NetworkConnection conn, GameObject CarryAble)
    {
        CarryAble carryAble = CarryAble.GetComponent<CarryAble>();
        carryAble.carryPos = carryPos;
        currentCarryAble = CarryAble;
        possibleCoverAble = null;
    }

    [Command]
    private void CmdPlace(GameObject CarryAble,Vector3 pos,NetworkConnectionToClient conn = null)
    {
        CarryAble carryAble = CarryAble.GetComponent<CarryAble>();
        Collider c = CarryAble.GetComponent<Collider>();
        if (carryAble.isBeingCarried)
        {
            carryAble.isBeingCarried = false;
            carryAble.carryPos = null;
            carryAble.placePos = pos;
            carryAble.isPlacing = true;
            TargetPlace(conn, CarryAble);
            isCarrying = false;
        }

    }
    [TargetRpc]
    private void TargetPlace(NetworkConnection conn,GameObject CarryAble)
    {
        CarryAble carryAble = CarryAble.GetComponent<CarryAble>();
        carryAble.carryPos = null;
        cylinderMat.SetColor("cylinderColor", Color.white);
        currentCarryAble = null;
        animator.SetInteger("hold", 0);
    }

    private void Place(CarryAble carryAble)
    {
        Collider c = carryAble.GetComponent<Collider>();
        if (carryAble.isBeingCarried)
        {
            carryAble.isBeingCarried = false;
            carryAble.carryPos = null;
            carryAble.isPlacing = true;
            isCarrying = false;
            cylinderMat.SetColor("cylinderColor", Color.white);
            currentCarryAble = null;
            animator.SetInteger("hold", 0);
            cylinder.gameObject.SetActive(true);
        }
    }
    private void Carry(CarryAble carryAble)
    {
        if (!carryAble.isBeingCarried)
        {
            carryAble.isBeingCarried = true;
            carryAble.hasSlerped = false;
            carryAble.carryPos = carryPos;
            isCarrying = true;
            currentCarryAble = carryAble.gameObject;
            currCarryAble = carryAble;
            possibleCoverAble = null;
            carryAble.carrier = this;
            cylinder.gameObject.SetActive(false);
        }
    }


    private void CarryTechnologies()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        string[] layers = { "Player", "Enemies", "Armature bones", "bullets" };
        int mask = LayerMask.GetMask(layers);
        mask = ~mask;
        if (Physics.Raycast(ray, out RaycastHit hit, 1000, mask))
        {
            // Debug.Log(hit.collider.name);
            CarryAble carryAble = hit.collider.GetComponent<CarryAble>();
            if (carryAble == null)
            {

                if (possibleCarryAble != null)
                {
                    FresnelEffect effect = possibleCarryAble.GetComponent<FresnelEffect>();
                    FresnelHighlight highlight = possibleCarryAble.GetComponent<FresnelHighlight>();
                    if (effect != null)
                    {
                        effect.defresnate();
                    }
                    if (highlight != null)
                    {
                        highlight.defresnate();
                    }
                    possibleCoverAble = null;
                }
                if (currentCarryAble != null)
                {
                    if (currCarryAble != null)
                    {
                        float dist = Vector3.Distance(currCarryAble.transform.position, hit.point);
                        
                        if (dist <= 5)
                        {
                            Vector3 pos = hit.point + new Vector3(0, currCarryAble.extents.y, 0);
                            currCarryAble.placePos = pos;
                            //currCarryAble.placePos = Vector3.Lerp(currCarryAble.placePos, pos, 20 * Time.deltaTime);
                            yCarryLock = currCarryAble.placePos.y;
                        }
                        else
                        {
                            float ratio = dist / 5;
                            Vector3 direction = (-currCarryAble.transform.position +cylinder.position);
                            Vector3 pos = currCarryAble.transform.position + direction / ratio;

                            if (Physics.Raycast(pos,Vector3.down,out RaycastHit floor))
                            {
                                float x = pos.y;
                                pos.y = x - floor.distance + currCarryAble.extents.y;
                            }
                            else
                            {
                                pos.y = yCarryLock;
                            }
                            currCarryAble.placePos = pos;
                            
                            //currCarryAble.placePos = Vector3.Lerp(currCarryAble.placePos, pos, 20 * Time.deltaTime);
                        }
                        
                    }
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (playerMultiDetails.isMultiPlayer)
                        {
                            if (isLocalPlayer)
                            {
                                CmdPlace(currentCarryAble, hit.point);
                            }
                        }
                        else
                        {
                            Place(currentCarryAble.GetComponent<CarryAble>());
                        }
                    }
                }

            }
            else
            {
                if (Vector3.Distance(transform.position, carryAble.transform.position) <= 5)
                {
                    possibleCarryAble = carryAble.gameObject;
                    FresnelEffect effect = possibleCarryAble.GetComponent<FresnelEffect>();
                    FresnelHighlight highlight = possibleCarryAble.GetComponent<FresnelHighlight>();
                    if (effect != null)
                    {
                        effect.fresnate();
                    }
                    if (highlight != null)
                    {
                        highlight.fresnate();
                    }
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (currentCarryAble == null)
                        {

                            if (playerMultiDetails.isMultiPlayer)
                            {
                                if (isLocalPlayer)
                                {
                                    CmdCarry(possibleCarryAble);
                                }
                            }
                            else
                            {
                                Carry(carryAble);
                            }

                        }


                    }
                }
            }
        }
        else
        {
            if (possibleCarryAble != null)
            {
                FresnelEffect effect = possibleCarryAble.GetComponent<FresnelEffect>();
                FresnelHighlight highlight = possibleCarryAble.GetComponent<FresnelHighlight>();
                if (effect != null)
                {
                    effect.defresnate();
                }
                if (highlight != null)
                {
                    highlight.defresnate();
                }
                possibleCoverAble = null;
            }
        }
        if (isCarrying)
        {
            animator.SetInteger("hold", 1);
        }
        else
        {
            animator.SetInteger("hold", 0);
        }
      
    }
   
    private Vector3 MoveCylinder()
    {

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        string[] layers = { "Player", "Enemies", "Armature bones", "bullets","Ignore Raycast" };
        int mask = LayerMask.GetMask(layers);
        mask = ~mask;
        if (Physics.Raycast(ray, out RaycastHit hit, 1000, mask))
        {
            // Debug.Log(hit.collider.name);
           
            return hit.point;
        }
        else
        {
            
            return curPosition;
        }


    }
    
    
    
    


    private void MoveWhenTold(bool isUsingKeys = true)
    {

        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
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
                        if (!isTakingCover)
                        {
                            capsuleCollider.center = new Vector3(0.0008849353f, 0.773929f, 0.01664619f);
                            capsuleCollider.height = 1.569248f;
                        }
                    }

                }
                else
                {
                    if(!isCrouchJumping && !isRolling)
                    {
                        isCrouching = false;
                        if (!isTakingCover)
                        {
                            capsuleCollider.center = new Vector3(0.0008849353f, 0.773929f, 0.01664619f);
                            capsuleCollider.height = 1.569248f;
                        }
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
            if (isTakingCover)
            {
                transform.LookAt(coverPoint);
                //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(coverPoint), 5 * Time.deltaTime);
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(coverPoint - transform.position), 5*Time.deltaTime);
            }
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
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
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

    }


    public void endRoll()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            isRolling = false;
            jumpAnim = false;
            isCrouchJumping = false;
        }
    }
    public void endLand()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            isLanding = false;
            jumpAnim = false;
            isCrouchJumping = false;
            isEndingJump = false;
        }
    }


    public void jump()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            if (!isCrouchJumping && !isJumping)
            {

                if (!isCrouching)
                {
                    isJumping = true;
                    rigidBody.AddForce(transform.up * jumpHeight);
                    if (isMoving)
                    {
                        jumpDirection = Vector3.forward;
                        jumpMotion = true;

                        //rigidBody.AddForce(transform.forward * jumpForce);
                    }
                }
                
            }
        }

        //Debug.Log(direction.magnitude);

    }


    public void endJump()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            if (!isCrouchJumping)
            {
                jumpAnim = false;
                isEndingJump = true;
            }
            //isRolling = false;
            diveInterruptable = true;
            
            
        }
    }
    public void JokeTrue()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            if (joked)
            {
                joked = ragdoll.ragdollJokes(true);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer )
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
    }


    private void OnCollisionStay(Collision collision)
    {


        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
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
            if (isTakingCover)
            {
                if (collision.collider.GetComponent<CoverAble>())
                {
                    coverPoint = collision.GetContact(0).point;
                    coverPoint.y = transform.position.y;
                    transformToWall = collision.GetContact(0).point;
                }
            }

            if (isCrouchJumping && diveInterruptable )
            {
                jumpAnim = false;
                isEndingJump = false;
                isCrouchJumping = false; 
                jumpMotion = false;
                //isRolling = true;
                isJumping = false;
            }
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            if (collision.collider.GetComponent<CoverAble>())
            {
                possibleCoverAble = null;
            }
        }
    }


    public void stopPunchImpact()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            isBeingPunched = false;
        }
    }


    public void setAnimatorWeight(float weight)
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            animator.SetLayerWeight(2, weight);
        }
    }
    private void SetTarget()
    {
        if(currentEnemyManager != null)
        {
            int c =  currentEnemyManager.enemies.Count;
            if (c > 0)
            {
                for(int i = 0; i < c; i++)
                {
                    if (currentTarget == null)
                    {
                        Vector3 screenPoint = Camera.main.WorldToViewportPoint(currentEnemyManager.enemies[i].transform.position);
                        if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
                        {
                            if (Physics.Raycast(cam.transform.position, currentEnemyManager.enemies[i].transform.position - cam.transform.position, out RaycastHit hit,100, LayerMask.NameToLayer("Player")))
                            {
                                if (hit.collider.gameObject.layer == 19 || hit.collider == currentEnemyManager.enemies[i].GetComponent<Collider>())
                                {
                                    if (hit.collider.gameObject.layer == 19)
                                    {
                                        BodyPart bo = hit.collider.gameObject.GetComponent<BodyPart>();
                                        if (bo != null)
                                            if (bo.mainBody.GetComponent<EnemyMech>())
                                            {
                                                //currentTarget = bo.mainBody.gameObject;
                                                Debug.Log("Found one");
                                                break;
                                            }
                                    }
                                    else
                                    {
                                        //currentTarget = currentEnemyManager.enemies[i].gameObject;
                                        Debug.Log("Found one");
                                        break;
                                    }
                                }
                                else
                                {
                                    Debug.Log(hit.collider.name);
                                }
                            }
                            else
                                Debug.Log("not found");
                        }
                        else
                            Debug.Log("not found");

                    }
                    else
                        break;
                }
            }
        }
    }


    public void slowMo()
    {
        /*if (!isSlowMo && slowMoMeter >= 2 )
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
            
        }*/

    }


    private void slowMoTechnologies()
    {
        /*if (isSlowMo)
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
        }*/
    }

}
