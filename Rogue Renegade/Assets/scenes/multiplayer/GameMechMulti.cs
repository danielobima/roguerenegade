using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameMechMulti : NetworkManager 
{

    public enum GameMode
    {
        Survival,
        Deathmatch,
        Possession

    }

    public static string IPAddress = "localhost";
    public static bool isHost = false;
    public static ushort port = 7777;
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
        if (isHost)
        {
            StartHost();
        }
        else
        {
            networkAddress = IPAddress;
            TelepathyTransport telepathyTransport = GetComponent<TelepathyTransport>();
            telepathyTransport.port = port;
            StartClient();
        }
       
        
    }
    public void Exit()
    {
        if (isHost)
        {
            StopHost();
            
        }
        else
        {
            StopClient();
        }
    }
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        survivalMechMulti.TargetConnected(conn);
    }


    public override void OnStopClient()
    {
        Cursor.visible = true;
        base.OnStopClient();
        
        //SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
    public override void OnStopHost()
    {
        Cursor.visible = true;
        base.OnStopHost();
        
        //SceneManager.LoadScene(0, LoadSceneMode.Single);
    }




    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<SendUsername>(OnUsernameReceived);
    }
   
    private void OnUsernameReceived(NetworkConnection conn, SendUsername sendUsername)
    {
       
        switch (gameMode)
        {
            case GameMode.Survival:
                
                SurvivalSetup(conn,sendUsername.username);
                break;
        }
    }
    
    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        base.OnClientError(conn, errorCode);
        Debug.Log("Teeee");
    }
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        //IveDipped();
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        
        switch (gameMode)
        {
            case GameMode.Survival:

               
                GameMessage gameMessage = new GameMessage
                {
                    theMessage = survivalMechMulti.playerscores[conn.identity.netId].name + " left"
                };
                NetworkServer.SendToAll(gameMessage);
                survivalMechMulti.playerscores.Remove(conn.identity.netId);
                break;
        }
        base.OnServerDisconnect(conn);
        
    }

    private class SendUsername : MessageBase
    {
        public string username;
    }
    
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        if (!isHost)
        {
            PlayerDetails playerDetails = new PlayerDetails();
            string playerName = playerDetails.getUserName();
            SendUsername sendUsername = new SendUsername()
            {
                username = playerName
            };

            NetworkClient.Send(sendUsername);
        }
    }
    public override void OnStartHost()
    {
        base.OnStartHost();
        StartCoroutine(registerHostUsername());
    }
    IEnumerator registerHostUsername()
    {
        yield return new WaitForSeconds(0.5f);
        PlayerDetails playerDetails = new PlayerDetails();
        string playerName = playerDetails.getUserName();
        SendUsername sendUsername = new SendUsername()
        {
            username = playerName
        };
       
        NetworkClient.Send(sendUsername);
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
                
                //SurvivalSetup(conn);
                break;
        }
       

    }

    private void SurvivalSetup(NetworkConnection conn,string userName = "No namaewa")
    {
        
        if(conn.identity != null)
        {
            if (survivalMechMulti.playerscores.ContainsKey(conn.identity.netId))
            {
                /*if (!userName.Equals(""))
                {
                    survivalMechMulti.playerscores[conn.identity.netId] = new NameAndScore { name = userName, score = 0 };
                }
                else
                {
                    survivalMechMulti.playerscores[conn.identity.netId] = new NameAndScore { name = "No name", score = 0 };
                }*/
                survivalMechMulti.playerscores[conn.identity.netId] = new NameAndScore { name = userName, score = 0 };
            }
            else
            {
                /*if (!userName.Equals(""))
                {
                    survivalMechMulti.playerscores.Add(conn.identity.netId, new NameAndScore { name = userName, score = 0 });
                }
                else
                {
                    survivalMechMulti.playerscores.Add(conn.identity.netId, new NameAndScore { name = "No Name", score = 0 });

                }*/
                //Debug.Log(userName);
                survivalMechMulti.playerscores.Add(conn.identity.netId, new NameAndScore { name = userName, score = 0 });
                /*uint[] ids = new uint[survivalMechMulti.playerscores.Keys.Count];
                survivalMechMulti.playerscores.Keys.CopyTo(ids, 0);
                NameAndScore[] nameAndScores = new NameAndScore[survivalMechMulti.playerscores.Values.Count];
                survivalMechMulti.playerscores.Values.CopyTo(nameAndScores, 0);*/
                List<uint> ids = new List<uint>(survivalMechMulti.playerscores.Keys);
                for (int i = 0; i < survivalMechMulti.playerscores.Count; i++)
                {
                    survivalMechMulti.TargetSetUpScores(conn, ids[i], survivalMechMulti.playerscores[ids[i]]);
                }
            }
            GameMessage gameMessage = new GameMessage
            {
                theMessage = userName +" has joined"
            };
            NetworkServer.SendToAll(gameMessage);
        }



    }
    
   
    
   
   
   
    private void OnServerInitialized()
    {
        
    }
    private void Update()
    {
        
    }
    
    public void respawnPlayer(NetworkConnection conn,GameObject prevPlayer,string name)
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
        switch (gameMode)
        {
            case GameMode.Survival:
                
                SurvivalSetup(conn,name);
                break;
        }
    }
    
}
