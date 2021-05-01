using UnityEngine;
using System.Collections.Generic;
using WJF_CodeLibrary.UIFramework;
using WJF_CodeLibrary.Extension;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace WJF
{
	public class StatisticsPanel : UIPanelBase
	{
        private Transform grid;

        public override void Init()
        {
            base.Init();

            CurrentUIType.panelType = UIPanelType.Popup;
            CurrentUIType.showType = UIShowType.Irrelevant;

            grid = transform.GetChild("Grid");

            RegisterEvent("ButtonOK", EventType.Click_Left, p => OnComplete());
        }

        protected override void OnShow()
        {
            base.OnShow();

            //显示统计数据
            List<Statistics.StudyItem> list = Statistics.GetItemsCounter();
            for (int i = 0, j = 0; i < list.Count; i++, j += 2)
            {
                Text textName = grid.GetChild(j).GetComponentInChildren<Text>();
                Text textCount = grid.GetChild(j + 1).GetComponentInChildren<Text>();

                textName.text = list[i].title;
                textCount.text = list[i].wrongCount.ToString();
            }
        }

        /// <summary>
        /// 结束
        /// </summary>
        private void OnComplete()
        {
            HidePanel<StatisticsPanel>();
            PanelActivator.MessageBox("实验结束", () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });
        }
    }
}