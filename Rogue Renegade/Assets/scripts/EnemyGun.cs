using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemyGun : NetworkBehaviour {

    private ParticleSystem muzzleFlash;
    public ParticleSystem smoke;
    public GameObject bullet;
    public GameObject shotgunBullet;
    public GunDetails gunDetails;
    private Transform bulletSpawnPos;
    private float f = 0;
    private float aimVariationAmount;
    private GameMechMulti gameMechMulti;

    private void Start()
    {
        muzzleFlash = gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
        bulletSpawnPos = gameObject.transform.GetChild(1);
        gameMechMulti = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMechMulti>();
        if(gameMechMulti != null)
        {
            bullet = gameMechMulti.spawnPrefabs.Find(prefab => prefab.name == "bullet");
            shotgunBullet = gameMechMulti.spawnPrefabs.Find(prefab => prefab.name == "shotgun bullet");
        }
    }
    private void FixedUpdate()
    {
        if (f < gunDetails.timeToNextShot)
        {
            f += 1 * Time.deltaTime;
        }
    }
    public void Shoot(bool isMultiPlayer)
    {
        
        if (f >= gunDetails.timeToNextShot)
        {

            if (!gunDetails.gunType.Equals("Bennelli M4"))
            {
                GameObject go;
                go = Instantiate(bullet, bulletSpawnPos.position, bulletSpawnPos.rotation);
                go.GetComponent<Gunshot>().damage = gunDetails.damage;
                
                muzzleFlash.Play();
                ParticleSystem smo = Instantiate(smoke, bulletSpawnPos.position, bulletSpawnPos.rotation);
                if (isMultiPlayer)
                {
                    NetworkServer.Spawn(go);
                    NetworkServer.Spawn(smo.gameObject);
                }
            }
            else
            {
                GameObject go;
                go = Instantiate(shotgunBullet, bulletSpawnPos.position, bulletSpawnPos.rotation);
                
                muzzleFlash.Play();
                ParticleSystem smo =  Instantiate(smoke, bulletSpawnPos.position, bulletSpawnPos.rotation);
                if (isMultiPlayer)
                {
                    NetworkServer.Spawn(go);
                    NetworkServer.Spawn(smo.gameObject);
                }
            }
            if (!gunDetails.gunSound.isPlaying)
            {
                gunDetails.gunSound.Play();
            }
            /*RaycastHit hit;
            if (Physics.Raycast(gameObject.transform.position, gameObject.transform.right * -1, out hit, 30))
            {
                if (hit.collider.GetComponent<Target>())
                {
                    Target t;
                    t = hit.collider.GetComponent<Target>();
                    t.TakeDamage(gunDetails.enemyDamage);
                    
                }
                


            }*/
            //Debug.Log("Pew!!!");
            f = 0;
        }
        

    }
    
}
