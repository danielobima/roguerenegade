using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalRuby.Tween;
using UnityEngine.Animations.Rigging;
using UnityEditor.Animations;



public class PlayerGun : WeaponController {

    [HideInInspector]
    public WeaponPickup possibleGunPickUp;
    
    private float turnSmoothing = 0.01f;
    private Vector3 turnSmoothRef;
    [HideInInspector]
    public string ammoText = "0/0";

    //Use this instead of Start()
    public override void StartFunc()
    {
        base.StartFunc();
    }

    //Use this instead of Update()
    public override void UpdateFunc()
    {
        
        if (gun)
        {
            ammoText = gunDetails.ammoLoaded + "/" + gunDetails.ammoSpare;
            if (Input.GetButtonDown("Fire1"))
            {
                if (gunDetails.isContinuousShooting)
                {
                    gunDetails.startShooting();
                }
                else
                {
                    gunDetails.singleShot();
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                gunDetails.stopShooting();
            }

            if (!hasHandgun)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    
                    switchingGun = false;
                    HolsterRifle();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                   
                    if (holsteredHandGun)
                    {
                        switchingGun = true;
                    }
                    else
                    {
                        switchingGun = false;
                    }
                    HolsterRifle();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    
                    if (holsteredRifle)
                    {
                        switchingGun = true;
                    }
                    else
                    {
                        switchingGun = false;
                    }
                    HolsterHandGun();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                   
                    switchingGun = false;
                    HolsterHandGun();
                }
            }

        }
        else
        {
            if (holsteredRifle)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    UnHolsterRifle();
                }
            }
            if (holsteredHandGun)
            {
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    UnHolsterHandgun();
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

    private void FixedUpdate()
    {
        aimPos.position = Vector3.SmoothDamp(aimPos.position, Camera.main.transform.GetChild(0).position, ref turnSmoothRef, turnSmoothing);
        
    }
}
