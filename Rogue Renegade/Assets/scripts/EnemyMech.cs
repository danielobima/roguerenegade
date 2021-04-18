using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class EnemyMech : NetworkBehaviour {
    
    /// <summary>
    /// Not this object's transfrom, please remember always.
    /// </summary>
    public Transform enemyTransform;//I said enemy over here because as the player you are the enemy's enemy
    public bool timeToAttack = false;
    public int attackType = 0;
    // 0 is weapon
    // 1 is melee
    //public GameObject myCanvas;
    public EnemyGun enemyGun;
    public Animator animator;
    public bool isBeingPunched = false;
    private bool isMeleeAttacking = false;
    private bool isPunching = false;
    public float punchRate = 0;
    public float pp = 2;
    public bool canAttack = false;
    public float deathTime = 4;
    public float meleeDamage = 0;
    public float shootSeconds = 0.5f;
    private bool isOnfloor = false;
    private bool isSeekingCover = false;

    
    private float a = 0;
    private NavAgent navAgent;
    private bool isSearchingForPlayer = false;
    public float distanceToStartAttack = 5;
    public static float shootTime = 5;
    private float b = 0;
    private Target t;
    private GunDetails gunDetails;
    public Target enemyTarget;
    private bool hasReachedPlayer;
    public Transform myMiddleSpine;
    private float weight;
    private int prevPunch;
    private EnemyMultiDetails enemyMultiDetails;
    private GameMechMulti gameMechMulti;
    private GameMech gameMech;
    public GameObject virtualMiddleSpine;
    public bool ShouldSeePlayer;
    private GameObject handContainer;


    private float maxUpRecoil = -5;
    private float minLeftRightRecoil = 0;
    private float maxLeftRightRecoil = 5;
    private float minUpRecoil = 0;
    private float currentUpRecoil;
    private float currentLeftRightRecoil;
    private float recoilX;
    private float recoilY;
    private float recoilXRef;
    private float recoilYRef;

    public EnemyVision vision;
    public EnemyManager enemyManager;
    public bool searchingAround = false;

    private bool canCheckIfReachedDestination;



    private void Start()
    {
        
        if(enemyGun != null)
        {
            gunDetails = enemyGun.GetComponent<GunDetails>();
            gunDetails.gunSound.loop = false;
        }
        navAgent = GetComponent<NavAgent>();
        t = GetComponent<Target>();
        
        enemyMultiDetails = GetComponent<EnemyMultiDetails>();
        gameMech = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMech>();
        if (enemyMultiDetails.isMultiPlayer)
        {
            gameMechMulti = GameObject.FindGameObjectWithTag("GameMechMulti").GetComponent<GameMechMulti>();
            setPlayerInstanceM();
        }
        else
        {
            setEnemyInstance();
        }

        setVirtualMiddleSpine();
        setRecoil();
        if (ShouldSeePlayer)
        {
            vision.isChecking = true;
            vision.raycastStart = transform.position;
            vision.parent = gameObject;
            vision.visionCallBack = OnSeeEnemy;
            
            
        }
        
    }
    
    private void OnSeeEnemy()
    {
        if(enemyManager != null)
        {
            enemyTarget = vision.whatISaw.GetComponent<Target>();
            enemyTransform = vision.whatISaw.transform;
            distanceToStartAttack = Vector3.Distance(transform.position, enemyTransform.position);
            enemyManager.enemyTarget = enemyTarget;
            enemyManager.EnemySpotted();
        }
        //Debug.Log("Ive seen you :). by: " + gameObject.name + ". Sent from my IPhone.");
    }
    private void setVirtualMiddleSpine()
    {
        virtualMiddleSpine.transform.SetParent(myMiddleSpine.parent);
        virtualMiddleSpine.transform.localPosition =
        new Vector3(2.035827e-16f, 0.00865172f, 2.793968e-09f);
        virtualMiddleSpine.transform.localEulerAngles = new Vector3();
        myMiddleSpine.SetParent(virtualMiddleSpine.transform);

        handContainer = Instantiate(new GameObject("handContainer"), myMiddleSpine);
        handContainer.transform.localPosition = new Vector3();
        handContainer.transform.localEulerAngles = new Vector3();
        myMiddleSpine.GetChild(2).SetParent(handContainer.transform);
        myMiddleSpine.GetChild(1).SetParent(handContainer.transform);
    }

    private void FixedUpdate()
    {

        if (enemyMultiDetails.isMultiPlayer)
        {
            LogicM();
        }
        else
        {
            Logic();
        }
        
    }
    public void Logic()
    {
        if (!t.isDead)
        {
            if (timeToAttack)
            {
                if (!isSeekingCover )
                {
                    checkDistanceFromPlayer();
                }
                if (canAttack)
                {
                    Attack(attackType);
                }
                else
                {
                    if (!ShouldSeePlayer)
                    {
                        searchForPlayer();
                    }
                    animator.SetInteger("upperbody", 0);
                }
            }
            else
            {
                idleStance();
            }

        }
        if (enemyTarget != null)
        {
            if (enemyTarget.isDead)
            {
                if (enemyMultiDetails.isMultiPlayer)
                {
                    if (!setPlayerInstanceM())
                    {
                        timeToAttack = false;
                    }
                }
                else
                {
                    if (!setEnemyInstance())
                    {
                        timeToAttack = false;
                    }
                }
            }
        }
        else
        {
            if (timeToAttack)
            {
                if (enemyMultiDetails.isMultiPlayer)
                {
                    if (!setPlayerInstanceM())
                    {
                        timeToAttack = false;
                    }
                }
                else
                {
                    if (!setEnemyInstance())
                    {
                        timeToAttack = false;
                    }
                }
            }
        }

        //myCanvas.transform.LookAt(Camera.main.transform);

        if (Physics.Raycast(transform.position, transform.up * -1, out RaycastHit hit))
        {
            if (hit.distance >= 1.2f)
            {
                isOnfloor = false;
            }
            else
            {
                isOnfloor = true;
            }

        }
        if (canCheckIfReachedDestination)
        {
            CheckIfReachedDestination();
        }
    }
    [Server]
    public void LogicM()
    {
        Logic();
    }
    private void checkDistanceFromPlayer()
    {
        if (CheckCloseToTag("Player", distanceToStartAttack))
        {
            if (CheckCloseToTag("Player", distanceToStartAttack - 1))
            {
                if (isSearchingForPlayer)
                {
                    canAttack = true;
                    isSearchingForPlayer = false;
                    if (!searchingAround)
                    {
                        navAgent.StopNav();
                    }
                    
                }
            }
        }
        else
        {
            if (!isSearchingForPlayer && !ShouldSeePlayer)
            {
                searchForPlayer();
                isSearchingForPlayer = true;
                canAttack = false;
            }
        }
    }
    private void idleStance()
    {
        if (isOnfloor)
        {
            navAgent.StopNav();
        }
        if (attackType == 1)
        {
            isMeleeAttacking = false;
        }

        animator.SetInteger("upperbody", 0);
    }
    private bool CheckCloseToTag(string tag, float minimumDistance)
    {
        /*GameObject[] goWithTag = GameObject.FindGameObjectsWithTag(tag);

        for (int i = 0; i < goWithTag.Length; ++i)
        {
            if (Vector3.Distance(transform.position, goWithTag[i].transform.position) <= minimumDistance)
                return true;
        }

        return false;*/
        if (Vector3.Distance(transform.position, enemyTransform.position) <= minimumDistance)
            return true;
        return false;
    }

    public void Die()
    {
        if (!t.isDead)
        {
            //Debug.Log("Am ded");
            Rigidbody r = GetComponent<Rigidbody>();
            r.isKinematic = false;
            canAttack = false;
           
            if(enemyGun != null)
            {
                if (enemyMultiDetails.isMultiPlayer)
                {
                    int gunInt = gunDetails.gunInt;
                    Vector3 pos = enemyGun.transform.position;
                    Quaternion rot = enemyGun.transform.rotation;
                    if (isServer)
                    {
                        RpcDestroyGun();
                        gameMech.enemyDeathCallBack(t);
                        GameObject g = Instantiate(gameMechMulti.networkedGuns[gunInt], pos, rot);
                        NetworkServer.Spawn(g);
                    }
                    
                    
                }
                else
                {
                    enemyGun.transform.parent = null;
                    enemyGun.tag = "DroppedGun";
                    //gameMech.enemyDeathCallBack(t);
                }
            }

            Destroy(gameObject, 10);
            
            
        }
        
    }

    [ClientRpc]
    private void RpcDestroyGun()
    {
        Destroy(enemyGun.gameObject);
    }
    private void middleSpineLook(Vector3 target)
    {
        Vector3 lMTargetDir = new Vector3(target.x, target.y, target.z) - virtualMiddleSpine.transform.position;
        Quaternion mTargetRotation = Quaternion.LookRotation(lMTargetDir);
        virtualMiddleSpine.transform.rotation = Quaternion.Slerp(virtualMiddleSpine.transform.rotation, mTargetRotation, 10 * Time.deltaTime);
    }
    private void middleSpineLook(Transform target)
    {
        middleSpineLook(target.position);
    }
    private void fullBodyLook(Vector3 target)
    {
        Vector3 lTargetDir = new Vector3(target.x, target.y, target.z) - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(lTargetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
    }
    private void fullBodyLook(Transform target)
    {
        fullBodyLook(target.position);
    }
    private void RecoilUp()
    {
        recoilX = Mathf.SmoothDampAngle(handContainer.transform.localEulerAngles.x, currentUpRecoil, ref recoilXRef, 0.1f);
        recoilY = Mathf.SmoothDampAngle(handContainer.transform.localEulerAngles.y, currentLeftRightRecoil, ref recoilYRef, 0.1f);
    }
    private void RecoilDown()
    {
        recoilX = Mathf.SmoothDampAngle(handContainer.transform.localEulerAngles.x, 0, ref recoilXRef, 0.1f);
        recoilY = Mathf.SmoothDampAngle(handContainer.transform.localEulerAngles.y, 0, ref recoilYRef, 0.1f);
    }
    private void setRecoil()
    {
        currentUpRecoil = Random.Range(maxUpRecoil, minUpRecoil);
        int i = Random.Range(0, 2);
        bool left = i < 1 ? true : false;
        if (left)
        {
            currentLeftRightRecoil = Random.Range(minLeftRightRecoil, maxLeftRightRecoil);
        }
        else
        {
            currentLeftRightRecoil = Random.Range(-maxLeftRightRecoil, -minLeftRightRecoil);
        }
       
    }
    private void WeaponAttack()
    {
        
        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("easy") || animator.GetCurrentAnimatorStateInfo(0).IsName("medium")
        //    || animator.GetCurrentAnimatorStateInfo(0).IsName("hard"))
        if(!isSeekingCover && enemyTarget != null)
        {
            //transform.LookAt(new Vector3(playerTransform.position.x,transform.position.y,playerTransform.position.z));
            fullBodyLook(enemyTransform);
            middleSpineLook(enemyTransform);
            if(enemyManager != null)
            {
                enemyManager.lastKnownPos = enemyTransform.position;
            }

            a += 1 * Time.deltaTime; //The one that goes like taptaptap
            b += 1 * Time.deltaTime; //How long the taptaptap is
            if (b <= shootTime)
            {
                animator.SetInteger("upperbody", 4);
                enemyGun.transform.rotation = Quaternion.FromToRotation(Vector3.forward * -1, myMiddleSpine.transform.forward);
                if (a <= shootSeconds && !gunDetails.gunSound.isPlaying)
                {
                    enemyGun.Shoot(enemyMultiDetails.isMultiPlayer);
                    RecoilUp();
                }
                else
                {
                    if (a >= shootSeconds + 1)
                    {
                        setRecoil();
                        a = 0;
                        
                    }
                    RecoilDown();
                }
                handContainer.transform.localEulerAngles = new Vector3(recoilX, recoilY);
            }
            else
            {
                seekCover();
                animator.SetInteger("upperbody", 0);
            }
        }
        else
        {
            if (isSeekingCover)
            {
                seekCover();
                animator.SetInteger("upperbody", 0);
            }
            
        }
       
        
    }
    
    private bool CanSeeTarget()
    {
        if (enemyTransform != null)
        {
            if (Physics.Raycast(new Ray(transform.position, enemyTransform.position - transform.position), out RaycastHit hit
                , Vector3.Distance(transform.position, enemyTransform.position)))
            {
                if (hit.collider.gameObject.layer == 19 || hit.collider.gameObject.layer == 9 || hit.collider.gameObject.layer == 13)
                {
                    switch (hit.collider.gameObject.layer)
                    {
                        case 19:
                            BodyPart bo = hit.collider.gameObject.GetComponent<BodyPart>();
                            if (bo != null)
                                if (bo.mainBody.GetComponent<TeammateMech>() || bo.mainBody.isPlayer)
                                {
                                    return true;
                                }
                            break;
                        case 9:
                            if (hit.collider.GetComponent<TeammateMech>())
                            {
                                return true;
                            }
                            break;
                        case 13:
                            return true;

                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }
        else
            return false;
    }
    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        else
        {
            Debug.Log("NONONONO");
        }
        return finalPosition;
    }
    private void seekCover()
    {
        isSeekingCover = true;
        NavAgent.NavCallBack callBack = resumeShooting;
        navAgent.navToThenStop(RandomNavmeshLocation(10), callBack);
    }
    private void resumeShooting()
    {
        
        b = 0;
        if (CanSeeTarget())
        {
            distanceToStartAttack = Vector3.Distance(transform.position, enemyTransform.position);
        }
        else
        {
            if (!ShouldSeePlayer)
            {
                searchForPlayer();
            }
            else
            {
                searchAroundForPlayer();
            }
        }
        isSeekingCover = false;

    }
    private void Attack(int AttackType)
    {
        switch (attackType)
        {
            case 0:
                WeaponAttack();
                break;
            case 1:
                MeleeAttack();
                break;
        } 
    }
    IEnumerator frozenAnim()
    {
        yield return new WaitForSeconds(.5f);
        if (isPunching)
        {
            endPunch();
        }
    }
    public bool fightingMode(bool on)
    {
        if (on)
        {
            weight = 1;
            animator.SetLayerWeight(1, weight);
            return true;
        }
        else
        {
            if (weight > 0)
            {
                weight -= 3 * Time.deltaTime;
            }
            animator.SetLayerWeight(1, weight);
            return false;
        }
    }
    public void MeleeAttack()
    {
        if (!hasReachedPlayer)
        {
            
            transform.LookAt(new Vector3(enemyTransform.position.x, transform.position.y, enemyTransform.position.z));
            isMeleeAttacking = true;
            if (!isPunching)
            {
                fightingMode(true);
                animator.SetInteger("punch", 1);
                transform.Translate(Vector3.forward * Time.deltaTime * 2.5f);
            }
            
        }

    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (isMeleeAttacking)
            {
                isPunching = true;
                hasReachedPlayer = true;
                prevPunch = Random.Range(2, 7);
                animator.SetInteger("punch", prevPunch);
                
            }
        }
       
    }
    private void OnCollisionStay(Collision collision)
    {
        
    }
    public void endPunch()
    {
        animator.SetInteger("punch", 0);
       
        if (hasReachedPlayer)
        {
            int newPunch = Random.Range(2, 7);
            if (newPunch != prevPunch)
            {
                animator.SetInteger("punch", newPunch);
            }
            else
            {
                if(newPunch <= 6 && newPunch > 2)
                {
                    newPunch -= 1;
                    animator.SetInteger("punch", newPunch);
                }
                else
                {
                    newPunch += 1;
                    animator.SetInteger("punch", newPunch);
                }
            }
        }
        else
        {
            isPunching = false;
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (isMeleeAttacking)
            {
                
                hasReachedPlayer = false;
            }
        }
        

    }
    /// <summary>
    /// Used if enemy knows where the player is
    /// </summary>
    public void searchForPlayer()
    {
        if (isOnfloor)
        {
            navAgent.NavToTransform(enemyTransform);
            //animator.SetInteger("state", 1);
            canAttack = false;
            isSearchingForPlayer = true;
            fightingMode(false);
        }
    }
    /// <summary>
    /// Used if enemy does not know where the player is
    /// </summary>
    private void searchAroundForPlayer()
    {
        Debug.Log("Aaa");
        if (isOnfloor)
        {
            canAttack = false;
            vision.canCheck = true;
            searchingAround = true;
            if (!navAgent.agent.isStopped)
            {
                navAgent.StopNav();
                Debug.Log("HAHAHA YOU THOUGHT I WAS STOPPED!");
            }
            Debug.Log("Going to " + enemyManager.lastKnownPos);
            //NavAgent.NavCallBack callBack = searchingAroundCallBack;
            //navAgent.navToThenStop(enemyManager.lastKnownPos, callBack);
            navAgent.justNavMan(enemyManager.lastKnownPos);
            canCheckIfReachedDestination = true;

            
        }
    }
    private void CheckIfReachedDestination()
    {
        if(navAgent.agent.remainingDistance <= navAgent.agent.stoppingDistance)
        {
            searchingAroundCallBack();
            navAgent.StopNav();
            Debug.Log("VICTORY!!!!!");
            canCheckIfReachedDestination = false;
        }
    }


    private void searchingAroundCallBack()
    {
        Debug.Log("bbb");
        b = 0;
        vision.canCheck = false;
        if (vision.stillSeeing)
        {
            canAttack = true;
        }
        else
        {
            enemyManager.SearchingAroundForEnemy(this);
        }
        searchingAround = false;
    }


    
    public bool setEnemyInstance()
    {
        if (!enemyMultiDetails.isMultiPlayer)
        {
            int i = Random.Range(0, 2);
            bool getTeammate = i < 1 ? true : false;
            Target playerTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Target>(); 
            if (!getTeammate)
            {
                enemyTransform = playerTarget.transform;
                enemyTarget = playerTarget;
                if (enemyTarget != null && enemyTransform != null)
                {
                    return !enemyTarget.isDead;
                }
                return false;
            }
            else
            {
                if (playerTarget.playerTeammates.Count > 0)
                {
                    int u = Random.Range(0, playerTarget.playerTeammates.Count);
                    enemyTransform = playerTarget.playerTeammates[u].transform;
                    enemyTarget = playerTarget.playerTeammates[u].GetComponent<Target>();
                    if (enemyTarget != null && enemyTransform != null)
                    {
                        return !enemyTarget.isDead;
                    }
                    return false;
                }
                else
                {
                    enemyTransform = playerTarget.transform;
                    enemyTarget = playerTarget;
                    if (enemyTarget != null && enemyTransform != null)
                    {
                        return !enemyTarget.isDead;
                    }
                    return false;
                }
            }
            
        }
        else
        {
            int i = Random.Range(0, gameMechMulti.playerTargets.Count );
            List<uint> ids = new List<uint>(gameMechMulti.playerTargets.Keys);
            enemyTransform = gameMechMulti.playerTargets[ids[i]].transform;
            enemyTarget = gameMechMulti.playerTargets[ids[i]];
            if (enemyTarget != null && enemyTransform != null)
            {
                return !enemyTarget.isDead;
            }
            else
            {
                return false;
            }
        }
    }
    [ServerCallback]
    public bool setPlayerInstanceM()
    {
        return setEnemyInstance();
    }




    public void stopPunchImpact()
    {
        isBeingPunched = false;
    }
    public void punch()
    {


        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 0.5f))
        {
            if (hit.collider.GetComponent<Target>())
            {
                Target t;
                t = hit.collider.GetComponent<Target>();
                t.TakeDamage(meleeDamage);
                t.TakePunchForce(transform, 1000);
            }
            if (hit.collider.CompareTag("enemy"))
            {
                EnemyMech em = hit.collider.GetComponent<EnemyMech>();
                em.isBeingPunched = true;
                Transform mepos = em.transform;
                em.transform.LookAt(new Vector3(transform.position.x, mepos.position.y, transform.position.z));
                Animator a = hit.collider.GetComponent<Animator>();
                a.SetTrigger("impact");
            }
            if (hit.collider.CompareTag("Player"))
            {
                PlayerMotion pm = hit.collider.GetComponent<PlayerMotion>();
                pm.isBeingPunched = true;
                Transform mepos = pm.transform;
                pm.transform.LookAt(new Vector3(transform.position.x, mepos.position.y, transform.position.z));
                Animator a = hit.collider.GetComponent<Animator>();
                a.SetTrigger("impact");
            }

        }
        StartCoroutine(frozenAnim());

    }
    public void setAnimatorWeight(float weight)
    {
        animator.SetLayerWeight(1, weight);
    }

}

