using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCountObject : MonoBehaviour
{
    public int diceCount = 1;
    public Rigidbody rigidbody;
    public float speed = 20.0f;
    public float rotspeed = 400.0f;
    public float upHeight = 5;

    public Transform[] countPoints;
    public bool isStoped = false;

    public Animator animator;

    public void LanchDiceModel(int _diceCount, bool isEnemy = false)
    {
        rigidbody.AddForce((transform.forward ) * (isEnemy? -speed: speed));
        rigidbody.AddTorque(Random.insideUnitSphere * rotspeed);
        //transform.rotation = Random.rotation;
        isStoped = false;
        StartCoroutine(PlayAnimation(_diceCount));
    }
    // Start is called before the first frame update

    public void FinishDiceRoll()
    {
        Debug.Log("Finish animation");
        isStoped = true;
        animator.Play("dice6_0");
        animator.SetInteger("dice", 0);
        animator.enabled = false;
    }

    IEnumerator PlayAnimation(int _diceCount)
    {
        yield return new WaitForSeconds(0.7f);
        animator.enabled = true;
        animator.SetInteger("dice", _diceCount);
    }

    // Update is called once per frame
    void Update()
    {
        List<DICE_HEIGHT> worldHeights = new List<DICE_HEIGHT>();
        int k = 0;
        foreach (Transform point in countPoints)
        {
            DICE_HEIGHT dice = new DICE_HEIGHT();
            dice.height = point.position.y;
            dice.value = k + 1;
            worldHeights.Add(dice); 
            k++;
        }
        worldHeights.Sort((DICE_HEIGHT a, DICE_HEIGHT b) =>
        {
            return (b.height > a.height) ? 1 : -1;
        });

        diceCount = worldHeights[0].value;

    }

    [System.Serializable]
    public struct DICE_HEIGHT
    {
        public float height;
        public int value;
    }
}
