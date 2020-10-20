using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMultiDetails : NetworkBehaviour
{
    public bool isMultiPlayer = false;
    private Target target;
    private PlayerGun playerGun;
    public GameMechMulti gameMechMulti;
    public string playerName = "No Name";
    private PlayerMotion playerMotion;

   
    private void Start()
    {
        target = GetComponent<Target>();
        playerGun = GetComponent<PlayerGun>();
        playerMotion = GetComponent<PlayerMotion>();
        gameMechMulti = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMechMulti>();
        if (!isMultiPlayer)
        {
            gameObject.SetActive(true);
        }
        else
        {
            playerGun.bullet = gameMechMulti.spawnPrefabs.Find(prefab => prefab.name == "bullet");
            playerGun.shotGunBullet = gameMechMulti.spawnPrefabs.Find(prefab => prefab.name == "shotgun bullet");
        }
       
        

        

        configureCanvas();
        
    }
    public void configureCanvas()
    {
        target.damagePointer = GameObject.FindGameObjectWithTag("damagePointer");
    }
    private void respawn()
    {
        Destroy(playerMotion.cylinder.gameObject);
        Destroy(playerGun.thirdPersonCam.gameObject);
        Respawn(gameObject);

    }
    [Command]
    public void Respawn(GameObject prevPlayer, NetworkConnectionToClient conn = null)
    {
        gameMechMulti = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMechMulti>();
        gameMechMulti.respawnPlayer(conn, prevPlayer);
    }
    private void Update()
    {
        if (target.isDead)
        {
            if (Input.GetKeyDown("p"))
            {
                respawn();
            }
        }
    }
}
