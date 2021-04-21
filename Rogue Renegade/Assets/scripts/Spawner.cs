using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] Enemies;
    public GameObject[] Guns;
    public GameObject health;
    private GameObject theSpawned;
    public ParticleSystem enemyParticleSystem;

    [Header("Leave as null")]
    public GameObject SpawnedHealth;

    /*Enemy types
     * 0 is ak74
     * 1 is melee
     */
    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
    public GameObject SpawnEnemy(int EnemyType,float distanceToAttackMultiplier = 1)
    {
        
       
        return null;
    }
    public GameObject SpawnHealth()
    {
        theSpawned = Instantiate(health, gameObject.transform.position, health.transform.rotation);
        Destroy(theSpawned, 60);
        return theSpawned;
    }
    public void SpawnGun(int GunType, Vector3 pos)
    {
        theSpawned = Instantiate(Guns[GunType], pos, Guns[GunType].transform.rotation);
        //Destroy(theSpawned, 60);
    }
}
