using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    

    
    public List<LobbyPlayer> lobbyPlayers;
    private void Start()
    {
        lobbyPlayers = new List<LobbyPlayer>();
    }

}
