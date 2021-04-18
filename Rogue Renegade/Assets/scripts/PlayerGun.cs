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

    public GameObject gun;
    public WeaponPickup possibleGunPickUp;
    public GameObject secondaryGun;


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
            handIKRig.weight = 1f;
            anim.SetLayerWeight(1, 1);
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
            
        }
        else
        {
            handIKRig.weight = 0f;
            anim.SetLayerWeight(1, 0);
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
        Invoke(nameof(setAnimDelay), 0.001f);
    }
    private void setAnimDelay()
    {
        overrider["noGun"] = gunDetails.handPose;
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
