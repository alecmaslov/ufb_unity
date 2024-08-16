using System.Collections;
using System.Collections.Generic;
using UI.ThreeDimensional;
using UnityEngine;

public class DiceRoll : MonoBehaviour
{
    public int diceCount = 1;
    public int diceType = 4;
    public UIObject3D dice3dObject;
    public Animator animator;
    public DiceCountObject diceCountObject;

    public void InitDice(int _diceCount)
    {
        diceCount = _diceCount;
        dice3dObject.TargetRotation = Vector3.zero;
        dice3dObject.GetComponent<RotateUIObject3D>().enabled = true;
        dice3dObject.GetComponent<RotateUIObject3D>().OnStartRotate();
        StartCoroutine(StopDice());
        animator.SetBool("isDice", true);
        diceCountObject.LanchDiceModel();
    }

    IEnumerator StopDice()
    {
        yield return new WaitForSeconds(0.5f);
        dice3dObject.GetComponent<RotateUIObject3D>().OnSnapBackObject();


    }

   
}
