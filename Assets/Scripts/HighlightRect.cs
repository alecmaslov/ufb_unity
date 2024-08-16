using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFB.Map;
using UFB.Character;
using UFB.StateSchema;

public class HighlightRect : MonoBehaviour
{
    public static HighlightRect Instance;

    public Transform highlightItem;
    public List<Transform> highlightObjects = new List<Transform>();

    List<UFB.Character.CharacterController> monsters = new List<UFB.Character.CharacterController>();

    public UFB.Character.CharacterController selectedMonster;

    public int selectedIdx = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for(int i = 0; i < 100; i ++)
        {
            Transform obj = Instantiate(highlightItem, transform);
            highlightObjects.Add(obj);
        }
    }

    public void SetHighLightRect(List<Tile> tiles)
    {
        ClearHighLightRect();

        int k = 0;
        foreach(var tile in tiles)
        {
            TileState state = tile.GetTileState();
            Debug.Log($"index : {k}, x : {tile.GetTileState().coordinates.x}, y : {tile.GetTileState().coordinates.y}");
            highlightObjects[k].transform.position = tile.transform.position;

            if(state.type == "Upper")
            {
                highlightObjects[k].transform.position += Vector3.up * 0.3f;
            } 
            else
            {
                highlightObjects[k].transform.position += Vector3.up * 0.1f;
            }

            CharacterManager.Instance.monsterKeys.ForEach(key => {
                UFB.Character.CharacterController monster = CharacterManager.Instance.GetMonsterFromId(key);
                if (monster.CurrentTile.Id == tile.Id) 
                {
                    highlightObjects[k].gameObject.SetActive(true);
                    monsters.Add(monster);
                }
            });
            highlightObjects[k].gameObject.SetActive(true);

            k++;
        }

        if (monsters.Count > 0) 
        { 
            selectedMonster = monsters[selectedIdx];
        }

        UIGameManager.instance.targetScreenPanel.SetTargetImage();
    }

    public void OnSetOtherMonster()
    {
        if (monsters.Count > 0) 
        { 
            selectedIdx = (++selectedIdx) % monsters.Count;
            selectedMonster = monsters[selectedIdx];
        }
        UIGameManager.instance.targetScreenPanel.SetTargetImage();
    }

    public void ClearHighLightRect()
    {
        for (int i = 0; i < highlightObjects.Count; i++)
        {
            highlightObjects[i].gameObject.SetActive(false);
        }
        monsters.Clear();
        selectedIdx = 0;
        selectedMonster = null;
    }

    public float delay = 1f;
    private float curTime = 0;
    public float tintAmount = 0f;
    public Material tintMat;
    public float blinkSpeed;
    public float setOpacity = 1;

    private void Update()
    {
        curTime += Time.deltaTime;

        tintAmount = Mathf.Lerp(tintAmount, 0, Time.deltaTime * blinkSpeed);
        tintAmount = Mathf.Clamp(tintAmount, 0, 1);
        tintMat.color = new Color(233f / 255f, 205f / 255f, 0f, tintAmount);

        if (curTime > delay)
        {
            tintAmount = setOpacity;
            curTime = 0;
        }

    }

    private void SetHighLightObjectBlink()
    {

    }

}
