using WJF_CodeLibrary.UIFramework;
using UnityEngine.UI;
using UnityEngine;

namespace WJF
{
    public partial class BellmanFordPanel : UIPanelBase
    {
        public int quitWrongVI = 0;
        public int quitWrongDI = 0;

        private BellmanBreadthFirstSearch bfsTraverse;
        private BellmanTraverseVariables variables;

        private BellmanTraverseBase traverse;

        public RectTransform rect;
        public GameObject bg;
        public GameObject hint;
        bool isShow = false;


        public Button btnStart;
        public Button btnFull;
        public Button btnNoFull;
        public Button btnBack;


        public override void Init()
        {
            base.Init();

            CurrentUIType.panelType = UIPanelType.Normal;
            CurrentUIType.showType = UIShowType.Relevant;

            bfsTraverse = GetComponentInChildren<BellmanBreadthFirstSearch>();
            variables = GetComponentInChildren<BellmanTraverseVariables>();

            variables.RegisterRunAction(OnBtnRunClicked);
            variables.RegisterResetAction(OnBtnResetClicked);

            btnStart.onClick.AddListener(() => { OnBtnStartClick(); });
            btnFull.onClick.AddListener(() => { OnBtnFullClick(); });
            btnNoFull.onClick.AddListener(() => { OnBtnNoFullClick(); });
            btnBack.onClick.AddListener(() => { OnBtnBackClick(); });

            InitWrong();
            InitBellmanCarmeras();
            initTipText();
    
            RegisterEvent("BtnPreviousPanel", EventType.Click_Left, p => UICommonEvent.Return());
            RegisterEvent("BtnDone", EventType.Click_Left, p => UICommonEvent.Quit());
        }

        protected override void OnShow()
        {
            base.OnShow();

            traverse = bfsTraverse;
            ResetAll();
        }

        /// <summary>
        /// 运行按键事件
        /// </summary>
        private void OnBtnRunClicked()
        {
            //当前遍历已结束，则切换
            if (traverse.IsTraverseEnd())
            {
                Statistics.setComplete(SysDefine.Statistics.Bellman_Vertex_Input);
                Statistics.setComplete(SysDefine.Statistics.Bellman_Dis_Input);

                PanelActivator.MessageBox("最短路径的内容学习完毕，再次点击提交返回主界面。", () =>
                        {
                            SetCamerasNotEnable();
                            ShowPanel<MainPanel>();
                        });
            }
            else
            {
                //执行遍历步骤
                variables.SetRunButtonText("执行");
                traverse.DoStep();
            }
        }

        /// <summary>
        /// 重置按键事件
        /// </summary>
        private void OnBtnResetClicked()
        {
            PanelActivator.MessageBox("当前遍历进度将被重置，是否继续？", () =>
            {
                ResetAll();
            }, () => { });
        }

        /// <summary>
        /// 重置
        /// </summary>
        private void ResetAll()
        {
            traverse.ResetAll();
            variables.ResetAll();
            variables.SetGraphLegend(traverse.Name);
            variables.SetRunButtonText("开始");
        }

        private void InitWrong()
        {
            quitWrongVI = Statistics.GetWrongCount(SysDefine.Statistics.Bellman_Vertex_Input);
            quitWrongDI = Statistics.GetWrongCount(SysDefine.Statistics.Bellman_Dis_Input);

            Statistics.ResetWrong(SysDefine.Statistics.Bellman_Vertex_Input);
            Statistics.ResetWrong(SysDefine.Statistics.Bellman_Dis_Input);
        }

        private void Update()
        {
            Control();

            isShow = RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition);
            if (rect.gameObject.activeSelf)
            {
                bg.SetActive(isShow);
                hint.SetActive(!isShow);
            }
        }

        void OnBtnStartClick()
        {
            UIManager.Instance.ShowUIPanel<StartPanel>("StartPanel");
        }

        void OnBtnFullClick()
        {
            Screen.fullScreen = true;
            btnFull.gameObject.SetActive(false);
            btnNoFull.gameObject.SetActive(true);

        }

        void OnBtnNoFullClick()
        {
            Screen.fullScreen = false;
            btnFull.gameObject.SetActive(true);
            btnNoFull.gameObject.SetActive(false);

        }

        void OnBtnBackClick()
        {
            BellManQuit();
            // UIManager.Instance.Return();
        }
    }
}