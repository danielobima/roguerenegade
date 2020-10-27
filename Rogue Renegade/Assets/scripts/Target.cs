using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Target : NetworkBehaviour {

    [SyncVar]
    public float health = 5;
    public float healthFull = 5;
    public bool isPlayer = false;
    //public GameObject myHealthBar;
    public GameObject player;
    public GameObject myRing;
    public GameObject currentLookAt;
    private RagdollSwitch ragdollSwitch;
    private Rigidbody r; 
    private PlayerGun playerGun;
    public bool isDead = false;
    public bool isAiming = false;
    private bool hasSwitchedRagdoll = false;
    private bool isTakingDamage = false;
    private float damageTime = 0.3f;
    private float dt = 0;
    private HealthBar healthBar;
    private float dc;
    public float damageCooldown = 4;
    private GameMechMulti gameMechMulti;
    public List<uint> attackers;
    private bool hasDroppedGun = false;

    [Header("LEAVE EMPTY")]
    public Transform playerMiddleSpine;

    [Header("Only for player")]
    public GameObject damagePointer;
    private PlayerMultiDetails playerMultiDetails;

    private void Start()
    {
        ragdollSwitch = GetComponent<RagdollSwitch>();
        r = GetComponent<Rigidbody>();
        
        
        
        gameMechMulti = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMechMulti>();
        if (isPlayer)
        {
            //playerMiddleSpine = GameObject.FindGameObjectWithTag("PlayerMiddleSpine").transform;
            healthBar = GetComponent<HealthBar>();
            playerGun = GetComponent<PlayerGun>();
            playerMultiDetails = GetComponent<PlayerMultiDetails>();
        }
        attackers = new List<uint>();
        
        
    }
    public void damagePoint(Vector3 pos)
    {

        if (isPlayer)
        {
            if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
            {

                float z = Camera.main.transform.eulerAngles.y - (Mathf.Atan2(pos.normalized.x, pos.normalized.z) * Mathf.Rad2Deg);
                //Debug.Log(Camera.main.transform.eulerAngles.y);
                damagePointer.transform.localRotation = Quaternion.Euler(0, 0, z);
                Animator[] a = damagePointer.transform.GetComponentsInChildren<Animator>();

                foreach (Animator an in a)
                {
                    an.SetTrigger("slow-glow");
                }
            }
        }
        
        
    }
    public void TakeDamage(float DamageAmount,uint attackerId = default)
    {

        if (!isDead)
        {
            if (health - DamageAmount >= 0)
            {
                if (isPlayer)
                {
                    health -= DamageAmount;
                    //healthBar.healthbarGlow();
                }
                else
                {
                    health -= DamageAmount;
                }

            }
            else
            {
                health = 0;
            }
            if (!isPlayer)
            {
                isTakingDamage = true;
                Image image = myRing.transform.GetChild(0).GetComponent<Image>();
                image.color = new Color(1, 0, 0);
            }
            dc = 0;
            if (attackerId != default)
            {
                attackers.Add(attackerId);
            }
        }

    }
    public void addHealth(float HealthAmount)
    {
        
            if (health + HealthAmount <= healthFull)
            {
                health += HealthAmount;
            }
            else
            {
                health = healthFull;
            }
        
        

    }
    public void TakePunchForce(Transform puncher, float force)
    {
        /*
        if(timeForPunchForce < time)
        {
            r.AddForce(puncher.forward * 1000);
            timeForPunchForce += 1 * Time.deltaTime;
        }
        else
        {
            timeForPunchForce = 0;
        }*/
        if (isPlayer)
        {
            if (isLocalPlayer || !playerMultiDetails.isMultiPlayer)
            {
                r.AddForce(puncher.forward * force);
            }
        }
        else
        {
            r.AddForce(puncher.forward * force);
        }
    }
    private void Update()
    {
        
        
        if (health <= 0)
        {
            if (!isPlayer)
            {
                //Debug.Log("YEET");
                EnemyMech e = gameObject.GetComponent<EnemyMech>();
                if (e != null)
                {
                    e.Die();
                    //e.myCanvas.SetActive(false);
                }
               
                //myHealthBar.SetActive(false);
            }
            else
            {
                if (playerGun.playerMultiDetails.isMultiPlayer)
                {
                    if (isLocalPlayer && !hasDroppedGun)
                    {
                        playerGun.dropGun();
                        playerGun.dropSecondaryGun();
                        hasDroppedGun = true;
                    }
                    if (Input.GetKeyDown("p"))
                    {
                        //gameMechMulti.respawnPlayer(connectionToClient,gameObject);
                    }
                }
               
            }
            isDead = true;
            if (!hasSwitchedRagdoll)
            {
                ragdollSwitch.SwitchRagdoll(isDead);
            }
        }
        if (!isPlayer)
        {
            
            
            
            
#if UNITY_STANDALONE_WIN
                /*if ( Input.GetMouseButtonDown(1))
                {
                    RaycastHit hit = new RaycastHit();
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9))
                    {
                        if (hit.collider.gameObject == this.gameObject)
                        {
                            if (t.currentLookAt != gameObject || !t.isAiming)
                            {

                                t.currentLookAt = gameObject;
                                t.isAiming = true;
                                myRing.SetActive(true);
                                //dev only. comment out when done with.
                                //health = 0;
                            }
                            else
                            {
                                t.isAiming = false;
                            }

                        }

                    }
                }*/
#endif


            
            if (health < healthFull && health >0)
            {
                //myHealthBar.SetActive(true);
            }
            else
            {
                //myHealthBar.SetActive(false);
            }

            
        }
        else
        {
            if (!isDead)
            {
                /*if (health < healthFull)
                {
                    if (dc < damageCooldown)
                    {
                        dc += 1 * Time.deltaTime;
                    }
                    else
                    {
                        //health += 1 * Time.deltaTime;
                    }

                }
                else
                {
                    dc = 0;
                }*/
                if(gameMechMulti != null)
                {
                    if(gameMechMulti.survivalMechMulti != null)
                    {
                        if (gameMechMulti.survivalMechMulti.isOnCooldown)
                        {
                            if (health < healthFull)
                            {
                                health += 1 * Time.deltaTime;

                            }
                        }
                    }
                }
            }
            
        }
        if (isTakingDamage && !isPlayer)
        {
            dt += 1 * Time.deltaTime;
            if(dt >= damageTime)
            {
                Image image = myRing.transform.GetChild(0).GetComponent<Image>();
                image.color = new Color(1, 1, 1);
                dt = 0;
            }
        }
        if(transform.position.y <= -30)
        {
            health = 0;
        }
        
        
    }
    /*private void LateUpdate()
    {
      
        
       
        if (t.currentLookAt == gameObject && t.isAiming && !t.playerGun.isPunching)
        {
            playerMiddleSpine.LookAt(gameObject.transform);

        }
    }
    public void enemyTouched()
    {
        
        if (t.currentLookAt != gameObject || !t.isAiming)
        {

            t.currentLookAt = gameObject;
            t.isAiming = true;
            myRing.SetActive(true);
            //dev only. comment out when done with.
            //health = 0;
        }
        else
        {
            t.isAiming = false;
        }
    }*/
    
    
}
