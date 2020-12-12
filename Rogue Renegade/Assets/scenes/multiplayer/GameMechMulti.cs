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
    public GameObject aimCylinder;
    public GameObject tpp; 
    public Dictionary<uint, GameObject> playerDictionary;
    public Dictionary<uint, Target> playerTargets;
    public Dictionary<int, GameObject> guns;
    public Dictionary<int, GameObject> networkedGuns;
    public Dictionary<GameObject, ClothSaveData> clothSaves;
    public static GameMode gameMode;
    [Scene]
    public string survivalScene;
    [Scene]
    public string deathmatchScene;
    private string gameScene;
    public EnemyMech[] enemies;
    public GameObject[] enemySpawners;

    public DeathmatchMech deathmatchMech;
    public SurvivalMechMulti survivalMechMulti;

    private delegate void GameLogic();
    private GameLogic gameLogic;


    [Header("For the lobby scene only")]
    public Transform lobbyListView;
    public LobbyManager lobbyManager;
    public bool isLobby = false;
    

    [Header("Not networked")]
    public GameObject RDG5;

    [Header("Not networked")]
    public GunDetails[] gunDetailss;

    
    


    public override void Start()
    {
        base.Start();
        
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
        //survivalMechMulti.TargetConnected(conn);
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
   
    
    
    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        base.OnClientError(conn, errorCode);
        //Debug.Log("Teeee");
    }
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        //IveDipped();
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {

        if (!isLobby)
        {
            GameMessage gameMessage;
            switch (gameMode)
            {
                case GameMode.Survival:
                    survivalMechMulti.playerscores.Remove(conn.connectionId);
                    gameMessage = new GameMessage
                    {
                        theMessage = survivalMechMulti.playerscores[conn.connectionId].name + " left"
                    };
                    NetworkServer.SendToAll(gameMessage);
                    break;
                case GameMode.Deathmatch:
                    deathmatchMech.playerscores.Remove(conn.connectionId);
                    gameMessage = new GameMessage
                    {
                        theMessage = deathmatchMech.playerscores[conn.connectionId].name + " left"
                    };
                    NetworkServer.SendToAll(gameMessage);
                    break;
            }
            
        }
        base.OnServerDisconnect(conn);
        
    }
    private void OnUsernameReceived(NetworkConnection conn, SendUsername sendUsername)
    {

        if (!isLobby)
        {
            switch (gameMode)
            {
                case GameMode.Survival:

                    SurvivalSetup(conn, sendUsername.username);
                    break;
                case GameMode.Deathmatch:

                    DeathmatchSetup(conn, sendUsername.username);
                    break;
            }
        }
    }
    public  class SendUsername : MessageBase
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
        /*GameObject player = gameMech.spawnAndReturnPlayer(true);
        PlayerMotion playerMotion = player.GetComponent<PlayerMotion>();
        PlayerGun playerGun = player.GetComponent<PlayerGun>();
        


        NetworkServer.AddPlayerForConnection(conn, player);
        playerDictionary.Add(playerMotion.netId, player);
        playerTargets.Add(playerMotion.netId, player.GetComponent<Target>());
        PlayerMultiDetails playerMultiDetails = player.GetComponent<PlayerMultiDetails>();*/

        
        GameObject playerNameObj = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "playerNameObj"), lobbyListView);
        lobbyManager.lobbyPlayers.Add(playerNameObj.GetComponent<LobbyPlayer>());
        if(NetworkServer.connections.Count <= 1)
        {
            playerNameObj.GetComponent<LobbyPlayer>().isLeader = true;
        }
        NetworkServer.AddPlayerForConnection(conn, playerNameObj);
        
        Debug.Log("YES");

    }
    public void AddPlayersInNewScene()
    {
     
        gameMech = GameObject.FindGameObjectWithTag("Mech").transform.GetChild(0).GetComponent<GameMech>();
        switch (gameMode)
        {
            case GameMode.Survival:
                survivalMechMulti = GameObject.FindGameObjectWithTag("Mech").transform.GetChild(0).GetComponent<SurvivalMechMulti>();
                gameMech.enemyDeathCallBack = survivalMechMulti.enemyKilled;
                gameLogic = survivalMechMulti.GameLogic;
                break;
            case GameMode.Deathmatch:
                deathmatchMech = GameObject.FindGameObjectWithTag("Mech").transform.GetChild(0).GetComponent<DeathmatchMech>();
                //gameMech.enemyDeathCallBack = survivalMechMulti.enemyKilled;
                gameLogic = deathmatchMech.GameLogic;
                break;
        }
        foreach (NetworkConnection conn in NetworkServer.connections.Values)
        {
            
            GameObject player = gameMech.spawnAndReturnPlayer(true);
            
            PlayerMotion playerMotion = player.GetComponent<PlayerMotion>();
            PlayerGun playerGun = player.GetComponent<PlayerGun>();
            
            if(conn.identity == null)
            {
                NetworkServer.AddPlayerForConnection(conn, player);
            }
            else
            {
                NetworkServer.ReplacePlayerForConnection(conn, player);
            }
            playerDictionary.Add(playerMotion.netId, player);
            playerTargets.Add(playerMotion.netId, player.GetComponent<Target>());
            PlayerMultiDetails playerMultiDetails = player.GetComponent<PlayerMultiDetails>();

        }
        isLobby = false;
    }
    public void MoveToGameScene()
    {
        switch (gameMode)
        {
            case GameMode.Survival:
                gameScene = survivalScene;
                break;
            case GameMode.Deathmatch:
                gameScene = deathmatchScene;
                break;

        }
        ServerChangeScene(gameScene);
        
    }
    

    private void SurvivalSetup(NetworkConnection conn,string userName = "No namaewa",int prevId = -69)
    {
     
        if(conn.identity != null)
        {
            int mScore = 0;
            if (prevId != -69)
            {
                /*uint prev = (uint)prevId;
                if (survivalMechMulti.playerscores.ContainsKey(prev))
                {
                    survivalMechMulti.playerscores.Remove(prev);
                }*/
            }
            else
            {
                GameMessage gameMessage = new GameMessage
                {
                    theMessage = userName + " has joined"
                };
                NetworkServer.SendToAll(gameMessage);
            }
            if (survivalMechMulti.playerscores.ContainsKey(conn.connectionId))
            {
                /*if (!userName.Equals(""))
                {
                    survivalMechMulti.playerscores[conn.identity.netId] = new NameAndScore { name = userName, score = 0 };
                }
                else
                {
                    survivalMechMulti.playerscores[conn.identity.netId] = new NameAndScore { name = "No name", score = 0 };
                }*/
                survivalMechMulti.playerscores[conn.connectionId] = new NameAndScore { name = userName, score = mScore };
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
                survivalMechMulti.playerscores.Add(conn.connectionId, new NameAndScore { name = userName, score = mScore });
                /*uint[] ids = new uint[survivalMechMulti.playerscores.Keys.Count];
                survivalMechMulti.playerscores.Keys.CopyTo(ids, 0);
                NameAndScore[] nameAndScores = new NameAndScore[survivalMechMulti.playerscores.Values.Count];
                survivalMechMulti.playerscores.Values.CopyTo(nameAndScores, 0);*/
                List<int> ids = new List<int>(survivalMechMulti.playerscores.Keys);
                for (int i = 0; i < survivalMechMulti.playerscores.Count; i++)
                {
                    survivalMechMulti.TargetSetUpScores(conn, ids[i], survivalMechMulti.playerscores[ids[i]]);
                }
            }
           
        }
      



    }
    private void DeathmatchSetup(NetworkConnection conn, string userName = "No namaewa", int prevId = -69)
    {

        if (conn.identity != null)
        {
            int mScore = 0;
            if (prevId != -69)
            {
               /* uint prev = (uint)prevId;
                if (deathmatchMech.playerscores.ContainsKey(prev))
                {
                    mScore = deathmatchMech.playerscores[prev].score;
                    deathmatchMech.playerscores.Remove(prev);
                }*/
            }
            else
            {
                GameMessage gameMessage = new GameMessage
                {
                    theMessage = userName + " has joined"
                };
                NetworkServer.SendToAll(gameMessage);
            }


            if (deathmatchMech.playerscores.ContainsKey(conn.connectionId))
            {
                /*if (!userName.Equals(""))
                {
                    survivalMechMulti.playerscores[conn.identity.netId] = new NameAndScore { name = userName, score = 0 };
                }
                else
                {
                    survivalMechMulti.playerscores[conn.identity.netId] = new NameAndScore { name = "No name", score = 0 };
                }*/
                mScore = deathmatchMech.playerscores[conn.connectionId].score;
                deathmatchMech.playerscores[conn.connectionId] = new NameAndScore { name = userName, score = mScore };
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
                deathmatchMech.playerscores.Add(conn.connectionId, new NameAndScore { name = userName, score = mScore });
                /*uint[] ids = new uint[survivalMechMulti.playerscores.Keys.Count];
                survivalMechMulti.playerscores.Keys.CopyTo(ids, 0);
                NameAndScore[] nameAndScores = new NameAndScore[survivalMechMulti.playerscores.Values.Count];
                survivalMechMulti.playerscores.Values.CopyTo(nameAndScores, 0);*/
                List<int> ids = new List<int>(deathmatchMech.playerscores.Keys);
                for (int i = 0; i < deathmatchMech.playerscores.Count; i++)
                {
                    deathmatchMech.TargetSetUpScores(conn, ids[i], deathmatchMech.playerscores[ids[i]]);
                }
            }
            
        }




    }






    private void OnServerInitialized()
    {
        
    }
    private void FixedUpdate()
    {
        if (!isLobby)
        {
            gameLogic();
        }
    }
    
    public void respawnPlayer(NetworkConnection conn,GameObject prevPlayer,string name,uint prevId)
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
                
                SurvivalSetup(conn,name,(int)prevId );
                break; 
            case GameMode.Deathmatch:

                DeathmatchSetup(conn, name, (int)prevId);
                break;
        }
       
    }
    
}
