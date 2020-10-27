using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    public delegate void NavCallBack();
    private NavCallBack collisionCallBack;
    Rigidbody r;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        r = GetComponent<Rigidbody>();
    }

   
    public void NavToTransform(Transform destination)
    {
        
        if(agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if (agent.isOnNavMesh)
        {
            agent.destination = destination.position;
            agent.isStopped = false;
            if (!r.isKinematic)
            {
                r.isKinematic = true;
            }
        }
    }
    

    public void navToThenStop(Vector3 destination, NavCallBack callBack)
    {
       
        if (agent.isStopped)
        {
            agent.isStopped = false;
            agent.destination = destination;
            collisionCallBack = callBack;
            if (!r.isKinematic)
            {
                r.isKinematic = true;
            }
        }
        
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    StopNav();
                    callBack();
                    
                }
            }
        }
    }
    public void StopNav()
    {
       
        agent.isStopped = true;
        if (!r.isKinematic)
        {
            r.isKinematic = false;
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<InvisibleWall>())
        {
            if (agent.enabled)
            {
                collisionCallBack();
            }
        }
    }
}
