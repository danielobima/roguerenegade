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
    public Dictionary<uint, Target> playerTargets;
    public Dictionary<int, GameObject> guns;
    public Dictionary<int, GameObject> networkedGuns;

    public GunDetails[] gunDetailss;

    
   

    
    
    private void Start()
    {
       
        gameMech = GetComponent<GameMech>();
        playerDictionary = new Dictionary<uint, GameObject>();
        playerTargets = new Dictionary<uint, Target>();
        guns = new Dictionary<int, GameObject>();
        networkedGuns = new Dictionary<int, GameObject>();
        foreach(GameObject g in spawnPrefabs)
        {
            GunDetails gunDetails = GetComponent<GunDetails>(); 
            if(gunDetails != null)
            {
                networkedGuns.Add(gunDetails.gunInt, g);
            }
        }
        foreach (GunDetails g in gunDetailss)
        {
            guns.Add(g.gunInt, g.gameObject);
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
        playerDictionary.Add(playerMotion.netIdentity.netId, player);
        playerTargets.Add(playerMotion.netIdentity.netId, player.GetComponent<Target>());


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
