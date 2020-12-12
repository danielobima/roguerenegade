using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class TeammateMech : NetworkBehaviour
{
    private Transform enemyTransform;
    public Transform playerTransform;
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
    private bool canAttack = false;
    public float deathTime = 4;
    public float meleeDamage = 0;
    public float shootSeconds = 0.5f;
    private bool isOnfloor = false;
    private bool isSeekingCover = false;


    private float a = 0;
    private NavAgent navAgent;
    private bool isSearchingForEnemy = false;
    private bool isFollowingPlayer = false;
    public float distanceToStartAttack = 5;
    public static float shootTime = 5;
    private float b = 0;
    private Target t;
    private GunDetails gunDetails;
    private Target enemyTarget;
    private bool hasReachedPlayer;
    public Transform myMiddleSpine;
    private float weight;
    private int prevPunch;
    private EnemyMultiDetails enemyMultiDetails;
    private GameMechMulti gameMechMulti;
    private GameMech gameMech;
    public GameObject virtualMiddleSpine;
    public bool ShouldFollowPlayer = true;
    private Target playerTarget;
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





    private void Start()
    {

        if (enemyGun != null)
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
            setEnemyInstanceM();
        }
        else
        {
            setEnemyInstance();
        }
        setVirtualMiddleSpine();
        setRecoil();


    }
    public void notifyBoss()
    {
        playerTarget = playerTransform.GetComponent<Target>();
        if(playerTarget.playerTeammates == null)
        {
            playerTarget.playerTeammates = new List<TeammateMech>();
        }
        playerTarget.playerTeammates.Add(this);

    }
    private void setVirtualMiddleSpine()
    {
        virtualMiddleSpine.transform.SetParent(myMiddleSpine.parent);
        virtualMiddleSpine.transform.position = myMiddleSpine.position;
        virtualMiddleSpine.transform.rotation = myMiddleSpine.rotation;
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
    private void Logic()
    {
        if (!t.isDead)
        {
            if (timeToAttack)
            {
                if (!isSeekingCover)
                {
                    checkDistanceFromEnemy();
                }
                else
                {
                    //animator.SetInteger("state",1);
                }
                if (canAttack)
                {
                    Attack(attackType);
                }
                else
                {
                    if (!ShouldFollowPlayer)
                    {
                        searchForEnemy();
                        animator.SetInteger("upperbody", 0);
                    }
                }
            }
            else
            {
                followPlayer();
                animator.SetInteger("upperbody", 0);
            }

            if (enemyTarget != null)
            {
                if (enemyTarget.isDead)
                {
                    if (enemyMultiDetails.isMultiPlayer)
                    {
                        if (setEnemyInstanceM())
                            timeToAttack = true;
                        else
                            followPlayer();
                    }
                    else
                    {
                        if (setEnemyInstance())
                            timeToAttack = true;
                        else
                            followPlayer();
                    }
                    enemyTarget = null;
                    enemyTransform = null;
                    canAttack = false;
                    
                }
            }
            else
            {
                if (enemyMultiDetails.isMultiPlayer)
                {
                    if (setEnemyInstanceM())
                        timeToAttack = true;
                }
                else
                {
                    if (setEnemyInstance())
                        timeToAttack = true;
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
    }
    [Server]
    private void LogicM()
    {
        Logic();
    }
    private void checkDistanceFromEnemy()
    {
        if (enemyTransform != null)
        {
            
            if (ShouldFollowPlayer)
            {
                if (CanSeeTarget())
                {
                    canAttack = true;
                }
                followPlayer();
            }
            else
            {
                if (CheckCloseToTransform(distanceToStartAttack))
                {
                    if (CheckCloseToTransform(distanceToStartAttack - 1))
                    {
                        if (isSearchingForEnemy)
                        {
                            canAttack = true;
                            isSearchingForEnemy = false;
                            navAgent.StopNav();
                            //animator.SetInteger("state", 0);

                        }
                    }
                }
                else
                {
                    if (!isSearchingForEnemy && !isFollowingPlayer)
                    {
                        
                        searchForEnemy();
                        isSearchingForEnemy = true;
                        canAttack = false;


                    }
                }
            }
        }
    }
    
    private bool CheckCloseToTransform( float minimumDistance)
    {
        if (Vector3.Distance(transform.position, enemyTransform.position) <= minimumDistance)
            return true;
        else
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

            if (enemyGun != null)
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
            playerTarget.playerTeammates.Remove(this);
            Destroy(gameObject, 10);


        }

    }

    [ClientRpc]
    private void RpcDestroyGun()
    {
        Destroy(enemyGun.gameObject);
    }
    private void WeaponAttack()
    {
        if (ShouldFollowPlayer)
        {
            followPlayerWhileAttacking();
        }
        else
        {
            freeToMoveWeaponAttack();
        }
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
            currentLeftRightRecoil = Random.Range(minLeftRightRecoil,maxLeftRightRecoil );
        }
        else
        {
            currentLeftRightRecoil = Random.Range(-maxLeftRightRecoil, -minLeftRightRecoil);
        }
        
    }
    private void shootLogic()//When b is less than shoot time
    {
        animator.SetInteger("upperbody", 4);
        enemyGun.transform.rotation = Quaternion.FromToRotation(Vector3.forward * -1, myMiddleSpine.transform.forward);
        if (a <= shootSeconds && !gunDetails.gunSound.isPlaying)
        {
            //animator.SetInteger("upperbody", GameMech.Difficulty
            enemyGun.Shoot(enemyMultiDetails.isMultiPlayer);
            RecoilUp();
        }
        else
        {
            RecoilDown();
            if (a >= shootSeconds + 1)
            {
                setRecoil();
                a = 0;

            }
        }
        handContainer.transform.localEulerAngles = new Vector3(recoilX, recoilY);
    }
    private void freeToMoveWeaponAttack()
    {
        if (!isSeekingCover && enemyTarget != null && enemyTransform != null)
        {
            fullBodyLook(enemyTransform);
            middleSpineLook(enemyTransform);

            a += 1 * Time.deltaTime; //The one that goes like taptaptap
            b += 1 * Time.deltaTime; //How long the taptaptap is
            if (b <= shootTime)
            {
                shootLogic();
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
    private void followPlayerWhileAttacking()
    {
        if (enemyTransform != null && enemyTarget != null)
        {
            
            middleSpineLook(enemyTransform);
            a += 1 * Time.deltaTime; //The one that goes like taptaptap
            b += 1 * Time.deltaTime; //How long the taptaptap is
            if (b <= shootTime)
            {
                shootLogic();
            }
            else
            {
                if (!CanSeeTarget())
                {
                    canAttack = false;
                }
            }
        }
        

    }
    private void middleSpineLook(Vector3 target)
    {
        if (!isFollowingPlayer)
        {
            //Vector3 lMTargetDir = new Vector3(target.x, target.y, target.z) - virtualMiddleSpine.transform.position;
            //Quaternion mTargetRotation = Quaternion.LookRotation(lMTargetDir);
            //virtualMiddleSpine.transform.rotation = Quaternion.Slerp(virtualMiddleSpine.transform.rotation, mTargetRotation, 10 * Time.deltaTime);
            virtualMiddleSpine.transform.LookAt(target);
        }
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
    private void resetMiddleSpine()
    {
        virtualMiddleSpine.transform.localRotation = Quaternion.Slerp(virtualMiddleSpine.transform.localRotation, Quaternion.Euler(0,0,0), 10 * Time.deltaTime);
    }
    private bool CanSeeTarget()
    {
        if(enemyTransform != null)
        {
           
            if (Physics.Raycast(new Ray(transform.position, enemyTransform.position - transform.position), out RaycastHit hit
                ,Vector3.Distance(transform.position,enemyTransform.position)))
            {
                if (hit.collider.gameObject.layer ==  19 || hit.collider.gameObject.layer == 9)
                {
                    

                    switch (hit.collider.gameObject.layer)
                    {
                        case 19:
                            BodyPart bo = hit.collider.gameObject.GetComponent<BodyPart>();
                            if (bo != null)
                                if (bo.mainBody.GetComponent<EnemyMech>() )
                                {
                                    if (ShouldFollowPlayer)
                                    {
                                        b = 0;
                                    }
                                    distanceToStartAttack = Vector3.Distance(transform.position, enemyTransform.position);
                                    return true;
                                }
                            break;
                        case 9:
                            if (hit.collider.GetComponent<EnemyMech>())
                            {
                                if (ShouldFollowPlayer)
                                {
                                    b = 0;
                                }
                                distanceToStartAttack = Vector3.Distance(transform.position, enemyTransform.position);
                                return true;
                            }
                            break;
                        

                    }
                    return false;
                }
                else
                {
                    //Debug.Log("No");

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
        if (!CanSeeTarget())
        {
            
            searchForEnemy();
        }
     
        isSeekingCover = false;
    }
    private void LateUpdate()
    {
        /*if (animator.GetCurrentAnimatorStateInfo(0).IsName("idle with gun") && !t.isDead)
        {
            myMiddleSpine.LookAt(new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z));
        }*/
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
                if (newPunch <= 6 && newPunch > 2)
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
    private void searchForEnemy()
    {
        if (isOnfloor )
        {
            if(enemyTransform != null)
            {
                navAgent.NavToTransform(enemyTransform);
                //animator.SetInteger("state", 1);
                canAttack = false;
                isSearchingForEnemy = true;
                fightingMode(false);
            }
            
        }
        
    }


    private void followPlayer()
    {
        if(playerTransform != null)
        {
            if (Vector3.Distance(transform.position, playerTransform.position) <= 4)
            {
                //animator.SetInteger("state", 0);
                navAgent.StopNav();
                isSeekingCover = false;
                isFollowingPlayer = false;
            }
            else
            {
                navAgent.NavToTransform(playerTransform);
                //animator.SetInteger("state", 1);
                canAttack = false;
                isFollowingPlayer = true;
                resetMiddleSpine();
                fightingMode(false);
            }
            
            
        }
    }
    public bool setEnemyInstance()
    {
        if (!enemyMultiDetails.isMultiPlayer)
        {

            

            GameObject[] transforms = GameObject.FindGameObjectsWithTag("enemy");

            foreach(GameObject g in transforms)
            {
                if(g != gameObject)
                {
                   
                    enemyTransform = g.transform;
                    enemyTarget = g.GetComponent<Target>();
                    if (enemyTarget != null && enemyTransform != null)
                    {
                        if (!enemyTarget.isDead)
                        {
                            
                            return true;

                        }
                    }
                    
                }
            }
            timeToAttack = false;
            return false;
            
        }
        else
        {
            timeToAttack = false;
            return false;
            /*
            int i = Random.Range(0, gameMechMulti.playerTargets.Count);
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
            */
        }

    }
    [ServerCallback]
    public bool setEnemyInstanceM()
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
