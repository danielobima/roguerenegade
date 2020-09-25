using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public Target mainBody;
    public PlayerMotion playerMotion;
    public bool isHead = false;
    public bool isPlayer;

    private void Start()
    {
        if (gameObject.name.Equals("head"))
        {
            isHead = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
       
    }
}
