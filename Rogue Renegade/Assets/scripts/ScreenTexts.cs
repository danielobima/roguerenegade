using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTexts : MonoBehaviour
{
    public Text AmmoText;
    public GameObject AmmoTextP;
    public PlayerGun playerGun;
    private GameMech gameMech;
    public GameObject pickupGunButton;
    private bool hasSetOff = false;
    private bool hasSetOn = false;
    private float a = 0;
    private bool hasShownWave = false;
    public GameObject[] guns;
    private GameObject primaryGun;
    private GameObject secondaryGun;
    private GameObject gunDrop;
    private GameObject gunDropCache;
    public GameObject AmmoBar;
    
    private float abf = 0;

    [Header("Leave empty if not survival mode")]
    public Text WaveText;
    public Text FloorText;
    public Text Kills;
    public Text WaveCountdown;

    private void Start()
    {

        gameMech = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMech>();
        if (gameMech.playerSpawned)
        {
            getPlayer();
        }
    }
    public void getPlayer()
    {
        playerGun = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerGun>();
    }
    public void setPlayer(GameObject go)
    {
        playerGun = go.GetComponent<PlayerGun>();
    }
    public void changeGunIcon(){
        
        if (primaryGun != null)
        {
            Destroy(primaryGun);
        }
        primaryGun = Instantiate(guns[playerGun.gun.GetComponent<GunDetails>().gunInt], AmmoTextP.transform);
        primaryGun.transform.localPosition = new Vector3(70, 35, 0);
        primaryGun.transform.localEulerAngles = new Vector3(0, 90, 0);
        if (playerGun.gun.GetComponent<GunDetails>().gunType.Equals("RPG7"))
        {
            primaryGun.transform.localScale = new Vector3(80, 80, 80);
        }
        else
        {
            primaryGun.transform.localScale = new Vector3(120, 120, 120);
        }
    }
    public void changeSecondaryGunIcon()
    {
        
        if (secondaryGun != null)
        {
            Destroy(secondaryGun);
        }
        if(playerGun.secondaryGun != null)
        {
            secondaryGun = Instantiate(guns[playerGun.secondaryGun.GetComponent<GunDetails>().gunInt], AmmoTextP.transform);
            secondaryGun.transform.localPosition = new Vector3(100, 50, 0);
            secondaryGun.transform.localEulerAngles = new Vector3(0, 90, 0);
            if (playerGun.secondaryGun.GetComponent<GunDetails>().gunType.Equals("RPG7"))
            {
                secondaryGun.transform.localScale = new Vector3(40, 40, 40);
            }
            else
            {
                secondaryGun.transform.localScale = new Vector3(50, 50, 50);
            }
        }
    }
    public void removeSecondaryGunIcon()
    {
        
        if (secondaryGun != null)
        {
            Destroy(secondaryGun);
        }
    }
    private void changeGunDropIcon()
    {
        
        if (gunDrop != null)
        {
            Destroy(gunDrop);
            if (playerGun.gunDrop != null)
            {
                gunDropCache = playerGun.gunDrop;
                gunDrop = Instantiate(guns[playerGun.gunDrop.GetComponent<GunDetails>().gunInt], pickupGunButton.transform);
                gunDrop.transform.localPosition = new Vector3(-120, 20, 0);
                gunDrop.transform.localEulerAngles = new Vector3(0, 90, 0);
                gunDrop.transform.localScale = new Vector3(150, 150, 150);
                if (playerGun.gunDrop.GetComponent<GunDetails>().gunType.Equals("RPG7"))
                {
                    gunDrop.transform.localScale = new Vector3(100, 100, 100);
                }
                else
                {

                    gunDrop.transform.localScale = new Vector3(150, 150, 150);
                }
            }
            else
            {
                if (playerGun.grenadeDrop != null)
                {
                    gunDropCache = playerGun.grenadeDrop;
                    gunDrop = Instantiate(guns[8], pickupGunButton.transform);
                    gunDrop.transform.localPosition = new Vector3(-120, 0, 0);
                    gunDrop.transform.localEulerAngles = new Vector3(0, 90, 0);
                    gunDrop.transform.localScale = new Vector3(200, 500, 500);
                }

            }
        }
        else
        {
            if (playerGun.gunDrop != null)
            {
                gunDrop = Instantiate(guns[playerGun.gunDrop.GetComponent<GunDetails>().gunInt], pickupGunButton.transform);
                gunDrop.transform.localPosition = new Vector3(-120, 20, 0);
                gunDrop.transform.localEulerAngles = new Vector3(0, 90, 0);
                gunDrop.transform.localScale = new Vector3(150, 150, 150);
                if (playerGun.gunDrop.GetComponent<GunDetails>().gunType.Equals("RPG7"))
                {
                    gunDrop.transform.localScale = new Vector3(100, 100, 100);
                }
                else
                {

                    gunDrop.transform.localScale = new Vector3(150, 150, 150);
                }
            }
            else
            {
                if (playerGun.grenadeDrop != null)
                {
                    gunDrop = Instantiate(guns[8], pickupGunButton.transform);
                    gunDrop.transform.localPosition = new Vector3(-120, 0, 0);
                    gunDrop.transform.localEulerAngles = new Vector3(0, 90, 0);
                    gunDrop.transform.localScale = new Vector3(200, 500, 500);
                }

            }
        }
        

#if (UNITY_IPHONE || UNITY_ANDROID)
        Text t = pickupGunButton.GetComponentInChildren<Text>();
        t.text = "Pick";
#endif

    }

    private void Update()
    {


        if (playerGun != null)
        {
            if (playerGun.gun != null)
            {
                if (!hasSetOn)
                {
                    AmmoText.gameObject.SetActive(true);
                    AmmoTextP.gameObject.SetActive(true);

                    hasSetOn = true;
                    hasSetOff = false;
                }

                AmmoText.text = playerGun.ammoText;
                if (playerGun.ammoText == "0/0")
                {
                    AmmoText.color = new Color(1, 0, 0);
                }
                else
                {
                    AmmoText.color = new Color(1, 1, 1);
                }
                if (playerGun.isReloading)
                {
                    AmmoBar.gameObject.SetActive(true);
                    abf += 1 * Time.deltaTime;
                    if (abf <= 1)
                    {
                        AmmoBar.transform.localScale = new Vector3(abf, 1, 1);
                    }
                    else
                    {
                        AmmoBar.transform.localScale = new Vector3(1, 1, 1);
                    }
                }
                else
                {
                    AmmoBar.gameObject.SetActive(false);
                    abf = 0;
                }
            }
            else
            {
                if (!hasSetOff)
                {
                    AmmoText.gameObject.SetActive(false);
                    if (secondaryGun == null)
                    {
                        AmmoTextP.gameObject.SetActive(false);
                    }
                    if (primaryGun != null)
                    {
                        Destroy(primaryGun);
                    }
                    hasSetOn = false;
                    hasSetOff = true;
                }

            }
            if (playerGun.grenadeDrop != null || playerGun.gunDrop != null)
            {
                if (gunDropCache != null)
                {
                    if (gunDropCache != playerGun.grenadeDrop && gunDropCache != playerGun.gunDrop)
                    {
                        changeGunDropIcon();
                    }
                }
                else
                {
                    changeGunDropIcon();
                }
            }
            else
            {
                if (gunDrop != null)
                {
                    Destroy(gunDrop);
                    gunDropCache = null;
                }
            }
        }


        if (gameMech.GetComponent<SurvivalMech>() != null)
        {
            if (SurvivalMech.survivalOngoing)
            {
                WaveText.gameObject.SetActive(false);
                Kills.gameObject.SetActive(true);
                Kills.text = "Kills " + gameMech.GetComponent<SurvivalMech>().kills;
                if (!gameMech.GetComponent<SurvivalMech>().hasSpawned)
                {
                    if (!hasShownWave)
                    {
                        a += 1 * Time.deltaTime;
                        WaveText.gameObject.SetActive(true);
                        WaveText.text = "Wave " + gameMech.GetComponent<SurvivalMech>().waveNo;
                        FloorText.text = "Level " + gameMech.GetComponent<SurvivalMech>().floor;
                        if (a >= 3)
                        {
                            WaveText.gameObject.SetActive(false);
                            hasShownWave = true;
                            a = 0;
                        }
                    }
                }
                else
                {
                    hasShownWave = false;
                }
            }
        }

    }
}
