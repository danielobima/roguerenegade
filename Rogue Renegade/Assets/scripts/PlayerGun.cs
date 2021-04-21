using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using DigitalRuby.Tween;
using UnityEngine.Animations.Rigging;
using UnityEditor.Animations;

using Mirror;


public class PlayerGun : NetworkBehaviour {

    /// <summary>
    /// What your currently holding
    /// </summary>
    public GameObject gun;
    public WeaponPickup possibleGunPickUp;
    /// <summary>
    /// The gun on your back
    /// </summary>
    public GameObject holsteredRifle;
    /// <summary>
    /// The gun on your waist
    /// </summary>
    public GameObject holsteredHandGun;


    private GunDetails gunDetails;

    [Header("Leave empty")]
    public Transform bulletSpawnPos;
    public GameObject grenade;
    public bool hasHandgun = false;

    public Transform rightHandGrip;
    public Transform leftHandGrip;
    public Transform playerMiddleSpine;
    public TwoBoneIKConstraint rightHandIK;
    public TwoBoneIKConstraint leftHandIK;
    public Rig handIKRig;
    public Transform aimPos;
    public WeaponPivot weaponPivot;
    public Transform RifleSlot;
    public Transform HandgunSlot;
    public MultiAimConstraint spine1;
    private bool isHolsteringRifle = true;
    private bool isHolsteringHandgun = true;
    private bool switchingGun = false;

    public bool attacking = false;
    public float addSpeed = 0.5f;
    public bool isReloading = false;
    public GameObject bloodParticleSystem;
    public string ammoText;
    
    public static bool isFineAim;
    public GameObject aimRef;
    
    public bool isRecoil = false;

    private Animator anim;
    private AnimatorOverrideController overrider;



   


    //To make gun drops accessible by the pick up button
    [Header("LEAVE THIS EMPTY PLEASE")]
    public GameObject gunDrop;
    public GameObject grenadeDrop;

    

    private void Start()
    {

        anim = GetComponent<Animator>();
        overrider = anim.runtimeAnimatorController as AnimatorOverrideController;

        Invoke(nameof(initAnim), 0.001f);


        gunDetails = GetComponentInChildren<GunDetails>();
        if (gunDetails)
        {
            EquipWeapon(gunDetails);
        }

    }

