using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class MyMenu
{
    [MenuItem("MyTools/Create AB Json")]
    static void CreateABJson()
    {
        string path = Tools.GetAbsolutePath(Application.streamingAssetsPath, "AssetBundles/");
        DirectoryInfo direction = new DirectoryInfo(path);
        DirectoryInfo[] dirInfos = direction.GetDirectories();
        ABJson abJson = new ABJson();
        abJson.abNames = new Dictionary<string, List<string>>();
        for (int i = 0; i < dirInfos.Length; i++)
        {
            string dirName = dirInfos[i].Name;
            abJson.abNames.Add(dirName, new List<string>());
            string dirPath = Tools.GetAbsolutePath(path, dirName);
            FileInfo[] files = new DirectoryInfo(dirPath).GetFiles();
            for (int j = 0; j < files.Length; j++)
            {
                if (files[j].Name.EndsWith(".ab"))
                {
                    string abPath = "AssetBundles/" + dirName + "/" + files[j].Name;
                    abJson.abNames[dirName].Add(abPath);
                }
            }
        }
        string jsonStr = JsonMapper.ToJson(abJson);
        StreamWriter sw;
        FileInfo fi = new FileInfo(path + "AB.json");
        sw = fi.CreateText();
        sw.WriteLine(jsonStr);
        sw.Close();
        sw.Dispose();
        Debug.Log("创建AB.json文件完成");
    }


    [MenuItem("MyTools/Clear Cache")]
    static void OnClearCache()
    {
        Caching.ClearCache();
    }
}
