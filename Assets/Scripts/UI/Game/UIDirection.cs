using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDirection : MonoBehaviour, IPointerClickHandler
{
    public static UIDirection instance;

    public SpriteRenderer[] dirctions;

    public SpriteRenderer[] moveItems;

    public GameObject posObject;

    public GameObject moveItemObject;

    private bool isLeft = true;
    private bool isRight = true;
    private bool isTop = true;
    private bool isDown = true;

    private bool isItemTop = true;
    private bool isItemDown = true;
    private bool isItemLeft = true;
    private bool isItemRight = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void InitInstance()
    {
        if (instance == null)
            instance = this;
    }

    public void InitPosDirection(Sprite left, Sprite right, Sprite top, Sprite down, bool _isLeft, bool _isRight, bool _isTop, bool _isDown)
    {
        dirctions[0].sprite = top;
        dirctions[1].sprite = down;
        dirctions[2].sprite = left;
        dirctions[3].sprite = right;

        isLeft = _isLeft;
        isRight = _isRight;
        isTop = _isTop;
        isDown = _isDown;

        dirctions[0].gameObject.SetActive(isTop);
        dirctions[1].gameObject.SetActive(isDown);
        dirctions[2].gameObject.SetActive(isLeft);
        dirctions[3].gameObject.SetActive(isRight);

        posObject.SetActive(true);
        moveItemObject.SetActive(false);
    }

    public void InitItemDirection(Sprite left, Sprite right, Sprite top, Sprite down)
    {
        moveItems[0].sprite = top;
        moveItems[1].sprite = down;
        moveItems[2].sprite = left;
        moveItems[3].sprite = right;

        isItemTop = top != null;
        isItemDown = down != null;
        isItemLeft = left != null;
        isItemRight = right != null;

        posObject.SetActive(false);
        moveItemObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CLiCK DIRECTION DEVET");
    }
}
