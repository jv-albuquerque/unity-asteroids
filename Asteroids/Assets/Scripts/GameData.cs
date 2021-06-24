using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int highScore = 0;

    public bool isMuted = false;

    public void UpdateDataByData(GameData data)
    {
        highScore = data.highScore;
        isMuted = data.isMuted;
    }
}
