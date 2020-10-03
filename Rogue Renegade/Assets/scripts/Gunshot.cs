using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Gunshot : NetworkBehaviour {

    public float speed = 50;
    private Rigidbody r;
    public float damage;
    public GameObject bloodParicleSystem;
    private float t;
    private GameMechMulti gameMechMulti;
    

    private void Start()
    {
        r = GetComponent<Rigidbody>();
        
    }
    //use this to ricoche as well
    private bool canDestroy(Collider collider)
    {
        if (collider.GetComponent<Target>())
        {
            Target t;
            t = collider.GetComponent<Target>();
            if (t.isPlayer)
            {
                return t.isLocalPlayer;

            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }
    private void FixedUpdate()
    {
        r.velocity = transform.forward * Time.deltaTime * speed;
        t += 1 * Time.deltaTime * Time.timeScale;
        if (t >= 1)
        {
            Destroy(gameObject);
            NetworkServer.Destroy(gameObject);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (NetworkServer.active)
        {
            BulletCollision(collision.GetContact(0).point, collision.collider, damage, bloodParicleSystem);
        }
        else
        {
            BulletCollision2(collision.GetContact(0).point, collision.collider, damage, bloodParicleSystem);
        }

        if (canDestroy(collision.collider))
        {
            Destroy(gameObject);
            NetworkServer.Destroy(gameObject);
        }
        // Debug.Log(collision.collider.name);
       
    }
    [Server]
    public static void BulletCollision(Vector3 collisionPoint, Collider collider,float damage,GameObject bloodParticleSystem)
    {
        if (collider.GetComponent<Target>())
        {
            Target t;
            t = collider.GetComponent<Target>();

            if (t.isPlayer)
            {
                if (t.p.playerMultiDetails.isMultiPlayer)
                {
                    t = t.p.playerMultiDetails.gameMechMulti.playerTargets[t.netIdentity.netId];
                    t.TakeDamage(damage);
                    t.damagePoint(collisionPoint - collider.transform.position);
                    if (collider.GetComponent<Bloody>())
                    {
                        GameObject g = Instantiate(bloodParticleSystem, collisionPoint, bloodParticleSystem.transform.rotation);
                        NetworkServer.Spawn(g);
                    }

                }
                else
                {
                    t.TakeDamage(damage);
                    t.damagePoint(collisionPoint - collider.transform.position);
                    if (collider.GetComponent<Bloody>())
                    {
                        Instantiate(bloodParticleSystem, collisionPoint, bloodParticleSystem.transform.rotation);
                    }
                }
            }
            else
            {
                
                t.TakeDamage(damage);
                t.damagePoint(collisionPoint - collider.transform.position);
                if (collider.GetComponent<Bloody>())
                {
                    Instantiate(bloodParticleSystem, collisionPoint, bloodParticleSystem.transform.rotation);
                }
            }
            
            
        }
        if (collider.GetComponent<BodyPart>())
        {
            BodyPart b = collider.GetComponent<BodyPart>();
            if (collider.GetComponent<Bloody>())
            {
                Instantiate(bloodParticleSystem, collisionPoint, bloodParticleSystem.transform.rotation);
            }
            b.mainBody.damagePoint(collisionPoint - collider.transform.position);
            
            if (b.isHead && !b.mainBody.isPlayer)
            {
                b.mainBody.health = 0;
            }
            else
            {
                b.mainBody.TakeDamage(damage);
            }
            
        }
        if (collider.GetComponent<Explosive>())
        {
            collider.GetComponent<Explosive>().Explode();
        }
    }

    public static void BulletCollision2(Vector3 collisionPoint, Collider collider, float damage, GameObject bloodParticleSystem)
    {
        if (collider.GetComponent<Target>())
        {
            Target t;
            t = collider.GetComponent<Target>();

            if (t.isPlayer)
            {
                if (t.p.playerMultiDetails.isMultiPlayer)
                {
                    t = t.p.playerMultiDetails.gameMechMulti.playerTargets[t.netIdentity.netId];
                    t.TakeDamage(damage);
                    t.damagePoint(collisionPoint - collider.transform.position);
                    if (collider.GetComponent<Bloody>())
                    {
                        GameObject g = Instantiate(bloodParticleSystem, collisionPoint, bloodParticleSystem.transform.rotation);
                        NetworkServer.Spawn(g);
                    }

                }
                else
                {
                    t.TakeDamage(damage);
                    t.damagePoint(collisionPoint - collider.transform.position);
                    if (collider.GetComponent<Bloody>())
                    {
                        Instantiate(bloodParticleSystem, collisionPoint, bloodParticleSystem.transform.rotation);
                    }
                }
            }
            else
            {

                t.TakeDamage(damage);
                t.damagePoint(collisionPoint - collider.transform.position);
                if (collider.GetComponent<Bloody>())
                {
                    Instantiate(bloodParticleSystem, collisionPoint, bloodParticleSystem.transform.rotation);
                }
            }


        }
        if (collider.GetComponent<BodyPart>())
        {
            BodyPart b = collider.GetComponent<BodyPart>();
            if (collider.GetComponent<Bloody>())
            {
                Instantiate(bloodParticleSystem, collisionPoint, bloodParticleSystem.transform.rotation);
            }
            b.mainBody.damagePoint(collisionPoint - collider.transform.position);

            if (b.isHead && !b.mainBody.isPlayer)
            {
                b.mainBody.health = 0;
            }
            else
            {
                b.mainBody.TakeDamage(damage);
            }

        }
        if (collider.GetComponent<Explosive>())
        {
            collider.GetComponent<Explosive>().Explode();
        }
    }



}
