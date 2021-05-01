using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WJF
{
	public class AdjacencyGroup : MonoBehaviour
	{
        private List<AdjacencyItem> adjItems = new List<AdjacencyItem>();//邻接点集合

        /// <summary>
        /// 添加邻接点
        /// </summary>
        /// <param name="item"></param>
        public void Add(AdjacencyItem item)
        {
            if (!adjItems.Contains(item))
            {
                adjItems.Add(item);
            }                
        }

        /// <summary>
        /// 删除邻接点
        /// </summary>
        /// <param name="item"></param>
        public void Remove(AdjacencyItem item)
        {
            if (adjItems.Contains(item))
                adjItems.Remove(item);
        }

        /// <summary>
        /// 清空邻接点
        /// </summary>
        public void Clear()
        {
            for (int i = adjItems.Count - 1; i >= 0; i--)
            {
                AdjacencyItem item = adjItems[i];

                //如果是顶点则不删除，只删除邻接点
                if (item.type != AdjacencyItem.ItemType.Vertex)
                {
                    Destroy(item.arrowGo);
                    Destroy(item.gameObject);
                    adjItems.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 获取邻接点数量
        /// </summary>
        /// <returns></returns>
        public int GetItemsCount()
        {
            return adjItems.Count;
        }

        /// <summary>
        /// 获取顶点
        /// </summary>
        /// <returns></returns>
        public AdjacencyItem GetRootVertex()
        {
            return adjItems.Find(p => p.type == AdjacencyItem.ItemType.Vertex);
        }

        /// <summary>
        /// 获取顶点序号
        /// </summary>
        /// <returns></returns>
        public int GetRootVertexNum()
        {
            return GetRootVertex().num;
        }

        /// <summary>
        /// 指定点是否是该顶点的邻接点
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool ContainsAdjNode(int num)
        {
            AdjacencyItem item = GetRootVertex().next;

            while (item != null)
            {
                if (item.num == num)
                    return true;

                item = item.next;
            }
            return false;
        }

        /// <summary>
        /// 验证正确性
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            return ValidateAccuracy() && ValidateIntegrity();
        }

        /// <summary>
        /// 检查是否填写完整
        /// </summary>
        /// <returns></returns>
        private bool ValidateAccuracy()
        {
            List<AdjacencyItem> adjList = adjItems.FindAll(p => p.type == AdjacencyItem.ItemType.Adjacency);
            foreach (var item in adjList)
                if (!item.validation)
                    return false;

            return true;
        }

        /// <summary>
        /// 检查是否有错误填写
        /// </summary>
        /// <returns></returns>
        private bool ValidateIntegrity()
        {
            int vertNum = GetRootVertexNum();
            AdjacencyInfoItem v = AdjacencyInfo.Instance.vertices.Find(p => p.num == vertNum);
            List<int> nextNums = v.GetNextNums();
            AdjacencyItem item = adjItems.Find(p => p.type == AdjacencyItem.ItemType.Vertex && p.num == v.num);

            foreach (var nextNum in nextNums)
            {
                if (!ContainsAdjNode(nextNum))
                    return false;
            }

            return true;
        }
	}
}