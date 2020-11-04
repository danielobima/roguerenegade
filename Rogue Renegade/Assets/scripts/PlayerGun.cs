using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using DigitalRuby.Tween;

using Mirror;


public class PlayerGun : NetworkBehaviour {

    private ParticleSystem muzzleFlash;
    public ParticleSystem smoke;
    public GameObject bullet;
    public GameObject shotGunBullet;
    
    public GameObject gun;
    public GameObject gun2;
    public GameObject secondaryGun;

    private GameObject multiGun;
    private GameObject multiSecondaryGun;
    private GameObject multiGrenade;
    private GunDetails multiGunDetails;

    private GunDetails gunDetails;
    private Transform bulletSpawnPos;
    public GameObject grenade;
    private Rigidbody gunRigidBody;
    public bool hasHandgun = false;

    public Transform leftHip;
    public Transform leftElbow;
    public Transform rightElbow;
    public Transform rightArm;
    public Transform head;
    private Animator animator;
    public Transform playerMiddleSpine;

    private int ammoMax;
    private int newAmmo;
    private int ammoPreserve;
    public bool attacking = false;
    private float timeToShoot;
    public float addSpeed = 0.5f;
    public float meleeDamage = 0;
    private float throwForce = 500;
    public bool isReloading = false;
    public bool isPunching = false;
    public bool meleeMode = false;
    private float meleeCountdown = 0;
    public float weight = 0;
    private bool isLoadingThrow = false;
    public float throwForceConstant = 16.6666f;
    public float throwForceConstantUp = 30;
    public GameObject bloodParticleSystem;

    private Target t;
    private RagdollSwitch ragdollSwitch;
    private PlayerMotion playerMotion;
    private GameMech gameMech;

    
    private ScreenObjects screenObjects;
    private ScreenTexts screenTexts;
    private bool hasChangedGun = false;
    private bool setGunPos = false;
    private bool hasRemovedParent = false;
    private bool isSwitchingGun = false;
    public string ammoText;

    
    public static float mouseSensitivity = 100f;
    private float xRotation = 0f;
    private float yRotation = 0f;
    public static bool isFineAim;
    public static bool isVeryFineAim;
    //public CinemachineVirtualCamera veryFineAimCam;
    //public CinemachineVirtualCamera fineAimCam;
    public CinemachineFreeLook thirdPersonCam;
    //private Image crossHair;
    //private GameObject scopeBlackness;
    //private bool hasNotLooked = true;
    private GameObject veryFineAimRef;
    private GameObject aimRef;
    
    private float recoilRef = 0;
    private float recoilTime = 0;
    public bool isRecoil = false;
    private bool recoilUp = true;
    private Vector3 defaultRightArmRot = new Vector3();
    private Vector3 defaultLeftArmRot;
    private float recoilEasing = 0;


    //Networking Stuffs
    public PlayerMultiDetails playerMultiDetails;
    private GameMechMulti gameMechMulti;

    [SyncVar]
    public int primaryGunInt = -1;
    [SyncVar]
    public int secondaryGunInt = -1;


    //To make gun drops accessible by the pick up button
    [Header("LEAVE THIS EMPTY PLEASE")]
    public GameObject gunDrop;
    public GameObject grenadeDrop;

