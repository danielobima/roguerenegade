using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerMultiDetails : NetworkBehaviour
{
    public bool isMultiPlayer = false;
    private Target target;
    private PlayerGun playerGun;
    public GameMechMulti gameMechMulti;
    [SyncVar(hook =nameof(UpdateUsername))]
    public string playerName = "Name";
   
    private PlayerMotion playerMotion;
    [SyncVar]
    public bool canStartGame = false;
    private ScreenObjects screenObjects;
    private GameObject chatObjPreset;

    public TextMeshProUGUI nameDisplay;
    public Transform nameCanvas;
    public bool isTyping = false;

    
   
    private void Start()
    {
        target = GetComponent<Target>();
        playerGun = GetComponent<PlayerGun>();
        playerMotion = GetComponent<PlayerMotion>();
        gameMechMulti = GameObject.FindGameObjectWithTag("GameMechMulti").GetComponent<GameMechMulti>();
        screenObjects = GameObject.FindGameObjectWithTag("screen objects").GetComponent<ScreenObjects>();
        TMP_InputField tMP_InputField = screenObjects.chatMessager.GetComponent<TMP_InputField>();
        tMP_InputField.onEndEdit.AddListener(sendchatMsg);
        chatObjPreset = screenObjects.chatbar.transform.GetChild(0).gameObject;
        chatObjPreset.SetActive(false);
        if (!isMultiPlayer)
        {
            gameObject.SetActive(true);
            nameCanvas.gameObject.SetActive(false);
        }
        else
        {
            playerGun.bullet = gameMechMulti.spawnPrefabs.Find(prefab => prefab.name == "bullet");
            playerGun.shotGunBullet = gameMechMulti.spawnPrefabs.Find(prefab => prefab.name == "shotgun bullet");
            if (isLocalPlayer)
            {
                if (GameMechMulti.isHost)
                {
                    CmdSetCanStartGameOnServer(true);
                }
                else
                {
                    CmdSetCanStartGameOnServer(false);
                }
                if (!canStartGame)
                {
                    screenObjects.startButton.SetActive(false);
                }
                nameCanvas.gameObject.SetActive(false);
                NetworkClient.RegisterHandler<ChatMessage>(OnChatMessageReceived);
                NetworkClient.RegisterHandler<GameMessage>(OnGameMessageReceived);
            }
        }
       
        

        

        configureCanvas();
        
        
    }
    
    public void OnGameMessageReceived(NetworkConnection conn, GameMessage gameMessage)
    {
        StartCoroutine(showChat(gameMessage.theMessage));
    }
    public void OnChatMessageReceived(NetworkConnection conn, ChatMessage chatMessage)
    {
        StartCoroutine(showChat(chatMessage.name + ": " + chatMessage.theMessage));
    }
    IEnumerator showChat(string message)
    {
        screenObjects.chatMessager.transform.SetAsLastSibling();
        GameObject bar = Instantiate(chatObjPreset, screenObjects.chatbar.transform);
        
        TextMeshProUGUI textBar = bar.GetComponent<TextMeshProUGUI>();
        
        textBar.SetText(message);
        bar.SetActive(true);
        screenObjects.chatbar.SetActive(true);
        screenObjects.chatMessager.transform.SetAsLastSibling();
        screenObjects.chatMessager.SetActive(false);
        yield return new WaitForSeconds(10);
        if (!screenObjects.chatMessager.activeSelf)
        {
            screenObjects.chatbar.SetActive(false);
        }

    }
    private void OpenMessenger()
    {



        screenObjects.chatMessager.SetActive(true);
        screenObjects.chatbar.SetActive(true);
        screenObjects.chatMessager.transform.SetAsLastSibling();
        TMP_InputField tMP_InputField = screenObjects.chatMessager.GetComponent<TMP_InputField>();
        tMP_InputField.ActivateInputField();
        //EventSystem.current.SetSelectedGameObject(screenObjects.chatMessager);
        isTyping = true;
        Cursor.visible = true;
    }
    private void sendchatMsg(string message)
    {
        TMP_InputField tMP_InputField = screenObjects.chatMessager.GetComponent<TMP_InputField>();
        tMP_InputField.text = "";
        isTyping = false;
        Cursor.visible = false;
        if (message != "")
        {
            CmdSendChatMessage(message);
        }
        screenObjects.chatMessager.SetActive(false);
    }
    [Command]
    private void CmdSendChatMessage(string message)
    {
        ChatMessage chatMessage = new ChatMessage
        {
            theMessage = message,
            name = playerName
        };
        NetworkServer.SendToAll(chatMessage);
        
    }
    
    
    
    [Command]
    private void CmdSetNameOnServer(string theName)
    {
        playerName = theName;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isLocalPlayer)
        {
            playerName = new PlayerDetails().getUserName();
            CmdSetNameOnServer(playerName);
            GameMechMulti.SendUsername sendUsername = new GameMechMulti.SendUsername()
            {
                username = playerName
            };
            NetworkClient.Send(sendUsername);
        }
        nameDisplay.text = playerName;
        
    }
   
    private void UpdateUsername(string oldValue,string newValue)
    {
        nameDisplay.text = newValue;

    }
    







    [Command]
    private void CmdSetCanStartGameOnServer(bool can)
    {
        canStartGame = can;
    }
   
    public void configureCanvas()
    {
        target.damagePointer = GameObject.FindGameObjectWithTag("damagePointer");
    }
    private void respawn()
    {
        Destroy(playerMotion.cylinder.gameObject);
        Destroy(playerGun.thirdPersonCam.gameObject);
        CmdRespawn(gameObject, playerName);

    }
    private void revive(GameObject g)
    {
        PlayerMultiDetails multiDetails = g.GetComponent<PlayerMultiDetails>();
        Destroy(multiDetails.playerMotion.cylinder.gameObject);
        Destroy(multiDetails.playerGun.thirdPersonCam.gameObject);
        CmdRespawn(g, multiDetails.playerName);
    }
    [Command]
    public void CmdStartGame(NetworkConnectionToClient conn = null)
    {
        if (canStartGame && !gameMechMulti.survivalMechMulti.gameStarted)
        {
            gameMechMulti.survivalMechMulti.gameStarted = true;
            
        }
        TargetRemoveStartGameButton(conn);

    }
    [TargetRpc]
    public void TargetRemoveStartGameButton(NetworkConnection conn)
    {
        screenObjects.startButton.SetActive(false);
    }
    [Command]
    public void CmdRespawn(GameObject prevPlayer,string name, NetworkConnectionToClient conn = null)
    {
        if (gameMechMulti == null)
        {
            gameMechMulti = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMechMulti>();
        }
        gameMechMulti.respawnPlayer(conn, prevPlayer,name);
    }


    [Command]
    public void CmdRestartGame()
    {
        if (gameMechMulti == null)
        {
            gameMechMulti = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMechMulti>();
        }
        foreach(GameObject g in gameMechMulti.survivalMechMulti.spawnedEnemies)
        {
            Destroy(g);
        }
        
        gameMechMulti.survivalMechMulti.playerscores.Clear();
        gameMechMulti.survivalMechMulti.spawnedEnemies.Clear();
        gameMechMulti.survivalMechMulti.hasSpawned = false;
        gameMechMulti.survivalMechMulti.gameStarted = false;
        gameMechMulti.survivalMechMulti.waveNo = 1;

        RpcRespawn();
    }
    [ClientRpc]
    public void RpcRespawn()
    {
        respawn();
        screenObjects.survivalScoresPanel.SetActive(false);
        if (gameMechMulti == null)
        {
            gameMechMulti = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMechMulti>();
        }
        if (GameMechMulti.isHost && !gameMechMulti.survivalMechMulti.gameStarted)
        {
            screenObjects.startButton.SetActive(true);
        }
    }
    
    private void Update()
    {
        if(isMultiPlayer && !isLocalPlayer)
        {
            nameCanvas.LookAt(Camera.main.transform);
            return;
        }
        if (!isTyping)
        {
            if (Input.GetKeyDown("k"))
            {
                if (target.isDead)
                {
                    if (GameMechMulti.isHost && !gameMechMulti.survivalMechMulti.gameStarted)
                    {
                        CmdRestartGame();
                    }

                }
                else
                {
                    if (Input.GetKeyDown("k"))
                    {
                        CmdStartGame();
                    }

                }

            }
            if (Input.GetKeyDown("t"))
            {
                OpenMessenger();
            }
        }
        
    }
}
public class ChatMessage : MessageBase
{
    public string theMessage;
    public string name;
}
public class GameMessage : MessageBase
{
    public string theMessage;
}
