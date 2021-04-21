using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using Mirror;

public class HealthBar : NetworkBehaviour {

    private Target target;
    [Header("PLEASE LEAVE THE HEALTHBAR SPACE EMPTY")]
    public GameObject healthBar;
    public float reductionSpeed = 0.5f;
    private GameObject healthBarBackground;
    private float ratio;
    //private int noOfExceptions = 0;
    public Animator glowAnim;
    private GameMech gameMech;
    private ColorAdjustments colorAdjustments;

    private void Start()
    {
        target = GetComponent<Target>();
        if(gameObject.tag == "Player")
        {
            healthBar = GameObject.FindGameObjectWithTag("healthBar");
            //glowAnim = healthBar.transform.GetChild(0).gameObject.GetComponent<Animator>();
            gameMech = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMech>();
            ColorAdjustments co;
            if (gameMech.volume.profile.TryGet(out co))
            {
                colorAdjustments = co;
            }
        }
        //healthBarBackground = healthBar.transform.parent.GetChild(0).gameObject;



    }
    public void healthbarGlow()
    {
        //glowAnim.SetTrigger("glow");
    }
    private void FixedUpdate()
    {

        ratio = (target.health / target.healthFull);
        if (gameObject.tag == "Player")
        {
            if (colorAdjustments != null)
            {
                colorAdjustments.saturation.value = (100 - (ratio * 100)) * -1;
            }
            else
            {
                ColorAdjustments co;
                if (gameMech.volume.profile.TryGet(out co))
                {
                    colorAdjustments = co;
                }
            }
        }
        /*try
        {
            healthBar.transform.localScale = new Vector3(ratio, 1, 1);
            if(healthBarBackground.transform.localScale.x > ratio)
            {
                float n = healthBarBackground.transform.localScale.x - reductionSpeed * Time.deltaTime;
                healthBarBackground.transform.localScale = new Vector3(n, 1, 1);
            }
            else
            {
                
                healthBarBackground.transform.localScale = new Vector3(ratio, 1, 1);
            }
        }
        catch(Exception e) {
            //e.ToString();
            noOfExceptions += 1;
            if(noOfExceptions > 1 && e!= null)
            {
                //Debug.LogError(e);
            }
        }*/


    }
}
