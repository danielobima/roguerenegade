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

    [Client]
    private void Start()
    {
        target = GetComponent<Target>();
        playerGun = GetComponent<PlayerGun>();
        gameMechMulti = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMechMulti>();

        playerGun.bullet = gameMechMulti.spawnPrefabs.Find(prefab => prefab.name == "bullet");
        playerGun.shotGunBullet = gameMechMulti.spawnPrefabs.Find(prefab => prefab.name == "shotgun bullet");

        configureCanvas();
    }
    [Client]
    public void configureCanvas()
    {
        target.damagePointer = GameObject.FindGameObjectWithTag("damagePointer");
    }
}
