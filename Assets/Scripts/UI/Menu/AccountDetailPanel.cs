using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UFB.Character;
using UFB.Core;
using UFB.Network;
using UFB.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class AccountDetailPanel : MonoBehaviour
{
    public Image heroAvatar;
    
    public Image skillTreeBtnImage;
    public Image heroDetailImage;
    
    public Sprite selectedImage;
    public Sprite unselectedImage;

    public GameObject skillTreePart;
    public GameObject heroStatusPart;
    
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

    public string characterClassName = "";
    
    [SerializeField]
    private LinearIndicatorBar _healthBar;

    [SerializeField]
    private LinearIndicatorBar _energyBar;

    [SerializeField]
    private LinearIndicatorBar _ultimateBar;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void InitPanel(string characterClass)
    {
        var userData = MainScene.instance.userData;
        var gold = userData.gold;
        
        levelText.text = $"Lvl. 1";
        goldText.text = gold.ToString();

        characterClassName = characterClass;
        
        Addressables
            .LoadAssetAsync<UfbCharacter>("UfbCharacter/" + characterClass)
            .Completed += (op) =>
        {
            if (
                op.Status
                == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded
            )
            {
                heroAvatar.sprite = op.Result.avatar;
            }
            else
                Debug.LogError(
                    "Failed to load character avatar: " + op.OperationException.Message
                );
        };
        
        OnHeroDetailClick();
        gameObject.SetActive(true);
    }

    public async void OnHeroDetailClick()
    {
        skillTreeBtnImage.sprite = unselectedImage;
        heroDetailImage.sprite = selectedImage;
        
        await GetHeroDetail(characterClassName);
        
        heroStatusPart.SetActive(true);
        skillTreePart.SetActive(false);
    }

    public void OnHeroSkillTreeClick()
    {
        skillTreeBtnImage.sprite = selectedImage;
        heroDetailImage.sprite = unselectedImage;
        
        heroStatusPart.SetActive(false);
        skillTreePart.SetActive(true);
    }
    
    public async Task GetHeroDetail(string characterClass)
    {
        var userId = MainScene.instance.userData.id;

        Debug.Log($"------------{characterClass} : {userId} ");
        
        var data = await ServiceLocator.Current.Get<NetworkService>().GetHeroDetail(userId, characterClass);
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
            
            InitDetailInfo(data);
        }
        else
        {
            MainScene.instance.ShowNotificationMessage("error", "Server error occured.");
        }
    }

    void InitDetailInfo(HeroData data)
    {
        lossesText.text = data.losses.ToString();
        winsText.text = data.wins.ToString();
        killsText.text = data.kills.ToString();
        battlesText.text = data.battles.ToString();
        damageTakenText.text = data.damage_taken.ToString();
        itemBagsText.text = data.item_bags.ToString();
        usedEnergiesText.text = data.used_energies.ToString();
        damageDealText.text = data.damage_deal.ToString();
        usedStacksText.text = data.used_stacks.ToString();
        damageHealText.text = data.damage_heal.ToString();
        collectGoldText.text = data.collect_golds.ToString();
        traveledTilesText.text = data.traveled_tiles.ToString();
        chestsText.text = data.chests.ToString();

        _healthBar.SetCharacterState(40, 40);
        _energyBar.SetCharacterState(20, 20);
        _ultimateBar.SetCharacterState(0, 100);
    }
}
