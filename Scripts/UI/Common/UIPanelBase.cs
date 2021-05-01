using UnityEngine;
using WJF_CodeLibrary.Extension;
using System;
using UnityEngine.UI;
using System.Collections;

namespace WJF_CodeLibrary.UIFramework
{
    public class UIPanelBase : MonoBehaviour
    {
        public GameObject cameraD, cameraF, cameraB;

        public void Start()
        {
            cameraD = GameObject.Find("target").transform.Find("djMainCamera").gameObject;
            cameraF = GameObject.Find("cameras").transform.Find("site1").gameObject;
            cameraB = GameObject.Find("BellmanCameras").transform.Find("cameraA").gameObject;
        }

        //=================属性=================//
        /// <summary>
        /// 事件触发类型
        /// </summary>
        protected enum EventType
        {
            NoDo,//什么都不做，过2秒后自动关闭
            Click_Left,//左键单击
            Click_Right,//右键单击
            Click_Middle,//中键单击
            Down,//按下
            Press,//持续按下
            HoverIn,//进入悬浮
            HoverOut//退出悬浮
        }

        /// <summary>
        /// 面板的类型
        /// </summary>
        private UIType currentUIType = new UIType();
        public UIType CurrentUIType
        {
            set { currentUIType = value; }
            get { return currentUIType; }
        }

        //=================方法=================//
        /// <summary>
        /// 初始化，UI整体初始化时被调用
        /// </summary>
        public virtual void Init() { }

        /// <summary>
        /// 显示时事件，子类继承复写
        /// </summary>
        protected virtual void OnShow() { }

        /// <summary>
        /// 隐藏时事件，子类继承复写
        /// </summary>
        protected virtual void OnHide() { }

        /// <summary>
        /// 显示当前面板
        /// </summary>
        public void Show()
        {
            //显示效果，等待效果结束执行后面逻辑
            CustomShowEffect(() =>
            {
                //如果是弹出面板
                if (currentUIType.panelType == UIPanelType.Popup)
                {
                    //显示遮罩
                    UIMaskManager.Instance.SetMask(transform, currentUIType.maskType);
                }

                //执行子类的自定义事件
                OnShow();
            });
        }


        /// <summary>
        /// 隐藏当前面板
        /// </summary>
        public void Hide()
        {
            //隐藏效果，等待效果结束执行后面逻辑
            CustomHideEffect(() =>
            {
                //如果是弹出面板
                if (currentUIType.panelType == UIPanelType.Popup)
                {
                    //取消遮罩
                    UIMaskManager.Instance.CancelMask();
                }

                //执行子类的自定义事件
                OnHide();
            });
        }


        /// <summary>
        /// 显示指定面板
        /// </summary>
        /// <typeparam name="T">指定面板类型</typeparam>
        /// <param name="panelName">指定的面板名字</param>
        protected void ShowPanel<T>(string panelName = null) where T : UIPanelBase
        {
            //可按名字或直接控制
            if (panelName == "")
                UIManager.Instance.ShowUIPanel<T>();
            else
            {
                switch (panelName)
                {
                    case "DjPanel":
                        cameraD.SetActive(true);
                        break;
                    case "FloydPanel":
                        cameraF.SetActive(true);
                        break;
                    case "BellmanFordPanel":
                        cameraB.SetActive(true);
                        break;
                }

                UIManager.Instance.ShowUIPanel<T>(panelName);
            }

        }


        /// <summary>
        /// 隐藏指定面板
        /// </summary>
        /// <typeparam name="T">指定面板类型</typeparam>
        /// <param name="panelName">指定的面板名字</param>
        protected void HidePanel<T>(string panelName = null) where T : UIPanelBase
        {
            //可按名字或直接控制
            if (panelName == "")
                UIManager.Instance.HideUIPanel<T>();
            else
                UIManager.Instance.HideUIPanel<T>(panelName);
        }


        /// <summary>
        /// 自定义显示效果
        /// </summary>
        protected virtual void CustomShowEffect(Action onComplete)
        {
            gameObject.SetActive(true);

            if (onComplete != null)
                onComplete();
        }


