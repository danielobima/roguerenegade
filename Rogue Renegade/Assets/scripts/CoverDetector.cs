using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverDetector : MonoBehaviour
{
    public PlayerMotion playerMotion;

    
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CoverAble>())
        {
            playerMotion.possibleCoverAble = other.GetComponent<CoverAble>();
            //float angle = Vector3.Angle(other.ClosestPointOnBounds(transform.position), Vector3.forward);
            //Debug.Log(angle);
            //if (Mathf.Approximately(angle, 0))
            //{
            //    playerMotion.coverDirection = other.transform.forward * -1;
            //}
            //if (Mathf.Approximately(angle, 180))
            //{
            //  playerMotion.coverDirection = other.transform.forward ;
            //}
            //if (Mathf.Approximately(angle, 90))
            //{
            //    Vector3 cross = Vector.Cross(Vector3.forward, hit.normal);
            //    if (cross.y > 0) { }// Right
            //    else { } // left
            //}
        }
    }
    


    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CoverAble>())
        {
            playerMotion.possibleCoverAble = null;
        }
    }*/
}
