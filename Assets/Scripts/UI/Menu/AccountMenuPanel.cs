using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UFB.Character;
using UFB.Core;
using UFB.Network;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

[Serializable]
public struct UserDataPanel
{
    public Text levelText;
    public Text goldText;
    
    public Text lossesText;
    public Text winsText;
    public Text killsText;
    public Text battlesText;
    public Text damageTakenText;
    public Text itemBagsText;
    public Text usedEnergiesText;
    public Text damageDealText;
    public Text usedStacksText;
    public Text damageHealText;
    public Text collectGoldText;
    public Text traveledTilesText;
    public Text chestsText;
}

public class AccountMenuPanel : MonoBehaviour
{
    public Text playerNameText;
    public Text emailText;
    public Text goldText;

    public Image heroBtnImage;
    public Image heroDetailImage;
    
    public Sprite selectedImage;
    public Sprite unselectedImage;
    
    public GameObject heroListPart;
    public GameObject userStatusPart;
    
    // USER EDIT PANEL
    public Text editEmailText;
    public InputField userNameField;
    public InputField oldPasswordField;
    public InputField newPasswordField;
    public InputField confirmPasswordField;
    
    public AccountDetailPanel accountDetailPanel;
    
    public ItemCard characterCard;
    public Transform characterCardHolder;
    
    public UserDataPanel userDataPanel;
    
    private void Start()
    {

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
        
        OnInitHeroList();
        gameObject.SetActive(true);
    }

    public void OnInitUserDetail(string characterClass)
    {
        accountDetailPanel.InitPanel(characterClass);
    }

    public async void OnInitHeroList()
    {
        heroBtnImage.sprite = selectedImage;
        heroDetailImage.sprite = unselectedImage;
        await GetUserHeroList();
        heroListPart.SetActive(true);
        userStatusPart.SetActive(false);
    }

    public async void OnInitUserDetail()
    {
        heroBtnImage.sprite = unselectedImage;
        heroDetailImage.sprite = selectedImage;
        await GetUserDetail();
        heroListPart.SetActive(false);
        userStatusPart.SetActive(true);
    }

    private async Task GetUserHeroList()
    {
        var userId = MainScene.instance.userData.id;
        Debug.Log($"------------ refresh hero list id: {userId} ");
        var data = await ServiceLocator.Current.Get<NetworkService>().GetHeroList(userId);
        string result = data.error;
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
            MainScene.instance.ShowNotificationMessage("error", "Hero Detail Loaded.");
            InitHeroList(data);
        }
        else
        {
            MainScene.instance.ShowNotificationMessage("error", "Server error occured.");
        }
    }
    
    private async Task GetUserDetail()
    {
        var userId = MainScene.instance.userData.id;

        Debug.Log($"------------ refresh user id: {userId} ");
        
        var data = await ServiceLocator.Current.Get<NetworkService>().GetUserDetail(userId);
        string result = data.error;
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
            MainScene.instance.ShowNotificationMessage("error", "Hero Detail Loaded.");
            InitUserDetailInfo(data);
        }
        else
        {
            MainScene.instance.ShowNotificationMessage("error", "Server error occured.");
        }
    }

    void InitHeroList(CharacterClassServerData data)
    {
        for (var i = 1; i < characterCardHolder.childCount; i++)
        {
            Destroy(characterCardHolder.GetChild(i).gameObject);
        }
        
        foreach (var characterClassData in data.data)
        {
            var item = Instantiate(characterCard, characterCardHolder);
            Addressables
                .LoadAssetAsync<UfbCharacter>("UfbCharacter/" + characterClassData.className)
                .Completed += (op) =>
            {
                if (
                    op.Status
                    == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded
                )
                {
                    item.InitData(characterClassData.className, op.Result.avatar);
                }
                else
                    Debug.LogError(
                        "Failed to load character avatar: " + op.OperationException.Message
                    );
            };
            item.InitText2($"Level {characterClassData.level}");
            item.InitStateText(new []{"40", "20", "0"});
            // item.InitStateText(new []{});
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnInitUserDetail(characterClassData.className);
            });
            item.gameObject.SetActive(true);
        }
        
    }
    
    void InitUserDetailInfo(HeroData data)
    {
        userDataPanel.lossesText.text = data.losses.ToString();
        userDataPanel.winsText.text = data.wins.ToString();
        userDataPanel.killsText.text = data.kills.ToString();
        userDataPanel.battlesText.text = data.battles.ToString();
        userDataPanel.damageTakenText.text = data.damage_taken.ToString();
        userDataPanel.itemBagsText.text = data.item_bags.ToString();
        userDataPanel.usedEnergiesText.text = data.used_energies.ToString();
        userDataPanel.damageDealText.text = data.damage_deal.ToString();
        userDataPanel.usedStacksText.text = data.used_stacks.ToString();
        userDataPanel.damageHealText.text = data.damage_heal.ToString();
        userDataPanel.collectGoldText.text = data.collect_golds.ToString();
        userDataPanel.traveledTilesText.text = data.traveled_tiles.ToString();
        userDataPanel.chestsText.text = data.chests.ToString();
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
