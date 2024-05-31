using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDirection : MonoBehaviour
{
    public static UIDirection instance;

    public SpriteRenderer[] dirctions;

    public SpriteRenderer[] moveItems;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void InitInstance()
    {
        if (instance == null)
            instance = this;
    }

}
