using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WJF
{
	public class BellmanGraph
	{
        private List<BellmanGraphVertex> vertices;//顶点集合

        public BellmanGraph()
        {
            vertices = new List<BellmanGraphVertex>();
        }

        /// <summary>
        /// 添加顶点
        /// </summary>
        /// <param name="vertex"></param>
        public void AddVertex(BellmanGraphVertex vertex)
        {
            if (!vertices.Contains(vertex))
            {
                vertices.Add(vertex);
            }
        }

        /// <summary>
        /// 获取顶点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public BellmanGraphVertex GetVertex(int index)
        {
            return vertices[index];
        }

        /// <summary>
        /// 获取顶点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BellmanGraphVertex GetVertex(string name)
        {
            return vertices.Find(p => p.name == name);
        }

        /// <summary>
        /// 获取顶点数量
        /// </summary>
        /// <returns></returns>
        public int GetVertexCount()
        {
            return vertices.Count;
        }

        /// <summary>
        /// 重置顶点
        /// </summary>
        public void ResetAllVertex()
        {
            foreach (var v in vertices)
            {
                v.SetState(BellmanGraphVertex.VertexState.Normal);
            }
        }
	}
}