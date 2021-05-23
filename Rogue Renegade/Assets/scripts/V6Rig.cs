using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V6Rig : MonoBehaviour
{
    [Header("This script is used on the rig so that the rig animator can use the weapon controller functions.")]
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
    public void Reload()
    {
        weaponController.Reload();
    }
    public void SpawnMag()
    {
        weaponController.SpawnMag();
    }
    public void DropMag()
    {
        weaponController.DropMag();
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
