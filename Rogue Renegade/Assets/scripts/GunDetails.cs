using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GunDetails : MonoBehaviour
{
    
    public string gunType = "AK47";
    public int gunInt = 0;
    public float timeToNextShot = 0;
    public Vector3 localPos;
    public Vector3 localRot;
    public Vector3 localScale;
    public float damage = 0;
    public float enemyDamage = 0;
    public int ammoMax = 30;
    public int ammoSpare = 170;
    public bool handgun = false;
    public int ammoLoaded = 0;
    public bool isContinuousShooting = true;
    public AudioSource gunSound;
    public bool gunSoundLoops = true;
    public float d = 0;
    public float yCamOffset = 0.05f;
    public float zCamOffset = 0f;
    public float xCamOffset = 0;
    public float recoilTime = 0.1f;
    public float recoilUpSpeed = -10f;
    public float recoilDownSpeed = 10f;
    public CinemachineVirtualCamera scopeCam;

    private void Start()
    {
        gunSound.volume = 1;
        gunSound.loop = false;
        
    }
    private void FixedUpdate()
    {
        if (gameObject.CompareTag("DroppedGun"))
        {
            d += 1 * Time.deltaTime;
            if(d >= 60)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            d = 0;
        }
        
    }
   
}
