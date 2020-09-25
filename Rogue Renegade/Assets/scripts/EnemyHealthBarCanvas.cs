using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBarCanvas : MonoBehaviour
{
    private HealthBar healthBar;
    void Start()
    {
        gameObject.transform.parent.transform.parent.transform.parent.GetComponent<HealthBar>().healthBar = gameObject;

    }

    
    
}
