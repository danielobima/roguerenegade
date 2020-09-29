using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;

public class PlayerGunMulti : NetworkBehaviour
{
    private ParticleSystem muzzleFlash;
    public ParticleSystem smoke;
    public GameObject bullet;
    public GameObject shotGunBullet;

    public GameObject gun;
    public GameObject secondaryGun;
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
    public GameObject bloodParticleSystem;

    private Target t;
    private RagdollSwitch ragdollSwitch;
    private PlayerMotion playerMotion;
    private GameMech gameMech;

    private GameObject pickUpGunButton;
    private GameObject switchGunButton;
    private GameObject dropGunButton;
    private GameObject throwButton;
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


    //To make gun drops accessible by the pick up button
    [Header("LEAVE THIS EMPTY PLEASE")]
    public GameObject gunDrop;
    public GameObject grenadeDrop;

    private void Start()
    {
        t = GetComponent<Target>();
        pickUpGunButton = GameObject.FindGameObjectWithTag("pickUpGun");
        switchGunButton = GameObject.FindGameObjectWithTag("switchGun");
        dropGunButton = GameObject.FindGameObjectWithTag("dropGun");
        throwButton = GameObject.FindGameObjectWithTag("throw");
        playerMiddleSpine = GameObject.FindGameObjectWithTag("PlayerMiddleSpine").transform;
        thirdPersonCam = GameObject.FindGameObjectWithTag("thirdPersonCam").GetComponent<CinemachineFreeLook>();
        thirdPersonCam.m_Follow = transform;
        thirdPersonCam.m_LookAt = transform;
        /*crossHair = GameObject.FindGameObjectWithTag("crossHair").GetComponent<Image>();
        scopeBlackness = GameObject.FindGameObjectWithTag("scopeBlackness");
        crossHair.gameObject.SetActive(false);
        scopeBlackness.GetComponent<SVGImage>().color = new Color(1, 1, 1, 1);
        scopeBlackness.SetActive(false);*/

        ragdollSwitch = GetComponent<RagdollSwitch>();
        animator = GetComponent<Animator>();
        playerMotion = GetComponent<PlayerMotion>();
        setGun(true, 36);
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
    private void setGun(bool overwriteAmmo, int ammoToPut = 0)
    {
        if (overwriteAmmo)
        {
            gun = GameObject.FindGameObjectWithTag("Playergun");
            if (gun != null)
            {
                gunDetails = gun.GetComponent<GunDetails>();
                muzzleFlash = gun.transform.GetChild(0).GetComponent<ParticleSystem>();
                bulletSpawnPos = gun.transform.GetChild(0).transform;
                ammoMax = gunDetails.ammoMax;
                //ammo = ammoMax;
                gunDetails.ammoSpare = ammoToPut;
                timeToShoot = gunDetails.timeToNextShot;
                hasHandgun = gunDetails.handgun;
                gunRigidBody = gun.GetComponent<Rigidbody>();
                setGunPos = false;
                updateGunPos();

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
        else
        {
            gun = GameObject.FindGameObjectWithTag("Playergun");
            if (gun != null)
            {
                gunDetails = gun.GetComponent<GunDetails>();
                muzzleFlash = gun.transform.GetChild(0).GetComponent<ParticleSystem>();
                bulletSpawnPos = gun.transform.GetChild(0).transform;
                ammoMax = gunDetails.ammoMax;
                gunDetails.ammoLoaded = ammoPreserve;
                timeToShoot = gunDetails.timeToNextShot;
                hasHandgun = gunDetails.handgun;
                gunRigidBody = gun.GetComponent<Rigidbody>();
                setGunPos = false;
                updateGunPos();

                gunDetails.ammoSpare = newAmmo;
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
                pickUpGunButton.SetActive(true);
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
                    pickUpGunButton.SetActive(true);
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
            pickUpGunButton.SetActive(false);
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
    public void punch()
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

    public void setAnimatorWeight(float weight)
    {
        //animator.SetLayerWeight(1, weight);

    }
    private bool isStoppingFightingMode = false;
    public void fightingMode(bool on)
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
    IEnumerator stopFightingMode()
    {
        for (weight = 1; weight > 0; weight -= 0.3f)
        {
            animator.SetLayerWeight(2, weight);
            yield return new WaitForSeconds(.1f);
        }
    }
    public void endPunch()
    {
        animator.SetInteger("punch", 0);
        isPunching = false;
    }
    public void punchWhenTold()
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
    private void Update()
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
                if (Input.GetMouseButtonDown(0))
                {
                    punchWhenTold();
                }


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
                if (gunDetails.ammoSpare > 0)
                {
                    startReloadAnimation();
                }

            }
            if (Input.GetKeyDown("x"))
            {
                dropGun();
            }
            if (Input.GetKeyDown("c") && !isLoadingThrow)
            {
                throwGrenade();
                changeToFineAim();
                if (meleeMode)
                {
                    fightingMode(false);
                    meleeMode = false;
                    animator.SetInteger("punch", 0);
                }


            }
            if (Input.GetKeyUp("c") && isLoadingThrow)
            {
                stopLoadingThrow();
                changeToTPP();

            }
            if (Input.GetKeyDown("e"))
            {
                if (gunDrop != null)
                {
                    changeGun(gunDrop);
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
                        pickupGrenade(grenadeDrop);
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

    private void CameraTechnologies()
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
    private void changeToFineAim()
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
    private void changeToVeryFineAim()
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
    private void changeFromVeryFineAim()
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
    private void changeToTPP()
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
    private void startReloadAnimation()
    {
        isReloading = true;
        setAnimatorWeight(1);
        animator.SetInteger("reload", 2);

    }
    public void reload()
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
    public void Shoot()
    {
        if (!isReloading && !isSwitchingGun && playerMotion.joked)
        {
            if (gunDetails.ammoLoaded >= 1)
            {
                if (!gunDetails.gunType.Equals("Bennelli M4") && !gunDetails.gunType.Equals("RPG7"))
                {
                    gunDetails.gunSound.Play();
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
                        GameObject go;


                        go = Instantiate(bullet, bulletSpawnPos.position, bulletSpawnPos.rotation);
                        go.transform.LookAt(playerMotion.cylinder);

                        go.GetComponent<Gunshot>().damage = gunDetails.damage;
                    }

                    muzzleFlash.Play();
                    Instantiate(smoke, bulletSpawnPos.position, bulletSpawnPos.rotation);
                    gunDetails.ammoLoaded -= 1;

                }
                else
                {
                    if (gunDetails.gunType.Equals("Bennelli M4"))
                    {
                        gunDetails.gunSound.Play();
                        GameObject go;
                        go = Instantiate(shotGunBullet, bulletSpawnPos.position, bulletSpawnPos.rotation);

                        muzzleFlash.Play();
                        Instantiate(smoke, bulletSpawnPos.position, bulletSpawnPos.rotation);
                        gunDetails.ammoLoaded -= 1;
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
            grenade = GameObject.FindGameObjectWithTag("Grenade");
            if (!playerMotion.isUsingKeyBoard)
            {
                throwButton.SetActive(true);
            }
            if (grenadePickup.GetComponent<FresnelHighlight>().isFresnating)
            {
                grenadePickup.GetComponent<FresnelHighlight>().defresnate();
            }
        }
    }
    public void throwGrenade()
    {
        if (grenade != null)
        {
            if (!playerMotion.isBeingPunched)
            {
                animator.SetInteger("throw", 3);
                isLoadingThrow = true;
                grenade.transform.SetParent(leftElbow);
                grenade.transform.localPosition = new Vector3(-0.0023f, 0.0109f, 0);
                grenade.transform.localEulerAngles = new Vector3(0, 0, 180);

            }
            if (!playerMotion.isUsingKeyBoard)
            {
                throwButton.SetActive(false);
            }
        }
    }
    public void stopLoadingThrow()
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
    public void releaseGrenade()
    {
        if (grenade != null)
        {
            grenade.transform.SetParent(null);
            Rigidbody rigid = grenade.AddComponent<Rigidbody>();
            rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rigid.AddForce((playerMiddleSpine.forward + transform.up) * throwForce);
            grenade.GetComponent<Explosive>().invokeExplosion();
            grenade.GetComponent<Collider>().enabled = true;
        }
    }
    public void endThrow()
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
    public void changeGun(GameObject newGun)
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
            }
            else
            {
                ammoPreserve = newGun.GetComponent<GunDetails>().ammoLoaded;
                newAmmo = newGun.GetComponent<GunDetails>().ammoSpare;
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
            newGun.tag = "Playergun";

            newGun.transform.SetParent(rightElbow);
            setGun(false);
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
            newGun.tag = "Playergun";
            newAmmo = newGun.GetComponent<GunDetails>().ammoSpare;
            ammoPreserve = newGun.GetComponent<GunDetails>().ammoLoaded;

            newGun.transform.SetParent(rightElbow);
            setGun(false);
        }
        hasChangedGun = true;
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
    public void switchGun()
    {
        if (gun != null)
        {
            gun.tag = "SecondaryGun";
            gun.transform.SetParent(playerMiddleSpine);
            gun.transform.localPosition = new Vector3(-0.001965688f, 0.004141954f, -0.01004855f);
            gun.transform.localEulerAngles = new Vector3(-52.211f, -76.771f, 164.239f);
            ragdollSwitch.SecondaryGun = gun.GetComponent<Collider>();

            if (secondaryGun != null)
            {
                secondaryGun.tag = "Playergun";
                secondaryGun.transform.SetParent(rightElbow);
                ragdollSwitch.gun = secondaryGun.GetComponent<Collider>();
                ammoPreserve = secondaryGun.GetComponent<GunDetails>().ammoLoaded;
                newAmmo = secondaryGun.GetComponent<GunDetails>().ammoSpare;
                setGun(false);

            }
            else
            {
                ragdollSwitch.gun = null;
                gun = null;
            }
            secondaryGun = GameObject.FindGameObjectWithTag("SecondaryGun");
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
                setGun(false);
            }
            else
            {
                ragdollSwitch.gun = null;
                gun = null;
            }
            ragdollSwitch.SecondaryGun = null;
            secondaryGun = null;
            gameMech.screentexts.removeSecondaryGunIcon();
        }


    }
    public void dropGun()
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
    private void StartRecoil()
    {
        isRecoil = true;
        recoilTime = 0;
    }
    //For fixedUpdate
    private void RecoilSciences(float recoilTimeFin)
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
