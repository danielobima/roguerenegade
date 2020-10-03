using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class RagdollSwitch : NetworkBehaviour
{
    public bool isPlayer = false;
    public Collider mainBody;
    public Collider gun;
    public Collider SecondaryGun;
    public Collider[] colliders;
    private NavMeshAgent navMeshAgent;
    public Animator animator;
    private bool hasDoneRigidBody = false;
    private bool hasDoneSRigidBody = false;
    //private bool isJoking = false;
    private bool hasInitedBodyParts = false;

    [Header("Only for players")]
    public PlayerMotion playerMotion;

    private NetworkAnimator networkAnimator;
    
    

    private void Start()
    {
       
        navMeshAgent = GetComponent<NavMeshAgent>();
        networkAnimator = GetComponent<NetworkAnimator>();
        SwitchRagdoll(false);
       
        
    }
    
    public void SwitchRagdoll(bool value)
    {

        if (networkAnimator != null)
        {
            networkAnimator.enabled = !value;
        }
        animator.enabled = !value;
        foreach (Collider c in colliders)
        {
            /*if (!isPlayer)
            {
                c.enabled = value;
            }*/
            if (!hasInitedBodyParts)
            {
                BodyPart b = c.gameObject.AddComponent<BodyPart>();
                Bloody B = c.gameObject.AddComponent<Bloody>();
                b.mainBody = gameObject.GetComponent<Target>();
                b.isPlayer = isPlayer;
                if (isPlayer)
                {
                    b.playerMotion = gameObject.GetComponent<PlayerMotion>();
                    
                }
                hasInitedBodyParts = true;
            }
            Rigidbody r = c.gameObject.GetComponent<Rigidbody>();
            if (r != null)
            {
                r.useGravity = value;
                r.freezeRotation = !value;
            }
        }
        if (!playerMotion.playerMultiDetails.isMultiPlayer)
        {
            if (gun != null)
            {
                if (value)
                {
                    if (!gun.GetComponent<Rigidbody>() && !hasDoneRigidBody)
                    {
                        gun.gameObject.AddComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                        hasDoneRigidBody = true;
                    }
                    else
                    {
                        if (gun.GetComponent<Rigidbody>())
                        {
                            gun.GetComponent<Rigidbody>().useGravity = true;
                        }

                    }

                }
                gun.enabled = value;
            }
            if (SecondaryGun != null)
            {
                if (value)
                {
                    if (!SecondaryGun.GetComponent<Rigidbody>() && !hasDoneSRigidBody)
                    {
                        SecondaryGun.gameObject.AddComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                        hasDoneSRigidBody = true;
                    }
                    else
                    {
                        if (SecondaryGun.GetComponent<Rigidbody>())
                        {
                            SecondaryGun.GetComponent<Rigidbody>().useGravity = true;
                        }

                    }

                }
                SecondaryGun.enabled = value;
            }
        }
        mainBody.enabled = !value;
        if (value)
        {
            Rigidbody u = mainBody.gameObject.GetComponent<Rigidbody>();
            if (u != null)
            {
                Destroy(u);
            }
        }

        if (value)
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = !value;
            }
        }
       
            
        
        
    }
    public bool ragdollJokes(bool value)
    {
        animator.enabled = !value;
        foreach (Collider c in colliders)
        {
            /*if (!isPlayer)
            {
                c.enabled = value;
            }*/
            
            Rigidbody r = c.gameObject.GetComponent<Rigidbody>();
            if (r != null)
            {
                r.useGravity = value;
                r.freezeRotation = !value;
            }
        }
        
        return !value;
    }
    public void getGuns()
    {
        try
        {
            gun = GameObject.FindGameObjectWithTag("Playergun").GetComponent<Collider>();
        }
        catch(Exception e)
        {
            //Gun not found
            e.ToString();
        }
        try
        {
            SecondaryGun = GameObject.FindGameObjectWithTag("SecondaryGun").GetComponent<Collider>();
        }
        catch (Exception e)
        {
            //Gun not found
            e.ToString();
        }
        
    }

}
