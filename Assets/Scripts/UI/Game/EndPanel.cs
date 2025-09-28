using System.Collections;
using System.Collections.Generic;
using UFB.Items;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndPanel : MonoBehaviour
{
    public Text desText;
    public Image panelImage;

    public Sprite[] sprites;

    public void InitData(END_TYPE type)
    {
        if (type == END_TYPE.DEFEAT) 
        { 
            panelImage.sprite = sprites[1];
            desText.text = "You are Defeated.";
        } 
        else
        {
            panelImage.sprite = sprites[0];
            desText.text = "You are Winner.";
        }
        gameObject.SetActive(true);
    }

    public void OnEndScene()
    {
        SceneManager.LoadScene(GlobalDefine.MAIN_SCENE);
    }
}
