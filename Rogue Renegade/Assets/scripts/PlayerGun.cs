using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using DigitalRuby.Tween;
using UnityEngine.Animations.Rigging;
using UnityEditor.Animations;



public class PlayerGun : WeaponController {

    [HideInInspector]
    public WeaponPickup possibleGunPickUp;
    
    

   public override void UpdateFunc()
    {
        if (gun)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                gunDetails.startShooting();
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
   
    
}
