using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorPanel : MonoBehaviour
{
    public Text description;


    public void InitData(string str)
    {
        description.text = str;
        gameObject.SetActive(true);
    }
    
}
