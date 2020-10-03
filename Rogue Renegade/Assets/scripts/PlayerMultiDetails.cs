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

   
    private void Start()
    {
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
        target = GetComponent<Target>();
        playerGun = GetComponent<PlayerGun>();
        

        

        configureCanvas();
        
    }
    public void configureCanvas()
    {
        target.damagePointer = GameObject.FindGameObjectWithTag("damagePointer");
    }
}
