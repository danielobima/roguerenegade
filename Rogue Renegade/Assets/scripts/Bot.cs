using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    BotRig botRig;
    BotWeapon botWeapon;
    BotMovement botMovement;
    void Start()
    {
        botRig = GetComponent<BotRig>();
        botWeapon = GetComponent<BotWeapon>();
        botMovement = GetComponent<BotMovement>();

        botRig.SetAim(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
