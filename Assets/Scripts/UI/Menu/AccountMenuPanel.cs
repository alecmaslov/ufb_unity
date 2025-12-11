using System;
using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Core;
using UFB.Network;
using UnityEngine;
using UnityEngine.UI;

public class AccountMenuPanel : MonoBehaviour
{
    public Text playerNameText;
    public Text emailText;
    public Text goldText;

    public Transform heroList;

    // USER EDIT PANEL
    public Text editEmailText;
    public InputField userNameField;
    public InputField oldPasswordField;
    public InputField newPasswordField;
    public InputField confirmPasswordField;
    
    public AccountDetailPanel accountDetailPanel;
    
    [SerializeField]
    private List<UfbCharacter> _characters;
    public ItemCard characterCard;
    public Transform characterCardHolder;
    
    private void Start()
    {
        _characters.ForEach(c =>
        {
            var item = Instantiate(characterCard, characterCardHolder);
            item.InitData(c.characterName, c.avatar);
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnInitUserDetail(c.characterName);
            });
            item.gameObject.SetActive(true);
        });
    }

    public void InitPanel()
    {
        var email = MainScene.instance.userData.email;
        var displayName = MainScene.instance.userData.displayName;
        var gold = MainScene.instance.userData.gold;

        playerNameText.text = displayName;
        editEmailText.text = email;
        emailText.text = email;
        goldText.text = gold.ToString();
        
        gameObject.SetActive(true);
    }

    public void OnInitUserDetail(string characterClass)
    {
        accountDetailPanel.InitPanel(characterClass);
    }

    public void OnUserEditPanel()
    {
        
        userNameField.text = MainScene.instance.userData.displayName;

        oldPasswordField.text = "";
        newPasswordField.text = "";
        confirmPasswordField.text = "";
    }

    public async void OnChangeUserPassword()
    {
        string email = MainScene.instance.userData.email;
        string oldPassword = oldPasswordField.text;
        string newPassword = newPasswordField.text;
        string confirm = confirmPasswordField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirm))
        {
            MainScene.instance.ShowNotificationMessage("error", "Please fill all fields");
            return;
        }

        if (newPassword != confirm)
        {
            MainScene.instance.ShowNotificationMessage("error", "Passwords do not match");
            return;
        }
        
        var data = await ServiceLocator.Current.Get<NetworkService>().ChangeUserPasswordHandler(email, oldPassword, newPassword);
        var result = data.error;
        Debug.Log(result);

        if (result == GlobalDefine.RESPONSE_MESSAGE.NOT_EXIST)
        {
            MainScene.instance.ShowNotificationMessage("error", "User does not exist.");
        }
        else if (result == GlobalDefine.RESPONSE_MESSAGE.WRONG_PASSWORD)
        {
            MainScene.instance.ShowNotificationMessage("error", "Passwords do not match.");
        }
        else if (result == GlobalDefine.RESPONSE_MESSAGE.ERROR)
        {
            MainScene.instance.ShowNotificationMessage("error", "An error occured.");
        }
        else if (result == GlobalDefine.RESPONSE_MESSAGE.SUCCESS)
        {
            PlayerPrefs.SetString("password", newPassword);
            MainScene.instance.ShowNotificationMessage("error", "Your user password has been successfully changed.");
            Debug.Log(data.clientId);
        }
        else
        {
            MainScene.instance.ShowNotificationMessage("error", "Server error occured.");
        }
    }

    public async void OnChangeUserName()
    {
        var email = MainScene.instance.userData.email;
        var displayName = userNameField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(displayName))
        {
            MainScene.instance.ShowNotificationMessage("error", "Please fill all fields");
            return;
        }

        if (MainScene.instance.userData.displayName == displayName)
        {
            MainScene.instance.ShowNotificationMessage("error", "The username is the same");
            return;
        }
        
        var data = await ServiceLocator.Current.Get<NetworkService>().ChangeUserNameHandler(email, displayName);
        var result = data.error;
        Debug.Log(result);

        if (result == GlobalDefine.RESPONSE_MESSAGE.NOT_EXIST)
        {
            MainScene.instance.ShowNotificationMessage("error", "User does not exist.");
        }
        else if (result == GlobalDefine.RESPONSE_MESSAGE.ERROR)
        {
            MainScene.instance.ShowNotificationMessage("error", "An error occured.");
        }
        else if (result == GlobalDefine.RESPONSE_MESSAGE.SUCCESS)
        {
            playerNameText.text = displayName;
            MainScene.instance.userData.displayName = displayName;
            MainScene.instance.ShowNotificationMessage("error", "Your user name has been successfully changed.");
            Debug.Log(data.clientId);
        }
        else
        {
            MainScene.instance.ShowNotificationMessage("error", "Server error occured.");
        }
    }
}