    private void Update()
    {
        aimPos.position = Camera.main.transform.GetChild(0).position;

        if (gun)
        {
            //handIKRig.weight = 1f;
            //anim.SetLayerWeight(1, 1);
            spine1.data.offset = new Vector3(0, 80, 0);
            if (Input.GetButtonDown("Fire1"))
            {
                gunDetails.startShooting();
            }

            if (Input.GetButtonUp("Fire1"))
            {
                gunDetails.stopShooting();
            }
            gunDetails.UpdateBullets(Time.deltaTime);
            if (gunDetails.isShooting)
            {
                gunDetails.UpdateFiring(Time.deltaTime);
            }
            if (!hasHandgun)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    holsterRifle();
                    switchingGun = false;
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    holsterRifle();
                    if (holsteredHandGun)
                    {
                        switchingGun = true;
                    }
                    else
                    {
                        switchingGun = false;
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    holsterHandGun();
                    if (holsteredRifle)
                    {
                        switchingGun = true;
                    }
                    else
                    {
                        switchingGun = false;
                    }
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    holsterHandGun();
                    switchingGun = false;
                }
            }
            
        }
        else
        {
            //handIKRig.weight = 0f;
            //anim.SetLayerWeight(1, 0);
            spine1.data.offset = new Vector3(0, 0, 0);
            if (holsteredRifle)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    unHolsterRifle();
                }
            }
            if (holsteredHandGun)
            {
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    unHolsterHandgun();
                }
            }
        }
        if (gunDrop)
        {
            if (Input.GetKeyDown("e"))
            {
                EquipWeapon(Instantiate(gunDrop).GetComponent<GunDetails>());
                Destroy(possibleGunPickUp.gameObject);
            }
        }

    }
    public void holsterRifle()
    {
        anim.SetInteger("holster gun", 1);
        isHolsteringRifle = true;
        holsteredRifle = gun;
        gun = null;
        
    }
    public void holsterHandGun()
    {
        anim.SetInteger("holster gun", 1);
        isHolsteringHandgun = true;
        holsteredHandGun = gun;
        gun = null;

    }
    public void endHolster1()
    {
        if (!isHolsteringRifle)
        {
            anim.SetInteger("holster gun", 0);
        }
    }
    public void endHolster2()
    {
        if (isHolsteringRifle)
        {
            if (!switchingGun)
            {
                anim.SetInteger("holster gun", 0);
            }
            else
            {
                unHolsterHandgun();
            }
        }
    }
    public void endHolster3()
    {
        if (!isHolsteringHandgun)
        {
            anim.SetInteger("holster gun", 0);
        }
    }
    public void endHolster4()
    {
        if (isHolsteringHandgun)
        {
            if (!switchingGun)
            {
                anim.SetInteger("holster gun", 0);
            }
            else
            {
                unHolsterRifle();
            }
        }
    }
    public void holsteringRifle()
    {
        if (isHolsteringRifle)
        {
            holsteredRifle.transform.SetParent(RifleSlot);
            holsteredRifle.transform.localPosition = new Vector3(0.1523762f, 0.1048426f, 0.01466675f);
            initAnim();
        }
    }
    public void holsteringHandGun()
    {
        if (isHolsteringHandgun)
        {
            holsteredHandGun.transform.SetParent(HandgunSlot);
            holsteredHandGun.transform.localPosition = new Vector3(0.19f, 0.701f, -0.044f);
            initAnim();
        }
    }
    public void unHolsterRifle()
    {
        hasHandgun = false;
        anim.SetBool("handgun", hasHandgun);
        anim.SetInteger("holster gun", 2);
        isHolsteringRifle = false;
    }
    public void unHolsteringRifle()
    {
        if (!isHolsteringRifle)
        {
            EquipWeapon(holsteredRifle.GetComponent<GunDetails>());
            holsteredRifle = null;
            
        }
       
    }
    public void unHolsterHandgun()
    {
        hasHandgun = true;
        anim.SetBool("handgun", hasHandgun);
        anim.SetInteger("holster gun", 2);
        isHolsteringHandgun = false;
    }
    public void unHolsteringHandgun()
    {
        if (!isHolsteringHandgun)
        {
            EquipWeapon(holsteredHandGun.GetComponent<GunDetails>());
            holsteredHandGun = null;

        }

    }


    private void FixedUpdate()
    {
        
    }
    public void EquipWeapon(GunDetails newGun)
    {
        if (gun)
        {
            Vector3 pos = gun.transform.position;
            pos.z += 1;
            Instantiate(gunDetails.gunDrop,pos,gun.transform.rotation);
            Destroy(gun);
        }
        gun = newGun.gameObject;
        gunDrop = null;
        gunDetails = newGun;
        gun.transform.SetParent(weaponPivot.transform);
        gun.transform.localPosition = gunDetails.localPos;
        gun.transform.localEulerAngles = gunDetails.localRot;
        gunDetails.impacts = weaponPivot.impacts;
        hasHandgun = gunDetails.handgun;
        anim.SetBool("handgun", hasHandgun);
        Invoke(nameof(setAnim), 0.001f);
    }
    private void setAnim()
    {
        overrider["New Animation"] = gunDetails.handPose;
    }
    private void initAnim()
    {
        overrider["New Animation"] = null;
    }


    [ContextMenu("Save weapon pose")]
    public void SaveWeaponPose()
    {
        GameObjectRecorder recorder = new GameObjectRecorder(gameObject);
        recorder.BindComponentsOfType<Transform>(weaponPivot.gameObject, false);
        recorder.BindComponentsOfType<Transform>(leftHandGrip.gameObject, false);
        recorder.BindComponentsOfType<Transform>(rightHandGrip.gameObject, false);
        recorder.TakeSnapshot(0);
        recorder.SaveToClip(gunDetails.handPose);
        UnityEditor.AssetDatabase.SaveAssets();
    }


   
    
}
