using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEditor.Animations;
using Cinemachine;

public class WeaponController : MonoBehaviour
{
    /// <summary>
    /// What your currently holding
    /// </summary>
    [HideInInspector]
    public GameObject gun;
    /// <summary>
    /// The gun on your back
    /// </summary>
    [HideInInspector]
    public GameObject holsteredRifle;
    /// <summary>
    /// The gun on your waist
    /// </summary>
    [HideInInspector]
    public GameObject holsteredHandGun;

    [HideInInspector]
    public GunDetails gunDetails;

    [HideInInspector]
    public Transform bulletSpawnPos;
    [HideInInspector]
    public GameObject grenade;
    [HideInInspector]
    public bool hasHandgun = false;
    
    [HideInInspector]
    public CinemachineFreeLook tpp;

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
    private bool unHolstering = false;

    [HideInInspector]
    public bool switchingGun = false;

    [HideInInspector]
    public bool attacking = false;
    [HideInInspector]
    public float addSpeed = 0.5f;
    [HideInInspector]
    public bool isReloading = false;

    public GameObject bloodParticleSystem;
    [HideInInspector]
    public string ammoText;

    

    public Animator rig;


    //To make gun drops accessible by the pick up button
    [HideInInspector]
    public GameObject gunDrop;
    [HideInInspector]
    public GameObject grenadeDrop;



    private void Start()
    {
        StartFunc();
    }
    public virtual void StartFunc()
    {
        Invoke(nameof(initAnim), 0.001f);


        gunDetails = GetComponentInChildren<GunDetails>();
        if (gunDetails)
        {
            EquipWeapon(gunDetails);
        }
    }

    private void Update()
    {

        

        if (gun)
        {
            //handIKRig.weight = 1f;
            //anim.SetLayerWeight(1, 1);
            spine1.data.offset = new Vector3(0, 80, 0);

            gunDetails.UpdateBullets(Time.deltaTime);
            if (gunDetails.isShooting || !gunDetails.isContinuousShooting)
            {
                gunDetails.UpdateFiring(Time.deltaTime);
            }


        }
        else
        {
            //handIKRig.weight = 0f;
            //anim.SetLayerWeight(1, 0);
            spine1.data.offset = new Vector3(0, 0, 0);

        }
        UpdateFunc();
    }
    public virtual void UpdateFunc()
    {

    }
    
    public void HolsterRifle()
    {
        rig.Play("holsterGun", 0);
        isHolsteringRifle = true;
        holsteredRifle = gun;
        gun = null;

    }
    public void HolsteringRifle()
    {
        if (isHolsteringRifle)
        {
            holsteredRifle.transform.SetParent(RifleSlot);
            //holsteredRifle.transform.localPosition = new Vector3(0.1523762f, 0.1048426f, 0.01466675f);
        }
    }
    public void EndHolsterRifle()
    {
        if (isHolsteringRifle)
        {
            if (!switchingGun)
            {
                initAnim();
                Debug.Log("YEET");
            }
            else
            {
                UnHolsterHandgun();
                Debug.Log("NICE");
            }
        }
    }


    public void HolsterHandGun()
    {
        rig.Play("holsterHandgun", 0);
        isHolsteringHandgun = true;
        holsteredHandGun = gun;
        gun = null;

    }
    public void HolsteringHandGun()
    {
        if (isHolsteringHandgun)
        {
            holsteredHandGun.transform.SetParent(HandgunSlot);
            //holsteredHandGun.transform.localPosition = new Vector3(0.19f, 0.701f, -0.044f);
        }
    }
    public void EndHolsterHandgun()
    {
        if (isHolsteringHandgun)
        {
            if (!switchingGun)
            {
                initAnim();
            }
            else
            {
                UnHolsterRifle();
            }
        }
    }

    public void UnHolsterRifle()
    {
        hasHandgun = false;
        rig.Play("unholster gun", 0);
        isHolsteringRifle = false;
        unHolstering = true;
    }
    public void UnHolsteringRifle()
    {
        if (!isHolsteringRifle)
        {
            EquipWeapon(holsteredRifle.GetComponent<GunDetails>());
            holsteredRifle = null;

        }

    }
    public void EndUnholsterRifle()
    {
        if (!isHolsteringRifle)
        {
            unHolstering = false;
            setAnim();
        }
    }

    public void UnHolsterHandgun()
    {
        hasHandgun = true;
        rig.Play("unholsterHandgun ", 0);
        isHolsteringHandgun = false;
        unHolstering = true;
    }
    public void UnHolsteringHandgun()
    {
        if (!isHolsteringHandgun)
        {
            EquipWeapon(holsteredHandGun.GetComponent<GunDetails>());
            holsteredHandGun = null;

        }

    }
    public void EndUnholsterHandgun()
    {
        if (!isHolsteringHandgun)
        {
            unHolstering = false;
            setAnim();
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
            Instantiate(gunDetails.gunDrop, pos, gun.transform.rotation);
            Destroy(gun);
        }
        gun = newGun.gameObject;
        gunDrop = null;
        gunDetails = newGun;
        gun.transform.SetParent(weaponPivot.transform);
        gun.transform.localPosition = gunDetails.localPos;
        gun.transform.localEulerAngles = gunDetails.localRot;
        gunDetails.impacts = weaponPivot.impacts;
        if (tpp)
        {
            gunDetails.cam = tpp;
        }
        gunDetails.rig = rig;
        hasHandgun = gunDetails.handgun;
        rig.SetBool("handgun", hasHandgun);
        if(!unHolstering)
        {
            Invoke(nameof(setAnim), 0.001f);
        }
        
    }
    private void setAnim()
    {
        rig.Play(gunDetails.gunType, 0);
    }
    private void initAnim()
    {
        rig.Play("BASE", 0);
    }
}
