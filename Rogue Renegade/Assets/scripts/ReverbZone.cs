﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverbZone : MonoBehaviour
{
    AudioReverbZone reverbZone;
    int defaultRoomSetting;

    private void Start()
    {
        reverbZone = GetComponent<AudioReverbZone>();
        defaultRoomSetting = reverbZone.room;
    }
    void Update()
    {
        if (PlayerMotion.isSlowMo)
        {
            reverbZone.room = defaultRoomSetting + 2000;
        }
        else
        {
            reverbZone.room = defaultRoomSetting;
        }
    }
}
