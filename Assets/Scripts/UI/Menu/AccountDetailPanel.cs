using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountDetailPanel : MonoBehaviour
{
    public Text playerNameText;
    public Text levelText;
    public Text goldText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitPanel(int type)
    {
        var email = MainScene.instance.userData.email;
        var displayName = MainScene.instance.userData.displayName;
        var gold = MainScene.instance.userData.gold;
        playerNameText.text = displayName;
        levelText.text = email;
        goldText.text = gold.ToString();

        
        gameObject.SetActive(true);
    }
}
