using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyVision : MonoBehaviour
{
    public delegate void VisionCallBack();
    public VisionCallBack visionCallBack;
    public GameObject whatISaw;
    public Vector3 raycastStart;
    private bool colliding;
    public GameObject parent;
    private Dictionary<Collider, Coroutine> coroutines;
    /// <summary>
    /// this one is for OnTriggerStay
    /// </summary>
    public bool canCheck = false;
    /// <summary>
    /// this one is for OnTriggerEnter
    /// </summary>
    public bool isChecking = false;
    public bool stillSeeing = false;
    private bool startedCountdown = false;
    public int countdown = 3;
    public GameObject enemyAlert;
    private AlertSign enemyAlertSign;
    public SVGImage alertImage;


    private void Start()
    {
        coroutines = new Dictionary<Collider, Coroutine>();
        enemyAlertSign = enemyAlert.GetComponent<AlertSign>();
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (isChecking)
        {
            if (other.CompareTag("Player") || other.GetComponent<TeammateMech>())
            {
                colliding = true;
                coroutines.Add(other, StartCoroutine(visionCheck(other)));
            }
        }
        
    }
    

    IEnumerator visionCheck(Collider other)
    {
        while (colliding)
        {
            if (Physics.Raycast(new Ray(transform.position, other.transform.position - transform.position), out RaycastHit hit
                   , Vector3.Distance(transform.position, other.transform.position)))
            {
                if (hit.collider.gameObject.layer == 19 || hit.collider.gameObject.layer == 9 || hit.collider.gameObject.layer == 13)
                {
                    switch (hit.collider.gameObject.layer)
                    {
                        case 19:
                            BodyPart bo = hit.collider.gameObject.GetComponent<BodyPart>();
                            if(bo != null)
                            {
                                if (bo.mainBody.GetComponent<TeammateMech>() || bo.mainBody.isPlayer)
                                {
                                    if (!startedCountdown)
                                    {
                                        StartCoroutine(callBackCountdown(other,countdown));
                                        startedCountdown = true;
                                    }
                                    
                                }
                                
                            }
                            break;
                        case 9:
                            if (hit.collider.GetComponent<TeammateMech>())
                            {
                                if (!startedCountdown)
                                {
                                    StartCoroutine(callBackCountdown(other, countdown));
                                    startedCountdown = true;
                                }
                                
                            }
                            
                            break;
                        case 13:
                            if (!startedCountdown)
                            {
                                StartCoroutine(callBackCountdown(other, countdown));
                                startedCountdown = true;
                            }
                            
                            break;

                    }
                }
               
                
            }
           
            yield return new WaitForSeconds(0.2f);
        }
    }
    private void FixedUpdate()
    {
        
        if (!isChecking)
        {
            colliding = false;
        }
    }
    IEnumerator callBackCountdown(Collider other,float countDown, float numberOfIterationsPerSecond = 10)
    {
        Debug.Log("Wait");
        enemyAlert.SetActive(true);
        enemyAlert.GetComponent<AlertSign>().isShowing = true;
        int misses = 0;
        enemyAlertSign.isAdding = true;
        float i = 0;
        while (i < countDown)
        {
            if (Physics.Raycast(new Ray(transform.position, other.transform.position - transform.position), out RaycastHit hit
                   , Vector3.Distance(transform.position, other.transform.position)))
            {
                if (hit.collider.gameObject.layer == 19 || hit.collider.gameObject.layer == 9 || hit.collider.gameObject.layer == 13)
                {
                    switch (hit.collider.gameObject.layer)
                    {
                        case 19:
                            BodyPart bo = hit.collider.gameObject.GetComponent<BodyPart>();
                            if (bo != null)
                            {
                                if (bo.mainBody.GetComponent<TeammateMech>() || bo.mainBody.isPlayer)
                                {
                                    if (bo.mainBody.gameObject == other.gameObject)
                                    {
                                        //Debug.Log("yeet");
                                    }
                                    else
                                    {
                                        misses++;
                                        Debug.Log("not yeet");
                                    }

                                }
                                else
                                {
                                    misses++;
                                    Debug.Log("not yeet");
                                }

                            }
                            else
                            {
                                misses++;
                                Debug.Log("not yeet");
                            }
                            break;
                        case 9:
                            if (hit.collider.GetComponent<TeammateMech>())
                            {
                                if (hit.collider.gameObject == other.gameObject)
                                {
                                    //Debug.Log("yeet");
                                }
                                else
                                {
                                    misses++;
                                    Debug.Log("not yeet");
                                }

                            }
                            else
                            {
                                misses++;
                                Debug.Log("not yeet");
                            }
                            break;
                        case 13:
                            //Debug.Log("yeet");
                            break;


                    }
                    
                }
                else
                {
                    misses++;
                    Debug.Log("not yeet");
                }

            }
            else
            {
                misses++;
                Debug.Log("not yeet");
            }
            if(misses >= 3)
            {
                break;
            }
            float add = 1 /  numberOfIterationsPerSecond;
            yield return new WaitForSeconds(add);
            i += add;
        }
       
        
        
        if(misses < 3)
        {
            whatISaw = other.gameObject;
            visionCallBack?.Invoke();
            enemyAlertSign.pulse();
            Debug.Log("Boom");
            StartCoroutine(turnOffAlertSign());
        }
        else
        {
            enemyAlertSign.isSubtracting = true;
            enemyAlertSign.isAdding = false;
            Debug.Log("not boom");
        }
        startedCountdown = false;
        
    }
    IEnumerator turnOffAlertSign(int wait = 7)
    {
        yield return new WaitForSeconds(wait);
        enemyAlert.SetActive(false);
        enemyAlert.GetComponent<AlertSign>().isShowing = false;
        enemyAlertSign.stopPulse();
        enemyAlertSign.img.color = new Color(1, 1, 1);
    }
    private void OnTriggerStay(Collider other)
    {
        if (canCheck)
        {
            stillSeeing = false;
            if (Physics.Raycast(new Ray(transform.position, other.transform.position - transform.position), out RaycastHit hit
                   , Vector3.Distance(transform.position, other.transform.position)))
            {
                if (hit.collider.gameObject.layer == 19 || hit.collider.gameObject.layer == 9 || hit.collider.gameObject.layer == 13)
                {
                    
                    switch (hit.collider.gameObject.layer)
                    {
                        case 19:
                            BodyPart bo = hit.collider.gameObject.GetComponent<BodyPart>();
                            if (bo != null)
                            {
                                if (bo.mainBody.GetComponent<TeammateMech>() || bo.mainBody.isPlayer)
                                {
                                    if (bo.mainBody.gameObject == whatISaw.gameObject)
                                    {
                                        stillSeeing = true;
                                    }
                                    
                                }
                            }
                            break;
                        case 9:
                            if (hit.collider.GetComponent<TeammateMech>())
                            {
                                if (hit.collider.gameObject == whatISaw.gameObject)
                                {
                                    stillSeeing = true;
                                }
                                
                            }
                            break;
                        case 13:
                            stillSeeing = true;
                            break;

                    }
                }
                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
       
        if (other.CompareTag("Player") || other.GetComponent<TeammateMech>())
        {
            /*if(waitingToSee == other.gameObject)
            {
                colliding = false;
                waitingToSee = null;
            }*/
            
            if (coroutines.ContainsKey(other))
            {
                StopCoroutine(coroutines[other]);
                coroutines.Remove(other);
            }
        }
    }
}
