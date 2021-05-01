using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WJF_CodeLibrary.UIFramework;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using DG.Tweening;
using WJF_CodeLibrary.CommonUtility;

namespace WJF
{
    public partial class VertexEdgePanel : UIPanelBase
    {
        public int wrongMD;
        public int wrongMU;

        private GraphType graphType;

        private Transform graphParent;
        private Transform collectionParent;
        private Transform matrixParent;

        private Button btnSubmit;

        private Dictionary<GraphType, bool> submitResult;
        private List<Transform> wrongList;
        private int wrongCount;

        private readonly string wrongHintName = "WrongHint";

        public RectTransform rect;
        public GameObject bg;
        public GameObject hint;
        bool isShow = false;


        public Button btnStart;
        public Button btnFull;
        public Button btnNoFull;
        public Button btnBack;

        /// <summary>
        /// 有向图顶点
        /// </summary>
        private readonly string[] digraphVertices = new string[]
        {
            //修改1
            "A", "B", "C", "D", "E","F","G"
        };

        /// <summary>
        /// 有向图边
        /// </summary>
        private readonly string[] digraphEdges = new string[]
        {
            //修改2
            "AB", "BC", "CD", "DE", "EA", "FG", "AF"
        };

        /// <summary>
        /// 无向图顶点
        /// </summary>
        private readonly string[] undigraphVertices = new string[]
        {
            //修改3
             "A", "B", "C", "D", "E","F","G"
        };

        /// <summary>
        /// 无向图边（待改）
        /// </summary>
        private readonly string[] undigraphEdges = new string[]
        {
           "AB","AE","AF","CD","DE","AG","BE"
        };


        /// <summary>
        /// 当前类型顶点
        /// </summary>
        private string[] CurrentVertices
        {
            get
            {
                return GetVertices(graphType);
            }
        }

        /// <summary>
        /// 当前类型边
        /// </summary>
        private string[] CurrentEdges
        {
            get
            {
                return GetEdges(graphType);
            }
        }

        //void Update()
        //{
        //    if (Input.GetKey(KeyCode.A))
        //    {
        //        ShowPanel<StorageStructurePanel>();
        //    }

        //    if (Input.GetKey(KeyCode.Q))
        //    {
        //        VeQuit();
        //    }
        //}

        public void VeQuit()
        {
            PanelActivator.MessageBox("返回后本模块学习记录将清空，确认返回吗？", () =>
            {
                Statistics.setScore(wrongMD, SysDefine.Statistics.MatrixInDigraph);
                Statistics.setScore(wrongMU, SysDefine.Statistics.MatrixInUndigraph);

                Debug.Log(wrongMD);

                ShowPanel<MainPanel>();
            }, () => { });
        }

        public override void Init()
        {
            base.Init();

            CurrentUIType.panelType = UIPanelType.Normal;
            CurrentUIType.showType = UIShowType.Relevant;

            graphParent = transform.Find("GraphParent");
            collectionParent = transform.Find("bottom/CollectionParent");
            //下面有用
            matrixParent = transform.Find("bottom/MatrixParent");
            btnSubmit = gameObject.GetComponentInChildrenByName<Button>("BtnSubmit");

            btnStart.onClick.AddListener(() => { OnBtnStartClick(); });
            btnFull.onClick.AddListener(() => { OnBtnFullClick(); });
            btnNoFull.onClick.AddListener(() => { OnBtnNoFullClick(); });
            btnBack.onClick.AddListener(() => { OnBtnBackClick(); });

            submitResult = new Dictionary<GraphType, bool>();
            submitResult.Add(GraphType.Digraph, false);
            submitResult.Add(GraphType.Undigraph, false);

            wrongList = new List<Transform>();

            InitGraph();
            InitCollection();
            InitMatrix();
            InitWrong();

            btnSubmit.onClick.AddListener(() => OnSubmitClicked());

            RegisterEvent("BtnNextPanel", EventType.Click_Left, p => UICommonEvent.Next<StorageStructurePanel>());
            RegisterEvent("BtnQuit", EventType.Click_Left, p => UICommonEvent.Quit());
        }

        /// <summary>
        /// 初始化错误数
        /// </summary>
        public void InitWrong()
        {
            wrongMD = Statistics.GetWrongCount(SysDefine.Statistics.MatrixInDigraph);
            wrongMU = Statistics.GetWrongCount(SysDefine.Statistics.MatrixInUndigraph);

            Statistics.ResetWrong(SysDefine.Statistics.MatrixInDigraph);
            Statistics.ResetWrong(SysDefine.Statistics.MatrixInUndigraph);
        }

        protected override void OnShow()
        {
            base.OnShow();

            ResetPanel();
            SwitchGraph(GraphType.Digraph);
        }

