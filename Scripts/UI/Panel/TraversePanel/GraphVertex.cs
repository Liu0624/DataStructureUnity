using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WJF
{
	public class GraphVertex : ICloneable
    {
        public enum VertexState { Normal, Current, Visited }//顶点状态：普通、当前、已访问

        public string name { private set; get; }//名字
        public VertexBehaviour behaviour { private set; get; }//显示行为
        public VertexState state { private set; get; }//状态

        private List<GraphVertex> nextList;//邻接点集合

        public GraphVertex(string name, VertexBehaviour behaviour)
        {
            this.name = name;
            this.behaviour = behaviour;
            nextList = new List<GraphVertex>();
        }

        /// <summary>
        /// 浅拷贝
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// 添加邻接点
        /// </summary>
        /// <param name="next"></param>
        public void AddNext(GraphVertex next)
        {
            if (!nextList.Contains(next))
            {
                nextList.Add(next);
            }
        }

        /// <summary>
        /// 获取邻接点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GraphVertex GetNext(string name)
        {
            return nextList.Find(p => p.name == name);
        }

        /// <summary>
        /// 是否是邻接点
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool IsNext(GraphVertex vertex)
        {
            return nextList.Contains(vertex);
        }

        /// <summary>
        /// 是否有可访问的邻接点
        /// </summary>
        /// <returns></returns>
        public bool HasVisitableNext()
        {
            if (nextList.Count == 0)
                return false;

            foreach (var next in nextList)
            {
                if (next.state == VertexState.Normal)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="state"></param>
        public void SetState(VertexState state)
        {
            this.state = state;
            behaviour.SetColor(state);
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <returns></returns>
        public VertexState GetState()
        {
            return state;
        }
    }
}