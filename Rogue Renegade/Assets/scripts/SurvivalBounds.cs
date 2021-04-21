using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalBounds : MonoBehaviour
{
    public static bool playerIsInBounds = true;
    private Transform player;
    private PlayerMotion playerMotion;
    public GameObject Rocket;
    private ParticleSystem system;
    private float t = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!playerIsInBounds)
        {
            t += 1 * Time.deltaTime;
            if(t >= 1)
            {
                LauchMissiles();
                t = 0;
            }
        }
    }
    private void LauchMissiles()
    {
        GameObject go;
        go = Instantiate(Rocket, player.position + new Vector3(0, 10, 0), Rocket.transform.rotation);
        go.GetComponent<Rocket>().isHoming = true;
        go.GetComponent<Collider>().enabled = true;
        go.GetComponent<Rocket>().isLaunched = true;
        go.GetComponent<Rocket>().homingTarget = player;
        system = go.transform.GetChild(0).GetComponent<ParticleSystem>();
        system.Play();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInBounds = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInBounds = false;
            Debug.Log("HEYYY!!!!");
            playerMotion = other.GetComponent<PlayerMotion>();
            player = other.transform;
            
        }
    }
}
