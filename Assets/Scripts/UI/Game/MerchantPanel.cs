using System.Collections;
using System.Collections.Generic;
using UFB.Events;
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

    public Animator animator;

    public Vector3[] panelAngles;

    public int panelIdx = 0;

    [HideInInspector]
    public Item[] itemData;
    [HideInInspector]
    public Item[] powerData;
    [HideInInspector]
    public Item[] stackData;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        UFB.Events.EventBus.Publish(
            RoomSendMessageEvent.Create(
                "getMerchantData",
                new RequestGetMerchantList
                {
                }
            )
        );
    }

    public void InitMerchantData(GetMerchantDataMessage message)
    {
        Debug.Log( "========>>>>" );
        Debug.Log(message);
        Debug.Log(message.items.Length);
        itemData = message.items;
        powerData = message.powers;
        stackData = message.stacks;
        InitData();
    }

    public void InitData()
    {
        ui3DModel.TargetRotation = panelAngles[0];
        questPanel.InitData();
        buyPanel.InitData();
        sellPanel.InitData();
        craftPanel.InitData();
        gameObject.SetActive(true);
    }

    public void UpdateIdx(int idx)
    {
        panelIdx += idx;
        if (panelIdx < 0) 
        { 
            panelIdx = 0;
        }
        panelIdx %= 5;

        OnOpenPanel();

        animator.SetInteger("panelIdx", panelIdx);
        if(panelIdx == 4)
        {
            StartCoroutine(resetValue());
        }
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

    IEnumerator resetValue()
    {
        yield return new WaitForSeconds(0.5f);
        panelIdx = 0;
        animator.SetInteger("panelIdx", panelIdx);
    }
}
