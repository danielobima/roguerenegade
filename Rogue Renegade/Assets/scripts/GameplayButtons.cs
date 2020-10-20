using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameplayButtons : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private PlayerGun playerGun;
    private PlayerMotion playerMotion;
    private GameMech gameMech;
    private Image button;
    public bool isRealButton = false;
    private TextMeshProUGUI text;
    public string buttonFunction;
    public static bool getPlayerFromScreenTexts;
    private ScreenTexts screenTexts;

    private void Start()
    {
       
        
        gameMech = GameObject.FindGameObjectWithTag("GameMech").GetComponent<GameMech>();
        screenTexts = gameMech.screentexts;
        if (gameMech.playerSpawned)
        {
            getPlayer();
        }
        button = GetComponent<Image>();
        if (!buttonFunction.Equals("pause") && !buttonFunction.Equals("resume"))
        {
            text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }
    }
    public void getPlayer()
    {
        playerGun = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerGun>();
        playerMotion = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotion>();
    }
    public void setPlayer(GameObject go)
    {
        playerGun = go.GetComponent<PlayerGun>();
        playerMotion = go.GetComponent<PlayerMotion>();
    }
    
    public virtual void OnDrag(PointerEventData ped)
    {

    }
    public virtual void OnPointerDown(PointerEventData ped)
    {
        if (!isRealButton)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(button.rectTransform, ped.position, ped.pressEventCamera, out Vector2 pos))
            {
                switch (buttonFunction)
                {
                    case "shoot":
                        if (playerGun.gun != null)
                        {
                            playerGun.attacking = true;
                        }
                        else
                        {
                            playerGun.punchWhenTold();
                        }
                        break;
                    case "switchGun":
                        playerGun.startSwitchGunAnim();
                        break;
                    case "dropGun":
                        playerGun.dropGun();
                        break;
                    case "jump":
                        playerMotion.jump();
                        break;
                    case "pickupGun":
#if (UNITY_IPHONE || UNITY_ANDROID)
                    if(playerGun.gunDrop != null)
                    {
                        playerGun.changeGun(playerGun.gunDrop);
                    }
                    else
                    {
                        if(playerGun.grenadeDrop != null)
                        {
                            playerGun.pickupGrenade(playerGun.grenadeDrop);
                        }
                    }
#endif
                        break;
                    case "pause":
                        gameMech.PauseGame();
                        break;
                    case "resume":
                        gameMech.ResumeGame();
                        break;
                    case "throw":
                        playerGun.throwGrenade();
                        break;
                    case "restart survival scene":
                        SceneManager.LoadScene("survival scene");
                        break;
                    case "start survival":
                        gameMech.StartSurvival();
                        gameObject.SetActive(false);
                        break;
                    case "say yeet":
                        Debug.Log("yeet");
                        break;
                    case "exit":
                        Application.Quit();
                        break;

                }
            }
        }
    }
    public void realButtonFunc()
    {
        switch (buttonFunction)
        {
            case "shoot":
                if (playerGun.gun != null)
                {
                    playerGun.attacking = true;
                }
                else
                {
                    playerGun.punchWhenTold();
                }
                break;
            case "switchGun":
                playerGun.startSwitchGunAnim();
                break;
            case "dropGun":
                playerGun.dropGun();
                break;
            case "jump":
                playerMotion.jump();
                break;
            case "pickupGun":
#if (UNITY_IPHONE || UNITY_ANDROID)
                    if(playerGun.gunDrop != null)
                    {
                        playerGun.changeGun(playerGun.gunDrop);
                    }
                    else
                    {
                        if(playerGun.grenadeDrop != null)
                        {
                            playerGun.pickupGrenade(playerGun.grenadeDrop);
                        }
                    }
#endif
                break;
            case "pause":
                gameMech.PauseGame();
                break;
            case "resume":
                gameMech.ResumeGame();
                break;
            case "throw":
                playerGun.throwGrenade();
                break;
            case "restart survival scene":
                SceneManager.LoadScene("survival scene");
                break;
            case "start survival":
                gameMech.StartSurvival();
                gameObject.SetActive(false);
                break;
            case "say yeet":
                Debug.Log("yeet");
                break;
            case "exit":
                Debug.Log("Exited");
                Application.Quit();
                break;

        }
    }
    public virtual void OnPointerUp(PointerEventData ped)
    {
        switch (buttonFunction)
        {
            case "shoot":
                if (playerGun.gun != null)
                {
                    playerGun.attacking = false;
                }
                break;
        }
    }
    private void Update()
    {

        if (getPlayerFromScreenTexts)
        {
            playerGun = screenTexts.playerGun;
            playerMotion = screenTexts.playerGun.GetComponent<PlayerMotion>();
            getPlayerFromScreenTexts = false;
        }
        switch (buttonFunction)
        {
            case "shoot":
                if (playerGun.gun != null)
                {
                    text.SetText("Shoot");
                }
                else
                {
                    text.SetText("Punch");
                }
                break;
        }
        
       
        
    }
}
