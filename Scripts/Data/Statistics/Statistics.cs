using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;

namespace WJF
{
    public class Statistics
    {
        public class StudyItem
        {
            public string title;
            public int wrongCount;
            public bool complete = false;

            public StudyItem(string title, int wrongCount)
            {
                this.title = title;
                this.wrongCount = wrongCount;
            }
        }

        private static List<StudyItem> itemsWrongCounter;

        public static String startTime;
        public static String endTime;

        public static void Init()
        {
            if (itemsWrongCounter == null)
            {
                itemsWrongCounter = new List<StudyItem>();
                itemsWrongCounter.Add(new StudyItem(SysDefine.Statistics.MatrixInDigraph, 0));
                itemsWrongCounter.Add(new StudyItem(SysDefine.Statistics.MatrixInUndigraph, 0));
                itemsWrongCounter.Add(new StudyItem(SysDefine.Statistics.AdjacencyList, 0));
                itemsWrongCounter.Add(new StudyItem(SysDefine.Statistics.AdjacencyListDigraph, 0));
                itemsWrongCounter.Add(new StudyItem(SysDefine.Statistics.DFS_Input, 0));
                itemsWrongCounter.Add(new StudyItem(SysDefine.Statistics.BFS_Input, 0));
                itemsWrongCounter.Add(new StudyItem(SysDefine.Statistics.DisInDigraph, 0));
                itemsWrongCounter.Add(new StudyItem(SysDefine.Statistics.MatrixFilling, 0));
                itemsWrongCounter.Add(new StudyItem(SysDefine.Statistics.ProgramFilling, 0));
                itemsWrongCounter.Add(new StudyItem(SysDefine.Statistics.ProgramRun, 0));
                itemsWrongCounter.Add(new StudyItem(SysDefine.Statistics.Bellman_Vertex_Input, 0));
                itemsWrongCounter.Add(new StudyItem(SysDefine.Statistics.Bellman_Dis_Input, 0));
            }
            else
            {
                itemsWrongCounter.ForEach(p =>
                {
                    p.wrongCount = 0;
                });
            }
        }

        public static List<StudyItem> GetItemsCounter()
        {
            return itemsWrongCounter;
        }

        public static void AddWrong(string title)
        {
            StudyItem item = itemsWrongCounter.Find(p => p.title == title);
            if (item != null)
            {
                item.wrongCount++;
            }
        }
        public static void setScore(int score, string title)
        {
            itemsWrongCounter.Find(p => p.title == title).wrongCount = score;
        }

        public static void setComplete(string title)
        {
            StudyItem item = itemsWrongCounter.Find(p => p.title == title);
            if (item != null)
            {
                item.complete = true;
            }
        }

        public static bool isComplete(string title)
        {
            return itemsWrongCounter.Find(p => p.title == title).complete;
        }

        public static int GetWrongCount(string title)
        {
            return itemsWrongCounter.Find(p => p.title == title).wrongCount;
        }

        /// <summary>
        /// 03-20添：重置分数
        /// </summary>
        /// <param name="title"></param>
        public static void ResetWrong(string title)
        {
            StudyItem item = itemsWrongCounter.Find(p => p.title == title);
            if(item != null)
            {
                item.wrongCount = 0;
            }
        }

        public static string GetResultJsonStr()
        {
            Dictionary<string, Dictionary<string, int>> result = new Dictionary<string, Dictionary<string, int>>();
            Dictionary<string, int> renzhi = new Dictionary<string, int>();
            Dictionary<string, int> dj = new Dictionary<string, int>();
            Dictionary<string, int> floyd = new Dictionary<string, int>();
            Dictionary<string, int> bellman = new Dictionary<string, int>();


            //图的认知
            renzhi.Add(SysDefine.Statistics.MatrixInDigraph, getMatrixScore(SysDefine.Statistics.MatrixInDigraph));
            renzhi.Add(SysDefine.Statistics.MatrixInUndigraph, getMatrixScore(SysDefine.Statistics.MatrixInUndigraph));

            renzhi.Add(SysDefine.Statistics.AdjacencyList, getMatScore(SysDefine.Statistics.AdjacencyList));
            renzhi.Add(SysDefine.Statistics.AdjacencyListDigraph, getMatScore(SysDefine.Statistics.AdjacencyListDigraph));

            renzhi.Add(SysDefine.Statistics.DFS_Input, getAdjacencyListScore(SysDefine.Statistics.DFS_Input));
            renzhi.Add(SysDefine.Statistics.BFS_Input, getAdjacencyListScore(SysDefine.Statistics.BFS_Input));

            //dj最短路径
            dj.Add(SysDefine.Statistics.DisInDigraph, getDJScore(SysDefine.Statistics.DisInDigraph));

            //Floyd最短路径
            floyd.Add(SysDefine.Statistics.MatrixFilling, getMatrixScore(SysDefine.Statistics.MatrixFilling));
            floyd.Add(SysDefine.Statistics.ProgramFilling, getMatrixScore(SysDefine.Statistics.ProgramFilling));
            floyd.Add( SysDefine.Statistics.ProgramRun, getFlyodScore(SysDefine.Statistics.ProgramRun));

            //Bellman最短路径
            bellman.Add(SysDefine.Statistics.Bellman_Vertex_Input, getAdjacencyListScore(SysDefine.Statistics.Bellman_Vertex_Input));
            bellman.Add(SysDefine.Statistics.Bellman_Dis_Input, getAdjacencyListScore(SysDefine.Statistics.Bellman_Dis_Input));

            

            result.Add("图的认知", renzhi);
            result.Add("Dijestra算法", dj);
            result.Add("floyd算法", floyd);
            result.Add("bellman_foyd算法", bellman);
            return Regex.Unescape(JsonConvert.SerializeObject(result));
        }

        private static int getMatrixScore(string name)
        {
            int score = 0;
            int count = GetWrongCount(name);
            bool complete = isComplete(name);

            if (!complete)
            {
                return score;
            }

            switch (count)
            {
                case 0:
                    score = 3;
                    break;
                case 1:
                    score = 2;
                    break;
                case 2:
                    score = 1;
                    break;
                case 3:
                default:
                    break;
            }

            return score;
        }

        private static int getMatScore(string name)
        {
            int score = 0;
            int count = GetWrongCount(name);
            bool complete = isComplete(name);

            if (!complete)
            {
                return score;
            }

            score = 6 - count;

            if (score < 0) score = 0;

            return score;
        }

        // 添加邻接点算分，错误5次以内，每次扣20分，5次以上0分
        private static int getAdjacencyListScore(string name)
        {
            int count = GetWrongCount(name);
            bool complete = isComplete(name);

            if (!complete)
            {
                return 0;
            }
            
            if (count <= 5)
            {
                return 5 - count ;
            }
            return 0;
        }

        //DJ算法算分规则，共3*4=12次错误，均分取值
        private static int getDJScore(string name)
        {
            int count = GetWrongCount(name);
            bool complete = isComplete(name);

            if (!complete)
            {
                return 0;
            }

            int score = 12 - 2 * count;
            if (score >= 0) return score;

            return 0;
        }

        private static int getFlyodScore(string name)
        {
            int count = GetWrongCount(name);
            bool complete = isComplete(name);

            if (!complete)
            {
                return 0;
            }

            int score = 24 - 2 * count;
            if (score >= 0) return score;

            return 0;
        }


    }
}