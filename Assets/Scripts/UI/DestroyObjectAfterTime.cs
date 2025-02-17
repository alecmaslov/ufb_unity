using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectAfterTime : MonoBehaviour
{
    public float delay = 3f;
    // Start is called before the first frame update

    Coroutine _startCoroutine;
    void Start()
    {
        _startCoroutine = StartCoroutine(OnDestroyObject());
    }

    IEnumerator OnDestroyObject()
    {
        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }

    public void OnClickDestroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_startCoroutine != null)
        {
            StopCoroutine(_startCoroutine);
        }
    }
}
