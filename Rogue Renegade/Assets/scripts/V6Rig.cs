﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V6Rig : MonoBehaviour
{
    private Animator rig;
    private WeaponController weaponController;

    private void Start()
    {
        rig = GetComponent<Animator>();
        weaponController = GetComponentInParent<WeaponController>();
    }
   
    public void HolsteringRifle()
    {
        weaponController.HolsteringRifle();
    }
    public void EndHolsterRifle()
    {
        weaponController.EndHolsterRifle();
    }


    
    public void HolsteringHandGun()
    {
        weaponController.HolsteringHandGun();
    }
    public void EndHolsterHandgun()
    {
        weaponController.EndHolsterHandgun();
    }

    
    public void UnHolsteringRifle()
    {
        weaponController.UnHolsteringRifle();

    }
    public void EndUnholsterRifle()
    {
        weaponController.EndUnholsterRifle();
    }

    
    public void UnHolsteringHandgun()
    {
        weaponController.UnHolsteringHandgun();

    }
    public void EndUnholsterHandgun()
    {
        weaponController.EndUnholsterHandgun();
    }

}
