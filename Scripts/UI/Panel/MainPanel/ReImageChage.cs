using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReImageChage: MonoBehaviour 
{
    private Image image;

    private bool isEnter = false;
    
    // Use this for initialization
    void Start()
    {
        image = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Transform t = this.transform.GetOverUI();

        if (isEnter)
        {
            if( t != this.transform)
            {
                image.overrideSprite = Resources.Load("Prefab/NewMainUI/R", typeof(Sprite)) as Sprite;
                isEnter = false;
            }
        }
        else
        {
            if(t == this.transform)
            {
                image.overrideSprite = Resources.Load("Prefab/NewMainUI/RPro", typeof(Sprite)) as Sprite;
                isEnter = true;
            }
        }

    }

}


public static class Tool
{
    public static Transform GetOverUI(this Transform trans)
    {
        Transform obj = null;

        EventSystem uiEventSystem = EventSystem.current;
        if (uiEventSystem != null)
        {
            PointerEventData uiPointerEventData = new PointerEventData(uiEventSystem);
            uiPointerEventData.position = Input.mousePosition;

            List<RaycastResult> uiRaycastResultCache = new List<RaycastResult>();

            uiEventSystem.RaycastAll(uiPointerEventData, uiRaycastResultCache);
            if (uiRaycastResultCache.Count > 0)
            {
                obj = uiRaycastResultCache[0].gameObject.transform;
            }
        }

        return obj;
    }
}


