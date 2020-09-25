using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 cameraCentre;
    private GameObject player;
    public GameObject castedWall;
    public GameObject prevCastedWall;




    /*Great camera positions(Add the numbers to the player position to get the camera position)
     * (0,3,-6)
     */

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        //Debug.DrawRay(player.transform.position, (player.transform.position - transform.position) * -1, Color.black);
        /*if (Physics.Raycast(player.transform.position, (player.transform.position - transform.position) * -1, out RaycastHit hit))
        {
            
            if (hit.collider.CompareTag("wall"))
            {
                castedWall = hit.collider.transform.parent.gameObject;
                if(prevCastedWall == null)
                {
                    prevCastedWall = castedWall;
                }
                else
                {
                    if (prevCastedWall != castedWall)
                    {
                        bool done;
                        foreach (Wall w in prevCastedWall.GetComponentsInChildren<Wall>())
                        {
                            done = w.IncreaseOpacity();
                            if (done)
                            {
                                prevCastedWall = null;
                            }
                        }
                    }
                }
                foreach (Wall w in hit.collider.transform.parent.GetComponentsInChildren<Wall>())
                {
                    w.ReduceOpacity();
                }
            }
            else
            {
                if (castedWall != null)
                {
                    bool done;
                    foreach (Wall w in castedWall.GetComponentsInChildren<Wall>())
                    {
                        done = w.IncreaseOpacity();
                        if (done)
                        {
                            castedWall = null;
                            prevCastedWall = null;
                        }
                    }
                }
            }
           
        }
        else
        {
            
            if (castedWall != null)
            {
                bool done;
                foreach (Wall w in castedWall.GetComponentsInChildren<Wall>())
                {
                    done = w.IncreaseOpacity();
                    if (done)
                    {
                        castedWall = null;
                        prevCastedWall = null;
                    }
                }
            }
        }
        if (Physics.Raycast(transform.position, transform.forward,out RaycastHit collider))
        {

        }*/

    }
   

}
