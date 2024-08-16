using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCountObject : MonoBehaviour
{
    public int diceCount = 1;
    public Rigidbody rigidbody;
    public float speed = 20.0f;

    public Transform[] countPoints;
    public bool isStoped = false;

    public void LanchDiceModel()
    {
        rigidbody.AddForce((transform.forward + transform.up * 2.3f + Vector3.left * (0.5f - Random.Range(0f, 1f)) * 2) * speed);
        transform.rotation = Random.rotation;

    }
    // Start is called before the first frame update


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
