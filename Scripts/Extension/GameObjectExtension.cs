using System;
using System.Collections.Generic;
using UnityEngine;

namespace WJF_CodeLibrary.Extension
{
    /// <summary>
    /// GameObject
    /// </summary>
    public static class GameObjectExtension
    {
        /// <summary>
        /// 只显示指定子物体（非孙子物体），其他隐藏
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="childName"></param>
        public static void ActiveChild(this GameObject gameObject, string childName)
        {
            gameObject.transform.ActiveChild(childName);
        }

        /// <summary>
        /// 只显示指定子物体（非孙子物体），其他隐藏
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="Condition">指定物体的条件</param>
        public static void ActiveChild(this GameObject gameObject, Func<Transform, bool> Condition)
        {
            gameObject.transform.ActiveChild(Condition);
        }

        /// <summary>
        /// 获得组件，若没有则添加
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">对应物体</param>
        /// <returns>指定的组件</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();

            if (component == null)
                component = gameObject.AddComponent<T>();

            return component;
        }

        /// <summary>
        /// 从兄弟节点上获取组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">指定物体</param>
        /// <returns>兄弟节点组件</returns>
        public static T GetComponentInBrother<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.transform.parent == null)
                return null;

            T component = null;

            Transform parent = gameObject.transform.parent;
            foreach (Transform child in parent)
            {
                if (child.gameObject != gameObject)
                {
                    component = child.GetComponent<T>();
                    if (component != null)
                        break;
                }
            }

            return component;
        }

        /// <summary>
        /// 获取所有兄弟节点上的指定组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">当前节点</param>
        /// <param name="filterSelf">是否过滤自己（true：过滤自己；false：不过滤）</param>
        /// <returns>组件序列</returns>
        public static T[] GetComponentsInBrother<T>(this GameObject gameObject, bool filterSelf = true) where T : Component
        {
            Transform parent = gameObject.transform.parent;

            if (parent == null)
                return null;

            int brotherCount = parent.childCount;

            T[] temp = new T[brotherCount];

            int validCount = 0;
            for (int i = 0; i < brotherCount; i++)
            {
                Transform child = parent.GetChild(i);
                T component = child.GetComponent<T>();

                if (component != null && !(filterSelf && child.gameObject == gameObject))
                    temp[validCount++] = component;
            }

            T[] result = new T[validCount];
            for (int i = 0; i < validCount; i++)
                result[i] = temp[i];

            return result;
        }

        /// <summary>
        /// 获取名字为name的子物体上的组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">指定物体</param>
        /// <param name="name">子物体名字</param>
        /// <returns>指定组件</returns>
        public static T GetComponentInChildrenByName<T>(this GameObject gameObject, string name) where T : Component
        {
            GameObject child = gameObject.GetChild(name);

            return child == null ? null : child.GetComponent<T>();
        }

        /// <summary>
        /// 激活/关闭物体
        /// </summary>
        /// <param name="gameObject">指定物体</param>
        public static void ToggleActive(this GameObject gameObject)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        /// <summary>
        /// 获取父物体
        /// </summary>
        /// <param name="gameObject">指定物体</param>
        /// <param name="name">父物体名字</param>
        /// <returns></returns>
        public static GameObject GetParent(this GameObject gameObject, string name)
        {
            Transform parent = gameObject.transform.parent;

            if (parent == null)
                return null;
            else if (parent.name == name)
                return parent.gameObject;
            else
                return gameObject.GetParent(name);
        }

        /// <summary>
        /// 根据索引获取兄弟节点
        /// </summary>
        /// <param name="gameObject">指定物体</param>
        /// <param name="index">在层级中的索引</param>
        /// <returns>兄弟节点物体</returns>
        public static GameObject GetBrother(this GameObject gameObject, int index)
        {
            if (gameObject.transform.parent == null)
                return null;

            if (index < 0 || index >= gameObject.transform.parent.childCount)
                return null;

            //只要index在范围内，则必定会有值，因此不需要再判断是否为空
            Transform brother = gameObject.transform.parent.GetChild(index);
            return brother.gameObject;
        }

        /// <summary>
        /// 根据名字获取兄弟节点
        /// </summary>
        /// <param name="gameObject">指定物体</param>
        /// <param name="name">兄弟节点的名字</param>
        /// <returns>兄弟节点物体</returns>
        public static GameObject GetBrother(this GameObject gameObject, string name)
        {
            if (gameObject.transform.parent == null)
                return null;
            
            Transform brother = gameObject.transform.parent.Find(name);
            return brother == null ? null : brother.gameObject;
        }

        /// <summary>
        /// 获取子物体，名字为指定物体的名字或其路径，即可以按路径查找，避免重名
        /// </summary>
        /// <param name="gameObject">指定物体</param>
        /// <param name="name">子物体名字</param>
        /// <returns>指定子物体</returns>
        public static GameObject GetChild(this GameObject gameObject, string name)
        {
            if (name.Contains("/"))
            {
                Transform child = gameObject.transform.Find(name);
                return child == null ? null : child.gameObject;
            }
            else
            {
                return gameObject.GetChild(child => child.name == name);
            }                
        }

        /// <summary>
        /// 获取满足条件的子物体
        /// </summary>
        /// <param name="gameObject">指定物体</param>
        /// <param name="condition">限制条件</param>
        /// <returns>结果子物体</returns>
        public static GameObject GetChild(this GameObject gameObject, Func<GameObject, bool> condition)
        {
            foreach (Transform child in gameObject.transform)
            {
                if (condition(child.gameObject))
                {
                    return child.gameObject;
                }
                else if (child.childCount > 0)
                {
                    GameObject temp = child.gameObject.GetChild(condition);
                    if (temp != null)
                        return temp;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取所有满足条件的子物体
        /// </summary>
        /// <param name="gameObject">指定物体</param>
        /// <param name="children">结果集合</param>
        /// <param name="condition">限制条件</param>
        public static void GetChildren(this GameObject gameObject, ref List<GameObject> children, Func<GameObject, bool> condition)
        {
            foreach (Transform child in gameObject.transform)
            {
                if (condition(child.gameObject))
                {
                    children.Add(child.gameObject);
                }
                else if (child.childCount > 0)
                {
                    GetChildren(child.gameObject, ref children, condition);
                }
            }
        }
    }
}