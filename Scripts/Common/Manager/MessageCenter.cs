using System.Collections.Generic;
using System;

public class MessageCenter
{
    //字典，存放ID及方法
    private static Dictionary<string, Action<object[]>> dictMessages = new Dictionary<string, Action<object[]>>();
    
    /// <summary>
    /// 添加监听事件
    /// </summary>
    /// <param name="id"></param>
    /// <param name="handler"></param>
    public static void AddListener(string id, Action<object[]> handler)
    {
        if (!dictMessages.ContainsKey(id))
            dictMessages.Add(id, null);
        dictMessages[id] += handler;
    }
    
    /// <summary>
    /// 删除对应id的所有监听事件
    /// </summary>
    /// <param name="id"></param>
    public static void RemoveListener(string id)
    {
        if (dictMessages.ContainsKey(id))
            dictMessages.Remove(id);
    }

    /// <summary>
    /// 删除对应id的指定事件
    /// </summary>
    /// <param name="id"></param>
    /// <param name="handler"></param>
    public static void RemoveListener(string id, Action<object[]> handler)
    {
        if (dictMessages.ContainsKey(id))
            dictMessages[id] -= handler;
    }

    /// <summary>
    /// 清空所有监听事件
    /// </summary>
    public static void ClearAllListeners()
    {
        if (dictMessages != null)
            dictMessages.Clear();
    }

    /// <summary>
    /// 发送消息，调用指定id的事件，传入给定参数
    /// </summary>
    /// <param name="id"></param>
    /// <param name="param"></param>
    public static void SendMessage(string id, object[] param)
    {
        if (dictMessages.ContainsKey(id))
        {
            Action<object[]> action = dictMessages[id];
            if (action != null)
                action(param);
        }            
    }
}