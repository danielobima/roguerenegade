using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;


public class SurvivalMechMulti : NetworkBehaviour
{

    private GameObject[] spawnerGOs;
    public int waveNo = 1;
    public  bool survivalOngoing = false;
    public bool hasSpawned = false;
    public float waveCooldown = 20;
    public List<GameObject> spawnedEnemies;
    public int floor;
    private int easyWeapons = 50;
    public GameMech gameMech;
    public GameMechMulti gameMechMulti;
    private int meduimWeapons = 80;
    private bool goingToNextWave = false;
    private bool spawning = false;
    private int noOfEnemies;


    public ScreenTexts ScreenTexts;
    //private int hardWeapons = 100;
   
    public Scores playerscores = new Scores();




    // Floor is now called level. PLEASE DONT CONFUSE AND MAKE RUSSIA EXPLODE FOR NO REASON!
    void Start()
    {
        spawnerGOs = GameObject.FindGameObjectsWithTag("Spawners");
        floor = 1;
       
        
        noOfEnemies = spawnerGOs[0].GetComponent<Spawner>().Enemies.Length;
    }


    private int calculateDifficulty(int waveNo, int floorNo)
    {
        return waveNo * floorNo * 2;
    }
    private int calculateBias(int waveNo, int floorNo)
    {
        return waveNo * floorNo;
    }
    private int randomEnemy()
    {
        return Random.Range(0,noOfEnemies);
    }
    
    public override void OnStartClient()
    {
        playerscores.Callback += scoresUpdated;
    }
    [TargetRpc]
    public void TargetSetUpScores(NetworkConnection target,  uint id,NameAndScore nameAndScore)
    {
        GameObject go = Instantiate(gameMechMulti.scorePreset, gameMechMulti.ScorePanel.transform);
        go.name = id.ToString();
        go.SetActive(true);
        TextMeshProUGUI name = go.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI score = go.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        //name.SetText(nameAndScore.name);
        name.SetText(id.ToString());
        score.SetText(nameAndScore.score.ToString());
        
    }

    //Called using a delegate void in enemy mech called enemydeathcallback
    public void enemyKilled(Target g)
    {
        uint killer = g.attackers[g.attackers.Count - 1];
        spawnedEnemies.Remove(g.gameObject);
        int score = playerscores[killer].score;
        string name = playerscores[killer].name;
        score += 1;
        playerscores[killer] = new NameAndScore { name = name, score = score };
        
    }
    static public GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) {
            if (t.gameObject.name == withName)
            {
                return t.gameObject;
            }
        } 
        return null;
    }
    private void scoresUpdated(Scores.Operation op, uint id, NameAndScore score)
    {
        //Debug.Log(id + ": " + score);
        //Put the scores in a list
        GameObject g = getChildGameObject(gameMechMulti.ScorePanel, id.ToString());
        if ( g!= null)
        {
            TextMeshProUGUI scoreText = g.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            scoreText.SetText(score.score.ToString());
        }
        else
        {
            GameObject go = Instantiate(gameMechMulti.scorePreset,gameMechMulti.ScorePanel.transform);
            go.SetActive(true);
            go.name = id.ToString();
            TextMeshProUGUI name = go.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI scoreText = go.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            name.SetText(id.ToString());
            scoreText.SetText(score.score.ToString());
        }
    }
    private void spawnEnemies()
    {
        if (!spawning)
        {
            StartCoroutine(spawnLoop());
            spawning = true;
        }
    }
    IEnumerator spawnLoop()
    {

        while (spawnedEnemies.Count < waveNo)
        {
            int i = Random.Range(0, spawnerGOs.Length - 1);
            GameObject go = spawnerGOs[i].GetComponent<Spawner>().SpawnEnemy(randomEnemy(), floor);
            spawnedEnemies.Add(go);
            NetworkServer.Spawn(go);
            yield return new WaitForSeconds(1);
        }
        spawning = false;
        hasSpawned = true;

    }
    private void GoToNextWave()
    {
        if (!goingToNextWave)
        {
            StartCoroutine(goToNextWave());
            goingToNextWave = true;
        }
    }
    IEnumerator goToNextWave()
    {
        yield return new WaitForSeconds(waveCooldown);
       
        waveNo += 1;
        
        if (waveNo == 10)
        {
            waveNo = 1;
            floor += 1;
            
        }
        hasSpawned = false;
        goingToNextWave = false;
        
    }
    
   
    [ServerCallback]
    void FixedUpdate()
    {

        if (gameMechMulti.gameStarted && gameMechMulti.gameMode == GameMechMulti.GameMode.Survival)
        {
            if (!hasSpawned)
            {
                spawnEnemies();
            }
            else
            {
                if (spawnedEnemies.Count == 0)
                {
                    GoToNextWave();
                }
                
            }
            /*if (playerTarget.isDead)
            {
                restartButton.SetActive(true);
                survivalOngoing = false;

            }*/
        }

    }
    

}
