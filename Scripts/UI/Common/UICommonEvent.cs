using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WJF_CodeLibrary.UIFramework;
using UnityEngine.SceneManagement;

namespace WJF
{
	public class UICommonEvent
	{	
		public static void Return()
        {
            PanelActivator.MessageBox("是否返回上一页重新学习？",
                () =>
                {
                    UIManager.Instance.Return();
                }, () => { });
        }

        public static void Next<T>() where T : UIPanelBase
        {
            PanelActivator.MessageBox("是否进入下一页", () =>
            {
                UIManager.Instance.ShowUIPanel<T>();
            }, () => { });
        }

        public static void Quit()
        {
            PanelActivator.MessageBox("确定退出？当前学习进度将被清空。",
                () =>
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }, () => { });
        }
	}
}