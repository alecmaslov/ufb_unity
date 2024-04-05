using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepPanel : MonoBehaviour
{
    [SerializeField] 
    MovePanel movePanel;

    [SerializeField]
    GameObject resourcePanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMoveBtn()
    {
        resourcePanel.SetActive(false);
        gameObject.SetActive(false);
        movePanel.Show();
    }

    public void OnHitBtn()
    {

    }

    public void OnConfirmBtn()
    {
        gameObject.SetActive(false);
    }

    public void OnStopBtn()
    {

    }
}
