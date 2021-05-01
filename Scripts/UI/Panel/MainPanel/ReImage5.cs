using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReImage5 : MonoBehaviour {

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
            if (t != this.transform)
            {
                image.overrideSprite = Resources.Load("Prefab/NewMainUI/Res", typeof(Sprite)) as Sprite;
                isEnter = false;
            }
        }
        else
        {
            if (t == this.transform)
            {
                image.overrideSprite = Resources.Load("Prefab/NewMainUI/ResPro", typeof(Sprite)) as Sprite;
                isEnter = true;
            }
        }

    }
}
