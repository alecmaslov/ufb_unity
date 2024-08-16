using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceArea : MonoBehaviour
{
    public static DiceArea instance;

    public DiceCountObject[] diceObject;
    public Transform firstPosObject;

    public int diceType;

    private void Awake()
    {
        instance = this;
    }

    public void SetDiceType(int _diceType)
    {
        diceType = _diceType;
        DiceCountObject obj = diceObject[diceType];
        obj.transform.position = firstPosObject.position;
        obj.rigidbody.velocity = Vector3.zero;
        obj.rigidbody.isKinematic = true;
        obj.transform.rotation = Quaternion.identity;
        obj.gameObject.SetActive(true);
    }


    public void LaunchDice()
    {
        DiceCountObject obj = diceObject[diceType];

        if (obj != null) 
        { 
            obj.rigidbody.isKinematic = false;
            obj.isStoped = false;
            obj.LanchDiceModel();
        }
    }


}
