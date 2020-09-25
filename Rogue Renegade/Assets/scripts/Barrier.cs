using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    private Collider myCollider;
    public int BarrierNo;
    public float maxDistance;

    [Header ("Leave all arrays that you do not need empty")]
    public EnemyMech[] enemies;


    void Start()
    {
        myCollider = GetComponent<Collider>();
    }
    public bool playerCrossed()
    {
        //Gizmos.DrawWireCube(transform.position + transform.forward * maxDistance, transform.localScale);
        if (Physics.BoxCast(myCollider.bounds.center,transform.localScale,transform.forward, out RaycastHit hit, transform.rotation,maxDistance))
        {

            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        else
        {
            return false;
        }
        
    }


    /*Trigger strings are 
     * Enemy off
     * Enemy on
     * Mission Completed
    */
    public void triggerBarrier(string[] triggers)
    {
       foreach (string trigger in triggers)
        {
            switch (trigger)
            {
                case "Enemy on":
                    foreach(EnemyMech e in enemies)
                    {
                        e.timeToAttack = true;
                    }
                    break;
            }
        }
    }
    

}
