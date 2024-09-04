using System.Collections;
using System.Collections.Generic;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using Unity.VisualScripting;
using UnityEngine;

public class DiceArea : MonoBehaviour
{
    public static DiceArea instance;

    public DiceCountObject[] diceObject;
    public Transform firstPosObject;
    public Transform secondPosObject;

    public Transform thirdPosObject;
    public Transform fourthPosObject;

    public DICE_TYPE diceType;
    public int diceResultCount = -1;
    private bool isEnemyDiceTurn = false;

    private void Awake()
    {
        instance = this;
    }

    public void SetDiceType(DICE_TYPE _diceType, bool isEnemyTurn = false)
    {
        foreach(var ob in diceObject) {
            ob.gameObject.SetActive(false);
        }
        diceType = _diceType;

        Transform first = firstPosObject;
        Transform second = secondPosObject;

        if (isEnemyTurn) 
        { 
            first = thirdPosObject;
            second = fourthPosObject;
        }

        if(diceType == DICE_TYPE.DICE_6 || diceType == DICE_TYPE.DICE_4)
        {
            DiceCountObject obj = diceObject[(int)diceType - 1];
            obj.transform.position = first.position + Vector3.forward * 1.6f;
            obj.rigidbody.velocity = Vector3.zero;
            obj.rigidbody.isKinematic = true;
            obj.transform.rotation = Quaternion.identity;
            obj.gameObject.SetActive(true);
        } 
        else if(diceType == DICE_TYPE.DICE_6_6)
        {
            DiceCountObject obj = diceObject[0];
            DiceCountObject obj1 = diceObject[2];
            obj.transform.position = first.position + Vector3.right;
            obj.rigidbody.velocity = Vector3.zero;
            obj.rigidbody.isKinematic = true;
            obj.transform.rotation = Quaternion.identity;
            obj.gameObject.SetActive(true);

            obj1.transform.position = second.position;
            obj1.rigidbody.velocity = Vector3.zero;
            obj1.rigidbody.isKinematic = true;
            obj1.transform.rotation = Quaternion.identity;
            obj1.gameObject.SetActive(true);
        } 
        else if(diceType == DICE_TYPE.DICE_6_4)
        {
            DiceCountObject obj = diceObject[0];
            DiceCountObject obj1 = diceObject[1];
            obj.transform.position = first.position + Vector3.right;
            obj.rigidbody.velocity = Vector3.zero;
            obj.rigidbody.isKinematic = true;
            obj.transform.rotation = Quaternion.identity;
            obj.gameObject.SetActive(true);

            obj1.transform.position = second.position;
            obj1.rigidbody.velocity = Vector3.zero;
            obj1.rigidbody.isKinematic = true;
            obj1.transform.rotation = Quaternion.identity;
            obj1.gameObject.SetActive(true);
        }
    }
    public DiceData[] diceData;
    public void LaunchDice(DiceData[] dices, bool isEnemy = false)
    {
        diceData = dices;
        isEnemyDiceTurn = isEnemy;
        diceResultCount = 0;
        foreach (DiceData dice in dices)
        {
            diceResultCount += dice.diceCount;
        }

        if (diceType == DICE_TYPE.DICE_6 || diceType == DICE_TYPE.DICE_4)
        {
            DiceCountObject obj = diceObject[(int)diceType - 1];
            obj.rigidbody.isKinematic = false;
            obj.isStoped = false;
            obj.LanchDiceModel(dices[0].diceCount, isEnemyDiceTurn);
        }
        else if (diceType == DICE_TYPE.DICE_6_6)
        {
            DiceCountObject obj = diceObject[0];
            DiceCountObject obj1 = diceObject[2];
            obj.rigidbody.isKinematic = false;
            obj.isStoped = false;
            obj.LanchDiceModel(dices[0].diceCount, isEnemyDiceTurn);

            obj1.rigidbody.isKinematic = false;
            obj1.isStoped = false;
            obj1.LanchDiceModel(dices[1].diceCount, isEnemyDiceTurn);
        }
        else if (diceType == DICE_TYPE.DICE_6_4)
        {
            DiceCountObject obj = diceObject[0];
            DiceCountObject obj1 = diceObject[1];
            obj.rigidbody.isKinematic = false;
            obj.isStoped = false;
            obj.LanchDiceModel(dices[0].diceCount, isEnemyDiceTurn);

            obj1.rigidbody.isKinematic = false;
            obj1.isStoped = false;
            obj1.LanchDiceModel(dices[1].diceCount, isEnemyDiceTurn);
        }
        isLaunched = true;
    }

    private bool isLaunched = false;

    private void Update()
    {
        if (isLaunched) {
            bool isChecked = diceObject[0].isStoped && diceObject[1].isStoped && diceObject[2].isStoped;
            if (isChecked)
            {
                if(isEnemyDiceTurn)
                {
                    UIGameManager.instance.attackPanel.OnFinishEnemy();
                }
                else
                {
                    if (UIGameManager.instance.stackTurnStartPanel.isStackTurn)
                    {
                        UIGameManager.instance.stackTurnStartPanel.OnFinishDice();
                    }
                    else
                    {
                        UIGameManager.instance.attackPanel.OnFinishDice();
                    }
                }
                isLaunched = false;
            }
        }

    }

}
