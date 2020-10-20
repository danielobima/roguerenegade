using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientToServerCommunicator : NetworkBehaviour
{
    private GameMechMulti gameMechMulti;
    private Target target;
    private void Start()
    {
        target = GetComponent<Target>();
    }
   
}
