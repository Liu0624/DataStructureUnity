using System;
using System.Collections.Generic;
using UnityEngine;
using WJF_CodeLibrary.UIFramework;

namespace WJF
{
    /// <summary>
    /// MessageBoxPanel
    /// </summary>
    public static partial class PanelActivator2
    {
        public static void MessageBox(string message, Action onOK, string txtOK = "确定")
        {
            UIManager.Instance.ShowUIPanel<MessageBoxPanel2>();
            MessageBoxPanel2.Option param = new MessageBoxPanel2.Option(message, onOK, txtOK);
            MessageCenter.SendMessage(MsgDefine.ShowMessageBox, new object[] { param });
        }

        public static void MessageBox(string message, Action onConfirm, Action onCancel, string txtOK = "确定", string txtCancel = "取消")
        {
            UIManager.Instance.ShowUIPanel<MessageBoxPanel2>();
            MessageBoxPanel2.Option param = new MessageBoxPanel2.Option(message, onConfirm, onCancel, txtOK, txtCancel);
            MessageCenter.SendMessage(MsgDefine.ShowMessageBox, new object[] { param });
        }
    }
}