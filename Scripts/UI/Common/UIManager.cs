using UnityEngine;
using System.Collections.Generic;
using WJF_CodeLibrary.CommonUtility.UI;

namespace WJF_CodeLibrary.UIFramework
{
    public class UIManager
    {
        private GameObject canvasGO; //UI物体
        private List<GameObject> lstPrefabs;//面板的预制体列表
        private List<UIPanelBase> lstPanels;//所有面板
        private Stack<UIPanelBase> stackCurrentPanels;//导航栈中的面板

        private Transform nomalNode;
        private Transform fixedNode;
        private Transform popupNode;

        #region 单例

        private static UIManager instance;
        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UIManager();
                }

                return instance;
            }
        }

        #endregion

        #region 初始化与销毁方法

        private UIManager()
        {
            //获取UI物体
            Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
            Canvas canvas = null;

            foreach(var item in canvases)
            {
                if(item.name == "Canvas")
                {
                    canvas = item;
                    break;
                }
            }

            if (canvas == null)
            {
                GameObject prefab = Resources.Load<GameObject>(UIDefine.CanvasPrefabPath);
                canvasGO = Object.Instantiate(prefab);
            }
            else
            {
                canvasGO = canvas.gameObject;
            }

            //保持UI不删除，只有一个
            Object.DontDestroyOnLoad(canvasGO);

            //初始化各集合
            lstPrefabs = UIPrefabConfig.GetPrefabs();
            lstPanels = new List<UIPanelBase>();
            stackCurrentPanels = new Stack<UIPanelBase>();

            //初始化所有面板
            InitializeNodes();

            //隐藏所有面板
            UIPanelBase[] parent = canvasGO.GetComponentsInChildren<UIPanelBase>();
            foreach (var panel in parent)
                panel.Hide();
        }

        /// <summary>
        /// 销毁事件
        /// </summary>
        public void OnDestroy()
        {
            if (instance != null)
            {
                //删除UI物体
                Object.DestroyImmediate(canvasGO);

                //清空面板列表
                lstPanels.Clear();
                lstPanels = null;

                //清空单例
                instance = null;
            }
        }

        #endregion

        #region 公有

        /// <summary>
        /// 获取当前管理的UI物体
        /// </summary>
        /// <returns></returns>
        public GameObject GetCurrentCanvas()
        {
            return canvasGO;
        }

        /// <summary>
        /// 获得指定面板，没有则通过prefab创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="panelName"></param>
        /// <returns></returns>
        public T GetPanel<T>(string panelName = null) where T : UIPanelBase
        {
            UIPanelBase panel = lstPanels.Find(p => panelName == null ? p is T : p is T && p.name == panelName);

            if (panel == null)
            {
                panel = CreatePanelFromPrefab<T>(panelName);
                lstPanels.Add(panel);
            }

            return panel as T;
        }

        /// <summary>
        /// 返回UI显示，只控制关联UI，即当前显示的关联UI隐藏，显示出前一个关联UI
        /// </summary>
        public void Return()
        {
            if (stackCurrentPanels.Count >= 2)
            {
                UIPanelBase current = stackCurrentPanels.Pop();
                current.Hide();

                UIPanelBase next = stackCurrentPanels.Peek();
                next.Show();
            }
        }

        #endregion

        #region 显示、隐藏控制

        /// <summary>
        /// 显示指定面板，可用于当一个类用于多个面板时根据名字获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="panelName"></param>
        public T ShowUIPanel<T>(string panelName = null) where T : UIPanelBase
        {
            T panel = GetPanel<T>(panelName);

            ShowPanel(panel);

            return panel;
        }

        /// <summary>
        /// 隐藏指定面板，可用于当一个类用于多个面板时根据名字获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="panelName"></param>
        public T HideUIPanel<T>(string panelName = null) where T : UIPanelBase
        {
            //HidePanel(lstPanels.Find(p => panelName == null ? p is T : p is T && p.name == panelName));
            T panel = GetPanel<T>(panelName);

            HidePanel(panel);

            return panel;
        }

        /// <summary>
        /// 显示面板
        /// </summary>
        /// <param name="panel"></param>
        private void ShowPanel(UIPanelBase panel)
        {
            if (panel == null)
                return;

            switch (panel.CurrentUIType.showType)
            {
                case UIShowType.Irrelevant://普通显示模式
                    break;
                case UIShowType.Relevant://关联节点
                                         //如果栈中有元素，则将栈顶面板隐藏
                    if (stackCurrentPanels.Count > 0)
                    {
                        UIPanelBase topPanel = stackCurrentPanels.Peek();
                        topPanel.Hide();
                    }

                    //将当前面板压入栈中
                    stackCurrentPanels.Push(panel);

                    break;
            }

            //显示面板
            panel.Show();
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        /// <param name="panel"></param>
        private void HidePanel(UIPanelBase panel)
        {
            if (panel == null)
                return;

            switch (panel.CurrentUIType.showType)
            {
                case UIShowType.Irrelevant://普通显示模式
                    break;
                case UIShowType.Relevant://关联节点
                                         //如果栈中有超过2个元素，则将栈顶弹出，再将下一个面板显示
                    if (stackCurrentPanels.Count >= 2)
                    {
                        stackCurrentPanels.Pop();

                        UIPanelBase nextPanel = stackCurrentPanels.Peek();
                        nextPanel.Show();
                    }
                    //如果栈中只有一个元素，则该元素即当前面板，弹出栈
                    else if (stackCurrentPanels.Count == 1)
                    {
                        stackCurrentPanels.Pop();
                    }
                    break;
            }

            //隐藏当前面板
            panel.Hide();
        }

        #endregion

        #region 私有

        /// <summary>
        /// 从预制体中创建指定的面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="panelName"></param>
        /// <returns></returns>
        private UIPanelBase CreatePanelFromPrefab<T>(string panelName = null) where T : UIPanelBase
        {
            //从prefab列表中获取指定物体
            GameObject prefab = lstPrefabs.Find(p =>
            {
                T pnl = p.GetComponent<T>();
                return (panelName == null ? pnl is T : pnl is T && p.name == panelName);
            });

            if (prefab == null)
            {
                Debug.LogError(string.Format("The {0:G} panel prefab is not found!", typeof(T)));
                return null;
            }

            GameObject clone = Object.Instantiate(prefab);

            UIPanelBase panel = clone.GetComponent<UIPanelBase>();
            InitializePanel(panel);

            return panel;
        }

        /// <summary>
        /// 初始化所有面板
        /// </summary>
        private void InitializeNodes()
        {
            //获取或生成父节点
            nomalNode = GetParentNode(UIDefine.NormalNode);
            fixedNode = GetParentNode(UIDefine.FixedNode);
            popupNode = GetParentNode(UIDefine.PopupNode);
        }

        /// <summary>
        /// 初始化指定面板
        /// </summary>
        /// <param name="panel"></param>
        private void InitializePanel(UIPanelBase panel)
        {
            panel.Init();
            switch (panel.CurrentUIType.panelType)
            {
                case UIPanelType.Normal:
                    panel.transform.SetParent(nomalNode, false);
                    break;
                case UIPanelType.Fixed:
                    panel.transform.SetParent(fixedNode, false);
                    break;
                case UIPanelType.Popup:
                    panel.transform.SetParent(popupNode, false);
                    break;
            }
        }

        /// <summary>
        /// 获取或生成父节点物体，主要用于分类面板
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        private Transform GetParentNode(string nodeName)
        {
            Transform node = canvasGO.transform.Find(nodeName);

            if (node == null)
            {
                RectTransform rtNode = (new GameObject(nodeName)).AddComponent<RectTransform>();
                rtNode.SetParent(canvasGO.transform);
                rtNode.transform.localPosition = Vector3.zero;
                rtNode.transform.localScale = Vector3.one;

                UIUtility.TileRectTransform(rtNode);

                node = rtNode.transform;
            }

            return node;
        }

        #endregion
    }
}