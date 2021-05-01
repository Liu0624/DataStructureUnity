using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WJF
{
    public class BellmanGraphVertex : ICloneable
    {
        public enum VertexState { Normal, Current, Visited }//顶点状态：普通、当前、已访问

        public string name { private set; get; }//名字
        public BellmanVertexBehaviour behaviour { private set; get; }//显示行为
        public VertexState state { private set; get; }//状态

        public BellmanGraphVertex fatherVertex { private set; get; } //父节点

        private List<BellmanGraphVertex> nextList;//邻接点集合

        public BellmanGraphVertex(string name, BellmanVertexBehaviour behaviour)
        {
            this.name = name;
            this.behaviour = behaviour;
            nextList = new List<BellmanGraphVertex>();
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
        public void AddNext(BellmanGraphVertex next)
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
        public BellmanGraphVertex GetNext(string name)
        {
            return nextList.Find(p => p.name == name);
        }

        /// <summary>
        /// 是否是邻接点
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool IsNext(BellmanGraphVertex vertex)
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
            {
                return false;
            }

            foreach (BellmanGraphVertex nextVertex in nextList)
            {
                if (nextVertex.state == VertexState.Normal)
                {
                    return true;
                }
                else
                {
                    if (!nextVertex.fatherVertex.name.Equals(name) && !name.Equals("A"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="state"></param>
        public void SetState(VertexState state)
        {
            //如果当前点的状态和要设置的一样就直接返回
            if (this.state == state)
            {
                return;
            }

            this.state = state;
            behaviour.SetColor(state);
        }


        /// <summary>
        /// 设置父节点值
        /// </summary>
        /// <param name="father"></param>
        public void SetFather(BellmanGraphVertex father)
        {
            this.fatherVertex = father;
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