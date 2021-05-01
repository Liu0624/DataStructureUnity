using UnityEngine;
using WJF_CodeLibrary.UIFramework;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.CommonUtility.UI;

namespace WJF
{
	public partial class StorageStructurePanel : UIPanelBase
	{
        public int wrongAL;
        public int wrongAD;

        private Transform graphParent;
        private Transform adjacencyListParent;
        private Transform programWritingParent;
        private AdjacencyGroup[] groups;

        public RectTransform rect;
        public GameObject bg;
        public GameObject hint;
        bool isShow = false;


        public Button btnStart;
        public Button btnFull;
        public Button btnNoFull;
        public Button btnBack;


        private GraphType graphType;

        //void Update () {
        //    if(Input.GetKey(KeyCode.S)) {
        //        ShowPanel<TraversePanel>();
        //    }

        //    if (Input.GetKey(KeyCode.Q))
        //    {
        //        StoreQuit();
        //    }
        //}

        public void StoreQuit()
        {
            PanelActivator.MessageBox("返回后本模块学习记录将清空，确认返回吗？", () =>
            {
                Statistics.setScore(wrongAD, SysDefine.Statistics.AdjacencyListDigraph);
                Statistics.setScore(wrongAL, SysDefine.Statistics.AdjacencyList);
                //HidePanel<VertexEdgePanel>();
                ShowPanel<MainPanel>();
            }, () => { });
        }

        public override void Init()
        {
            base.Init();

            CurrentUIType.panelType = UIPanelType.Normal;
            CurrentUIType.showType = UIShowType.Relevant;

            graphParent = transform.Find("bottom/GraphParent");
            adjacencyListParent = transform.Find("bottom/AdjacencyListParent");

            btnStart.onClick.AddListener(() => { OnBtnStartClick(); });
            btnFull.onClick.AddListener(() => { OnBtnFullClick(); });
            btnNoFull.onClick.AddListener(() => { OnBtnNoFullClick(); });
            btnBack.onClick.AddListener(() => { OnBtnBackClick(); });


            groups = adjacencyListParent.GetComponentsInChildren<AdjacencyGroup>(true);

            InitWrong();

            RegisterEvent("bottom/BtnSubmit", EventType.Click_Left, p => OnSubmitClicked());
        }

        private void InitWrong()
        {
            wrongAD = Statistics.GetWrongCount(SysDefine.Statistics.AdjacencyListDigraph);
            wrongAL = Statistics.GetWrongCount(SysDefine.Statistics.AdjacencyList);

            Statistics.ResetWrong(SysDefine.Statistics.AdjacencyListDigraph);
            Statistics.ResetWrong(SysDefine.Statistics.AdjacencyList);
        }

        protected override void OnShow()
        {
            base.OnShow();

            ResetPanel();
            Switch(GraphType.Digraph);
        }

        /// <summary>
        /// 重置面板
        /// </summary>
        private void ResetPanel()
        {
            foreach (var group in groups)
            {
                group.Clear();
            }
        }

        /// <summary>
        /// 切换图的显示
        /// </summary>
        /// <param name="type"></param>
        private void Switch(GraphType type)
        {
            graphType = type;

            string strGraphName;
            switch (type)
            {
                case GraphType.Digraph:
                    strGraphName = "有向图";
                    AdjacencyInfo.isDigraph = true;
                    break;
                case GraphType.Undigraph:
                    strGraphName = "无向图";
                    AdjacencyInfo.isDigraph = false;
                    break;
                default:
                    strGraphName = string.Empty;
                    break;
            }

            //切换图的显示，图显示在3d界面此处注释
            transform.Find("Digraph").gameObject.SetActive(type == GraphType.Digraph);
            transform.Find("Undigraph").gameObject.SetActive(type == GraphType.Undigraph);
            // graphParent.gameObject.GetComponentInChildrenByName<Text>("GraphName").text = strGraphName;

            //切换对应的内容
            // adjacencyListParent.gameObject.SetActive(type == GraphType.Digraph);
            // programWritingParent.gameObject.SetActive(type == GraphType.Undigraph);
            ResetPanel();
        }

        /// <summary>
        /// 提交按键
        /// </summary>
        private void OnSubmitClicked()
        {
            switch (graphType)
            {
                case GraphType.Digraph:
                    SubmitDigraphAdjacencyList();
                    break;
                case GraphType.Undigraph:
                    SubmitUndigraphAdjacencyList();
                    break;
            }
        }

        /// <summary>
        /// 提交有向图的邻接矩阵
        /// </summary>
        private void SubmitDigraphAdjacencyList()
        {
            bool result = true;
            foreach (var group in groups)
            {
                if (!group.Validate())
                {
                    result = false;
                    break;
                }                    
            }

            string msg = result ? "有向图的邻接表填写正确。" : "有向图的邻接表填写有误。";
            PanelActivator.MessageBox(msg, () =>
            {
                if (result)
                {
                    Statistics.setComplete(SysDefine.Statistics.AdjacencyList);
                    OnDigraphAdjacencyListComplete();
                }
                else
                {
                    //记录错误数
                    Statistics.AddWrong(SysDefine.Statistics.AdjacencyList);
                }
            });
        }

         /// <summary>
        /// 提交无向图的邻接矩阵
        /// </summary>
        private void SubmitUndigraphAdjacencyList()
        {
            bool result = true;
            foreach (var group in groups)
            {
                if (!group.Validate())
                {
                    result = false;
                    break;
                }                    
            }

            string msg = result ? "无向图的邻接表填写正确。" : "无向图的邻接表填写有误。";
            PanelActivator.MessageBox(msg, () =>
            {
                if (result)
                {
                    Statistics.setComplete(SysDefine.Statistics.AdjacencyListDigraph);
                    OnUndigraphAdjacencyListComplete();
                }
                else
                {
                    //记录错误数
                    Statistics.AddWrong(SysDefine.Statistics.AdjacencyListDigraph);
                }
            });
        }

        /// <summary>
        /// 无向图邻接矩阵完成
        /// </summary>
        private void OnUndigraphAdjacencyListComplete()
        {
            PanelActivator.MessageBox("继续学习图的遍历方法。", () =>
            {
                //跳转到图的深度和广度优先搜索遍历
                ShowPanel<TraversePanel>();
            });
        }

        /// <summary>
        /// 有向图邻接矩阵完成
        /// </summary>
        private void OnDigraphAdjacencyListComplete()
        {
            PanelActivator.MessageBox("继续学习无向图的邻接表结构。", () =>
            {
                Switch(GraphType.Undigraph);
            });
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.S))
            {
                ShowPanel<TraversePanel>();
            }

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
            StoreQuit();
            // UIManager.Instance.Return();
        }
    }
}