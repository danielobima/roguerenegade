
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GunManager : NetworkBehaviour
{
    [SyncVar(hook = nameof(deleteGunDrop))]
    public int DeleteGun;


    
   

    
    public Dictionary<int, GunDetails> gundrops;

    public GameMechMulti gameMechMulti;



    private void Start()
    {
        gundrops = new Dictionary<int, GunDetails>();
        
    }

    private void deleteGunDrop(int oldValue, int newValue)
    {
        Destroy(gundrops[newValue].gameObject);
    }
}
