using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WJF_CodeLibrary.CommonUtility;

namespace WJF
{
    /// <summary>
    /// 广度优先搜索
    /// </summary>
	public class BellmanBreadthFirstSearch : BellmanTraverseBase
    {
        private Queue<BellmanGraphVertex> traverseQueue;//遍历队列
        private int queueFirstIndex;//队列头在遍历列表中的下标

        public override string Name
        {
            get
            {
                return "Bellman–Ford算法";
            }
        }

        protected override void Awake()
        {
            base.Awake();

            traverseQueue = new Queue<BellmanGraphVertex>();
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

        protected override BellmanGraphVertex GetNextVertexInList()
        {
            return traverseQueue.Peek();
        }

        protected override void SetSelectVertexHint()
        {
            variables.SetStepHint("选择List中的第一个顶点");
        }

        //用户输入正确后，执行列表数据更新逻辑
        protected override void OnValidateInputComplete(string currentName, string nextName, string nextValue, BellmanTraverseVariables variables)
        {
            int currentValue = variables.GetVertexValueInForm(currentName);
            int nextVertexValue = variables.GetVertexValueInForm(nextName);
            int nextInputValue = int.Parse(nextValue);
            variables.SetFormChangeTip("");

            CallbackUtility.ExecuteDelay(0.5f, () =>
            {
                if (currentValue + nextInputValue < nextVertexValue)
                {
                    //如果有更新则更新标志位，代表本次遍历有值更新
                    formValueChanged = true;
                    variables.SetFormChangeTip(string.Format("表中【A -> {0:G}】 + 目前【{1:G} -> {2:G}】 小于右表 【A -> {3:G}】     更新距离表!\n  {4:G}   +   {5:G}   <   {6:G}", currentName, currentName, nextName, nextName, currentValue.ToString(), nextInputValue.ToString(), nextVertexValue == variables.formValueMax ? "∞" : nextVertexValue.ToString()));
                    variables.SetVertexValueInForm(nextName, currentName, (currentValue + nextInputValue).ToString());
                }else{
                    variables.SetFormChangeTip(string.Format("表中【A -> {0:G}】 + 目前【{1:G} -> {2:G}】 大于等于右表 【A -> {3:G}】     不更新!\n  {4:G}   +   {5:G}   >=   {6:G}", currentName, currentName, nextName, nextName, currentValue.ToString(), nextInputValue.ToString(), nextVertexValue == variables.formValueMax ? "∞" : nextVertexValue.ToString()));
                    variables.ResetFormChangeColor();
                }
                step = BellmanTraverseStep.ValidateCurrent;
            });
        }

        protected override void AddListItem(BellmanGraphVertex vert)
        {
            if (!traverseQueue.Contains(vert) && vert.state == BellmanGraphVertex.VertexState.Normal)
            {
                variables.SetListItemDisplay(traverseQueue.Count + queueFirstIndex, vert.name, true);
                traverseQueue.Enqueue(vert);
            }
        }

        protected override void RemoveListItem()
        {
            variables.FadeOutListItem(queueFirstIndex);
            traverseQueue.Dequeue();
            queueFirstIndex++;
        }

        protected override void OnVertexWrongCount()
        {
            Statistics.AddWrong(SysDefine.Statistics.Bellman_Vertex_Input);
        }

        protected override void OnDisWrongCount()
        {
            Statistics.AddWrong(SysDefine.Statistics.Bellman_Dis_Input);
        }
    }
}