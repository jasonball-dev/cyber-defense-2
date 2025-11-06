using System;
using UnityEngine;

public class MetaGameManager : Singleton<MetaGameManager>
{
    private string gameJoinCode;

    public string LocalPlayerProfileName { get; set; }

    public GameObject LocalPlayerGameObject { get; set; }

    // Function to set the gameJoinCode
    public void SetGameJoinCode(string code)
    {
        print("set: " + code);
        gameJoinCode = code;
    }

    // Function to get the gameJoinCode
    public string GetGameJoinCode()
    {
        print("accessed");
        return gameJoinCode;
    }
}