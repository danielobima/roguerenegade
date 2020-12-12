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
        
        theSpawned = Instantiate(Enemies[EnemyType], gameObject.transform.position, Enemies[EnemyType].transform.rotation);
        //theSpawned.GetComponent<EnemyMech>().timeToAttack = true;
        theSpawned.GetComponent<FresnelEffect>().fresnate(3);
        Instantiate(enemyParticleSystem, gameObject.transform.position, enemyParticleSystem.transform.rotation);
        if (distanceToAttackMultiplier == 1)
        {
            theSpawned.GetComponent<EnemyMech>().distanceToStartAttack = 5;
        }
        else
        {
            if(distanceToAttackMultiplier < 14)
            {
                theSpawned.GetComponent<EnemyMech>().distanceToStartAttack = Random.Range(5, 5 + distanceToAttackMultiplier * 2);
            }
            else
            {
                theSpawned.GetComponent<EnemyMech>().distanceToStartAttack = Random.Range(5,35);
            }
        }
        return theSpawned;
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
