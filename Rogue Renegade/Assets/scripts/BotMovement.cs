using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotMovement : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    /// <summary>
    /// Use this to determine if the bot has arrived or not.
    /// </summary>
    public bool isMoving;
    private float moveSpeed = 0;
    private Vector3 movingRifleWeaponPose = new Vector3(0.27044f, 1.1234f, 0.133f);
    private Vector3 stationaryRifleWeaponPose = new Vector3(0.265f, 1.222f, 0.122f);

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        MoveTo(new Vector3(),1);
    }
    private void Update()
    {
        agent.updatePosition = false;
        agent.updateRotation = true;

        agent.nextPosition = transform.position;

        if (isMoving)
        {
            anim.SetFloat("move-speed", moveSpeed, 0.25f, Time.deltaTime);
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                isMoving = false;
            }
        }
        else
        {
            anim.SetFloat("move-speed", 0, 0.25f, Time.deltaTime);
        }
           
    }
   

    /// <summary>
    /// Use isMoving to see if the agent has stopped.
    /// </summary>
    /// <param name="dest">Where to Go</param>
    /// <param name="speed">Set between 0 and 1 to blend between running and walking</param>
    public void MoveTo(Vector3 dest,float speed)
    {
        isMoving = true;
        moveSpeed = speed;
        agent.destination = dest;
    }

}
