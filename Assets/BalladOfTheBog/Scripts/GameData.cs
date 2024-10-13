using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int coinCount;
    public Vector3 playerPosition;
    
    public GameData()
    {
        this.coinCount = 0;
        playerPosition = new Vector3(0, 0, 0);
    }
}