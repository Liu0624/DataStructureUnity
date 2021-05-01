using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.UIFramework;

namespace WJF
{
    public class MainPanel : UIPanelBase
    {

        public Color clrBtnNormal;//普通颜色
        public Color clrBtnSelected;//选中颜色

        private enum SwitchType { Purpose, Content }
        private SwitchType type;

        private GameObject textPurpose;
        private GameObject textContent;
        private Button btn1, btn2, btn3, btn4, btn5, btn6, btn7;

        public override void Init()
        {
            base.Init();

            CurrentUIType.panelType = UIPanelType.Normal;
            CurrentUIType.showType = UIShowType.Relevant;

            textPurpose = gameObject.GetChild("TextPurpose");
            textContent = gameObject.GetChild("TextContent");

            btn1 = gameObject.GetComponentInChildrenByName<Button>("Button1");
            btn2 = gameObject.GetComponentInChildrenByName<Button>("Button2");
            btn3 = gameObject.GetComponentInChildrenByName<Button>("Button3");
            btn4 = gameObject.GetComponentInChildrenByName<Button>("Button4");
            btn5 = gameObject.GetComponentInChildrenByName<Button>("Button5");
            btn6 = gameObject.GetComponentInChildrenByName<Button>("Button6");

            btn1.onClick.AddListener(() => ShowPanel<VertexEdgePanel>("VertexEdgePanel"));
            btn2.onClick.AddListener(() =>
            {
                RenderSettings.skybox = ABManager.Instance.GetObj<Material>("Skybox10");
                ShowPanel<DjPanel>("DjPanel");
            });
            btn3.onClick.AddListener(() =>
            {
                RenderSettings.skybox = ABManager.Instance.GetObj<Material>("Skybox10");
                ShowPanel<FloydPanel>("FloydPanel");
            });
            btn4.onClick.AddListener(() =>
            {
                RenderSettings.skybox = ABManager.Instance.GetObj<Material>("Skybox14");
                ShowPanel<BellmanFordPanel>("BellmanFordPanel");
            });
            btn5.onClick.AddListener(() => ShowPanel<ResultPanel>("ResultPanel"));
            btn6.onClick.AddListener(() => ShowPanel<StartPanel>("StartPanel"));
        }

        protected override void OnShow()
        {
            base.OnShow();

            // SwitchDisplay(SwitchType.Purpose);
        }

        protected override void OnHide()
        {
            base.OnHide();
        }

        /// <summary>
        /// 切换显示
        /// </summary>
        /// <param name="type"></param>
        private void SwitchDisplay(SwitchType type)
        {
            // textPurpose.SetActive(type == SwitchType.Purpose);
            // textContent.SetActive(type == SwitchType.Content);

            SwitchButton(type);
        }

        /// <summary>
        /// 切换按键效果
        /// </summary>
        /// <param name="type"></param>
        private void SwitchButton(SwitchType type)
        {
            ColorBlock cb;

            cb = btn1.colors;
            cb.normalColor = type == SwitchType.Purpose ? clrBtnSelected : clrBtnNormal;
            btn1.colors = cb;

            cb = btn2.colors;
            cb.normalColor = type == SwitchType.Content ? clrBtnSelected : clrBtnNormal;
            btn2.colors = cb;

            btn1.targetGraphic.raycastTarget = !(type == SwitchType.Purpose);
            btn2.targetGraphic.raycastTarget = !(type == SwitchType.Content);
        }
    }
}