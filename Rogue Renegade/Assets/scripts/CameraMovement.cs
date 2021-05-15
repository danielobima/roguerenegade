using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [HideInInspector]
    public Transform AimTarget;
    Ray ray;
    RaycastHit hitInfo;

    void Start()
    {
        AimTarget = transform.GetChild(0);
    }

    private void Update()
    {
        ray.origin = transform.position;
        ray.direction = transform.forward;
        if (Physics.Raycast(ray, out hitInfo))
        {
            AimTarget.position = hitInfo.point;
        }
        else
        {
            AimTarget.localPosition = new Vector3(0, 0, 20);
        }
           
        
        
    }
   

}
