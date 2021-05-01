using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WJF_CodeLibrary.Extension;
using System.Text.RegularExpressions;

namespace WJF
{
    /// <summary>
    /// 深度优先搜索
    /// </summary>
	public class DepthFirstSearch : TraverseBase
	{
        //private enum DFS_Step
        //{
        //    Init = 0,//0.初始设置
        //    SetCurrent,//1.设置当前顶点
        //    ValidateCurrent,//2.判断是否有可到达的邻接点，有则等待输入，没有则删除列表中的当前点
        //    ValidateInput,//3.输入顶点并判断
        //    SwitchVertex,//4.切换当前顶点到下一个顶点
        //    SetNext,//5.设置下一个顶点为访问态，修改List，next，visited及显示
        //    Traceback,//6.回溯至上一个
        //    End,//7.列表空，结束
        //}

        private List<GraphVertex> traverseList;//遍历过程中用到的列表

        public override string Name
        {
            get
            {
                return "深度优先搜索";
            }
        }

        protected override void Awake()
        {
            base.Awake();

            traverseList = new List<GraphVertex>();
        }

        protected override bool IsListEmpty()
        {
            return traverseList.Count == 0;
        }

        protected override void OnReset()
        {
            traverseList.Clear();
        }

        protected override GraphVertex GetNextVertexInList()
        {
            return traverseList[traverseList.Count - 1];
        }

        protected override void SetSelectVertexHint()
        {
            variables.SetStepHint("选择List中最后的顶点");
        }

        protected override void OnValidateInputComplete()
        {
            step = TraverseStep.SetCurrent;
        }

        protected override void AddListItem(GraphVertex vert)
        {
            traverseList.Add(vert);
            variables.SetListItemDisplay(traverseList.Count - 1, vert.name, true);
        }

        protected override void RemoveListItem()
        {
            variables.FadeOutListItem(traverseList.Count - 1);
            traverseList.RemoveAt(traverseList.Count - 1);
        }

        protected override void OnWrongCount()
        {
            Statistics.AddWrong(SysDefine.Statistics.DFS_Input);
        }

        protected override void OnRecordUndo(ArrayList arr)
        {
            //4 记录遍历列表
            List<GraphVertex> preList = new List<GraphVertex>(traverseList);
            arr.Add(preList);
        }

        protected override void OnUndo(ArrayList arr)
        {
            List<GraphVertex> preList = arr[4] as List<GraphVertex>;
            traverseList = preList;
        }
    }
}