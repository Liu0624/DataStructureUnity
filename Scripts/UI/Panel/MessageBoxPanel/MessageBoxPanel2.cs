using System;
using UnityEngine;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.UIFramework;

namespace WJF
{
    public class MessageBoxPanel2 : UIPanelBase
    {
        private GameObject btnOK;//单一的确定键
        private GameObject btnConfirm;//确定取消中的确定键
        private GameObject btnCancel;//取消键

        private Action onOK;
        private Action onConfirm;
        private Action onCancel;

        public override void Init()
        {
            CurrentUIType.panelType = UIPanelType.Popup;
            CurrentUIType.showType = UIShowType.Irrelevant;

            btnOK = gameObject.GetChild("OK");
            btnConfirm = gameObject.GetChild("Confirm");
            btnCancel = gameObject.GetChild("Cancel");

            //接收的参数：0消息框类型、1文字内容、2单一确定按键事件、3确定取消中的确定事件、4确定取消中的取消事件、5确定键文字（两个按键都用这个）、6取消键文字、
            ReceiveMessage(MsgDefine.ShowMessageBox, p =>
            {
                Option option = p[0] as Option;
                print("xxx--option" + option.message);
                //设置消息内容
                Text messageContent = gameObject.GetComponentInChildrenByName<Text>("Message");
                messageContent.text = option.message;

                //按键文字
                string txtOK = option.txtOK;
                string txtCancel = option.txtCancel;

                //根据消息类型显示并设置事件
                switch (option.type)
                {
                    case MessageBoxType.OK://单一确定消息框
                        SetOKOption(option.txtOK, option.onOK);
                        break;
                    case MessageBoxType.ConfirmCancel://确认取消消息框
                        SetConfirmCancelOption(option.txtOK, option.txtCancel, option.onConfirm, option.onCancel);
                        break;
                }

                //显示面板
                if (!gameObject.activeSelf)
                    ShowPanel<MessageBoxPanel2>();
            });

            RegisterEvent(btnOK, EventType.Click_Left, p =>
            {
                //点击后自动隐藏
                HidePanel<MessageBoxPanel2>();

                if (onOK != null)
                    onOK();
            });

            RegisterEvent(btnConfirm, EventType.Click_Left, p =>
            {
                //点击后自动隐藏
                HidePanel<MessageBoxPanel2>();

                if (onConfirm != null)
                    onConfirm();
            });
            RegisterEvent(btnCancel, EventType.Click_Left, p =>
            {
                //点击后自动隐藏
                HidePanel<MessageBoxPanel2>();

                if (onCancel != null)
                    onCancel();
            });
        }
        
        /// <summary>
        /// 设置单一提示确定事件
        /// </summary>
        /// <param name="onOK"></param>
        private void SetOKOption(string txtOK, Action onOK)
        {
            //只显示单一的确定按键
            btnOK.SetActive(true);
            btnConfirm.SetActive(false);
            btnCancel.SetActive(false);

            //设置按键文字
            btnOK.GetComponentInChildren<Text>(true).text = txtOK;

            //执行确定事件
            this.onOK = onOK;
        }

        /// <summary>
        /// 这是确认取消事件
        /// </summary>
        /// <param name="onConfirm"></param>
        /// <param name="onCancel"></param>
        private void SetConfirmCancelOption(string txtOK, string txtCancel, Action onConfirm, Action onCancel)
        {
            //显示确认取消按键
            btnOK.SetActive(false);
            btnConfirm.SetActive(true);
            btnCancel.SetActive(true);

            //设置按键文字
            btnConfirm.GetComponentInChildren<Text>(true).text = txtOK;
            btnCancel.GetComponentInChildren<Text>(true).text = txtCancel;

            //确认事件
            this.onConfirm = onConfirm;

            //取消事件
            this.onCancel = onCancel;
        }

        /// <summary>
        /// 面板设置的类
        /// </summary>
        public class Option
        {
            public MessageBoxType type;
            public string message;
            public Action onOK;
            public Action onConfirm;
            public Action onCancel;
            public string txtOK;
            public string txtCancel;

            public Option(string message, Action onOK, string txtOK = "确定")
            {
                type = MessageBoxType.OK;
                this.message = message;
                this.onOK = onOK;
                this.txtOK = txtOK;
            }

            public Option(string message, Action onConfirm, Action onCancel, string txtOK = "确定", string txtCancel = "取消")
            {
                type = MessageBoxType.ConfirmCancel;
                this.message = message;
                this.onConfirm = onConfirm;
                this.onCancel = onCancel;
                this.txtOK = txtOK;
                this.txtCancel = txtCancel;
            }
        }
    }
}