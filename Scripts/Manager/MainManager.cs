using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WJF;
using WJF_CodeLibrary.UIFramework;

namespace WJF
{
    public class MainManager : MonoBehaviour
    {
        // Use this for initialization
        private void Start()
        {
            Statistics.Init();
            Communication.CreateInstance();
            UIManager.Instance.ShowUIPanel<StartPanel>();
            //UIManager.Instance.ShowUIPanel<StorageStructurePanel>();
            //UIManager.Instance.ShowUIPanel<TraversePanel>();
        }
    }
}