        /// <summary>
        /// 自定义隐藏效果
        /// </summary>
        protected virtual void CustomHideEffect(Action onComplete)
        {
            gameObject.SetActive(false);

            if (onComplete != null)
                onComplete();
        }


        /// <summary>
        /// 注册UI交互事件
        /// </summary>
        /// <param name="triggerName">交互物体名</param>
        /// <param name="type">交互方式</param>
        /// <param name="triggerEvent">交互事件</param>
        protected void RegisterEvent(string triggerName, EventType type, EventTriggerListener.VoidDelegate triggerEvent)
        {
            //获取交互物体
            GameObject tgr = gameObject.GetChild(triggerName);
            RegisterEvent(tgr, type, triggerEvent);
        }


        /// <summary>
        /// 注册UI交互事件
        /// </summary>
        /// <param name="go">交互物体</param>
        /// <param name="type">交互方式</param>
        /// <param name="triggerEvent">交互事件</param>
        protected void RegisterEvent(GameObject go, EventType type, EventTriggerListener.VoidDelegate triggerEvent)
        {
            if (go != null)
            {
                switch (type)
                {
                    case EventType.Click_Left://点击
                        EventTriggerListener.Get(go).onClickLeft = triggerEvent;
                        break;
                    case EventType.Click_Right://右击
                        EventTriggerListener.Get(go).onClickRight = triggerEvent;
                        break;
                    case EventType.Click_Middle://中键
                        EventTriggerListener.Get(go).onClickMiddle = triggerEvent;
                        break;
                    case EventType.Down://按下
                        EventTriggerListener.Get(go).onDown = triggerEvent;
                        break;
                    case EventType.Press://持续按下
                        KeepPressLogic keepPress = go.GetOrAddComponent<KeepPressLogic>();
                        keepPress.pressEvent = () => triggerEvent(go);
                        break;
                    case EventType.HoverIn://鼠标悬浮
                        EventTriggerListener.Get(go).onEnter = triggerEvent;
                        break;
                    case EventType.HoverOut://鼠标退出悬浮
                        EventTriggerListener.Get(go).onExit = triggerEvent;
                        break;
                }
            }

        }


        protected void Refresh(RectTransform rect, Action onComplete)
        {
            StartCoroutine(WaitForRefresh(rect, onComplete));
        }


        private IEnumerator WaitForRefresh(RectTransform rect, Action onComplete)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            yield return new WaitForEndOfFrame();
            while (rect.rect.width == 0)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
                yield return new WaitForEndOfFrame();
            }

            if (onComplete != null)
                onComplete();
        }


        /// <summary>
        /// 设置按键点击事件
        /// 不使用EventTriggerListener是因为其点击事件会影响滚动
        /// </summary>
        /// <param name="go"></param>
        /// <param name="clickEvent"></param>
        protected void SetButtonClickEvent(GameObject go, Action clickEvent)
        {
            Button btn = go.GetComponent<Button>();

            if (btn == null)
            {
                btn = go.AddComponent<Button>();
                btn.transition = Selectable.Transition.None;
                Navigation nav = btn.navigation;
                nav.mode = Navigation.Mode.None;
                btn.navigation = nav;
            }

            btn.onClick.AddListener(() =>
            {
                if (clickEvent != null)
                    clickEvent();
            });
        }


        /// <summary>
        /// 发送消息（只能发送已注册的消息）
        /// </summary>
        /// <param name="id">消息名</param>
        /// <param name="param">参数列表</param>
        protected void SendMessage(string id, object[] param)
        {
            MessageCenter.SendMessage(id, param);
        }


        /// <summary>
        /// 接收消息（即注册消息s）
        /// </summary>
        /// <param name="id">消息名</param>
        /// <param name="handler">消息具体内容</param>
        protected void ReceiveMessage(string id, Action<object[]> handler)
        {
            MessageCenter.AddListener(id, handler);
        }
    }
}