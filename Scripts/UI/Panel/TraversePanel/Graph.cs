using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WJF
{
	public class Graph
	{
        private List<GraphVertex> vertices;//顶点集合

        public Graph()
        {
            vertices = new List<GraphVertex>();
        }

        /// <summary>
        /// 添加顶点
        /// </summary>
        /// <param name="vertex"></param>
        public void AddVertex(GraphVertex vertex)
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
        public GraphVertex GetVertex(int index)
        {
            return vertices[index];
        }

        /// <summary>
        /// 获取顶点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GraphVertex GetVertex(string name)
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
                v.SetState(GraphVertex.VertexState.Normal);
            }
        }
	}
}