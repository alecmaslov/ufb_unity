using System.Collections;
using System.Collections.Generic;
using UFB.Network.RoomMessageTypes;
using UnityEngine;
using UnityEngine.UI;

public class BanStackItem : MonoBehaviour
{
    public Image stack1;
    public Image stack2;
    public Image background;
    public Image stackoperator;

    public Text count1;
    public Text count2;

    public float delayTime = 3f;
    private float transValue = 1;

    public void InitData(ToastBanStackMessage m)
    {
        transValue = delayTime;

        stack1.sprite = GlobalResources.instance.stacks[m.stack1];
        stack2.sprite = GlobalResources.instance.stacks[m.stack2];

        count1.text = m.count1.ToString();
        count2.text = m.count2.ToString();


        gameObject.SetActive(true);
    }

    public void InitPerkData(ToastStackPerkMessage m)
    {
        transValue = delayTime;

        stack1.sprite = GlobalResources.instance.perks[m.perkId];
        stack2.sprite = GlobalResources.instance.stacks[m.stackId];

        count2.text = m.count.ToString();

        gameObject.SetActive(true);
    }

    public void InitStackItemData(ToastBanStackMessage m)
    {
        transValue = delayTime;

        stack1.sprite = GlobalResources.instance.items[m.stack1];
        stack2.sprite = GlobalResources.instance.stacks[m.stack2];

        count1.text = m.count1.ToString();
        count2.text = m.count2.ToString();


        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        transValue -= Time.deltaTime;
        if (transValue < 0)
        {
            transValue = 0;
            Destroy(gameObject);
            return;
        }
        stack1.color = new Color(stack1.color.r, stack1.color.g, stack1.color.b, transValue);
        stack2.color = new Color(stack2.color.r, stack2.color.g, stack2.color.b, transValue);
        background.color = new Color(background.color.r, background.color.g, background.color.b, transValue);
        stackoperator.color = new Color(stackoperator.color.r, stackoperator.color.g, stackoperator.color.b, transValue);
    }
}
