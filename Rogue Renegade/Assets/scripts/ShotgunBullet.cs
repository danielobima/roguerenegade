using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunBullet : MonoBehaviour
{
    public float speed = 50;
    private Rigidbody r;
    public float damage = 1;
    public GameObject bloodParicleSystem;

    private void Start()
    {
        r = GetComponent<Rigidbody>();
        r.velocity = transform.forward * Time.deltaTime * speed;
    }
    private void FixedUpdate()
    {

        Destroy(gameObject, 1);

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Target>())
        {
            Target t;
            t = collision.collider.GetComponent<Target>();
            t.TakeDamage(damage);
            t.damagePoint(collision.GetContact(0).point - collision.collider.transform.position);
            if (collision.collider.GetComponent<Bloody>())
            {
                Instantiate(bloodParicleSystem, collision.GetContact(0).point, bloodParicleSystem.transform.rotation);
            }
        }
        Destroy(gameObject);
        // Debug.Log(collision.collider.name);

    }
}
