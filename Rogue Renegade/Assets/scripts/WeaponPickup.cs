﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject prefabToEquip;
    [HideInInspector]
    public FresnelHighlight highlight;

    private void Start()
    {
        highlight = GetComponent<FresnelHighlight>();
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerGun playerGun = other.gameObject.GetComponent<PlayerGun>();
        if (playerGun)
        {
            playerGun.gunDrop = prefabToEquip;
            if (playerGun.possibleGunPickUp)
            {
                playerGun.possibleGunPickUp.highlight.defresnate();
            }
            playerGun.possibleGunPickUp = this;
            highlight.fresnate();
        }
       
    }
    private void OnTriggerExit(Collider other)
    {
        PlayerGun playerGun = other.gameObject.GetComponent<PlayerGun>();
        if (playerGun)
        {
            if(playerGun.possibleGunPickUp == this)
            {
                playerGun.gunDrop = null;
                playerGun.possibleGunPickUp = null;
            }
            highlight.defresnate();
        }
        
    }
}