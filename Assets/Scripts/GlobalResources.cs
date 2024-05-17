using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalResources : MonoBehaviour
{
    public static GlobalResources instance;

    public Sprite[] items;
    public Sprite[] powers;
    public Sprite[] stacks;

    public Sprite coin;
    public Sprite range;
    public Sprite lightImage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
