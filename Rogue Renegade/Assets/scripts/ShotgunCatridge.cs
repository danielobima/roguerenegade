using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunCatridge : MonoBehaviour
{
    public uint shooterId = default;
    private void FixedUpdate()
    {

        Destroy(gameObject, 1);

    }
}
