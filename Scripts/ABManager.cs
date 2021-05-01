using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


//加载进度
public delegate void LoadProgress(float pro, string loadStr);
public delegate void LoadOverCallBack();

public class ABJson
{
    public Dictionary<string, List<string>> abNames;
}


/// <summary>
/// 加载AssetBundle
/// </summary>
public class ABManager : Singleton<ABManager>
{

    public bool isLoad = false;

    #region ---AssetBundle加载 
    private Dictionary<string, object> objDic = new Dictionary<string, object>();

    #region ---ab加载并从中load资源
    public void LoadAsyncAB(LoadProgress progress, LoadOverCallBack cb)
    {
        MonoHelper.Instance.StartCoroutine(ReadJson(progress, cb));
    }
    //读取json文件获取所有ab包名称
    IEnumerator ReadJson(LoadProgress progress, LoadOverCallBack cb)
    {
        //Debug.LogError("1readjson");
        ABJson abJson;
        string path = Tools.GetAbsolutePath(Application.streamingAssetsPath, "AssetBundles/AB.json");
        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            //Debug.LogError("2Using");
            yield return request.SendWebRequest();
            string jsonStr = request.downloadHandler.text;
            abJson = JsonMapper.ToObject<ABJson>(jsonStr);
            //Debug.LogError(abJson.ToString());
        }

        var p_it = abJson.abNames["AB"].GetEnumerator();
        while (p_it.MoveNext())
        {

            string abPath = Tools.GetAbsolutePath(Application.streamingAssetsPath, p_it.Current.ToString());
            // Debug.LogError(abPath);
            yield return MonoHelper.Instance.StartCoroutine(GetAsyncAB(abPath, progress));

        }

        cb();
        isLoad = true;
        AssetBundle.UnloadAllAssetBundles(false);
        yield break;
    }

    //异步加载ab包
    IEnumerator GetAsyncAB(string abPath, LoadProgress progress)
    {
        // Debug.LogError("AsyncAB");
        AssetBundle ab;
        using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(abPath))//UnityWebRequest.GetAssetBundle(abPath))
        {
            request.SendWebRequest();
            while (!request.isDone)
            {
                if (progress != null)
                    progress(request.downloadProgress, "资源加载中......");
                yield return null;
            }
            ab = DownloadHandlerAssetBundle.GetContent(request);
            if (ab == null)
            {
                Debug.LogError(abPath + "---AB包为空");
                yield break;
            }
        }
        yield return MonoHelper.Instance.StartCoroutine(GetAllAssetInAB(ab, progress));
        yield break;
    }

    //从ab包中load资源
    IEnumerator GetAllAssetInAB(AssetBundle ab, LoadProgress progress)
    {
        //Debug.LogError("loadAB");
        AssetBundleRequest abRequest = ab.LoadAllAssetsAsync();
        while (!abRequest.isDone)
        {
            if (progress != null)
                progress(abRequest.progress, ab.name + "资源解压中......");
            yield return null;
        }
        object[] objs = abRequest.allAssets;
        var it = objs.GetEnumerator();
        while (it.MoveNext())
        {
            UnityEngine.Object obj = (UnityEngine.Object)it.Current;
            if (!objDic.ContainsKey(obj.name))
                objDic.Add(obj.name, obj);
        }
        //ab.Unload(false);
        yield break;
    }
    #endregion

    //你想要加载的场景或者模型
    public T GetObj<T>(string objName) where T : UnityEngine.Object
    {
        T t = objDic.GetValue(objName) as T;
        return t;
    }
    #endregion
}
