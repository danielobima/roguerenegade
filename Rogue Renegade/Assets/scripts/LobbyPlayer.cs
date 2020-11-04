using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class LobbyPlayer : NetworkBehaviour
{
    [SyncVar]
    public bool readyToPlay = false;
    [SyncVar(hook = nameof(UpdateUsername))]
    public string playerName;
    public TextMeshProUGUI nameDisplay;
    public Image readySymbol;
    public Image hostSymbol;

    [SyncVar]
    public bool isLeader = false;

    private LobbyManager lobbyManager;
    private GameMechMulti gameMechMulti;


    private void Start()
    {
        gameMechMulti = GameObject.FindGameObjectWithTag("GameMechMulti").GetComponent<GameMechMulti>();
        transform.SetParent(gameMechMulti.lobbyListView);

      
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        lobbyManager = GameObject.FindGameObjectWithTag("GameMech").GetComponent<LobbyManager>();
        if (isLocalPlayer)
        {
            playerName = new PlayerDetails().getUserName();
            CmdSetNameOnServer(playerName);
            if (isLeader)
            {
                PleaseTellEmWhoseBoss(netId);
            }
            
        }
        else
        {
            TellMeWhoseBoss();
        }
        nameDisplay.text = playerName;

        
        
    }
    [Command]
    private void PleaseTellEmWhoseBoss(uint netID)
    {
        TellEmWhoseBoss(netID);
    }
    private void TellMeWhoseBoss()
    {

        if (isLeader)
        {
            hostSymbol.gameObject.SetActive(true);
        }
        if (readyToPlay)
        {
            readySymbol.gameObject.SetActive(true);
        }
    }
    [ClientRpc]
    private void TellEmWhoseBoss(uint netID)
    {
        if(netID == netId)
        {
            hostSymbol.gameObject.SetActive(true);
        }
    }
    private void UpdateUsername(string oldValue, string newValue)
    {
        nameDisplay.text = newValue;
       
    }
    [Command]
    private void CmdSetNameOnServer(string theName)
    {
        playerName = theName;
    }

    [Command]
    public void CmdReadyUp(uint NetId)
    {
        readyToPlay = true;
        RpcShowImReady(NetId);
    }

    [ClientRpc]
    private void RpcShowImReady(uint NetId)
    {
        if(netId == NetId)
        {
            readySymbol.gameObject.SetActive(true);
        }
    }
    [Command]
    private void CmdStartGame()
    {
        bool allReady = true;
        foreach(LobbyPlayer player in lobbyManager.lobbyPlayers)
        {
            if (!player.readyToPlay)
            {
                allReady = false;
                return;
            }
        }
        if (allReady)
        {
            if(gameMechMulti == null)
            {
                gameMechMulti = GameObject.FindGameObjectWithTag("GameMechMulti").GetComponent<GameMechMulti>();
            }
            gameMechMulti.MoveToGameScene();
        }
    }

    
    private void Update()
    {
        if (!readyToPlay)
        {
            if (Input.GetKeyDown("k"))
            {
                CmdReadyUp(netId);
            }
        }
        else
        {
            if (GameMechMulti.isHost)
            {
                if (Input.GetKeyDown("k"))
                {
                    CmdStartGame();
                }
            }
        }
    }

}
