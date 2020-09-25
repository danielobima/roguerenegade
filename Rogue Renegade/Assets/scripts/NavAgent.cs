using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    public delegate void NavCallBack();
    private NavCallBack collisionCallBack;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

   
    public void NavToTransform(Transform destination)
    {
        agent.enabled = true;
        agent.destination = destination.position;
        agent.isStopped = false;
    }
    

    public void navToThenStop(Vector3 destination, NavCallBack callBack)
    {
       
        if (!agent.enabled)
        {
            agent.enabled = true;
            agent.isStopped = false;
            agent.destination = destination;
            collisionCallBack = callBack;
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
        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
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
