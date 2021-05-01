using System.Collections.Generic;
using UnityEngine;

namespace WJF_CodeLibrary.Extension
{
    /// <summary>
    /// List
    /// </summary>
    public static class ListExtension
    {
        /// <summary>
        /// 若没有给定的元素则添加
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">对应列表</param>
        /// <param name="item">给定的元素</param>
        public static void AddIfNotContains<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        /// <summary>
        /// 列表元素随机排列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int random = Random.Range(i, list.Count - 1);
                T temp = list[i];
                list[i] = list[random];
                list[random] = temp;
            }
        }
    }
}