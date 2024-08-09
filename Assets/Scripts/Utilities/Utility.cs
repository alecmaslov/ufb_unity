using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static string GetTurnTimeString(float curTime)
    {
        int min = Mathf.FloorToInt( curTime / 60 );
        int sec = (int)curTime % 60;
        return $"{min.ToString("D2")}:" +
            $"{sec.ToString("D2")}";
    }
}
