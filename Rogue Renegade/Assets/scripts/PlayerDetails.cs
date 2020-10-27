using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class PlayerDetails 
{
    public PlayerDetails()
    {

    }
    public  void updateUsername(string username)
    {
        PlayerPrefs.SetString("Username", username);
    }
    public  string getUserName()
    {
        return PlayerPrefs.GetString("Username", "No Name");
    }
}
