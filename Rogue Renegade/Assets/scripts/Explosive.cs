using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{

    public float explosionRadius;
    public float explosionForce;
    public float upforce;
    public float damage;
    public float explostionDelay = 3;
    public GameObject explostionParticleSystem;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    void FixedUpdate()
    {
        //Invoke("Explode", 2);
    }
    public void Explode()
    {

        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach(Collider c in colliders)
        {
            if(!c.CompareTag("DroppedGun") && !c.CompareTag("pickup") && c != gameObject.GetComponent<Collider>())
            {
                Rigidbody rb = c.GetComponent<Rigidbody>();
                Target t = c.GetComponent<Target>();
                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, upforce, ForceMode.Impulse);
                }
                if(t != null)
                {
                    t.TakeDamage(damage * (1 / Vector3.Distance(t.transform.position, transform.position)));
                    if(t.health <= 0)
                    {
                        Rigidbody pelvis = c.transform.GetChild(0).GetChild(0).GetComponent<Rigidbody>();
                        pelvis.AddExplosionForce(explosionForce, explosionPos, explosionRadius, upforce, ForceMode.Impulse);
                    }
                }
            }
        }
        Instantiate(explostionParticleSystem, transform.position, explostionParticleSystem.transform.rotation);
        Destroy(gameObject);
    }
    public void invokeExplosion()
    {
        Invoke("Explode", explostionDelay);
    }
}
