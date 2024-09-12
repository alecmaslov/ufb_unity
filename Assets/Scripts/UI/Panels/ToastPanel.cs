using System.Collections;
using System.Collections.Generic;
using UFB.Network.RoomMessageTypes;
using UnityEngine;

public class ToastPanel : MonoBehaviour
{
    public BanStackItem banStackItem;
    public PunchPopupItem punchPopupItem;
    public BanStackItem banItemStack;
    public BanStackItem perkStackItem;
    public Transform toastParent;

    public void InitBanStackMessage(ToastBanStackMessage m)
    {
        BanStackItem item = Instantiate(banStackItem, toastParent);
        item.InitData(m);
    }

    public void InitPerkPopupMessage(ToastPerkMessage m)
    {
        PunchPopupItem item = Instantiate(punchPopupItem, toastParent);
        item.InitData(m);
    }

    public void InitStackPerkMessage(ToastStackPerkMessage m)
    {
        Debug.Log("ACTIVE STEADY STACK" + m.ToString());
        BanStackItem item = Instantiate(perkStackItem, toastParent);
        item.InitPerkData(m);
    }

    public void InitStackItemMessage(ToastBanStackMessage m)
    {
        BanStackItem item = Instantiate(banItemStack, toastParent);
        item.InitStackItemData(m);
    }

}
