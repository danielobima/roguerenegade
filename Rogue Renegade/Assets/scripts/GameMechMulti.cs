using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class GameMechMulti : NetworkManager 
{
    private GameMech gameMech;
    public GameObject mainCamera;
    public GameObject uiCamera;
    public GameObject aimCylinder;
    public GameObject Canvas;
    public GameObject tpp;
    public Dictionary<uint, GameObject> playerDictionary;

    
    private void Start()
    {
        gameMech = GetComponent<GameMech>();
        playerDictionary = new Dictionary<uint, GameObject>();
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

    }
}
