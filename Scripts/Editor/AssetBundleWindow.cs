using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundle : Editor
{

    public static Object[] Objs = new Object[] { };

    [MenuItem("MyTools/Build Select Objects")]
    static void BuildSelect()
    {
        //获取所有选中的对象
        Objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        //弹出一个编辑窗口；
        AssetBundleWindow.ShowWindow();
    }

    /// <summary>
    /// 开始打包；
    /// </summary>
    public static void AllStartBuild()
    {
        Debug.Log("开始打包！");
        string path = AssetBundleWindow.AsbPath;
        Debug.Log("选择路径：" + path);

        AssetBundleBuild abb = new AssetBundleBuild();
        abb.assetNames = new string[Objs.Length];
        for (int i = 0; i < Objs.Length; i++)
        {
            abb.assetNames[i] = AssetDatabase.GetAssetPath(Objs[i]);
        }
        if (AssetBundleWindow.IsWeb)
        {
            Debug.Log("将要打包到Web");
            abb.assetBundleName = AssetBundleWindow.AssetBudleName + "_web.ab";
            BuildPipeline.BuildAssetBundles(path, new AssetBundleBuild[] { abb }, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.WebGL);
        }
        if (AssetBundleWindow.IsWindows)
        {
            Debug.Log("将要打包到Windows");
            abb.assetBundleName = AssetBundleWindow.AssetBudleName + "_windows.ab";
            BuildPipeline.BuildAssetBundles(path, new AssetBundleBuild[] { abb }, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows64);
        }
        if (AssetBundleWindow.IsAndorid)
        {
            Debug.Log("将要打包到安卓");
            abb.assetBundleName = AssetBundleWindow.AssetBudleName + "_android.ab";
            BuildPipeline.BuildAssetBundles(path, new AssetBundleBuild[] { abb }, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);
        }
        if (AssetBundleWindow.IsApple)
        {
            Debug.Log("将要打包到苹果");
            abb.assetBundleName = AssetBundleWindow.AssetBudleName + "_ios.ab";
            BuildPipeline.BuildAssetBundles(path, new AssetBundleBuild[] { abb }, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.iOS);
        }
        AssetDatabase.Refresh();
        Debug.Log("打包完成！");
    }

    public static void AloneStartBuild()
    {
        Debug.Log("开始打包！");
        string path = AssetBundleWindow.AsbPath;
        Debug.Log("选择路径：" + path);

        AssetBundleBuild[] abbs = new AssetBundleBuild[Objs.Length];
        for (int i = 0; i < Objs.Length; i++)
        {
            abbs[i].assetNames = new string[] { AssetDatabase.GetAssetPath(Objs[i]) };
            abbs[i].assetBundleName = Objs[i].name + "_web.ab";
            if (AssetBundleWindow.IsWeb)
            {
                Debug.Log("将要打包到Web");
                abbs[i].assetBundleName = Objs[i].name + "_web.ab";
            }
            if (AssetBundleWindow.IsWindows)
            {
                Debug.Log("将要打包到Windows");
                abbs[i].assetBundleName = Objs[i].name + "_windows.ab";
            }
            if (AssetBundleWindow.IsAndorid)
            {
                Debug.Log("将要打包到安卓");
                abbs[i].assetBundleName = Objs[i].name + "_android.ab";
            }
            if (AssetBundleWindow.IsApple)
            {
                Debug.Log("将要打包到苹果");
                abbs[i].assetBundleName = Objs[i].name + "_ios.ab";
            }
        }
        BuildPipeline.BuildAssetBundles(path, abbs, BuildAssetBundleOptions.None, BuildTarget.WebGL);
        AssetDatabase.Refresh();
        Debug.Log("打包完成！");
    }
}

public class AssetBundleWindow : EditorWindow
{
    /// <summary>
    /// asb的名字；
    /// </summary>
    public static string AssetBudleName;
    /// <summary>
    /// asb包的路径；
    /// </summary>
    public static string AsbPath;
    /// <summary>
    /// 是否在Web下打包；
    /// </summary>
    public static bool IsWeb = false;
    /// <summary>
    /// 是否在Windows下打包；
    /// </summary>
    public static bool IsWindows = false;
    /// <summary>
    /// 是否在安卓下打包；
    /// </summary>
    public static bool IsAndorid = false;
    /// <summary>
    /// 是否在苹果下打包；
    /// </summary>
    public static bool IsApple = false;
    /// <summary>
    /// 是否单独打包
    /// </summary>
    public static bool IsAlone = false;


    public AssetBundleWindow()
    {
        titleContent = new GUIContent("资源打包");
    }


    public static void ShowWindow()
    {
        GetWindow(typeof(AssetBundleWindow));
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUI.skin.label.fontSize = 15;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        if (AssetBundle.Objs.Length > 0)
        {
            GUILayout.Label("当前总共选择：" + AssetBundle.Objs.Length + "个资源！");

            //绘制文件路径选择
            GUILayout.Space(10);
            if (GUILayout.Button("路径选择", GUILayout.Width(200)))
            {
                AsbPath = EditorUtility.SaveFolderPanel("请选择打包路径", Application.streamingAssetsPath, AssetBudleName);
                //这里开启窗口重绘制；
                Repaint();
            }

            GUI.skin.label.fontSize = 10;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            if (string.IsNullOrEmpty(AsbPath))
            {
                //这里绘制选择结果；
                GUILayout.Label("没有选择打包路径！");
            }
            else
            {
                //这里绘制选择结果；
                GUILayout.Label("当前选择打包路径：" + AsbPath);

                //放3个togle
                IsWeb = GUI.Toggle(new Rect(10, 100, 600, 20), IsWeb, "打包到Web平台");

                IsWindows = GUI.Toggle(new Rect(10, 120, 600, 20), IsWindows, "打包到Windows平台");

                IsAndorid = GUI.Toggle(new Rect(10, 140, 600, 20), IsAndorid, "打包到Android平台");

                IsApple = GUI.Toggle(new Rect(10, 160, 600, 20), IsApple, "打包到IOS平台");

                IsAlone = GUI.Toggle(new Rect(10, 180, 600, 20), IsAlone, IsAlone ? "分开单独打包" : "整体打一个包");

                GUILayout.Space(120);
                if (!IsAlone)
                {
                    GUILayout.Label("请输入导出的包名：");
                    //设置一个文字输入框；
                    AssetBudleName = EditorGUILayout.TextField(AssetBudleName);
                    GUILayout.Space(10);
                }
                if (GUILayout.Button("开始打包", GUILayout.Width(200)))
                {
                    if (IsAlone)
                    {
                        AssetBundle.AloneStartBuild();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(AssetBudleName))
                        {
                            Debug.LogError("请输入打包文件名！");
                        }
                        else
                        {
                            AssetBundle.AllStartBuild();
                        }
                    }
                }
            }
        }
        else
        {
            GUILayout.Label("当前未选择任何资源！");
        }
    }
}

