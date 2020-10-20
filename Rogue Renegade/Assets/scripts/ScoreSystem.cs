using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public struct NameAndScore
{
    public string name;
    public int score;
}
public class Scores : SyncDictionary<uint, NameAndScore> { }
