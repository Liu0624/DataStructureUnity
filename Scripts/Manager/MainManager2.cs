using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WJF;
using WJF_CodeLibrary.UIFramework;

namespace WJF
{
    public class MainManager2 : MonoBehaviour
    {
        // Use this for initialization
        private void Start()
        {
            Statistics.Init();
            UIManager.Instance.ShowUIPanel<DjPanel>();
            //UIManager.Instance.ShowUIPanel<StorageStructurePanel>();
            //UIManager.Instance.ShowUIPanel<TraversePanel>();
        }
    }
}