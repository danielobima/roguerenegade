using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.SceneManagement;


public class SurvivalMechMulti : NetworkBehaviour
{

    private GameObject[] spawnerGOs;
    public int waveNo = 1;
    public  bool survivalOngoing = false;
    public bool hasSpawned = false;
    public int waveCooldown = 10;
    public List<GameObject> spawnedEnemies;
    public int floor;
    private int easyWeapons = 50;
    private GameMech gameMech;
    private GameMechMulti gameMechMulti;
    private int meduimWeapons = 80;
    private bool goingToNextWave = false;
    private bool spawning = false;
    private int noOfEnemies;
    private bool canSpawnHealth = true;
    public int nextHealthInSeconds = 60;
    private bool hasStartedSpawningHealth;
    [SyncVar]
    public bool isOnCooldown;
    [SyncVar]
    public bool gameStarted = false;


    public ScreenTexts ScreenTexts;
    public ScreenObjects ScreenObjects;
    public int deadPlayers = 0;
    //private int hardWeapons = 100;
   
    public Scores playerscores = new Scores();

    public GameObject scorePreset;
    public GameObject ScorePanel;





    // Floor is now called level. PLEASE DONT CONFUSE AND MAKE RUSSIA EXPLODE FOR NO REASON!
    //Ive disabled floors for now, but on every 10th wave add a boss like a mecha suit or sth
    void Start()
    {
        spawnerGOs = GameObject.FindGameObjectsWithTag("Spawners");
        floor = 1;
       
        
        noOfEnemies = spawnerGOs[0].GetComponent<Spawner>().Enemies.Length;

        gameMech = GetComponent<GameMech>();
        gameMechMulti = GameObject.FindGameObjectWithTag("GameMechMulti").GetComponent<GameMechMulti>();


        TellManagerToSpawnPlayers();
        //SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(0));
        
    }
    [ServerCallback]
    private void TellManagerToSpawnPlayers()
    {
        //SceneManager.MoveGameObjectToScene(gameMechMulti.gameObject, SceneManager.GetSceneByName(gameMechMulti.gameScene));
        gameMechMulti.AddPlayersInNewScene();
       
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
        base.OnStartClient();
        playerscores.Callback += scoresUpdated;

    }
    [TargetRpc]
    public void TargetConnected(NetworkConnection connection)
    {
        HomeScreen.clientStarted = true;
    }
    [TargetRpc]
    public void TargetSetUpScores(NetworkConnection target,  uint id,NameAndScore nameAndScore)
    {
        GameObject g = getChildGameObject(ScorePanel, id.ToString());
        if(g == null)
        {
            GameObject go = Instantiate(scorePreset, ScorePanel.transform);
            go.name = id.ToString();
            go.SetActive(true);
            TextMeshProUGUI name = go.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI score = go.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            //name.SetText(nameAndScore.name);
            name.SetText(nameAndScore.name);
            score.SetText(nameAndScore.score.ToString());
        }
        
    }
    //Assigned as a delegate void in OnServerAddPlayer(). Called in Target script;
    public void playerDied()
    {
        deadPlayers++;
        Debug.Log("PLayer died");
        if(deadPlayers >= gameMechMulti.playerTargets.Count)
        {
            EndGame();
        }
        else
        {
            Debug.Log(gameMechMulti.playerTargets.Count);
        }
    }
    private void EndGame()
    {
        gameStarted = false;
        foreach(NameAndScore nameAndScore in playerscores.Values)
        {
            RpcAddPlayerToScoreList(nameAndScore.name, nameAndScore.score);
        }
        RpcShowScores(waveNo);
    }
    [ClientRpc]
    private void RpcShowScores(int waveNumber)
    {
        
        ScreenObjects.survivalScoresPanel.SetActive(true);
        ScreenObjects.survivalWaveText.GetComponent<TextMeshProUGUI>().SetText("Wave "+ waveNumber);
    }
    [ClientRpc]
    private void RpcAddPlayerToScoreList(string playersName, int kills)
    {
        TextMeshProUGUI nameText = Instantiate(ScreenObjects.survivalScoreItem, ScreenObjects.survivalScoresList.transform).GetComponent<TextMeshProUGUI>();
        nameText.gameObject.SetActive(true);
        nameText.SetText(playersName);
        nameText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(kills.ToString());
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
        
        if (op == SyncIDictionary<uint, NameAndScore>.Operation.OP_REMOVE)
        {
            GameObject g = getChildGameObject(ScorePanel, id.ToString());
            if (g != null)
            {
                Destroy(g);
            }
        }

        if (op == SyncIDictionary<uint, NameAndScore>.Operation.OP_ADD || op == SyncIDictionary<uint, NameAndScore>.Operation.OP_SET)
        {
            
            GameObject g = getChildGameObject(ScorePanel, id.ToString());
            if (g != null)
            {
                TextMeshProUGUI scoreText = g.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                scoreText.SetText(score.score.ToString());
                
            }
            else
            {
                
                GameObject go = Instantiate(scorePreset, ScorePanel.transform);
                go.SetActive(true);
                go.name = id.ToString();
                
                TextMeshProUGUI name = go.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI scoreText = go.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                name.SetText(score.name);
                scoreText.SetText(score.score.ToString());

            }
        }
        if (op == SyncIDictionary<uint, NameAndScore>.Operation.OP_CLEAR)
        {
            for (int i = 0; i < ScorePanel.transform.childCount; i++)
            {
                if(ScorePanel.transform.GetChild(i).gameObject.name != "Kills" &&
                    ScorePanel.transform.GetChild(i).gameObject.name != "Item preset")
                {
                    Destroy(ScorePanel.transform.GetChild(i).gameObject);
                }
            }
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

        while (spawnedEnemies.Count < waveNo * NetworkServer.connections.Count)
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
    [ClientRpc]
    private void RpcUpdateCoolDown(int i)
    {
       
        string s = "Time to next wave: " + i;
        ScreenTexts.WaveCountdown.text = s;
    }
    [ClientRpc]
    private void RpcShowCoolDown(bool active)
    {


        ScreenTexts.WaveCountdown.gameObject.SetActive(active);
    }
    IEnumerator goToNextWave()
    {
        isOnCooldown = true;
        RpcShowCoolDown(true);
        for(int i = 0; i < waveCooldown; i++)
        {
            RpcUpdateCoolDown(waveCooldown - i);
            yield return new WaitForSeconds(1);
        }
        RpcShowCoolDown(false);
        


        waveNo += 1;
        RpcShowWave(true, waveNo);
        yield return new WaitForSeconds(3);
        RpcShowWave(false, waveNo);

        /*if (waveNo == 10)
        {
            waveNo = 1;
            floor += 1;
            
        }*/
        hasSpawned = false;
        isOnCooldown = false;
        goingToNextWave = false;
        
        
    }
    [ClientRpc]
    private void RpcShowWave(bool active,int waveNo)
    {
        ScreenTexts.WaveText.gameObject.SetActive(active);
        ScreenTexts.WaveText.text = "Wave " + waveNo;
    }
    

    //called on gameMechMulti through a delegate void
    [Server]
    public void GameLogic()
    {
        if (gameStarted)
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
            
          
        }
    }
    
    

}
