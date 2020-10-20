using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class GameMechMulti : NetworkManager 
{

    public enum GameMode
    {
        Survival,
        Deathmatch,
        Possession

    }


    private GameMech gameMech;
    public GameObject mainCamera;
    public GameObject uiCamera;
    public GameObject aimCylinder;
    public GameObject Canvas;
    public GameObject tpp; 
    public Dictionary<uint, GameObject> playerDictionary;
    public Dictionary<uint, Target> playerTargets;
    public Dictionary<int, GameObject> guns;
    public Dictionary<int, GameObject> networkedGuns;
    public Dictionary<GameObject, ClothSaveData> clothSaves;
    public bool canSpawnEnemies = true;
    public GameMode gameMode;
    public EnemyMech[] enemies;
    public GameObject[] enemySpawners;
    public SurvivalMechMulti survivalMechMulti;
    public bool gameStarted = false;
    public GameObject ScorePanel;
    public GameObject scorePreset;
    
    

    [Header("Not networked")]
    public GameObject RDG5;

    [Header("Not networked")]
    public GunDetails[] gunDetailss;



   
    
    
    
    public override void Start()
    {
        base.Start();
        gameMech = GetComponent<GameMech>();
        clothSaves = new Dictionary<GameObject, ClothSaveData>();
        playerDictionary = new Dictionary<uint, GameObject>();
        playerTargets = new Dictionary<uint, Target>();
        guns = new Dictionary<int, GameObject>();
        networkedGuns = new Dictionary<int, GameObject>();
        foreach(GameObject g in spawnPrefabs)
        {
            GunDetails gunDetails = g.GetComponent<GunDetails>(); 
            if(gunDetails != null)
            {
                networkedGuns.Add(gunDetails.gunInt, g);
                //Debug.Log(gunDetails.gunInt + ", " + gunDetails.gunType);
            }
        }
        foreach (GunDetails g in gunDetailss)
        {
            guns.Add(g.gunInt, g.gameObject);
        }
        switch (gameMode)
        {
            case GameMode.Survival:
                gameMech.enemyDeathCallBack = survivalMechMulti.enemyKilled;
                break;
        }
        
    }
   
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        
        /*Instantiate(mainCamera);
        mainCamera.SetActive(true);
        Instantiate(uiCamera);
        uiCamera.SetActive(true);
        Instantiate(Canvas);
        Canvas.SetActive(true);
        Instantiate(aimCylinder);
        aimCylinder.SetActive(true);
        Instantiate(tpp);
        tpp.SetActive(true);*/
        GameObject player = gameMech.spawnAndReturnPlayer(true);
        PlayerMotion playerMotion = player.GetComponent<PlayerMotion>();
        PlayerGun playerGun = player.GetComponent<PlayerGun>();
        


        NetworkServer.AddPlayerForConnection(conn, player);
        playerDictionary.Add(playerMotion.netId, player);
        playerTargets.Add(playerMotion.netId, player.GetComponent<Target>());
        PlayerMultiDetails playerMultiDetails = player.GetComponent<PlayerMultiDetails>();
        //gameStarted = true;
        
        switch (gameMode)
        {
            case GameMode.Survival:
                
                SurvivalSetup(conn,playerMotion,playerMultiDetails);
                break;
        }
        /*if (!spawn && canSpawnEnemies)
        {
            spawn = true;
            StartCoroutine(spawnLoop());
        }*/


    }
    private void SurvivalSetup(NetworkConnection conn,PlayerMotion playerMotion,PlayerMultiDetails playerMultiDetails)
    {
        survivalMechMulti.playerscores.Add(playerMotion.netId, new NameAndScore { name = playerMultiDetails.playerName, score = 0 });
        /*uint[] ids = new uint[survivalMechMulti.playerscores.Keys.Count];
        survivalMechMulti.playerscores.Keys.CopyTo(ids, 0);
        NameAndScore[] nameAndScores = new NameAndScore[survivalMechMulti.playerscores.Values.Count];
        survivalMechMulti.playerscores.Values.CopyTo(nameAndScores, 0);*/

        List<uint> ids = new List<uint>(survivalMechMulti.playerscores.Keys);
        for(int i = 0; i < survivalMechMulti.playerscores.Count; i++)
        {
            survivalMechMulti.TargetSetUpScores(conn,  ids[i], survivalMechMulti.playerscores[ids[i]]);
        }
        
    }
   
    
   
   
   
    private void OnServerInitialized()
    {
        
    }
    private void Update()
    {
        
    }
    private bool spawn = false;
    IEnumerator spawnLoop()
    {

        while (spawn)
        {
            int i = Random.Range(0, enemies.Length);
            int s = Random.Range(0, enemySpawners.Length);
            GameObject go = Instantiate(enemies[i].gameObject, enemySpawners[s].transform.position, enemies[i].transform.rotation);
            NetworkServer.Spawn(go);
            yield return new WaitForSeconds(10);
        }    
       
        
        
    }
    public static void respawnMessage(NetworkConnection conn)
    {
        
    }
    public void respawnPlayer(NetworkConnection conn,GameObject prevPlayer)
    {
        PlayerMotion playerMotion = prevPlayer.GetComponent<PlayerMotion>();
        PlayerGun playerGun = prevPlayer.GetComponent<PlayerGun>();
        playerDictionary.Remove(playerMotion.netId);
        playerTargets.Remove(playerMotion.netId);

        GameObject player = gameMech.spawnAndReturnPlayer(true);
        playerMotion = player.GetComponent<PlayerMotion>();
        playerGun = player.GetComponent<PlayerGun>();
        


        NetworkServer.ReplacePlayerForConnection(conn, player);
        playerDictionary.Add(playerMotion.netId, player);
        playerTargets.Add(playerMotion.netId, player.GetComponent<Target>());
        NetworkServer.Destroy(prevPlayer);
    }
    
}
