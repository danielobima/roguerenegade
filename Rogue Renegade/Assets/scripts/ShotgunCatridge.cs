using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunCatridge : MonoBehaviour
{
    private void FixedUpdate()
    {

        Destroy(gameObject, 1);

    }
}
