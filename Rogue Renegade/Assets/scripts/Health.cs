﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float amountOfHealth;
    public float speed;
    public GameObject pSystem;
    public bool isMultiplayer;

    
    private void OnCollisionEnter(Collision collision)
    {
        if (!isMultiplayer)
        {
            if (collision.collider.CompareTag("Player"))
            {
                collision.collider.GetComponent<Target>().addHealth(amountOfHealth);
                collision.collider.GetComponent<FresnelEffect>().fresnate(3);
                GameObject system;
                system = Instantiate(pSystem, collision.collider.transform.position, pSystem.transform.rotation, collision.collider.transform);
                system.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                Destroy(system, 3);
                Destroy(gameObject);
            }
        }
        else
        {
            Collided(collision);
        }
    }
    private void Collided(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
           
            collision.collider.GetComponent<Target>().addHealth(amountOfHealth);
            
            
            Destroy(gameObject);
        }
    }
    
  
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * speed * Time.deltaTime);
    }
}
