namespace WJF
{
    /// <summary>
    /// Normal
    /// </summary>
    public partial class SysDefine
    {
        public const string NoData = "-";

        public struct Statistics
        {
            public const string CollectionInDigraph   = "有向图的顶点与边的集合";
            public const string MatrixInDigraph       = "有向图邻接矩阵";
            public const string CollectionInUndigraph = "无向图的顶点与边的集合";
            public const string MatrixInUndigraph     = "无向图邻接矩阵";

            // public const string ProgramFilling        = "有向图程序代码填空";
            // public const string ProgramRun            = "有向图程序运行填空";
            public const string AdjacencyList         = "有向图邻接表";
            public const string  AdjacencyListDigraph = "无向图邻接表";

            public const string DFS_Input             = "深度优先搜索顶点输入";
            public const string BFS_Input             = "广度优先搜索顶点输入";

            //第二幅图
            public const string DisInDigraph = "Dijkstra算法有向图填空";
            public const string DisInUndigraph = "Dijkstra算法无向图填空";

            //第三幅图
            public const string MatrixFilling = "Floyd算法邻接矩阵填空";
            public const string ProgramFilling = "Floyd算法程序代码填空";
            public const string ProgramRun = "Floyd算法程序运行填空";

            //第四幅图
            public const string Bellman_Vertex_Input = "Bellman–Ford算法顶点输入";
            public const string Bellman_Dis_Input = "Bellman–Ford算法权值输入";
        }
    }
}