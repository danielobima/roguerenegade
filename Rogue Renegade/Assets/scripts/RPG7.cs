using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG7 : MonoBehaviour
{
    public Rocket rocket;
    public GameObject rocketAsset;
    private ParticleSystem system;
    public bool loaded = true;
    public void Launch()
    {
        rocket.transform.SetParent(null);
        rocket.isLaunched = true;
        rocket.GetComponent<Collider>().enabled = true;
        system = rocket.transform.GetChild(0).GetComponent<ParticleSystem>();
        system.Play();
        loaded = false;
        Destroy(rocket.gameObject, 5);
    }
    public void LoadRocket()
    {
        
        rocket = Instantiate(rocketAsset).GetComponent<Rocket>();
        rocket.transform.SetParent(transform);
        system = rocket.transform.GetChild(0).GetComponent<ParticleSystem>();
        rocket.transform.localPosition = new Vector3(0, 0.002953624f, 0);
        rocket.transform.localEulerAngles = new Vector3(0, 0, 0);
        rocket.transform.localScale = new Vector3(1, 1, 1);
        loaded = true;
    }
}
