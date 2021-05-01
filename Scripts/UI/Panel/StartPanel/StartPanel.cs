using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.UIFramework;
using UnityEngine.SceneManagement;

namespace WJF
{
    public class StartPanel : UIPanelBase
    {
        //是否已验证token
        bool isToken = false;

        public RightPanel rightPanel;

        //灯光控制物体
        private GameObject LightControl;

        //进度条相关
        public GameObject loadUI;
        public Slider sli;
        public Text loadText;
        float targetValue = 0;

        public Color clrBtnNormal;//普通颜色
        public Color clrBtnSelected;//选中颜色

        private enum SwitchType { Purpose, Content }
        private SwitchType type;

        private GameObject textPurpose;
        private GameObject textContent;
        private Button btnPurpose;
        private Button btnContent;

        public GameObject startButton;

        public override void Init()
        {
            base.Init();

            LightControl = GameObject.Find("LightControl");

            CurrentUIType.panelType = UIPanelType.Normal;
            CurrentUIType.showType = UIShowType.Relevant;

            RegisterEvent("Start", EventType.Click_Left, p =>
            {
#if UNITY_EDITOR
                doStart();
#endif
#if UNITY_WEBGL                           //打包WebGL需要解开下面代码--------！！！！！
                if (isToken)
                    doStart();
                else
                    Communication.Instance.CheckToken(doStart);
#endif
            });
        }


        /// <summary>
        /// 加载资源进度条显示
        /// </summary>
        /// <param name="pro"></param>
        /// <param name="hintStr"></param>
        void OnProgress(float pro, string hintStr)
        {
            sli.value = pro;
            targetValue = pro;
            if (pro >= 0.9f)
            {
                //值最大为0.9
                targetValue = 1.0f;
            }
            //为滑动条赋值
            if (targetValue != sli.value)
            {
                sli.value = Mathf.Lerp(sli.value, targetValue, Time.deltaTime * 3);
                if (Mathf.Abs(sli.value - targetValue) < 0.01f)
                {
                    sli.value = targetValue;
                }
                loadText.text = hintStr;
            }
        }

        /// <summary>
        /// 加载资源完毕执行的逻辑
        /// </summary>
        private void LoadOver()
        {
            GameObject djScene = ABManager.Instance.GetObj<GameObject>("dj");
            GameObject floydScene = ABManager.Instance.GetObj<GameObject>("floyd");
            GameObject bellManScene = ABManager.Instance.GetObj<GameObject>("bellman-ford");

            Instantiate(djScene);
            Instantiate(floydScene);
            Instantiate(bellManScene);

            loadUI.SetActive(false);
            LightControl.SetActive(true);

            ShowPanel<MainPanel>();
        }

        private void doStart()
        {
            isToken = true;
            Statistics.startTime = Communication.Instance.GetTimeStamp();
            //Debug.LogError("0");
            LightControl.SetActive(false);
            if (!ABManager.Instance.isLoad)
            {
                loadUI.SetActive(true);
                startButton.SetActive(false);
                ABManager.Instance.LoadAsyncAB(OnProgress, LoadOver);
            }
            else
            {
                IsLoaded();
            }

        }

        void IsLoaded()
        {
            loadUI.SetActive(false);
            LightControl.SetActive(true);

            ShowPanel<MainPanel>();
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

        // /// <summary>
        // /// 切换显示
        // /// </summary>
        // /// <param name="type"></param>
        // private void SwitchDisplay(SwitchType type)
        // {
        //     textPurpose.SetActive(type == SwitchType.Purpose);
        //     textContent.SetActive(type == SwitchType.Content);

        //     SwitchButton(type);
        // }

        // /// <summary>
        // /// 切换按键效果
        // /// </summary>
        // /// <param name="type"></param>
        // private void SwitchButton(SwitchType type)
        // {
        //     ColorBlock cb;

        //     cb = btnPurpose.colors;
        //     cb.normalColor = type == SwitchType.Purpose ? clrBtnSelected : clrBtnNormal;
        //     btnPurpose.colors = cb;

        //     cb = btnContent.colors;
        //     cb.normalColor = type == SwitchType.Content ? clrBtnSelected : clrBtnNormal;
        //     btnContent.colors = cb;

        //     btnPurpose.targetGraphic.raycastTarget = !(type == SwitchType.Purpose);
        //     btnContent.targetGraphic.raycastTarget = !(type == SwitchType.Content);
        // }
    }
}