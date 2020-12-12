using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [Header("Leave empty if not deathmatch multiplayer")]
    public TextMeshProUGUI timeRemaining;
}
