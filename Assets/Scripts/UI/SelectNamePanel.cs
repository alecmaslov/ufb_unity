using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectNamePanel : MonoBehaviour
{
    public Text nameText1;
    public Text nameText2;
    
    public Vector3 uiPosition;
    public float uiWidth;
    
    public Transform target;
    public RectTransform uiRect;
    public Camera cam;

    public Vector3 offset;
    
    public void UpdateTarget( Transform _target, string _name )
    {
        target = _target;
        nameText1.text = _name.ToUpper();
        nameText2.text = _name.ToUpper();
        gameObject.SetActive(true);

        uiRect.sizeDelta = new Vector2(uiWidth, uiWidth);
        
        UIGameManager.instance.SetSelectedTileEffect(target);
    }
    
    public void UpdateUIPosition()
    {
        if (target != null)
        {
            uiPosition = cam.WorldToScreenPoint( target.position );
            if (uiPosition.x < Screen.width / 2)
            {
                nameText1.transform.parent.gameObject.SetActive(false);
                nameText2.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                nameText1.transform.parent.gameObject.SetActive(true);
                nameText2.transform.parent.gameObject.SetActive(false);
            }
        }
        else
        {
            uiPosition = Vector3.zero - Vector3.left * 10000;
        }
    }
    
    
    // Update is called once per frame
    void Update()
    {
        UpdateUIPosition();
        uiRect.anchoredPosition = new Vector3(uiPosition.x, uiPosition.y, 0) + offset;
    }
    
}