        /// <summary>
        /// 重置界面
        /// </summary>
        private void ResetPanel()
        {
            wrongCount = 0;

            InputField[] inputs = GetComponentsInChildren<InputField>(true);
            foreach (var input in inputs)
            {
                input.text = "";
            }

            ResetCollection();
            ResetMatrix();
        }

        /// <summary>
        /// 切换图的显示
        /// </summary>
        /// <param name="type"></param>
        private void SwitchGraph(GraphType type)
        {
            graphType = type;

            //切换图的显示
            SwitchGraphDisplay(type);

            //切换按钮效果
            SwitchSelectionButton(type);
            SwitchSubmitButton(type);

            //切换集合输入内容
            SwitchCollection(type);

            //切换矩阵输入内容
            SwitchMatrix(type);
        }

        /// <summary>
        /// 提交按键事件
        /// </summary>
        private void OnSubmitClicked()
        {
            List<Transform> result = new List<Transform>();
            // result.AddRange(ValidateCollection());
            result.AddRange(ValidateMatrix());

            //如果有错误项
            if (result.Count > 0)
            {
                wrongCount++;

                //如果错误3次以上，则直接显示正确答案
                if (wrongCount >= 3)
                {
                    PanelActivator.MessageBox("超过三次填写错误，已给出正确答案", () =>
                    {
                        //填写顶点和边
                        FillCollectionAnswer();
                        //填写矩阵
                        FillMatrixAnswer();
                    });
                }
                //如果错误3次以下，则标记
                else
                {
                    PanelActivator.MessageBox("填写错误，已标记错误项", () =>
                    {
                        result.ForEach(item =>
                        {
                            Image img = item.Find(wrongHintName).GetComponent<Image>();
                            img.color = ColorCalculateUtility.GetFadeColor(img.color, 0f);
                            img.DOFade(0.8f, 0.25f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.Linear);
                        });
                    });
                }
            }
            else
            {
                submitResult[graphType] = true;
                btnSubmit.interactable = false;

                bool isAllSubmitted = true;
                GraphType notSubmittedType = GraphType.Digraph;
                foreach (var key in submitResult.Keys)
                {
                    if (!submitResult[key])
                    {
                        isAllSubmitted = false;
                        notSubmittedType = key;
                        break;
                    }
                }

                if (isAllSubmitted)
                {
                    PanelActivator.MessageBox("图的邻接矩阵学习完毕，进入下一阶段的学习。", () =>
                    {
                        Statistics.setComplete(SysDefine.Statistics.MatrixInDigraph);
                        Statistics.setComplete(SysDefine.Statistics.MatrixInUndigraph);
                        ShowPanel<StorageStructurePanel>();
                    });
                }
                else
                {

                    if (graphType == GraphType.Digraph)
                    {
                        Statistics.setComplete(SysDefine.Statistics.MatrixInDigraph);
                    }
                    else
                    {
                        Statistics.setComplete(SysDefine.Statistics.MatrixInUndigraph);
                    }

                    string msg = string.Format("{0:G}填写完毕，继续学习{1:G}", GetGraphChineseName(graphType), GetGraphChineseName(notSubmittedType));

                    PanelActivator.MessageBox(msg, () =>
                    {
                        wrongCount = 0;
                        SwitchGraph(notSubmittedType);
                    });
                }
            }
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                ShowPanel<StorageStructurePanel>();
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
            VeQuit();
            // UIManager.Instance.Return();
        }

        /// <summary>
        /// 切换提交按键效果
        /// </summary>
        /// <param name="type"></param>
        private void SwitchSubmitButton(GraphType type)
        {
            btnSubmit.interactable = !submitResult[type];
        }


        #region 功能

        /// <summary>
        /// 获取指定图的顶点
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string[] GetVertices(GraphType type)
        {
            switch (graphType)
            {
                case GraphType.Digraph:
                    return digraphVertices;
                case GraphType.Undigraph:
                    return undigraphVertices;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取指定图的边
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string[] GetEdges(GraphType type)
        {
            switch (type)
            {
                case GraphType.Digraph:
                    return digraphEdges;
                case GraphType.Undigraph:
                    return undigraphEdges;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取图的中文名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetGraphChineseName(GraphType type)
        {
            switch (type)
            {
                case GraphType.Digraph:
                    return "有向图";
                case GraphType.Undigraph:
                    return "无向图";
                default:
                    return null;
            }
        }

        /// <summary>
        /// 边是否存在
        /// </summary>
        /// <param name="type"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        private bool IsEdgeExist(GraphType type, string edge)
        {
            string[] edges = GetEdges(type);
            string value = edge.Contains(inverseStr) ? (edge[2].ToString() + edge[1].ToString()).ToUpper() : edge.ToUpper();

            foreach (var e in edges)
            {
                if (e == value)
                {
                    return true;
                }
                else if (type == GraphType.Undigraph)
                {
                    if (e == (value[1].ToString() + value[0].ToString()).ToUpper())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}