using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{

    public float explosionRadius;
    public float explosionForce;
    public float upforce;
    public float damage;
    public float speed = 50;
    public GameObject explostionParticleSystem;
    private Rigidbody rb;
    public bool isLaunched;
    public Transform homingTarget;
    public bool isHoming;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isLaunched)
        {
            if (GetComponent<Rigidbody>())
            {
                if (!isHoming)
                {
                    rb.AddForce(transform.forward * -1 * speed * Time.deltaTime);
                }
                else
                {
                    Vector3 direction = transform.position - homingTarget.position;
                    transform.rotation = Quaternion.LookRotation(direction);
                    rb.AddForce(transform.forward * -1 * speed * Time.deltaTime);
                }
            }
            else
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.useGravity = false;
            }
            
        }
    }
    public void Explode()
    {

        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach (Collider c in colliders)
        {
            if (!c.CompareTag("DroppedGun") && !c.CompareTag("pickup") && c != gameObject.GetComponent<Collider>())
            {
                Rigidbody rb = c.GetComponent<Rigidbody>();
                Target t = c.GetComponent<Target>();
                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, upforce, ForceMode.Impulse);
                }
                if (t != null)
                {
                    t.TakeDamage(damage * (1 / Vector3.Distance(t.transform.position, transform.position)));
                    if (t.health <= 0)
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
    private void OnCollisionEnter(Collision collision)
    {
        if (isLaunched)
        {
            Explode();
        }
    }
}
