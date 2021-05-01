using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Tools
{

    public static Tvalue GetValue<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dic, Tkey key)
    {
        Tvalue value = default(Tvalue);
        dic.TryGetValue(key, out value);
        return value;
    }

    /// <summary>
    /// 获取项目下绝对路径
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetAbsolutePath(string path, string fileName)
    {
        Uri uri = new Uri(Path.Combine(path, fileName));
        return uri.AbsolutePath;
    }
}
