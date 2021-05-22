using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BotRig : MonoBehaviour
{
    public Rig BodyAim;
    public Rig WeaponAiming;
    public Rig WeaponHandIk;
    public Rig WeaponPose;
    public Transform AimLookAt;
    private BotMovement botMovement;

    private void Start()
    {
        botMovement = GetComponent<BotMovement>();
    }

    public void LookAt(Vector3 pos)
    {
        if (!botMovement.isMoving)
        {
            transform.LookAt(pos);
        }
        AimLookAt.position = pos;
        BodyAim.weight = 1;
    }

    /// <summary>
    /// For Switching between weapon pose and weapon aim
    /// </summary>
    /// <param name="on">True is weapon aim, False is weapon pose</param>
    public void SetAim(bool on)
    {
        if (on)
        {
            WeaponAiming.weight = 1;
            WeaponPose.weight = 0;
        }
        else
        {
            WeaponAiming.weight = 0;
            WeaponPose.weight = 1;
        }
    }
}
