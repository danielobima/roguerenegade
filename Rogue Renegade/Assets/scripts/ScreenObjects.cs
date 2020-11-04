using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenObjects : MonoBehaviour
{
    public GameObject pickUpGunButton;
    public GameObject switchGunButton;
    public GameObject dropGunButton;
    public GameObject throwButton;
    public GameObject startButton;

    [Header("Leave empty if not multiplayer")]
    public GameObject chatbar;
    public GameObject chatMessager;

    [Header("Leave empty if not survival multiplayer")]
    public GameObject survivalScoresPanel;
    public GameObject survivalScoresList;
    public GameObject survivalWaveText;
    public GameObject survivalScoreItem;
}
