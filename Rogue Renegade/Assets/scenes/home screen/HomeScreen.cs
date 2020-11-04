﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Mirror;

public class HomeScreen : MonoBehaviour
{
    public Window home;
    public Window CustomizeCharacter;
    public Window changeName;
    public Window IpAddressWindow;
    public Window ErrorMessagePreset;
    private Window ErrorMessage;
    public Transform canvas;
    public TMP_InputField nameInputField;
    public TMP_InputField IPAddressInputField;
    public TMP_InputField PortInputField;
    [Scene]
    public string gameScene;

    private Window currentWidow;
    private Window prevWindow;
    public static bool error = false;
    public static string errorMsg = "Failed to connect";
    public Animator cameraAnimator;
    public static bool clientStarted = true;

    
    private void Start()
    {
        currentWidow = home;
        prevWindow = home;
        PlayerDetails playerDetails = new PlayerDetails();

        if (error)
        {
            showErrorMessage(errorMsg);
        }
    }
    public void showErrorMessage(string message)
    {
        currentWidow.hideWindow(WindowAnimationTriggers.popOut);
        ErrorMessage = Instantiate(ErrorMessagePreset, canvas).GetComponent<Window>();
        ErrorMessage.gameObject.SetActive(false);
        ErrorMessage.GetComponent<TextMeshProUGUI>().SetText(message);
        ErrorMessage.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText(message);
        ErrorMessage.showWindow(WindowAnimationTriggers.popIn);
        currentWidow = ErrorMessage;
    }
    public void showChangeName()
    {
        currentWidow.hideWindow(WindowAnimationTriggers.popOut);
        changeName.showWindow(WindowAnimationTriggers.popIn);

        currentWidow = changeName;
    }
    public void doneChangingName()
    {
        PlayerDetails playerDetails = new PlayerDetails();
        playerDetails.updateUsername(nameInputField.text);
        back();
    }
    public void showCustomizeCharacter()
    {
        cameraAnimator.SetTrigger("go back");
        currentWidow.hideWindow(WindowAnimationTriggers.popOut);
        CustomizeCharacter.showWindow(WindowAnimationTriggers.popIn);
        
        currentWidow = CustomizeCharacter;
    }
    public void showIpAddressWindow()
    {
        currentWidow.hideWindow(WindowAnimationTriggers.popOut);
        IpAddressWindow.showWindow(WindowAnimationTriggers.popIn);

        currentWidow = IpAddressWindow;
    }

    public void JoinGame()
    {
        GameMechMulti.IPAddress = IPAddressInputField.text;
        GameMechMulti.isHost = false;
        GameMechMulti.port = (ushort)int.Parse(PortInputField.text);
        SceneManager.LoadScene(gameScene,LoadSceneMode.Single);
        HomeScreen.clientStarted = false;
    }
    public void HostGame()
    {
        GameMechMulti.isHost = true;
        SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
    }
    public void back()
    {
        Debug.Log("Sema YEEE");
        if(currentWidow != prevWindow)
        {
            if (currentWidow == CustomizeCharacter)
            {
                cameraAnimator.SetTrigger("go front");
            }
            

            prevWindow.showWindow(WindowAnimationTriggers.popIn);
            
            if (currentWidow == ErrorMessage)
            {
                Destroy(ErrorMessage.gameObject);
            }
            else
            {
                currentWidow.hideWindow(WindowAnimationTriggers.popOut);
            }
           
           
            currentWidow = prevWindow;
        }
    }
   
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            back();
        }
    }
    
}