using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class EnemyMech : NetworkBehaviour {
    
    private Transform playerTransform;
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
    private bool isSearchingForPlayer = false;
    public float distanceToStartAttack = 5;
    public static float shootTime = 5;
    private float b = 0;
    private Target t;
    private GunDetails gunDetails;
    private Target playerTarget;
    private bool hasReachedPlayer;
    public Transform myMiddleSpine;
    private float weight;
    private int prevPunch;
    private EnemyMultiDetails enemyMultiDetails;
    private GameMechMulti gameMechMulti;
    private GameMech gameMech;



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
            gameMechMulti = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMechMulti>();
        }
        setPlayerInstance();

    }
    [Server]
    private void FixedUpdate()
    {
        
        
        if (!t.isDead)
        {
            if (timeToAttack)
            {
                if (!isSeekingCover)
                {
                    checkDistanceFromPlayer();
                }
                if (canAttack)
                {
                    Attack(attackType);
                }
                else
                {
                    searchForPlayer();
                }
            }
            else
            {
                idleStance();
            }
            
        }
        if(playerTarget != null)
        {
            if (playerTarget.isDead)
            {
                if (!setPlayerInstance())
                {
                    timeToAttack = false;
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
                    navAgent.StopNav();
                    
                }
            }
        }
        else
        {
            if (!isSearchingForPlayer)
            {
                searchForPlayer();
                canAttack = false;
                isSearchingForPlayer = true;
                
            }
        }
    }
    private void idleStance()
    {
        if (isOnfloor)
        {
            navAgent.StopNav();
        }
        if (attackType == 0)
        {
            if (!isBeingPunched)
            {
                animator.SetInteger("state", 5);
            }
                
        }
        else if (attackType == 1)
        {
            if (!isBeingPunched)
            {
                animator.SetInteger("state", 0);
            }
            
            isMeleeAttacking = false;
        }
    }
    private bool CheckCloseToTag(string tag, float minimumDistance)
    {
        GameObject[] goWithTag = GameObject.FindGameObjectsWithTag(tag);

        for (int i = 0; i < goWithTag.Length; ++i)
        {
            if (Vector3.Distance(transform.position, goWithTag[i].transform.position) <= minimumDistance)
                return true;
        }

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
                    gameMech.enemyDeathCallBack(t);
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
    private void WeaponAttack()
    {
        
        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("easy") || animator.GetCurrentAnimatorStateInfo(0).IsName("medium")
        //    || animator.GetCurrentAnimatorStateInfo(0).IsName("hard"))
        if(!isSeekingCover)
        {
            transform.LookAt(new Vector3(playerTransform.position.x,transform.position.y,playerTransform.position.z));
            a += 1 * Time.deltaTime;
            b += 1 * Time.deltaTime;
            if (b <= shootTime)
            {
                if (a <= shootSeconds && !gunDetails.gunSound.isPlaying)
                {
                    animator.SetInteger("state", GameMech.Difficulty + 1);
                    enemyGun.Shoot(enemyMultiDetails.isMultiPlayer);

                }
                else
                {
                    animator.SetInteger("state", 5);
                    if (a >= shootSeconds + 1)
                    {
                        a = 0;
                    }
                }
            }
            else
            {
                seekCover();
                animator.SetInteger("state", 1);
            }
        }
        else
        {
            if (isSeekingCover)
            {
                seekCover();
                animator.SetInteger("state", 1);
            }
        }
       
        
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
        if (Vector3.Distance(transform.position, playerTransform.position) >= distanceToStartAttack)
        {
            distanceToStartAttack = Vector3.Distance(transform.position, playerTransform.position);
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
            
            transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
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
    private void searchForPlayer()
    {
        if (isOnfloor)
        {
            navAgent.NavToTransform(playerTransform);
            animator.SetInteger("state", 1);
            canAttack = false;
            isSearchingForPlayer = true;
            fightingMode(false);
        }
    }


    [ServerCallback]
    public bool setPlayerInstance()
    {
        if (!enemyMultiDetails.isMultiPlayer)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            playerTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Target>();
            if (playerTarget != null && playerTransform != null)
            {
                return !playerTarget.isDead;
            }
            else
            {
                return false;
            }
        }
        else
        {
            int i = Random.Range(0, gameMechMulti.playerTargets.Count);
            List<uint> ids = new List<uint>(gameMechMulti.playerTargets.Keys);
            playerTransform = gameMechMulti.playerTargets[ids[i]].transform;
            playerTarget = gameMechMulti.playerTargets[ids[i]];
            if (playerTarget != null && playerTransform != null)
            {
                return !playerTarget.isDead;
            }
            else
            {
                return false;
            }
        }
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

