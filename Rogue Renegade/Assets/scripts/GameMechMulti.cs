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

    private void Start()
    {
        gameMech = GetComponent<GameMech>();

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
        playerMotion.cylinder = aimCylinder.transform;
        CinemachineFreeLook tp = tpp.GetComponent<CinemachineFreeLook>();
        playerGun.thirdPersonCam = tp;
        tp.m_Follow = player.transform;
        tp.m_LookAt = player.transform;

        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
