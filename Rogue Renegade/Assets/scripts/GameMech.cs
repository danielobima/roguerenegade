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
    [Header("Dont assign")]
    public Transform player;
    private DepthOfField depthOfField;
    public GameObject[] playerSpawners;
    public GameObject playerAsset;
    public bool playerSpawned = true;
    public bool canSpawnPlayer = true;
    public bool multiplayer = true;
    public delegate void EnemyDeathCallBack(Target g);
    public EnemyDeathCallBack enemyDeathCallBack;

    [Header("Only for Single player")]
    public CinemachineFreeLook tpp;
    public Transform cylinder;
   

    void Start()
    {
        //updateDifficulty();
        //Application.targetFrameRate = 60;
        //QualitySettings.SetQualityLevel(5);
        //quality.value = QualitySettings.GetQualityLevel();
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
        if (playerSpawned)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        else
        {
            if (canSpawnPlayer)
            {
                spawnPlayer(true);
            }
        }
        Cursor.visible = false;
    }
    public void spawnPlayer(bool randomPoint, int spawner = 0)
    {
        if (randomPoint)
        {
            spawner = Random.Range(0, playerSpawners.Length);
            Instantiate(playerAsset, playerSpawners[spawner].transform.position, playerSpawners[spawner].transform.rotation);
        }
        else
        {
            Instantiate(playerAsset, playerSpawners[spawner].transform.position, playerSpawners[spawner].transform.rotation);
        }
        extraSetup();
    }
    public GameObject spawnAndReturnPlayer(bool randomPoint, int spawner = 0)
    {
        GameObject go;
        if (randomPoint)
        {
            spawner = Random.Range(0, playerSpawners.Length);
            go = Instantiate(playerAsset, playerSpawners[spawner].transform.position, playerSpawners[spawner].transform.rotation);
        }
        else
        {
           go = Instantiate(playerAsset, playerSpawners[spawner].transform.position, playerSpawners[spawner].transform.rotation);
        }
        extraSetup(go);
        return go;
    }
    public void extraSetup(GameObject go = null)
    {
        playerSpawned = true;
        if (go == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
          
        }
        else
        {
            player = go.transform;
            
        }
        
     
       
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
        if (!multiplayer)
        {
            Time.timeScale = 0;
        }


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
        if (!multiplayer)
        {
            Time.timeScale = 1;
        }

        Cursor.visible = false;
    }
    public void StartSurvival()
    {
        
        FullPlaying = true;
        SurvivalMech.survivalOngoing = true;
    }
    
}
