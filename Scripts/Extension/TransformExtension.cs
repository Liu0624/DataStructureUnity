using System;
using UnityEngine;

namespace WJF_CodeLibrary.Extension
{
    /// <summary>
    /// Transform
    /// </summary>
    public static class TransformExtension
    {
        /// <summary>
        /// 深度遍历子物体
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="onGet"></param>
        public static void TraverseChildrenInDepth(this Transform transform, Action<Transform> onGet)
        {
            foreach (Transform child in transform)
            {
                onGet(child);

                if (child.childCount != 0)
                {
                    TraverseChildrenInDepth(child, onGet);
                }
            }
        }

        /// <summary>
        /// 只显示指定子物体（非孙子物体），其他隐藏
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="childName">子物体名字</param>
        public static void ActiveChild(this Transform transform, string childName)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(child.name == childName);
            }
        }

        /// <summary>
        /// 只显示指定子物体（非孙子物体），其他隐藏
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="condition">指定物体的条件</param>
        public static void ActiveChild(this Transform transform, Func<Transform, bool> condition)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(condition(child));
            }
        }

        /// <summary>
        /// 深度搜索子物体，只显示指定名字的物体
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="childName"></param>
        public static void ActiveChildInDepth(this Transform transform, string childName)
        {
            transform.TraverseChildrenInDepth(child =>
            {
                child.gameObject.SetActive(child.name == childName);
            });
        }

        /// <summary>
        /// 深度搜索子物体，只显示满足条件的物体
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="condition"></param>
        public static void ActiveChildInDepth(this Transform transform, Func<Transform, bool> condition)
        {
            transform.TraverseChildrenInDepth(child =>
            {
                child.gameObject.SetActive(condition(child));
            });
        }

        /// <summary>
        /// 获得组件，若没有则添加
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="transform">指定的transform</param>
        /// <returns>组件</returns>
        public static T GetOrAddComponent<T>(this Transform transform) where T : Component
        {
            return transform.gameObject.GetOrAddComponent<T>();
        }

        /// <summary>
        /// 获取父物体的Transform
        /// </summary>
        /// <param name="transform">指定的transform</param>
        /// <param name="name">父物体的名字</param>
        /// <returns>父物体的Transform</returns>
        public static Transform GetParent(this Transform transform, string name)
        {
            GameObject go = transform.gameObject.GetParent(name);
            return go == null ? null : go.transform;
        }

        /// <summary>
        /// 根据索引获取兄弟节点
        /// </summary>
        /// <param name="transform">指定的transform</param>
        /// <param name="index">在层级中的索引</param>
        /// <returns>兄弟节点</returns>
        public static Transform GetBrother(this Transform transform, int index)
        {
            GameObject go = transform.gameObject.GetBrother(index);
            return go == null ? null : go.transform;
        }

        /// <summary>
        /// 根据名字获取兄弟节点
        /// </summary>
        /// <param name="transform">指定的transform</param>
        /// <param name="name">兄弟节点的名字</param>
        /// <returns>兄弟节点</returns>
        public static Transform GetBrother(this Transform transform, string name)
        {
            GameObject go = transform.gameObject.GetBrother(name);
            return go == null ? null : go.transform;
        }

        /// <summary>
        /// 获得子物体的Transform
        /// </summary>
        /// <param name="transform">指定的transform</param>
        /// <param name="name">子物体的名字</param>
        /// <returns>子物体的Transform</returns>
        public static Transform GetChild(this Transform transform, string name)
        {
            GameObject go = transform.gameObject.GetChild(name);
            return go == null ? null : go.transform;
        }
    }
}