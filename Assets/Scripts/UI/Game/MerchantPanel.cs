using System;
using System.Collections;
using System.Collections.Generic;
using UFB.Events;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UI.ThreeDimensional;
using UnityEngine;

public class MerchantPanel : MonoBehaviour
{
    public static MerchantPanel instance;

    public BuyPanel buyPanel;
    public SellPanel sellPanel;
    public QuestPanel questPanel;
    public CraftPanel craftPanel;
    public MerchantModalPanel merchantModalPanel;
    public UIObject3D ui3DModel;

    public string tileId;

    public Vector3[] panelAngles;

    public int panelIdx = 0;

    [HideInInspector]
    public Item[] itemData;
    [HideInInspector]
    public Item[] itemData1;
    [HideInInspector]
    public Item[] itemData2;
    [HideInInspector]
    public Item[] powerData;
    [HideInInspector]
    public Item[] stackData;
    [HideInInspector]
    public Quest[] questData;
    
    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {

    }

    public void InitMerchantData(GetMerchantDataMessage message)
    {
        Debug.Log( "========>>>>" );
        Debug.Log(message);
        itemData = message.items;
        itemData1 = message.items1;
        itemData2 = message.items2;
        powerData = message.powers;
        stackData = message.stacks;
        questData = message.quests;

        panelIdx = 0;
        tileId = message.tileId;
        InitData();
    }

    public void InitData()
    {
        ui3DModel.TargetRotation = panelAngles[0];
        buyPanel.ClearBoughtItems();
        questPanel.InitData();
        buyPanel.InitData();
        sellPanel.InitData();
        craftPanel.InitData();
        questPanel.gameObject.SetActive(true);
        merchantModalPanel.gameObject.SetActive(false);

        UpdateIdx(0);
        gameObject.SetActive(true);
    }

    public void UpdateIdx(int idx)
    {
        panelIdx += idx;
        if (panelIdx < 0) 
        { 
            panelIdx = 0;
        }
        panelIdx %= 4;

        OnOpenPanel();
    }

    public void OnOpenPanel()
    {
        questPanel.gameObject.SetActive(false);
        buyPanel.gameObject.SetActive(false);
        craftPanel.gameObject.SetActive(false);
        sellPanel.gameObject.SetActive(false);


        if (panelIdx == 0 || panelIdx == 4)
        {
            questPanel.InitData();
            questPanel.gameObject.SetActive(true);
        }
        else if (panelIdx == 1)
        {
            buyPanel.ClearBoughtItems();
            buyPanel.InitData();
            buyPanel.gameObject.SetActive(true);
        }
        else if (panelIdx == 2)
        {
            craftPanel.InitData();
            craftPanel.gameObject.SetActive(true);
        }
        else if (panelIdx == 3)
        {
            sellPanel.InitData();
            sellPanel.gameObject.SetActive(true);
        }
    }

    public void CloseMerchant()
    {
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                "leaveMerchant",
                new RequestTile
                {
                    tileId = tileId,
                }
            )
        );
        gameObject.SetActive( false );
    }

    private float targetAngle = -180;
    private void Update()
    {
        targetAngle = Mathf.Lerp(targetAngle, panelAngles[panelIdx].y, Time.deltaTime * 8f);
        ui3DModel.TargetRotation = new Vector3(ui3DModel.TargetRotation.x, targetAngle, ui3DModel.TargetRotation.z);
    }
}
