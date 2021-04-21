using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotWeapon : MonoBehaviour
{
    [HideInInspector]
    public GunDetails gunDetails;
    public GameObject gun;
    private Animator anim;
    private AnimatorOverrideController overrider;
    public WeaponPivot weaponPivot;
    private BotRig botRig;


    private void Start()
    {
        anim = GetComponent<Animator>();
        botRig = GetComponent<BotRig>();

        Invoke(nameof(initAnim), 0.001f);


        gunDetails = GetComponentInChildren<GunDetails>();
        if (gunDetails)
        {
            EquipWeapon(gunDetails);
        }
    }

    public void EquipWeapon(GunDetails newGun)
    {
        gun = newGun.gameObject;
        gunDetails = newGun;
        gun.transform.SetParent(weaponPivot.transform);
        gun.transform.localPosition = gunDetails.localPos;
        gun.transform.localEulerAngles = gunDetails.localRot;
        gunDetails.impacts = weaponPivot.impacts;
        Invoke(nameof(setAnim), 0.001f);
        botRig.SetAim(false);
    }
    private void setAnim()
    {
        anim.Play(gunDetails.gunType, 1);
    }
    private void initAnim()
    {
        anim.Play("New Animation", 1);
    }
}
