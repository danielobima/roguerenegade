using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivalMech : MonoBehaviour
{
    
    public GameObject[] spawnerGOs;
    private float a = 0;
    public float w = 0;
    public int waveNo = 1;
    public static bool survivalOngoing = false;
    public bool hasSpawned = false;
    public float spawnBeneficiaryAfterSeconds = 30;
    private float b = 0;
    public float waveCooldown = 20;
    public int kills = 0;
    public List<GameObject> spawnedEnemies;
    private Target playerTarget;
    public GameObject restartButton;
    public GameObject startButton;
    public int floor;
    private int easyWeapons = 50;
    private GameMech gameMech;
    private int meduimWeapons = 80;
    //private int hardWeapons = 100;


    // Floor is now called level. PLEASE DONT CONFUSE AND MAKE RUSSIA EXPLODE FOR NO REASON!
    void Start()
    {
        //spawnerGOs = GameObject.FindGameObjectsWithTag("Spawners");
        playerTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Target>();
        
        floor = PlayerPrefs.GetInt("Survival-floor", 1);
        spawnBeneficiaryAfterSeconds = 30 + (floor - 1) * 2;
        playerTarget.healthFull = 20 - 1.5f * floor;
        if (playerTarget.health > playerTarget.healthFull)
        {
            playerTarget.health = playerTarget.healthFull;
        }
        gameMech = GetComponent<GameMech>();
    }

   
    private int calculateDifficulty(int waveNo, int floorNo)
    {
        return waveNo * floorNo * 2;
    }
    private int calculateBias(int waveNo, int floorNo)
    {
        return waveNo * floorNo ;
    }
    private int randomEnemy(int Difficulty, int bias = 1)
    {
        /*int random = Random.Range(bias < 80 ? bias : 80, Difficulty < 100? Difficulty : 100);
        if (random < easyWeapons)
        {
            return Random.Range(1, 3);
        }
        else
        {
            if(random < meduimWeapons)
            {
                return Random.Range(3, 5);
            }
            else
            {
                return Random.Range(5, 8);
            }
        }*/
        return Random.Range(0, 8);
    }
    
    private void goToNextWave()
    {
        w += 1 * Time.deltaTime;

        if (w >= waveCooldown)
        {
            waveNo += 1;
            if (waveNo == 2)
            {
                spawnerGOs[Random.Range(0, spawnerGOs.Length - 1)].GetComponent<Spawner>()
                    .SpawnGun(Random.Range(0, spawnerGOs[Random.Range(0, spawnerGOs.Length - 1)].GetComponent<Spawner>().Guns.Length - 1),
                    new Vector3(0, 1.12f, 0));
            }
            if (waveNo == 10)
            {
                waveNo = 1;
                floor += 1;
                if(floor < 13)
                {
                    playerTarget.healthFull = 20 - 1.5f * floor;
                    if(playerTarget.health > playerTarget.healthFull)
                    {
                        playerTarget.health = playerTarget.healthFull;
                    }
                }
                else
                {
                    playerTarget.healthFull = 1;
                    if (playerTarget.health > playerTarget.healthFull)
                    {
                        playerTarget.health = playerTarget.healthFull;
                    }
                }
                PlayerPrefs.SetInt("Survival-floor", floor);
                spawnBeneficiaryAfterSeconds = 30 + (floor - 1) * 2;
            }
            hasSpawned = false;
            w = 0;
        }
    }
    private void checkIfEnemiesAreDead()
    {
        for (int i = 0; i < spawnedEnemies.Count; i++)
        {
            if (spawnedEnemies[i] != null)
            {
                Target t = spawnedEnemies[i].GetComponent<Target>();
                if (t != null)
                {
                    if (t.isDead)
                    {
                        spawnedEnemies.Remove(spawnedEnemies[i]);
                        kills += 1;
                    }
                }
                else
                {
                    spawnedEnemies.Remove(spawnedEnemies[i]);
                }
            }
            else
            {
                spawnedEnemies.Remove(spawnedEnemies[i]);
            }

        }
    }
    private void spawnBeneficiaries()
    {
        b += 1 * Time.deltaTime;
        if (b >= spawnBeneficiaryAfterSeconds)
        {

            int h = Random.Range(0, spawnerGOs.Length - 1);
            if (spawnerGOs[h].GetComponent<Spawner>().SpawnedHealth == null)
            {
                spawnerGOs[h].GetComponent<Spawner>().SpawnedHealth = spawnerGOs[h].GetComponent<Spawner>().SpawnHealth();
                Destroy(spawnerGOs[h].GetComponent<Spawner>().SpawnedHealth, 60);
            }

            b = 0;
        }
    }
    
    void FixedUpdate()
    {
       
        if(survivalOngoing )
        {
            if (!hasSpawned)
            {
                //spawnEnemies();
            }
            else
            {
                if(spawnedEnemies.Count == 0)
                {
                    goToNextWave();
                }
                else
                {
                    checkIfEnemiesAreDead();
                }
            }
            if (playerTarget.isDead)
            {
                restartButton.SetActive(true);
                survivalOngoing = false;
                
            }
            //spawnBeneficiaries();
        }
        
    }
    private void Update()
    {
        if (!survivalOngoing && gameMech.playerSpawned)
        {
            if (!playerTarget.isDead)
            {
                if (Input.GetKey("p"))
                {
                    survivalOngoing = true;
                    startButton.SetActive(false);
                }
            }
            else
            {
                if (Input.GetKey("p"))
                {
                    SceneManager.LoadScene("survival scene");
                }
            }

           
        }
    }

}
