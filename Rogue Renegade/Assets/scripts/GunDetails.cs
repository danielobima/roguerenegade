using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunDetails : MonoBehaviour
{
    
    public string gunType = "AK47";
    public int gunInt = 0;
    public AudioSource gunSound;
    public bool gunSoundLoops = true; 
    private float d = 0;
    public int gunID = 0;
    public bool canDestroy = true;
    public bool handgun = false;
    public bool isContinuousShooting = true; 
    public float damage = 0;
    public float enemyDamage = 0;

    public Vector3 localPos;
    public Vector3 localRot;
    public AnimationClip handPose;

    public float fireRate = 0;
    private float accumulatedTime = 0;
    public int ammoMax = 30;
    public int ammoSpare = 170;
    public int ammoLoaded = 0;
    public bool isShooting = false;
    private ParticleSystem muzzleFlash;
    public TrailRenderer bulletTracer;
    [HideInInspector]
    public ParticleSystem[] impacts;
    public float bulletSpeed = 1000;
    public float bulletDrop = 0;
    public float maxLifeTime = 2f;
    private CameraMovement cameraMovement;

    public GameObject gunDrop;






    private Ray ray;
    private RaycastHit hitInfo;
    private List<Bullet> bullets = new List<Bullet>();
    


    private void Start()
    {
        gunSound.volume = 1;
        gunSound.loop = false;
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
        cameraMovement = Camera.main.GetComponent<CameraMovement>();
        
       
    }

    class Bullet
    {
        public float time;
        public Vector3 initialVelocity;
        public Vector3 initialPosition;
        public TrailRenderer tracer;

        public Bullet(Vector3 initPos, Vector3 initVelocity)
        {
            initialPosition = initPos;
            initialVelocity = initVelocity;
            
            time = 0;
        }
    }
    Bullet createBullet(Vector3 initPos, Vector3 initVelocity)
    {
        Bullet b = new Bullet(initPos,initVelocity);
        b.tracer = Instantiate(bulletTracer, initPos, Quaternion.identity);
        b.tracer.AddPosition(initPos);
        return b;
    }
    Vector3 GetBulletPos(Bullet bullet)
    {
        Vector3 gravity = Vector3.down * bulletDrop;
        return bullet.initialPosition + bullet.initialVelocity * bullet.time + 0.5f * gravity * bullet.time * bullet.time;
    }
    public void startShooting()
    {
        isShooting = true;
        shoot();
    }
    public void singleShot()
    {
        shoot();
    }
    public void UpdateFiring(float deltaTime)
    {
        if (isContinuousShooting)
        {
            accumulatedTime += deltaTime;
            float interval = 1 / fireRate;
            while (accumulatedTime >= 0.0f)
            {
                shoot();
                accumulatedTime -= interval;
            }
        }
    }
    public void UpdateBullets(float deltaTime)
    {
        SimulateBullets(deltaTime);
        DestroyBullets();
    }
    private void DestroyBullets()
    {
        bullets.RemoveAll(bullet => bullet.time >= maxLifeTime);
    }
    private void SimulateBullets(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = GetBulletPos(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetBulletPos(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }
    private void RaycastSegment(Vector3 start,Vector3 end,Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        ray.origin = start;
        ray.direction = direction;
        if (Physics.Raycast(ray, out hitInfo,distance))
        {
            impacts[0].transform.position = hitInfo.point;
            impacts[0].transform.forward = hitInfo.normal;
            impacts[0].Emit(1);
            bullet.tracer.transform.position = hitInfo.point;
            bullet.time = maxLifeTime;

        }
        else
        {
            bullet.tracer.transform.position = end;
        }
    }

    private void shoot()
    {
        muzzleFlash.Emit(1);

        Vector3 velocity = (cameraMovement.AimTarget.position - muzzleFlash.transform.position).normalized * bulletSpeed;
        Bullet bullet = createBullet(muzzleFlash.transform.position, velocity);
        bullets.Add(bullet);
        
    }

    public void stopShooting()
    {
        isShooting = false;
    }
    private void FixedUpdate()
    {
        if (gameObject.CompareTag("DroppedGun"))
        {
            d += 1 * Time.deltaTime;
            if(d >= 60 && canDestroy)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            d = 0;
        }
        
        
    }
   
}
