using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Reflection;

public class CarryAble : NetworkBehaviour
{

    [SyncVar]
    public bool isBeingCarried;
    [SyncVar]
    public bool hasSlerped = false;
    [SyncVar]
    public bool isPlacing = false;
    public Transform carryPos;
    private Collider mCollider;
    public Vector3 placePos;
    private float yExtents;
    private Rigidbody r;
    private bool changed = true;
    private float mg;
    public Vector3 extents;
    public Material transparentFrensel;
    public GameObject targetPosObj;
    public float forwardMultiplier = 5;
    public PlayerMotion carrier;
    
    

    private void Start()
    {
        mCollider = GetComponent<Collider>();
        yExtents = mCollider.bounds.extents.y;
        r = GetComponent<Rigidbody>();
        mg = mCollider.bounds.extents.magnitude;
        extents = mCollider.bounds.extents;
    }

    public void Slerp()
    {
        ToggleCollider(false);
        if(targetPosObj == null)
        {
            MeshRenderer m = GetComponent<MeshRenderer>();
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            targetPosObj = new GameObject(gameObject.name + " targetPos");
            targetPosObj.transform.position = transform.position;
            targetPosObj.AddComponent<MeshFilter>(meshFilter);
            targetPosObj.AddComponent<MeshRenderer>(m).material = transparentFrensel;
        }
        Vector3 targetpos = new Vector3(carryPos.position.x, carryPos.position.y + yExtents, carryPos.position.z);
        transform.position = Vector3.Lerp(transform.position,targetpos , 10 * Time.deltaTime);
        if(Vector3.Distance(transform.position, targetpos) <= 0.5f)
        {
            Debug.Log("Holla Compadre");
            hasSlerped = true;
            placePos = transform.position;
        }
    }
    private void ToggleCollider(bool on)
    {
        if (on)
        {
            if (!changed)
            {
                mCollider.enabled = true;
                r.useGravity = true;
                changed = true;
            }
        }
        else
        {
            if (changed)
            {
                mCollider.enabled = false;
                r.useGravity = false;
                changed = false;
                
            }
        }
    }
    public void SlerpP()
    {
        if(targetPosObj != null)
        {
            Destroy(targetPosObj);
        }
        transform.position = Vector3.Lerp(transform.position, placePos, 10 * Time.deltaTime);
        if (Vector3.Distance(transform.position, placePos) <= 0.25f)
        {
            ToggleCollider(true);
            isPlacing = false;
            
        }
        
    }
    private void Update()
    {
        if (isBeingCarried )
        {
            
            if(carryPos != null)
            {
                if (hasSlerped)
                {
                    transform.position = new Vector3(carryPos.position.x, carryPos.position.y + yExtents, carryPos.position.z);

                }
                else
                {
                    Slerp();
                }


                transform.rotation = carryPos.rotation;
            }
            if (!isPlacing)
            {
                
                if (targetPosObj != null)
                {
                    targetPosObj.transform.position = Vector3.Lerp(targetPosObj.transform.position, placePos,20*Time.deltaTime) ;
                    targetPosObj.transform.rotation = transform.rotation;
                }

            }
            
        }
        else
        {
            if (isPlacing)
            {
                SlerpP();
            }
            else
            {
                placePos = transform.position;
            }
           
        }
    }
    

}
public static class ComponentCopyMech
{
    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }
    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }

}

