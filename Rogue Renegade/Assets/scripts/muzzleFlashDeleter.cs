using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class muzzleFlashDeleter : MonoBehaviour {

    private void FixedUpdate()
    {
        float t=0;
        t += 1 * Time.deltaTime;
        if (t >= 2.9)
        {
            ParticleSystem muzzle;
            muzzle = GetComponent<ParticleSystem>();
            muzzle.Stop();
        }
        Destroy(gameObject, 3);
    }
}
