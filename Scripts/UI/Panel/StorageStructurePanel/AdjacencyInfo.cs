using System.Collections.Generic;
using UnityEngine;

namespace WJF
{
    /// <summary>
    /// 图的邻接点信息
    /// </summary>
	public class AdjacencyInfo
	{
        public List<AdjacencyInfoItem> vertices { private set; get; }//顶点信息集合

        public static bool isDigraph = true;

        private static AdjacencyInfo digraphInstance;
        private static AdjacencyInfo unDigraphInstance;

        public static AdjacencyInfo Instance
        {
            get
            {
                if (isDigraph)
                {
                   return AdjacencyInfo.DigraphInstance;
                }
                return AdjacencyInfo.UndigraphInstance;
            }
        }

        private static AdjacencyInfo DigraphInstance
        {
            get
            {
                if (digraphInstance == null)
                {
                    digraphInstance = new AdjacencyInfo();
                    digraphInstance.initDigraphVertices();
                }
                return digraphInstance;
            }
        }

        private static AdjacencyInfo UndigraphInstance
        {
            get
            {
                if (unDigraphInstance == null)
                {
                    unDigraphInstance = new AdjacencyInfo();
                    unDigraphInstance.initUndigraphVertices();
                }
                return unDigraphInstance;
            }
        }

        //初始化有向图的节点信息
        private void initDigraphVertices(){
            //构建图
            vertices = new List<AdjacencyInfoItem>();

            AdjacencyInfoItem v0 = new AdjacencyInfoItem(0, "A");
            v0.AddNext(1, 3);
            v0.AddNext(6, 2);
            vertices.Add(v0);

            AdjacencyInfoItem v1 = new AdjacencyInfoItem(1, "B");
            v1.AddNext(2, 3);
            vertices.Add(v1);

            AdjacencyInfoItem v2 = new AdjacencyInfoItem(2, "C");
            v2.AddNext(3, 4);
            vertices.Add(v2);

            AdjacencyInfoItem v3 = new AdjacencyInfoItem(3, "D");
            v3.AddNext(4, 6);
            vertices.Add(v3);

            AdjacencyInfoItem v4 = new AdjacencyInfoItem(4, "E");
            v4.AddNext(0, 9);
            v4.AddNext(5, 7);
            vertices.Add(v4);

            AdjacencyInfoItem v5 = new AdjacencyInfoItem(5, "F");
            v5.AddNext(0, 8);
            vertices.Add(v5);

            AdjacencyInfoItem v6 = new AdjacencyInfoItem(6, "G");
            vertices.Add(v6);
        }

        //初始化无向图的节点信息
        private void initUndigraphVertices(){
             //构建图
            vertices = new List<AdjacencyInfoItem>();

            AdjacencyInfoItem v0 = new AdjacencyInfoItem(0, "A");
            v0.AddNext(1, 0);
            v0.AddNext(4, 0);
            v0.AddNext(5, 0);
            vertices.Add(v0);

            AdjacencyInfoItem v1 = new AdjacencyInfoItem(1, "B");
            v1.AddNext(0, 0);
            v1.AddNext(2, 0);
            v1.AddNext(5, 0);
            vertices.Add(v1);

            AdjacencyInfoItem v2 = new AdjacencyInfoItem(2, "C");
            v2.AddNext(1, 0);
            v2.AddNext(3, 0);
            vertices.Add(v2);

            AdjacencyInfoItem v3 = new AdjacencyInfoItem(3, "D");
            v3.AddNext(2, 0);
            v3.AddNext(4, 0);
            vertices.Add(v3);

            AdjacencyInfoItem v4 = new AdjacencyInfoItem(4, "E");
            v4.AddNext(0, 0);
            v4.AddNext(3, 0);
            vertices.Add(v4);

            AdjacencyInfoItem v5 = new AdjacencyInfoItem(5, "F");
            v5.AddNext(0, 0);
            v5.AddNext(1, 0);
            v5.AddNext(6, 0);
            vertices.Add(v5);

            AdjacencyInfoItem v6 = new AdjacencyInfoItem(6, "G");
            v6.AddNext(5, 0);
            vertices.Add(v6);
        }

        /// <summary>
        /// 获取边的权重
        /// </summary>
        /// <param name="v1Num"></param>
        /// <param name="v2Num"></param>
        /// <returns></returns>
        public int GetWeight(int v1Num, int v2Num)
        {
            AdjacencyInfoItem v1 = vertices.Find(p => p.num == v1Num);
            return v1.GetWeight(v2Num);
        }
	}

    /// <summary>
    /// 顶点信息
    /// </summary>
    public class AdjacencyInfoItem
    {
        public int num { private set; get; }//序号
        public string name { private set; get; }//名字

        private List<KeyValuePair<int, int>> nextList;//对应点的序号，权重

        public AdjacencyInfoItem(int num, string name)
        {
            this.num = num;
            this.name = name;
            nextList = new List<KeyValuePair<int, int>>();
        }

        /// <summary>
        /// 添加邻接点
        /// </summary>
        /// <param name="num"></param>
        /// <param name="weight"></param>
        public void AddNext(int num , int weight)
        {
            nextList.Add(new KeyValuePair<int, int>(num, weight));
        }

        /// <summary>
        /// 获取权重
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public int GetWeight(int num)
        {
            if (!nextList.Exists(p => p.Key == num))
                return -1;

            return nextList.Find(p => p.Key == num).Value;
        }

        /// <summary>
        /// 获取邻接点数量
        /// </summary>
        /// <returns></returns>
        public int GetNextCount()
        {
            return nextList.Count;
        }

        /// <summary>
        /// 获取所有邻接点的序号
        /// </summary>
        /// <returns></returns>
        public List<int> GetNextNums()
        {
            List<int> nums = new List<int>();

            foreach (var next in nextList)
            {
                nums.Add(next.Key);
            }

            return nums;
        }
    }
}