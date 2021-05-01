using System;
using System.Collections.Generic;
using UnityEngine;
using WJF_CodeLibrary.UIFramework;

namespace WJF
{
    /// <summary>
    /// MessageBoxPanel
    /// </summary>
    
    public static partial class PanelActivator
    {
        public static void MessageBox(string message)
        {
            UIManager.Instance.ShowUIPanel<MessageBoxPanel>();
            MessageBoxPanel.Option param = new MessageBoxPanel.Option(message);
            MessageCenter.SendMessage(MsgDefine.ShowMessageBox, new object[] { param });
        }


        public static void MessageBox(string message, Action onOK, string txtOK = "确定")
        {
            UIManager.Instance.ShowUIPanel<MessageBoxPanel>();
            MessageBoxPanel.Option param = new MessageBoxPanel.Option(message, onOK, txtOK);
            MessageCenter.SendMessage(MsgDefine.ShowMessageBox, new object[] { param });
        }

        public static void MessageBox(string message, Action onConfirm, Action onCancel, string txtOK = "确定", string txtCancel = "取消")
        {
            UIManager.Instance.ShowUIPanel<MessageBoxPanel>();
            MessageBoxPanel.Option param = new MessageBoxPanel.Option(message, onConfirm, onCancel, txtOK, txtCancel);
            MessageCenter.SendMessage(MsgDefine.ShowMessageBox, new object[] { param });
        }
    }
}