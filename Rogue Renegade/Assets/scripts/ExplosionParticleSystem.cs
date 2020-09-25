using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionParticleSystem : MonoBehaviour
{

    private Light mylight;
    private AudioSource explosionSound;
    // Start is called before the first frame update
    void Start()
    {
        mylight = GetComponent<Light>();
        explosionSound = GetComponent<AudioSource>();
        explosionSound.loop = false;
        explosionSound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        Invoke("stopLight", 0.5f);
        Destroy(gameObject, 5);
    }
    private void stopLight()
    {
        mylight.enabled = false;
    }
}
