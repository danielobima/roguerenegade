using System.Collections;
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
        reverbZone.room = defaultRoomSetting;
    }
}
