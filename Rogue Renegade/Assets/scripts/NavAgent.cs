using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgent : MonoBehaviour
{
    public NavMeshAgent agent;
    public delegate void NavCallBack();
    public NavCallBack callback;
    private Animator animator;
    private NavCallBack collisionCallBack;
    Rigidbody r;
    private bool isMoving;
    private Vector3 navToTransformPos;
    private Vector3 dest;
    private bool canCheckForStoppage;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        r = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }


    private void FixedUpdate()
    {
        if (isMoving)
        {
            animator.SetInteger("state", 1);
        }
        else
        {
            animator.SetInteger("state", 0);
        }
        if (canCheckForStoppage)
        {
            checkForStoppage();
        }
    }
    public void NavToTransform(Transform destination)
    {
        
        if(agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if (agent.isOnNavMesh && destination != null)
        {
            agent.destination = destination.position;
            navToTransformPos = destination.position;
            agent.isStopped = false;
            if (!r.isKinematic)
            {
                r.isKinematic = true;
            }
        }
        isMoving = true;
       
    }
    public void justNavMan(Vector3 destination)
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if (agent.isStopped)
        {
            agent.isStopped = false;

        }
        agent.destination = destination;
        isMoving = true;
        canCheckForStoppage = false;
        if (!r.isKinematic)
        {
            r.isKinematic = true;
        }
    }
    

    public void navToThenStop(Vector3 destination, NavCallBack callBack)
    {
        
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if (agent.isStopped)
        {
            agent.isStopped = false;
            
        }
        agent.destination = destination;
        dest = destination;
        //collisionCallBack = callBack;
        if (!r.isKinematic)
        {
            r.isKinematic = true;
        }
        isMoving = true;
        canCheckForStoppage = true;
        callback = callBack;
       
        
       
    }
    int i = 0;
    private void checkForStoppage()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            StopNav();
            callback();
            i++;

            canCheckForStoppage = false;
        }
        else
        {
            agent.destination = dest;
            //collisionCallBack = callBack;
            if (!r.isKinematic)
            {
                r.isKinematic = true;
            }
            isMoving = true;
        }
    }
    public void StopNav()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        agent.isStopped = true;
        isMoving = false;
        if (r == null)
        {
            r = GetComponent<Rigidbody>();
        }
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
                isMoving = false;
            }
        }
    }
}