    private void Start()
    {
        gameMechMulti = GameObject.FindGameObjectWithTag("GameMechMulti").GetComponent<GameMechMulti>();
        gameMech = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMech>();
        ragdollSwitch = GetComponent<RagdollSwitch>();
        playerMotion = GetComponent<PlayerMotion>();
        animator = GetComponent<Animator>();
        playerMultiDetails = GetComponent<PlayerMultiDetails>();
        
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            primaryGunInt = -1;
            secondaryGunInt = -1;

            if (playerMultiDetails.isMultiPlayer)
            {
                
                thirdPersonCam = Instantiate(gameMechMulti.tpp).GetComponent<CinemachineFreeLook>();
                thirdPersonCam.gameObject.SetActive(true);
                
            }
            else
            {
                thirdPersonCam = gameMech.tpp;

            }
                
            t = GetComponent<Target>();
            screenObjects = GameObject.FindGameObjectWithTag("screen objects").GetComponent<ScreenObjects>();
            screenTexts = gameMech.screentexts;
            screenTexts.playerGun = this;
            //playerMiddleSpine = GameObject.FindGameObjectWithTag("PlayerMiddleSpine").transform;


            //thirdPersonCam = GameObject.FindGameObjectWithTag("thirdPersonCam").GetComponent<CinemachineFreeLook>();
            thirdPersonCam.m_Follow = transform;
            thirdPersonCam.m_LookAt = transform;

            /*crossHair = GameObject.FindGameObjectWithTag("crossHair").GetComponent<Image>();
            scopeBlackness = GameObject.FindGameObjectWithTag("scopeBlackness");
            crossHair.gameObject.SetActive(false);
            scopeBlackness.GetComponent<SVGImage>().color = new Color(1, 1, 1, 1);
            scopeBlackness.SetActive(false);*/




            if (!playerMultiDetails.isMultiPlayer)
            {
                setGun(true, 36);
            }
            if (gun != null || secondaryGun != null)
            {
                ragdollSwitch.getGuns();
            }
#if (UNITY_IPHONE || UNITY_ANDROID)
        if (!playerMotion.isUsingKeyBoard)
        {
            throwButton.SetActive(false);
        }
#endif
        }
        else
        {
            if (playerMultiDetails.isMultiPlayer)
            {

                if (primaryGunInt != -1)
                {
                    multiGun = Instantiate(gameMechMulti.guns[primaryGunInt],rightElbow);
                    GunDetails g = multiGun.GetComponent<GunDetails>();
                    Rigidbody r = multiGun.GetComponent<Rigidbody>();
                    if (r != null) Destroy(r);
                    Collider c = multiGun.GetComponent<Collider>();
                    if (c != null) Destroy(c);
                    multiGun.transform.localPosition = g.localPos;
                    multiGun.transform.localEulerAngles = g.localRot;
                    multiGun.transform.localScale = g.localScale;
                    multiGun.tag = "Untagged";
                }
                if (secondaryGunInt != -1)
                {

                    multiSecondaryGun = Instantiate(gameMechMulti.guns[secondaryGunInt]);
                    Rigidbody r = multiSecondaryGun.GetComponent<Rigidbody>();
                    if (r != null) Destroy(r);
                    Collider c = multiSecondaryGun.GetComponent<Collider>();
                    if (c != null) Destroy(c);
                    multiSecondaryGun.transform.SetParent(playerMiddleSpine);
                    multiSecondaryGun.transform.localPosition = new Vector3(-0.001965688f, 0.004141954f, -0.01004855f);
                    multiSecondaryGun.transform.localEulerAngles = new Vector3(-52.211f, -76.771f, 164.239f);
                    multiSecondaryGun.tag = "Untagged";

                }

            }
        }



    }
    private void setGun(bool overwriteAmmo, int ammoToPut = 0)
    {

        
        gun = GameObject.FindGameObjectWithTag("Playergun");
        if (gun != null)
        {
            gunDetails = gun.GetComponent<GunDetails>();
            muzzleFlash = gun.transform.GetChild(0).GetComponent<ParticleSystem>();
            bulletSpawnPos = gun.transform.GetChild(0).transform;
            ammoMax = gunDetails.ammoMax;
        //ammo = ammoMax;
        if (overwriteAmmo)
        {
            gunDetails.ammoSpare = ammoToPut;
        }
        else
        {
            gunDetails.ammoLoaded = ammoPreserve;
        }
            timeToShoot = gunDetails.timeToNextShot;
            hasHandgun = gunDetails.handgun;
            gunRigidBody = gun.GetComponent<Rigidbody>();
            setGunPos = false;
            updateGunPos();

            if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
            {
                if (gameMech == null)
                {
                    gameMech = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMech>();
                    gameMech.screentexts.changeGunIcon();
                }
                else
                {
                    gameMech.screentexts.changeGunIcon();
                }
                if (veryFineAimRef != null)
                {
                    Destroy(veryFineAimRef);
                    veryFineAimRef = null;
                }
            }
        }

        
        

    }
    [Command]
    private void updatePrimaryGun(int no, uint net)
    {
        if(net == netId)
        {
            primaryGunInt = no;
        }
    }
    [Command]
    private void updateSecondaryGun(int no, uint net)
    {
        if(net == netId)
        {
            secondaryGunInt = no;
        }
    }
    private void setgun2(bool overwriteAmmo, int ammoToPut = 0 )
    {
       
        if (gun != null)
        {
            gunDetails = gun.GetComponent<GunDetails>();
            muzzleFlash = gun.transform.GetChild(0).GetComponent<ParticleSystem>();
            bulletSpawnPos = gun.transform.GetChild(0).transform;
            ammoMax = gunDetails.ammoMax;
            //ammo = ammoMax;
            if (overwriteAmmo)
            {
                gunDetails.ammoSpare = ammoToPut;
            }
            else
            {
                gunDetails.ammoLoaded = ammoPreserve;
            }
               
            timeToShoot = gunDetails.timeToNextShot;
            hasHandgun = gunDetails.handgun;
            gunRigidBody = gun.GetComponent<Rigidbody>();
            setGunPos = false;
            updateGunPos();
            if(isLocalPlayer) updatePrimaryGun(gunDetails.gunInt, netId);
            if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
            {
                if (gameMech == null)
                {
                    gameMech = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMech>();
                    gameMech.screentexts.changeGunIcon();
                }
                else
                {
                    gameMech.screentexts.changeGunIcon();
                }
                if (veryFineAimRef != null)
                {
                    Destroy(veryFineAimRef);
                    veryFineAimRef = null;
                }
            }
        }
        
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            if (gun != null && !t.isDead)
            {

                if (attacking)
                {
                    if (!gunDetails.gunType.Equals("RPG7"))
                    {

                        if (timeToShoot >= gunDetails.timeToNextShot)
                        {

                            Shoot();
                        }
                        else
                        {

                            if (!PlayerMotion.isSlowMo)
                            {
                                timeToShoot += addSpeed * Time.deltaTime;
                            }
                            else
                            {
                                //timeToShoot += addSpeed * Time.deltaTime * 5;
                                timeToShoot += addSpeed * Time.deltaTime;
                            }
                        }
                    }
                    else
                    {
                        if (gunDetails.GetComponent<RPG7>().loaded)
                        {

                            Shoot();
                        }


                    }
                }
                else
                {
                    if (gunDetails.gunSound.isPlaying)
                    {
                        gunDetails.gunSound.loop = false;
                    }
                }
                if (timeToShoot < gunDetails.timeToNextShot)
                {
                    timeToShoot += 1 * Time.deltaTime;
                }
                if (gunDetails.gunType.Equals("RPG7"))
                {
                    if (!gunDetails.GetComponent<RPG7>().loaded)
                    {
                        if (timeToShoot >= gunDetails.timeToNextShot)
                        {
                            gunDetails.GetComponent<RPG7>().LoadRocket();
                            timeToShoot = 0;


                        }
                        else
                        {
                            timeToShoot += 1 * Time.deltaTime;
                        }
                    }
                }

            }





            gunDrop = gunDroppedNearby();
            grenadeDrop = grenadeDroppedNearby();
            if (gunDrop != null || grenadeDrop != null)
            {
                if (gunDrop != null)
                {
                    screenObjects.pickUpGunButton.SetActive(true);
                    GameObject[] otherGuns = GameObject.FindGameObjectsWithTag("DroppedGun");
                    foreach (GameObject g in otherGuns)
                    {
                        if (g.GetComponent<FresnelHighlight>().isFresnating)
                        {
                            g.GetComponent<FresnelHighlight>().defresnate();
                        }
                    }
                    gunDrop.GetComponent<FresnelHighlight>().fresnate();


                }
                if (grenadeDrop != null)
                {
                    if (grenade == null)
                    {
                        screenObjects.pickUpGunButton.SetActive(true);
                    }
                    GameObject[] otherGrenades = GameObject.FindGameObjectsWithTag("DroppedGrenade");
                    foreach (GameObject g in otherGrenades)
                    {
                        if (g.GetComponent<FresnelHighlight>().isFresnating)
                        {
                            g.GetComponent<FresnelHighlight>().defresnate();
                        }
                    }
                    grenadeDrop.GetComponent<FresnelHighlight>().fresnate();
                }
            }
            else
            {
                screenObjects.pickUpGunButton.SetActive(false);
            }


            if (gun != null)
            {
                if (t.isDead && !hasRemovedParent)
                {
                    gun.transform.SetParent(null);
                    if (secondaryGun != null)
                    {
                        secondaryGun.transform.SetParent(null);
                    }
                    hasRemovedParent = true;
                }
                RecoilSciences(gunDetails.recoilTime);
            }
            else
            {
                if (hasChangedGun)
                {
                    setGun(false);
                    hasChangedGun = false;
                }
            }
            //animator.Play("punch",1);
            if (!t.isDead && gun != null)
            {
                gun.GetComponent<Collider>().enabled = false;
            }
            /*if (!playerMotion.isUsingKeyBoard)
            {
                if (gun != null || secondaryGun != null)
                {
                    switchGunButton.SetActive(true);
                }
                else
                {
                    switchGunButton.SetActive(false);
                }
                if (gun != null)
                {
                    dropGunButton.SetActive(true);
                }
                else
                {
                    dropGunButton.SetActive(false);
                }
            }*/
            if (meleeCountdown < 4)
            {
                meleeCountdown += 1 * Time.deltaTime;
            }
            else
            {
                if (meleeMode)
                {
                    fightingMode(false);
                    meleeMode = false;
                    animator.SetInteger("punch", 0);
                }
            }
            if (animator.GetInteger("punch") == 0 || animator.GetInteger("punch") == 1)
            {
                isPunching = false;
            }
        }
    }
    public void punch()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 0.5f))
            {
                if (hit.collider.GetComponent<Target>())
                {
                    Target t;
                    t = hit.collider.GetComponent<Target>();
                    t.TakeDamage(meleeDamage);
                    t.TakePunchForce(transform, 1000);
                }
                if (hit.collider.CompareTag("enemy"))
                {
                    EnemyMech em = hit.collider.GetComponent<EnemyMech>();
                    em.isBeingPunched = true;


                    Transform mepos = em.transform;
                    em.transform.LookAt(new Vector3(transform.position.x, mepos.position.y, transform.position.z));
                    Animator a = hit.collider.GetComponent<Animator>();
                    a.SetTrigger("impact");
                }

            }
        }
       

    }
    
    public void setAnimatorWeight(float weight)
    {
        //animator.SetLayerWeight(1, weight);

    }
    private bool isStoppingFightingMode = false;
    public void fightingMode(bool on)
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            if (on)
            {
                weight = 1;
                animator.SetLayerWeight(2, weight);
                isStoppingFightingMode = false;
            }
            else
            {

                if (!isStoppingFightingMode)
                {
                    StartCoroutine(stopFightingMode());
                    isStoppingFightingMode = true;
                }
            }
        }
    }
    IEnumerator stopFightingMode()
    {
        for(weight = 1; weight > 0; weight -= 0.3f)
        {
            animator.SetLayerWeight(2, weight);
            yield return new WaitForSeconds(.1f);
        }
    }
    public void endPunch()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            animator.SetInteger("punch", 0);
            isPunching = false;
        }
    }
    public void punchWhenTold()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            if (!t.isDead && playerMotion.isOnFloor)
            {
                if (!isPunching)
                {
                    meleeMode = true;
                    fightingMode(true);
                    playerMotion.isMoving = false;
                    animator.SetInteger("punch", Random.Range(2, 7));

                    isPunching = true;
                    playerMiddleSpine.eulerAngles = new Vector3();
                    if (t.isAiming)
                    {
                        Transform target = t.currentLookAt.transform;
                        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
                    }
                    meleeCountdown = 0;
                }
            }
        }
        
        
        
    }
    private void Update()
    {
        if ((isLocalPlayer || !playerMultiDetails.isMultiPlayer) && !GameMech.gameIsPaused && !playerMultiDetails.isTyping)
        {
            CameraTechnologies();
            if (gun != null)
            {
                if (!t.isDead)
                {
                    gun.GetComponent<Collider>().enabled = false;
                    if (secondaryGun != null)
                    {
                        secondaryGun.GetComponent<Collider>().enabled = false;
                    }

                }
                else
                {
                    isFineAim = false;
                    //hasNotLooked = true;
                    thirdPersonCam.Priority = 10;
                    //veryFineAimCam.Priority = 9;
                    //crossHair.gameObject.SetActive(false);
                }
                if (hasHandgun)
                {
                    animator.SetInteger("upperbody", 2);
                }
                else
                {
                    animator.SetInteger("upperbody", 1);
                }
                //Debug.DrawRay(gun.transform.position, gun.transform.forward * -20, Color.blue);
                //Debug.DrawRay(veryFineAimCam.transform.position, veryFineAimCam.transform.forward * 20, Color.green);
            }

            else
            {
                animator.SetInteger("upperbody", 0);
            }
            if (playerMotion.isUsingKeyBoard)
            {
                if (gun != null)
                {
                    if (Input.GetMouseButton(0))
                    {
                        attacking = true;

                    }
                    else
                    {
                        attacking = false;
                    }
                }
                else
                {
                    /*if (Input.GetMouseButtonDown(0))
                    {
                        punchWhenTold();
                    }*/


                }
                if (Input.GetKeyDown("q"))
                {
                    startSwitchGunAnim();
                    if (meleeMode)
                    {
                        fightingMode(false);
                        meleeMode = false;
                        animator.SetInteger("punch", 0);
                    }
                }
                if (Input.GetKeyDown("r"))
                {
                    if(gun != null)
                    {
                        if (gunDetails.ammoSpare > 0)
                        {
                            startReloadAnimation();
                        }
                    }

                }
                if (Input.GetKeyDown("x"))
                {
                    dropGun();
                }
                if (Input.GetKeyDown("c") )
                {
                    throwGrenade();
                    //changeToFineAim();
                    if (meleeMode)
                    {
                        fightingMode(false);
                        meleeMode = false;
                        animator.SetInteger("punch", 0);
                    }


                }
                /*if (Input.GetKeyUp("c") && isLoadingThrow)
                {
                    stopLoadingThrow();
                    changeToTPP();

                }*/
                if (Input.GetKeyDown("e"))
                {
                    if (gunDrop != null)
                    {
                        
                        if (playerMultiDetails.isMultiPlayer)
                        {
                            GunDetails n = gunDrop.GetComponent<GunDetails>();

                           
                            CmdchangeGun(gunDrop, n.gunInt,n.gunID, netId);
                            //Debug.Log(n.gunInt);
                        }
                        else
                        {
                            changeGun(gunDrop);
                        }
                        if (meleeMode)
                        {
                            fightingMode(false);
                            meleeMode = false;
                            animator.SetInteger("punch", 0);
                        }
                    }
                    else
                    {
                        if (grenadeDrop != null)
                        {
                            if (playerMultiDetails.isMultiPlayer)
                            {
                                CmdPickupGrenade(grenadeDrop, netId);
                            }
                            else
                            {
                                pickupGrenade(grenadeDrop);
                            }
                        }
                        if (meleeMode)
                        {
                            fightingMode(false);
                            meleeMode = false;
                            animator.SetInteger("punch", 0);
                        }
                    }

                }
            }

            if (gun != null)
            {
                if (!isReloading)
                {
                    if (!gunDetails.gunType.Equals("RPG7"))
                    {
                        ammoText = string.Format("{0}/{1}", gunDetails.ammoLoaded, gunDetails.ammoSpare);
                    }
                    else
                    {
                        ammoText = gunDetails.ammoSpare.ToString();
                    }
                }
                else
                {

                }
            }
        }
    }
   
    private void CameraTechnologies()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            /*if (Input.GetMouseButtonDown(1))
        {
            changeToFineAim();
            //if (hasHandgun)
            //{
            //   crossHair.rectTransform.localPosition = new Vector3(-30, -5, 0);
            //}
            //else
            //{
            //    crossHair.rectTransform.localPosition = new Vector3(-10, 0, 0);
            //}

        }
        if (Input.GetMouseButton(1) && Input.GetKeyDown(KeyCode.LeftShift))
        {
            changeToVeryFineAim();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && isVeryFineAim)
        {
            changeFromVeryFineAim();
        }
        if (Input.GetMouseButtonUp(1))
        {
            changeToTPP();
        }
        if (playerMotion.isTakingCover)
        {
            if(isFineAim && animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
            {
                thirdPersonCam.Priority = 9;
                veryFineAimCam.Priority = 9;
                fineAimCam.Priority = 10;
            }
        }*/
            if (Input.GetMouseButton(1))
            {
                //thirdPersonCam.Priority = 10;
                //fineAimCam.Priority = 9;
                thirdPersonCam.m_XAxis.m_InputAxisName = "Mouse X";
            }
            if (!Input.GetMouseButton(1))
            {
                thirdPersonCam.m_XAxis.m_InputAxisName = "";
                thirdPersonCam.m_XAxis.m_InputAxisValue = 0;
            }
        }
    }
    private void changeToFineAim()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            isFineAim = true;

            Vector3 Norm = thirdPersonCam.transform.position - transform.position;
            float sinTheta = Norm.normalized.z;
            float cosTheta = Norm.normalized.x;

            float theta = Mathf.Atan2(sinTheta, cosTheta) * Mathf.Rad2Deg;

            yRotation = (theta + 90) * -1;


            if (!playerMotion.isTakingCover)
            {
                thirdPersonCam.Priority = 9;
                //veryFineAimCam.Priority = 9;
                //fineAimCam.Priority = 10;
            }
            if (gunDetails.scopeCam != null)
            {
                gunDetails.scopeCam.Priority = 9;
                //scopeBlackness.SetActive(false);
            }

            // crossHair.gameObject.SetActive(true);
        }
    }
    private void changeToVeryFineAim()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            if (gunDetails.scopeCam == null)
            {
                isVeryFineAim = true;
                //veryFineAimCam.Priority = 11;
                if (veryFineAimRef == null)
                {
                    aimRef = new GameObject("fine aim ref");
                    veryFineAimRef = Instantiate(aimRef, gun.transform.position, gun.transform.rotation, gun.transform);
                    veryFineAimRef.transform.localEulerAngles = new Vector3(0, 180, 0);
                    veryFineAimRef.transform.SetParent(playerMiddleSpine);
                }


                //veryFineAimCam.m_Follow = veryFineAimRef.transform;
                //veryFineAimCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(gunDetails.xCamOffset, gunDetails.yCamOffset, gunDetails.zCamOffset);
                // crossHair.gameObject.SetActive(false);
            }
            else
            {
                isVeryFineAim = true;
                gunDetails.scopeCam.Priority = 11;
                //crossHair.gameObject.SetActive(false);
                if (gunDetails.gunType.Equals("M107"))
                {
                    // scopeBlackness.SetActive(true);
                }
            }
        }
    }
    private void changeFromVeryFineAim()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            if (gunDetails.scopeCam == null)
            {
                isVeryFineAim = false;
                //veryFineAimCam.Priority = 9;
                //veryFineAimCam.m_Follow = playerMiddleSpine;
                if (Input.GetMouseButton(1))
                {
                    //crossHair.gameObject.SetActive(true);
                }
                //Destroy(veryFineAimRef);
                if (aimRef != null)
                {
                    Destroy(aimRef);
                    aimRef = null;
                }
            }
            else
            {
                isVeryFineAim = false;
                gunDetails.scopeCam.Priority = 9;
                if (Input.GetMouseButton(1))
                {
                    //crossHair.gameObject.SetActive(true);
                }
                if (gunDetails.gunType.Equals("M107"))
                {
                    //scopeBlackness.SetActive(false);
                }
            }
        }

    }
    private void changeToTPP()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            isFineAim = false;
            isVeryFineAim = false;
            //hasNotLooked = true;
            thirdPersonCam.Priority = 10;
            //veryFineAimCam.Priority = 9;
            //fineAimCam.Priority = 9;
            if (gunDetails.scopeCam != null)
            {
                gunDetails.scopeCam.Priority = 9;
                //scopeBlackness.SetActive(false);
            }
            // crossHair.gameObject.SetActive(false);
            //veryFineAimCam.m_Follow = playerMiddleSpine;
            //Destroy(veryFineAimRef);
            if (aimRef != null)
            {
                Destroy(aimRef);
                aimRef = null;
            }
        }
    }
    private void startReloadAnimation()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            isReloading = true;
            setAnimatorWeight(1);
            animator.SetInteger("reload", 2);

        }
    }
    public void reload()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            if (gunDetails.ammoSpare >= ammoMax)
            {
                gunDetails.ammoSpare -= ammoMax;
                gunDetails.ammoLoaded += ammoMax;
            }
            else
            {
                if (gunDetails.ammoSpare > 0)
                {
                    gunDetails.ammoLoaded += gunDetails.ammoSpare;
                    gunDetails.ammoSpare = 0;

                }
            }
            switch (animator.GetInteger("upperbody"))
            {
                case 0:
                    animator.SetInteger("reload", 0);
                    break;
                case 1:
                    animator.SetInteger("reload", 1);
                    break;
                case 2:
                    animator.SetInteger("reload", 3);
                    break;
            }
            isReloading = false;
        }
    }
    
    public void Shoot()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer )
        {
            if (!isReloading && !isSwitchingGun && playerMotion.joked)
            {
                if (gunDetails.ammoLoaded >= 1)
                {
                    if (!gunDetails.gunType.Equals("Bennelli M4") && !gunDetails.gunType.Equals("RPG7"))
                    {
                        
                        
                        if (gunDetails.scopeCam != null && gunDetails.gunType.Equals("M107") && isVeryFineAim)
                        {
                            string[] layers = { "Armature bones" };
                            LayerMask mask = LayerMask.GetMask(layers);
                            if (Physics.Raycast(gunDetails.scopeCam.transform.position, gunDetails.scopeCam.transform.forward, out RaycastHit hit, 1000, mask))
                            {
                                Gunshot.BulletCollision(hit.point, hit.collider, gunDetails.damage, bloodParticleSystem);
                                Debug.Log(hit.collider.name);
                            }
                        }
                        else
                        {
                            

                            if (playerMultiDetails.isMultiPlayer)
                            {

                                CmdrequestShooting(bulletSpawnPos.position, bulletSpawnPos.rotation, playerMotion.cylinder.position, gunDetails.damage,false, netId);
                            }
                            else
                            {
                                GameObject go;
                                go = Instantiate(bullet, bulletSpawnPos.position, bulletSpawnPos.rotation);
                                go.transform.LookAt(playerMotion.cylinder);
                                go.GetComponent<Gunshot>().damage = gunDetails.damage;
                                muzzleFlash.Play();
                                Instantiate(smoke, bulletSpawnPos.position, bulletSpawnPos.rotation);
                                gunDetails.ammoLoaded -= 1;
                                gunDetails.gunSound.Play();
                            }
                            
                        }

                        

                    }
                    else
                    {
                        if (gunDetails.gunType.Equals("Bennelli M4"))
                        {
                           


                            if (playerMultiDetails.isMultiPlayer)
                            {
                                CmdrequestShooting(bulletSpawnPos.position, bulletSpawnPos.rotation, playerMotion.cylinder.position, gunDetails.damage, true,netId);
                            }
                            else
                            {
                               
                                GameObject go;
                                go = Instantiate(shotGunBullet, bulletSpawnPos.position, bulletSpawnPos.rotation);
                                go.transform.LookAt(playerMotion.cylinder);
                                muzzleFlash.Play();
                                Instantiate(smoke, bulletSpawnPos.position, bulletSpawnPos.rotation);
                                gunDetails.ammoLoaded -= 1;
                                gunDetails.gunSound.Play();
                            }
                           

                        }


                    }
                    /*if (Physics.Raycast(gun.transform.position, gun.transform.right * -1, out RaycastHit hit, 30))
                    {
                        if (hit.collider.GetComponent<Target>())
                        {
                            Target t;
                            t = hit.collider.GetComponent<Target>();
                            t.TakeDamage(gunDetails.damage);

                        }

                    }*/

                    timeToShoot = 0;
                    //StartRecoil();
                }
                else
                {

                    if (gunDetails.ammoSpare > 0 && !gunDetails.gunType.Equals("RPG7"))
                    {
                        startReloadAnimation();
                    }
                    if (gunDetails.gunType.Equals("RPG7"))
                    {
                        if (gunDetails.ammoSpare > 0)
                        {

                            gunDetails.GetComponent<RPG7>().Launch();
                            gunDetails.gunSound.PlayOneShot(gunDetails.gunSound.clip);
                            gunDetails.ammoSpare -= 1;
                        }
                    }

                }

            }
        }
       
    }
    [TargetRpc]
    private void TargetReduceAmmo(NetworkConnection conn)
    {
        gunDetails.ammoLoaded -= 1;
        gunDetails.gunSound.Play();
        muzzleFlash.Play();
    }
    [ClientRpc(excludeOwner =true)]
    private void RpcFlashMuzzle(uint Id)
    {
        if(netId == Id && multiGun != null)
        {
            if(muzzleFlash == null)
            {
                muzzleFlash = multiGun.transform.GetChild(0).GetComponent<ParticleSystem>();

            }
            if(multiGunDetails == null)
            {
                multiGunDetails = multiGun.GetComponent<GunDetails>();
            }
            multiGunDetails.gunSound.Play();
            muzzleFlash.Play();

        }
    }
    [Command]
    private void CmdrequestShooting(Vector3 pos,Quaternion rot,Vector3 lookat,float damage,bool shotGun,uint id,NetworkConnectionToClient conn = null)
    {
        //Validate logic 
        GameObject go;
        ParticleSystem smo;
        if (!shotGun)
        {
            go = Instantiate(bullet, pos, rot);
            go.transform.LookAt(lookat);
            go.GetComponent<Gunshot>().damage = damage;
            go.GetComponent<Gunshot>().shooterId = id;
            //Debug.Log(damage);
            NetworkServer.Spawn(go);
            TargetReduceAmmo(conn);
            RpcFlashMuzzle(id);
            smo = Instantiate(smoke, pos, rot);
            NetworkServer.Spawn(smo.gameObject);
        }
        else
        {
            go = Instantiate(shotGunBullet, pos, rot);
            go.transform.LookAt(lookat);
            go.GetComponent<ShotgunCatridge>().shooterId = id;
            NetworkServer.Spawn(go);
            TargetReduceAmmo(conn);
            RpcFlashMuzzle(id);
            smo = Instantiate(smoke, pos, rot);
            NetworkServer.Spawn(smo.gameObject);
        }
        

    }
   
    public void pickupGrenade(GameObject grenadePickup)
    {
       
        if (grenade == null)
        {
            grenadePickup.transform.SetParent(leftHip);
            grenadePickup.GetComponent<Collider>().enabled = false;
            Destroy(grenadePickup.GetComponent<Rigidbody>());
            grenadePickup.transform.localPosition = new Vector3(0.0042f, 0.0032f, 0);
            grenadePickup.transform.localEulerAngles = new Vector3(0, 0, 180);
            grenadePickup.tag = "Grenade";
            grenade = grenadePickup;
            if (!playerMotion.isUsingKeyBoard)
            {
                //screenObjects.throwButton.SetActive(true);
            }
            if (grenadePickup.GetComponent<FresnelHighlight>().isFresnating)
            {
                grenadePickup.GetComponent<FresnelHighlight>().defresnate();
            }
        }
        
       
    }

    [Command]
    private void CmdPickupGrenade(GameObject grenadeDrop,uint net,NetworkConnectionToClient conn = null)
    {
        RpcPickupGrenade(net);
        TargetPickupGrenade(conn);
        NetworkServer.Destroy(grenadeDrop);
    }
    [TargetRpc]
    private void TargetPickupGrenade(NetworkConnection conn)
    {
        GameObject go = Instantiate(gameMechMulti.RDG5);
        pickupGrenade(go);
    }
    [ClientRpc(excludeOwner =true)]
    private void RpcPickupGrenade(uint net)
    {
        if(net == netId)
        {
            multiGrenade = Instantiate(gameMechMulti.RDG5,leftHip);
            multiGrenade.GetComponent<Collider>().enabled = false;
            Destroy(multiGrenade.GetComponent<Rigidbody>());
            multiGrenade.transform.localPosition = new Vector3(0.0042f, 0.0032f, 0);
            multiGrenade.transform.localEulerAngles = new Vector3(0, 0, 180);
            multiGrenade.transform.localScale = new Vector3(0.052f, 0.052f, 0.052f);
            multiGrenade.tag = "Untagged";
        }
    }
    public void throwGrenade()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            if (grenade != null)
            {

                
                if (!playerMotion.isBeingPunched)
                {
                    animator.SetInteger("throw", 2);
                    RpcDestroyReleasedGrenade(netId, true); //true means just moving the grenade to the hand
                    isLoadingThrow = true;
                    grenade.transform.SetParent(leftElbow);
                    grenade.transform.localPosition = new Vector3(-0.0023f, 0.0109f, 0);
                    grenade.transform.localEulerAngles = new Vector3(0, 0, 180);

                }
                if (!playerMotion.isUsingKeyBoard)
                {
                    screenObjects.throwButton.SetActive(false);
                }
            }
        }
    }
    public void stopLoadingThrow()
    {

        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            animator.SetInteger("throw", 2);
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit))
            {
                throwForce = Vector3.Distance(transform.position, hit.point) * throwForceConstant;
            }
            else
            {
                throwForce = 500;
            }
        }
    }
    public void releaseGrenade()
    {
        if (!playerMultiDetails.isMultiPlayer)
        {
            if (grenade != null)
            {
                grenade.transform.SetParent(null);
                Rigidbody rigid = grenade.GetComponent<Rigidbody>();
                if(rigid == null)
                {
                    rigid = grenade.AddComponent<Rigidbody>();
                }
               
                rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                float dist = Vector3.Distance(transform.position, playerMotion.cylinder.position);
                throwForce = dist * throwForceConstant;
                //float fthrowForceConstantUp = (throwForceConstantUp * throwForceConstant / throwForce) >throwForceConstantUp? 
                //    throwForceConstantUp: (throwForceConstantUp * throwForceConstant / throwForce);
                float fthrowForceConstantUp = throwForceConstantUp;
                grenade.transform.LookAt(playerMotion.cylinder);
                //rigid.AddForce((playerMiddleSpine.forward + transform.up) * throwForce);
                //Debug.Log(throwForce);
                rigid.AddForce(grenade.transform.forward * throwForce + Vector3.up * fthrowForceConstantUp);
                grenade.GetComponent<Explosive>().invokeExplosion();
                grenade.GetComponent<Collider>().enabled = true;
            }
        }
        else
        {
            if (isLocalPlayer)
            {

                float dist = Vector3.Distance(transform.position, playerMotion.cylinder.position);
                throwForce = dist * throwForceConstant;
                float fthrowForceConstantUp = throwForceConstantUp;
                grenade.transform.LookAt(playerMotion.cylinder);
                Vector3 finalDirection = grenade.transform.forward * throwForce + Vector3.up * fthrowForceConstantUp;
                Vector3 pos = grenade.transform.position;
                Quaternion rot = grenade.transform.rotation;
                Destroy(grenade);
                CmdReleaseGrenade(pos, rot, finalDirection,netId);
                
                
            }
        }
    }
    [Command]
    private void CmdReleaseGrenade(Vector3 grenadePos,Quaternion grenadeRot,Vector3 throwDir,uint net)
    {
        GameObject go = Instantiate(gameMechMulti.spawnPrefabs.Find(prefab => prefab.name == "RGD-6"), grenadePos, grenadeRot);
        NetworkServer.Spawn(go);
        go.GetComponent<Rigidbody>().AddForce(throwDir);
        go.GetComponent<Explosive>().invokeExplosion();
        RpcDestroyReleasedGrenade(net,false);

    }
    [ClientRpc(excludeOwner =true)]
    private void RpcDestroyReleasedGrenade(uint netid,bool justMovingToHand)
    {
        if(netId == netid)
        {
            if (justMovingToHand)
            {
                multiGrenade.transform.SetParent(leftElbow);
                multiGrenade.transform.localPosition = new Vector3(-0.0023f, 0.0109f, 0);
                multiGrenade.transform.localEulerAngles = new Vector3(0, 0, 180);
            }
            else
            {
                Destroy(multiGrenade);
            }
        }
    }
    public void endThrow()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            isLoadingThrow = false;
            switch (animator.GetInteger("upperbody"))
            {
                case 0:
                    animator.SetInteger("throw", 0);
                    break;
                case 1:
                    animator.SetInteger("throw", 1);
                    break;
                case 2:
                    animator.SetInteger("throw", 3);
                    break;
            }
        }
    }

    public void changeGun(GameObject newGun)
    {
        if(isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            if (newGun.GetComponent<FresnelHighlight>().isFresnating)
            {
                newGun.GetComponent<FresnelHighlight>().defresnate();
            }
            if (gun != null)
            {
                if (newGun.GetComponent<GunDetails>().gunType.Equals(gunDetails.gunType))
                {
                    ammoPreserve = gunDetails.ammoLoaded;
                    newAmmo = newGun.GetComponent<GunDetails>().ammoSpare + gunDetails.ammoSpare;
                    Destroy(gun);
                    Debug.Log("same gun");
                }
                else
                {
                    ammoPreserve = newGun.GetComponent<GunDetails>().ammoLoaded;
                    newAmmo = newGun.GetComponent<GunDetails>().ammoSpare;
                    if (playerMultiDetails.isMultiPlayer)
                    {
                        int gunInt = gunDetails.gunInt;
                        Vector3 pos = gun.transform.position;
                        Quaternion rot = gun.transform.rotation;
                        CmdSpawnGun(gunInt, pos, rot, gunDetails.ammoLoaded, gunDetails.ammoSpare);
                        Destroy(gun);

                    }
                    else
                    {
                        gun.GetComponent<Collider>().enabled = true;
                        gun.tag = "DroppedGun";
                        gun.transform.SetParent(null);

                        if (!gun.GetComponent<Rigidbody>())
                        {
                            gun.AddComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                        }
                        else
                        {
                            gun.GetComponent<Rigidbody>().useGravity = true;
                        }
                    }
                }
                EnemyGun en = newGun.GetComponent<EnemyGun>();
                if (en != null)
                {
                    Destroy(en);
                }
                Rigidbody r = newGun.GetComponent<Rigidbody>();
                if (r != null)
                {
                    Destroy(r);
                }

                ragdollSwitch.gun = newGun.GetComponent<Collider>();
                Collider c = newGun.GetComponent<Collider>();
                c.enabled = false;
                newGun.transform.SetParent(rightElbow);
                newGun.tag = "Playergun";
                if (!playerMultiDetails.isMultiPlayer)
                {
                    setGun(false);
                }
                else
                {
                    gun = newGun;
                    setgun2(false);
                }

            }
            else
            {
                EnemyGun en = newGun.GetComponent<EnemyGun>();
                if (en != null)
                {
                    Destroy(en);
                }
                Rigidbody r = newGun.GetComponent<Rigidbody>();
                if (r != null)
                {
                    Destroy(r);
                }
                ragdollSwitch.gun = newGun.GetComponent<Collider>();
                Collider c = newGun.GetComponent<Collider>();
                c.enabled = false;
                newAmmo = newGun.GetComponent<GunDetails>().ammoSpare;
                ammoPreserve = newGun.GetComponent<GunDetails>().ammoLoaded;
                newGun.transform.SetParent(rightElbow);
                newGun.tag = "Playergun";
                if (!playerMultiDetails.isMultiPlayer)
                {
                    setGun(false);
                }
                else
                {
                    gun = newGun;
                    gun2 = newGun;
                    setgun2(false);
                }




            }
            hasChangedGun = true;
        }
            
        
        
    }
    [Command]
    private void CmdSpawnGun(int gunInt,Vector3 pos,Quaternion rot, int ammoLoaded, int ammoSpare)
    {
        GameObject g = Instantiate(gameMechMulti.networkedGuns[gunInt], pos, rot);
        g.GetComponent<GunDetails>().ammoLoaded = ammoLoaded;
        g.GetComponent<GunDetails>().ammoSpare = ammoSpare;
        NetworkServer.Spawn(g);
    }
    [Command]
    private void CmdchangeGun(GameObject gunObject,int gunInt,int gunID,uint NetID,NetworkConnectionToClient conn = null)
    {
        //Validate input
        RpcChangeGun(gunInt, gunID, NetID);
        GunDetails g = gunObject.GetComponent<GunDetails>();

        TargetChangeGun(conn, gunInt,g.ammoLoaded,g.ammoSpare);
        
        //Debug.Log(gunInt);
        NetworkServer.Destroy(gunObject);
    }
    [TargetRpc]
    private void TargetChangeGun(NetworkConnection conn, int gunInt,int ammoLoaded,int ammoSpare)
    {
        GameObject go = Instantiate(gameMechMulti.guns[gunInt]);
        go.GetComponent<GunDetails>().ammoLoaded = ammoLoaded;
        go.GetComponent<GunDetails>().ammoSpare = ammoSpare;
        changeGun(go);
    }
    [ClientRpc(excludeOwner =true)]
    private void RpcChangeGun(int gunInt, int gunID, uint NetID)
    {
        if(NetID == netId )
        {
            if(multiGun != null)
            {
                Destroy(multiGun);
            }
            multiGun = Instantiate(gameMechMulti.guns[gunInt], rightElbow);
            GunDetails g = multiGun.GetComponent<GunDetails>();
            Rigidbody r = multiGun.GetComponent<Rigidbody>();
            if (r != null) Destroy(r);
            Collider c = multiGun.GetComponent<Collider>();
            if (c != null) Destroy(c);
            multiGun.transform.localPosition = g.localPos;
            multiGun.transform.localEulerAngles = g.localRot;
            multiGun.transform.localScale = g.localScale;
            multiGun.tag = "Untagged";
            muzzleFlash = null;
            multiGunDetails = null;

        }
       
    }
    public void endSwitchGunAnim()
    {
      
        switch (animator.GetInteger("upperbody"))
        {
            case 0:
                animator.SetInteger("switch gun", 0);
                break;
            case 1:
                animator.SetInteger("switch gun", 1);
                break;
            case 2:
                animator.SetInteger("switch gun", 3);
                break;
        }
        isSwitchingGun = false;
        
    }
    public void startSwitchGunAnim()
    {
       
        if (!playerMotion.isBeingPunched)
        {
            isSwitchingGun = true;
            setAnimatorWeight(1);

            animator.SetInteger("switch gun", 2);
        }
       
    }
    [Command]
    public void CmdSwitchGun(uint netID,NetworkConnectionToClient conn = null)
    {
        RpcSwitchGun(netID);
        TargetSwitchGun(conn);
    }
    [TargetRpc]
    private void TargetSwitchGun(NetworkConnection conn) 
    {
        switchingGun();
    }

    [ClientRpc(excludeOwner = true)]
    public void RpcSwitchGun(uint netID)
    {
        if(netID == netId)
        {
            GameObject newPriGun = multiSecondaryGun;
            GameObject newSecGun = multiGun;
            multiGun = newPriGun;
            multiSecondaryGun = newSecGun;

            if(multiGun != null)
            {
                multiGun.transform.SetParent(rightElbow);
                GunDetails g = multiGun.GetComponent<GunDetails>();
                multiGun.transform.localPosition = g.localPos;
                multiGun.transform.localEulerAngles = g.localRot;
                multiGun.transform.localScale = g.localScale;

            }
            if(multiSecondaryGun != null)
            {
                multiSecondaryGun.transform.SetParent(playerMiddleSpine);
                multiSecondaryGun.transform.localPosition = new Vector3(-0.001965688f, 0.004141954f, -0.01004855f);
                multiSecondaryGun.transform.localEulerAngles = new Vector3(-52.211f, -76.771f, 164.239f);
            }
            muzzleFlash = null;
            multiGunDetails = null;
        }
    }

    public void switchGun()
    {
        if (playerMultiDetails.isMultiPlayer)
        {
            if (isLocalPlayer)
            {
                
                CmdSwitchGun(netId);
               
            }
        }
        else
        {
            switchingGun();
        }
    }
    private void switchingGun()
    {

        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            if (gun != null)
            {
                gun.tag = "SecondaryGun";
                GameObject gunPlaceHolder = gun;
                gun.transform.SetParent(playerMiddleSpine);
                gun.transform.localPosition = new Vector3(-0.001965688f, 0.004141954f, -0.01004855f);
                gun.transform.localEulerAngles = new Vector3(-52.211f, -76.771f, 164.239f);
                ragdollSwitch.SecondaryGun = gun.GetComponent<Collider>();
                if (playerMultiDetails.isMultiPlayer)
                {
                    updateSecondaryGun(gun.GetComponent<GunDetails>().gunInt, netId);
                }
                if (secondaryGun != null)
                {
                    secondaryGun.tag = "Playergun";
                    secondaryGun.transform.SetParent(rightElbow);
                    ragdollSwitch.gun = secondaryGun.GetComponent<Collider>();
                    ammoPreserve = secondaryGun.GetComponent<GunDetails>().ammoLoaded;
                    newAmmo = secondaryGun.GetComponent<GunDetails>().ammoSpare;
                    if (playerMultiDetails.isMultiPlayer)
                    {
                        gun = secondaryGun;
                        setgun2(false);
                    }
                    else
                        setGun(false);

                }
                else
                {
                    ragdollSwitch.gun = null;
                    gun = null;
                    if (playerMultiDetails.isMultiPlayer && isLocalPlayer)
                    {
                        updatePrimaryGun(-1, netId);
                    }
                }
                if (!playerMultiDetails.isMultiPlayer)
                {
                    secondaryGun = GameObject.FindGameObjectWithTag("SecondaryGun");
                }
                else
                {
                    secondaryGun = gunPlaceHolder;
                }
                gameMech.screentexts.changeSecondaryGunIcon();

            }
            else
            {

                if (secondaryGun != null)
                {
                    secondaryGun.tag = "Playergun";
                    secondaryGun.transform.SetParent(rightElbow);
                    ragdollSwitch.gun = secondaryGun.GetComponent<Collider>();
                    ammoPreserve = secondaryGun.GetComponent<GunDetails>().ammoLoaded;
                    newAmmo = secondaryGun.GetComponent<GunDetails>().ammoSpare;
                    if (playerMultiDetails.isMultiPlayer)
                    {
                        gun = secondaryGun;
                        setgun2(false);
                    }
                    else
                        setGun(false);
                }
                else
                {
                    ragdollSwitch.gun = null;
                    gun = null;
                    if (playerMultiDetails.isMultiPlayer )
                    {
                        updatePrimaryGun(-1, netId);
                    }
                }
                ragdollSwitch.SecondaryGun = null;
                secondaryGun = null;
                if (playerMultiDetails.isMultiPlayer )
                {
                    updateSecondaryGun(-1, netId);
                }
                gameMech.screentexts.changeSecondaryGunIcon();
            }
        }
    }
    public void dropGun()
    {

        if (!playerMultiDetails.isMultiPlayer)
        {
            if (gun != null)
            {
                gun.tag = "DroppedGun";
                gun.transform.SetParent(null);
                if (!gun.GetComponent<Rigidbody>())
                {
                    gun.gameObject.AddComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                }
                else
                {
                    gun.GetComponent<Rigidbody>().useGravity = true;
                }
                gun.GetComponent<Collider>().enabled = true;
                gun = null;

            }
        } 
        else
        {
            if(gun!= null)
            {
                int gunInt = gunDetails.gunInt;
                int ammoLoaded = gunDetails.ammoLoaded;
                int ammoSpare = gunDetails.ammoSpare;
                Vector3 pos = gun.transform.position;
                Quaternion rot = gun.transform.rotation;
                if( isLocalPlayer) updatePrimaryGun(-1, netId);
                CmdDropGun(gunInt, pos, rot, netId,ammoLoaded,ammoSpare);
            }
        }
        
    }
    public void dropSecondaryGun()
    {

        if(secondaryGun != null)
        {
            GunDetails s = secondaryGun.GetComponent<GunDetails>();
            int gunInt = s.gunInt;
            int ammoLoaded = s.ammoLoaded;
            int ammoSpare = s.ammoSpare;
            Vector3 pos = secondaryGun.transform.position;
            Quaternion rot = secondaryGun.transform.rotation;
            if(isLocalPlayer) updateSecondaryGun(-1, netId);
            CmdSecDropGun(gunInt, pos, rot, netId, ammoLoaded, ammoSpare);
        }

    }

    [Command]
    private void CmdDropGun(int gunInt,Vector3 pos,Quaternion rot,uint netID,int ammoLoaded,int ammoSpare)
    {
        GameObject g = Instantiate(gameMechMulti.networkedGuns[gunInt], pos, rot);
        g.GetComponent<GunDetails>().ammoLoaded = ammoLoaded;
        g.GetComponent<GunDetails>().ammoSpare = ammoSpare;
        NetworkServer.Spawn(g);
        RpcDropGun(netID);
    }
    [ClientRpc]
    private void RpcDropGun(uint netID)
    {
        if(netId == netID)
        {
            if (hasAuthority)
            {
                Destroy(gun);
            }
            else
            {
                Destroy(multiGun);
            }
        }
    }
    [Command]
    private void CmdSecDropGun(int gunInt, Vector3 pos, Quaternion rot, uint netID, int ammoLoaded, int ammoSpare)
    {
        GameObject g = Instantiate(gameMechMulti.networkedGuns[gunInt], pos, rot);
        g.GetComponent<GunDetails>().ammoLoaded = ammoLoaded;
        g.GetComponent<GunDetails>().ammoSpare = ammoSpare;
        NetworkServer.Spawn(g);
        RpcSecDropGun(netID);
    }
    [ClientRpc]
    private void RpcSecDropGun(uint netID)
    {
        if (netId == netID)
        {
            if (hasAuthority)
            {
                if(secondaryGun!= null)
                {
                    Destroy(secondaryGun);
                }
            }
            else
            {
               if(multiSecondaryGun != null)
                {
                    Destroy(multiSecondaryGun);
                }
            }
        }
    }





    private GameObject gunDroppedNearby()
    {
        
            GameObject[] goWithTag = GameObject.FindGameObjectsWithTag("DroppedGun");

            for (int i = 0; i < goWithTag.Length; ++i)
            {
                if (Vector3.Distance(transform.position, goWithTag[i].transform.position) <= 2)
                {
                    return goWithTag[i];
                }
                else
                {
                    if (goWithTag[i].GetComponent<FresnelHighlight>().isFresnating)
                    {
                        goWithTag[i].GetComponent<FresnelHighlight>().defresnate();
                    }
                }

            }
            return null;
        
    }
    private GameObject grenadeDroppedNearby()
    {
        GameObject[] goWithTag = GameObject.FindGameObjectsWithTag("DroppedGrenade");

        for (int i = 0; i < goWithTag.Length; ++i)
        {
            if (Vector3.Distance(transform.position, goWithTag[i].transform.position) <= 2)
            {
                return goWithTag[i];
            }
            else
            {
                if (goWithTag[i].GetComponent<FresnelHighlight>().isFresnating)
                {
                    goWithTag[i].GetComponent<FresnelHighlight>().defresnate();
                }
            }
                
        }
        return null;
    }

    private void updateGunPos()
    {
       
        if (!setGunPos)
        {
            gun.transform.localPosition = gunDetails.localPos;
            gun.transform.localEulerAngles = gunDetails.localRot;
            gun.transform.localScale = gunDetails.localScale;
            setGunPos = true;

        }
      
    }
    private void fineAim()
    {

        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            yRotation += mouseX;

            playerMiddleSpine.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.localRotation = Quaternion.Euler(0, yRotation, 0);




            /*if(Physics.Raycast(gun.transform.position, gun.transform.right * -1,out RaycastHit hit))
            {

            }*/
        }

    }
    private void StartRecoil()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            isRecoil = true;
            recoilTime = 0;
        }
    }
    //For fixedUpdate
    private void RecoilSciences(float recoilTimeFin)
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            if (isRecoil)
            {
                recoilTime += 1 * Time.deltaTime;
                if (recoilTime < recoilTimeFin)
                {
                    recoilEasing = (recoilTimeFin - recoilTime) / recoilTimeFin;
                    if (recoilRef > -62)
                    {
                        recoilUp = true;
                    }
                    else
                    {
                        if (recoilRef < xRotation)
                        {
                            recoilUp = false;
                        }
                        else
                        {
                            isRecoil = false;
                            recoilTime = 0;
                        }
                    }

                }
                else
                {
                    if (recoilRef < defaultRightArmRot.x)
                    {
                        recoilUp = false;
                    }
                    else
                    {
                        isRecoil = false;
                        recoilTime = 0;
                    }
                }
            }
        }
    }
    private void RecoilUp(float recoilUpSpeed)
    {
        recoilRef += recoilUpSpeed * Time.deltaTime;
        playerMiddleSpine.localEulerAngles = new Vector3(recoilRef + xRotation, 0, 0);
    }
    private void RecoilDown(float recoilDownSpeed)
    {
        recoilRef += recoilDownSpeed * Time.deltaTime;
        playerMiddleSpine.localEulerAngles = new Vector3(recoilRef + xRotation, 0, 0);
    }
    private void LateUpdate()
    {
        if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
        {
            if (isFineAim)
            {
                fineAim();
            }
            else
            {
                xRotation = 0;

            }
            if (isRecoil)
            {
                if (recoilUp)
                {
                    RecoilUp(gunDetails.recoilUpSpeed);
                }
                else
                {
                    RecoilDown(gunDetails.recoilDownSpeed);
                }
            }
            else
            {

                recoilRef = 0;

            }
        }
        
    }
}
