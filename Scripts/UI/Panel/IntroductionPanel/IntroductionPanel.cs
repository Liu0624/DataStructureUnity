using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.UIFramework;

namespace WJF
{
    public class IntroductionPanel : UIPanelBase
    {
        public Color clrBtnNormal;//普通颜色
        public Color clrBtnSelected;//选中颜色

        private enum SwitchType { Purpose, Content }
        private SwitchType type;

        private GameObject textPurpose;
        private GameObject textContent;
        private Button btn1, btn2, btn3, btn4, btn5;

        public override void Init()
        {
            base.Init();

            CurrentUIType.panelType = UIPanelType.Normal;
            CurrentUIType.showType = UIShowType.Relevant;

           
        }

        protected override void OnShow()
        {
            base.OnShow();

            // SwitchDisplay(SwitchType.Purpose);
        }
    }
}