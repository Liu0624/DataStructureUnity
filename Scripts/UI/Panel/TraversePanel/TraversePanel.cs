using WJF_CodeLibrary.UIFramework;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.CommonUtility.UI;
using UnityEngine;

namespace WJF
{
	public class TraversePanel : UIPanelBase
    {
        public int wrongBFS;
        public int wrongDFS;

        private DepthFirstSearch dfsTraverse;
        private BreadthFirstSearch bfsTraverse;
        private TraverseVariables variables;

        private TraverseBase traverse;

        public RectTransform rect;
        public GameObject bg;
        public GameObject hint;
        bool isShow = false;


        public Button btnStart;
        public Button btnFull;
        public Button btnNoFull;
        public Button btnBack;


        public void TravelQuit()
        {
            PanelActivator.MessageBox("返回后遍历学习记录将清空，确认返回吗？", () =>
            {
                Statistics.setScore(wrongBFS, SysDefine.Statistics.BFS_Input);
                Statistics.setScore(wrongDFS, SysDefine.Statistics.DFS_Input);

                Debug.Log(wrongBFS);

                ShowPanel<MainPanel>();
            }, () => { });
        }

        public override void Init()
        {
            base.Init();

            CurrentUIType.panelType = UIPanelType.Normal;
            CurrentUIType.showType = UIShowType.Relevant;

            dfsTraverse = GetComponentInChildren<DepthFirstSearch>();
            bfsTraverse = GetComponentInChildren<BreadthFirstSearch>();
            variables = GetComponentInChildren<TraverseVariables>();

            variables.RegisterRunAction(OnBtnRunClicked);
            variables.RegisterUndoAction(OnBtnUndoClicked);
            variables.RegisterResetAction(OnBtnResetClicked);
            variables.RegisterSwitchAction(OnBtnSwitchClicked);

            btnStart.onClick.AddListener(() => { OnBtnStartClick(); });
            btnFull.onClick.AddListener(() => { OnBtnFullClick(); });
            btnNoFull.onClick.AddListener(() => { OnBtnNoFullClick(); });
            btnBack.onClick.AddListener(() => { OnBtnBackClick(); });

            InitWrong();

            RegisterEvent("BtnPreviousPanel", EventType.Click_Left, p => UICommonEvent.Return());
            RegisterEvent("BtnQuit", EventType.Click_Left, p => UICommonEvent.Quit());

            
        }

        private void InitWrong()
        {
            wrongBFS = Statistics.GetWrongCount(SysDefine.Statistics.BFS_Input);
            wrongDFS = Statistics.GetWrongCount(SysDefine.Statistics.DFS_Input);

            Debug.Log("wrongDFS" + wrongDFS);

            Statistics.ResetWrong(SysDefine.Statistics.BFS_Input);
            Statistics.ResetWrong(SysDefine.Statistics.DFS_Input);
        }

        protected override void OnShow()
        {
            base.OnShow();

            traverse = dfsTraverse;
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
                string preTraverseName = traverse.Name;

                if (traverse is DepthFirstSearch)
                    traverse = bfsTraverse;
                else
                    traverse = dfsTraverse;

                bool isAllEnd = traverse.IsTraverseEnd();

                string msg = "";

                if (traverse is DepthFirstSearch && isAllEnd )
                {
                    msg = string.Format("{0:G}结束，\n图的遍历方法学习完毕。返回主页继续学习", preTraverseName);
                    bfsTraverse.setStart();
                    dfsTraverse.setStart();
                }
                else if(traverse is BreadthFirstSearch && !isAllEnd)
                {
                    msg = string.Format("{0:G}结束，\n继续学习{1:G}。", preTraverseName, traverse.Name);
                }
                
                print("xxx-- 深度优先结束");

                

                if (isAllEnd){
                    Statistics.setComplete(SysDefine.Statistics.BFS_Input);
                }else{
                    Statistics.setComplete(SysDefine.Statistics.DFS_Input);
                }

                PanelActivator.MessageBox(msg, () =>
                {
                    if (isAllEnd)
                    {
                        wrongBFS = Statistics.GetWrongCount(SysDefine.Statistics.BFS_Input);
                        wrongDFS = Statistics.GetWrongCount(SysDefine.Statistics.DFS_Input);
                        ShowPanel<MainPanel>();
                    }
                    else
                    {
                        ResetAll();
                    }
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
        /// 后退按键事件
        /// </summary>
        private void OnBtnUndoClicked()
        {
            traverse.Undo();
            variables.Undo();
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
        /// 切换按键事件
        /// </summary>
        private void OnBtnSwitchClicked()
        {
            PanelActivator.MessageBox("切换后当前遍历进度将被重置，\n是否继续？", () =>
            {
                if (traverse is DepthFirstSearch)
                    traverse = bfsTraverse;
                else
                    traverse = dfsTraverse;

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

        private void Update()
        {
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
            TravelQuit();
            // UIManager.Instance.Return();
        }
    }
}