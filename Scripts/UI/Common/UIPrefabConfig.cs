using UnityEngine;
using System.Collections.Generic;

namespace WJF_CodeLibrary.UIFramework
{
    public class UIPrefabConfig
    {
        private static List<GameObject> prefabs;

        public static List<GameObject> GetPrefabs()
        {
            if (prefabs == null)
            {
                SetPrefabs();
            }
            return new List<GameObject>(prefabs);
        }

        private static void SetPrefabs()
        {
            prefabs = new List<GameObject>();
            foreach (var path in UIDefine.PrefabFolders)
            {
                prefabs.AddRange(Resources.LoadAll<GameObject>(path));
            }
        }
    }
}