using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WJF
{
    /// <summary>
    /// 广度优先搜索
    /// </summary>
	public class BreadthFirstSearch : TraverseBase
	{	
        //private enum BFS_Step
        //{
        //    Init = 0,//初始化
        //    SetCurrent,//设置当前顶点
        //    ValidateCurrent,//验证当前顶点有效性
        //    ValidateInput,//验证输入
        //}

        private Queue<GraphVertex> traverseQueue;//遍历队列
        private int queueFirstIndex;//队列头在遍历列表中的下标

        public override string Name
        {
            get
            {
                return "广度优先搜索";
            }
        }

        protected override void Awake()
        {
            base.Awake();

            traverseQueue = new Queue<GraphVertex>();
            queueFirstIndex = 0;
        }

        protected override bool IsListEmpty()
        {
            return traverseQueue.Count == 0;
        }

        protected override void OnReset()
        {
            traverseQueue.Clear();
            queueFirstIndex = 0;
        }

        protected override GraphVertex GetNextVertexInList()
        {
            return traverseQueue.Peek();
        }

        protected override void SetSelectVertexHint()
        {
            variables.SetStepHint("选择List中的第一个顶点");
        }

        protected override void OnValidateInputComplete()
        {
            step = TraverseStep.ValidateCurrent;
        }

        protected override void AddListItem(GraphVertex vert)
        {
            variables.SetListItemDisplay(traverseQueue.Count + queueFirstIndex, vert.name, true);
            traverseQueue.Enqueue(vert);
        }

        protected override void RemoveListItem()
        {
            variables.FadeOutListItem(queueFirstIndex);
            traverseQueue.Dequeue();
            queueFirstIndex++;
        }

        protected override void OnWrongCount()
        {
            Statistics.AddWrong(SysDefine.Statistics.BFS_Input);
        }

        protected override void OnRecordUndo(ArrayList arr)
        {
            //4 记录队列
            Queue<GraphVertex> preQueue = new Queue<GraphVertex>(traverseQueue);
            arr.Add(preQueue);

            //5 记录队列头索引
            arr.Add(queueFirstIndex);
        }

        protected override void OnUndo(ArrayList arr)
        {
            //恢复队列
            Queue<GraphVertex> preQueue = arr[4] as Queue<GraphVertex>;
            traverseQueue = preQueue;

            //恢复队列头索引
            queueFirstIndex = (int)arr[5];
        }
    }
}