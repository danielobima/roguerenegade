using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ParticleSystemLight : MonoBehaviour
{
    private ParticleSystem system;
    private Light me;

    private void Start()
    {
        system = GetComponentInParent<ParticleSystem>();
        me = GetComponent<Light>();
       
    }
    private void Update()
    {
        if (system.isPlaying)
        {
            me.enabled = true;
        }
        else
        {
            me.enabled = false;
        }
    }
}
