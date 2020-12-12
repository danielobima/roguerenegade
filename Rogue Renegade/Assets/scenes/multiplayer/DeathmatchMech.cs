using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class DeathmatchMech : NetworkBehaviour
{
    private GameObject[] spawnerGOs;
    public bool deathmatchOngoing = false;
    public bool hasSpawned = false;
    public int floor;
    private int easyWeapons = 50;
    private GameMech gameMech;
    private GameMechMulti gameMechMulti;
    private int meduimWeapons = 80;
    private bool goingToNextWave = false;
    private bool spawning = false;
    private bool canSpawnHealth = true;
    public int nextHealthInSeconds = 60;
    private bool hasStartedSpawningHealth;
    public bool startedCounter = false;

    [SyncVar]
    public bool isOnCooldown;
    [SyncVar]
    public bool gameStarted = false;
    [SyncVar]
    public int gameTime;
    [SyncVar]
    public int timeElapsed;
    [SyncVar(hook =nameof(UpdateTimeRemaining))]
    public int timeRemaining;


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
    public void TargetSetUpScores(NetworkConnection target, int id, NameAndScore nameAndScore)
    {
        GameObject g = getChildGameObject(ScorePanel, id.ToString());
        if (g == null)
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
    public void playerDied(int killer,int killed)
    {
        /*deadPlayers++;
        Debug.Log("PLayer died");
        if (deadPlayers >= gameMechMulti.playerTargets.Count)
        {
            EndGame();
        }
        else
        {
            Debug.Log(gameMechMulti.playerTargets.Count);
        }*/

        string killedName = playerscores[killed].name;
        if (gameStarted)
        {
            int score = playerscores[killer].score;
            string name = playerscores[killer].name;
            score += 1;
            playerscores[killer] = new NameAndScore { name = name, score = score };
        }
        GameMessage gameMessage = new GameMessage
        {
            theMessage = name + " killed " + killedName
        };
        NetworkServer.SendToAll(gameMessage);

    }
    private void EndGame()
    {
        gameStarted = false;
        startedCounter = false;
        foreach (NameAndScore nameAndScore in playerscores.Values)
        {
            RpcAddPlayerToScoreList(nameAndScore.name, nameAndScore.score);
        }
        RpcShowScores();
    }
    [ClientRpc]
    private void RpcShowScores()
    {

        ScreenObjects.survivalScoresPanel.SetActive(true);
        //ScreenObjects.survivalWaveText.GetComponent<TextMeshProUGUI>().SetText("Wave " + waveNumber);
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
   
    static public GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t.gameObject.name == withName)
            {
                return t.gameObject;
            }
        }
        return null;
    }
    private void scoresUpdated(Scores.Operation op, int id, NameAndScore score)
    {
        //Debug.Log(id + ": " + score);
        //Put the scores in a list

        if (op == SyncIDictionary<int, NameAndScore>.Operation.OP_REMOVE)
        {
            GameObject g = getChildGameObject(ScorePanel, id.ToString());
            if (g != null)
            {
                Destroy(g);
            }
        }

        if (op == SyncIDictionary<int, NameAndScore>.Operation.OP_ADD || op == SyncIDictionary<int, NameAndScore>.Operation.OP_SET)
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
        if (op == SyncIDictionary<int, NameAndScore>.Operation.OP_CLEAR)
        {
            for (int i = 0; i < ScorePanel.transform.childCount; i++)
            {
                if (ScorePanel.transform.GetChild(i).gameObject.name != "Kills" &&
                    ScorePanel.transform.GetChild(i).gameObject.name != "Item preset")
                {
                    Destroy(ScorePanel.transform.GetChild(i).gameObject);
                }
            }
        }



    }


   
   
    private void UpdateTimeRemaining(int oldValue,int newValue)
    {
        string s;
        if(newValue >= 60)
        {
            int se = newValue - 60;
            int mi = newValue / 60;
            if (se < 10)
            {
                s = mi + ":0" + se;
            }
            else
            {
                s = mi + ":" + se;
            }
        }
        else
        {
            if (newValue < 10)
            {
                s = "0:0" + newValue;
            }
            else
            {
                s = "0:" + newValue;
            }
        }
        ScreenObjects.timeRemaining.text = s;
    }
    

    IEnumerator GameCounter()
    {
        for(int i = 0; i< gameTime; i++)
        {
            yield return new WaitForSeconds(1);
            timeElapsed = i;
            timeRemaining = gameTime - i;
        }
        EndGame();
    }
    [ClientRpc]
    private void RpcShowTimer()
    {
        ScreenObjects.timeRemaining.gameObject.SetActive(true);
    }
    private void StartGame()
    {
        if (!startedCounter)
        {
            RpcShowTimer();
            StartCoroutine(GameCounter());
            startedCounter = true;
        }
    }


    //called on gameMechMulti through a delegate void
    [Server]
    public void GameLogic()
    {
        if (gameStarted)
        {
            StartGame();
            

        }
    }
}
