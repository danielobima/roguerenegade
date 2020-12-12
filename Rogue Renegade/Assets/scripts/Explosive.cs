using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Explosive : NetworkBehaviour
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
    [Server]
    public void Explode()
    {

        Vector3 explosionPos = transform.position;
        Instantiate(explostionParticleSystem, explosionPos, explostionParticleSystem.transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach(Collider c in colliders)
        {
            if(!c.CompareTag("DroppedGun") && !c.CompareTag("pickup") && c != gameObject.GetComponent<Collider>())
            {
                Rigidbody rb = c.GetComponent<Rigidbody>();
                Target t = c.GetComponent<Target>();
                if (rb != null && !c.CompareTag("Player"))
                {
                    rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, upforce, ForceMode.Impulse);
                }
                if(t != null )
                {
                    t.TakeDamage(damage * (1 / Vector3.Distance(t.transform.position, transform.position)));
                    if(t.health <= 0 && !c.CompareTag("Player"))
                    {
                        Rigidbody pelvis = c.transform.GetChild(0).GetChild(0).GetComponent<Rigidbody>();
                        pelvis.AddExplosionForce(explosionForce/20, explosionPos, explosionRadius, upforce, ForceMode.Impulse);
                    }
                }
            }
        }
        RpcSpawnExplosion(explosionPos);
        NetworkServer.Destroy(gameObject);
    }

    [ClientRpc]
    private void RpcSpawnExplosion(Vector3 pos)
    {
        Instantiate(explostionParticleSystem, pos, explostionParticleSystem.transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(pos, explosionRadius);
        foreach (Collider c in colliders)
        {
            if (!c.CompareTag("DroppedGun") && !c.CompareTag("pickup") && c != gameObject.GetComponent<Collider>() && c.CompareTag("Player"))
            {
                Rigidbody rb = c.GetComponent<Rigidbody>();
                Target t = c.GetComponent<Target>();
                if (rb != null )
                {
                    rb.AddExplosionForce(explosionForce, pos, explosionRadius, upforce, ForceMode.Impulse);
                }
                if (t != null)
                {
                    if (t.health <= 0)
                    {
                        Rigidbody pelvis = c.transform.GetChild(0).GetChild(0).GetComponent<Rigidbody>();
                        pelvis.AddExplosionForce(explosionForce, pos, explosionRadius, upforce, ForceMode.Impulse);
                    }
                }

            }
        }
    }
    
    public void Explode2()
    {

        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach (Collider c in colliders)
        {
            if (!c.CompareTag("DroppedGun") && !c.CompareTag("pickup") && c != gameObject.GetComponent<Collider>() && !c.gameObject.layer.Equals(19))
            {
                Rigidbody rb = c.GetComponent<Rigidbody>();
                Target t = c.GetComponent<Target>();
                
                if (t != null)
                {
                    t.TakeDamage(damage * (1 / Vector3.Distance(t.transform.position, transform.position)));
                    if (t.health <= 0)
                    {
                        Rigidbody pelvis = c.transform.GetChild(0).GetChild(0).GetComponent<Rigidbody>();
                        pelvis.AddExplosionForce(explosionForce, explosionPos, explosionRadius, explosionForce, ForceMode.Impulse);
                        
                    }
                    
                }
                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, explosionForce, ForceMode.Impulse);
                }
            }
        }
        Instantiate(explostionParticleSystem, transform.position, explostionParticleSystem.transform.rotation);
        Destroy(gameObject);
    }
    public void invokeExplosion()
    {
        if (NetworkServer.active)
        {
            Invoke("Explode", explostionDelay);
        }
        else
        {
            Invoke("Explode2", explostionDelay);
        }
    }
}
