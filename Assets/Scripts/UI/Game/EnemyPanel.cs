using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.StateSchema;
using UFB.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class EnemyPanel : MonoBehaviour
{
    public Transform enemyList;
    public ItemCard enemyCard;

    public TopHeader selectedCharacter;
    
    public GameObject normalPanel;
    public GameObject detailPanel;

    public Text titleText;
    
    public int selectedIdx = 0;
    
    public void InitEnemy(int type = 0)
    {
        normalPanel.SetActive(true);
        detailPanel.SetActive(false);
        for (int i = 1; i < enemyList.childCount; i++)
        {
            Destroy(enemyList.GetChild(i).gameObject);
        }

        foreach (var character in CharacterManager.Instance.GetCharacterList())
        {
            CharacterState state = CharacterManager.Instance.GetCharacterFromId(character.Value.Id).State;
            
            if ((type == 0 && state.stats.health.current > 0 && state.id != CharacterManager.Instance.PlayerCharacter.Id) || (type == 1 && state.stats.health.current == 0))
            {
                AddEnemyItem(state, character.Value.transform.position);
            }
        }

        titleText.text = selectedIdx == 0 ? "ENEMIES" : "KILLS";
        
        gameObject.SetActive(true);
    }

    public void OnNextPanel(int val)
    {
        selectedIdx += val;
        selectedIdx %= 2;
        
        InitEnemy(selectedIdx);
    }

    private void AddEnemyItem(CharacterState state, Vector3 pos)
    {
        var enemyItem = Instantiate(enemyCard, enemyList);
            
        Addressables.LoadAssetAsync<UfbCharacter>("UfbCharacter/" + state.characterClass)
            .Completed += (op) =>
        {
            if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                enemyItem.InitImage(op.Result.avatar);
            }
            else
                Debug.LogError("Failed to load character avatar: " + op.OperationException.Message);
        };

        enemyItem.GetComponent<Button>().onClick.AddListener(() =>
        {
            selectedCharacter.OnSelectedCharacterEvent(state);
            normalPanel.SetActive(false);
            detailPanel.SetActive(true);
                
            CameraManager.instance.cameraTarget.position = pos;
        });
            
        enemyItem.gameObject.SetActive(true);
    }
    
}
