using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using Cinemachine;

public class GameMech : MonoBehaviour
{
    public static int Difficulty = 1;
    public static bool gameIsPaused = false;
    public static bool FullPlaying = true;
    public GameObject pauseMenu;
    public GameObject pauseButton;
    public GameObject[] notPauseMenu;
    public GameObject screenButtons;
    public ScreenTexts screentexts;
    public Slider quality;
    public Volume volume;
    private Transform player;
    private DepthOfField depthOfField;
   

    void Start()
    {
        //updateDifficulty();
        //Application.targetFrameRate = 60;
        //QualitySettings.SetQualityLevel(5);
        quality.value = QualitySettings.GetQualityLevel();
#if (UNITY_IPHONE || UNITY_ANDROID)
       screenButtons.SetActive(true);
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        screenButtons.SetActive(false);
#endif
        DepthOfField dof;
        if (volume.profile.TryGet(out dof))
        {
            depthOfField = dof;
        }
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Cursor.visible = false;
    }
    public void setQuality()
    {
        int q = (int) quality.value;
        QualitySettings.SetQualityLevel(q);
        //terrain.treeBillboardDistance = q * 10;
        if(q < 2)
        {
            if(q < 1)
            {
                //terrain.treeBillboardDistance = 10;
            }
            volume.enabled = false;
        }
        else
        {
            if(q > 4)
            {
                //terrain.treeBillboardDistance = 80;
            }
            volume.enabled = true;
        }
    }

    //Send the new difficulty to all other scripts
    public void updateDifficulty()
    {
       
    }

    //Change the difficulty
    public void changeDifficulty()
    {

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gameIsPaused)
            {
                
                PauseGame();
            }
            else
            {

                ResumeGame();
            }
        }
        if(depthOfField != null)
        {
            if (!PlayerGun.isFineAim)
            {
                depthOfField.active = true;
                depthOfField.focusDistance.value = Vector3.Distance(Camera.main.transform.position, player.position);
            }
            else
            {
                depthOfField.active = false;
                /*if(Physics.Raycast(Camera.main.transform.position,Camera.main.transform.forward,out RaycastHit hit))
                {
                    depthOfField.focusDistance.value = Vector3.Distance(Camera.main.transform.position, hit.collider.transform.position);
                }*/
            }
        }
    }
    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
#if (UNITY_IPHONE || UNITY_ANDROID)
       screenButtons.SetActive(false);
#endif
        foreach (GameObject g in notPauseMenu)
        {
            g.SetActive(false);
        }
        gameIsPaused = true;
        Time.timeScale = 0;


        Cursor.visible = true;
    }
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
#if (UNITY_IPHONE || UNITY_ANDROID)
       screenButtons.SetActive(true);
#endif
        foreach (GameObject g in notPauseMenu)
        {
            g.SetActive(true);
        }
        gameIsPaused = false;
        Time.timeScale = 1;

        Cursor.visible = false;
    }
    public void StartSurvival()
    {
        
        FullPlaying = true;
        SurvivalMech.survivalOngoing = true;
    }
    
}
