using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WJF_CodeLibrary.Singleton;
using System.Linq;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using System.Text.RegularExpressions;

namespace WJF
{
    [RequireComponent(typeof(TraverseVariables))]
	public abstract class TraverseBase : MonoBehaviour
	{
        public enum TraverseStep
        {
            Init = 0,//0.初始设置
            SetCurrent,//1.设置当前顶点
            ValidateCurrent,//2.判断是否有可到达的邻接点，有则等待输入，没有则删除列表中的当前点
            ValidateInput,//3.输入顶点并判断
            End,//结束
        }

        protected TraverseVariables variables;//界面控制
        protected Graph graph;//图
        protected List<GraphVertex> visited;//访问列表
        protected GraphVertex currentVertex;//当前顶点
       public TraverseStep step;//遍历步骤

        protected List<ArrayList> undoList;//撤销序列
        private int undoIndex;//撤销序列下标
        
        private const int maxVerticesCount = 9;

        public abstract string Name { get; }//遍历方法名

        protected virtual void Awake()
        {
            variables = GetComponent<TraverseVariables>();
            variables.Init();

            InitGraph();

            visited = new List<GraphVertex>();
            currentVertex = null;
            step = TraverseStep.Init;

            undoList = new List<ArrayList>();
            undoIndex = 0;
        }

        /// <summary>
        /// 初始化图
        /// </summary>
        private void InitGraph()
        {
            //获取所有顶点的行为
            List<VertexBehaviour> verticesBehaviour = GetComponentsInChildren<VertexBehaviour>().ToList();

            if (verticesBehaviour.Count != maxVerticesCount)
            {
                Debug.Log("顶点行为类数量有错误");
                return;
            }

            //构造图
            #region 设置顶点
            List<GraphVertex> vertices = new List<GraphVertex>();
            for (int i = 1; i <= maxVerticesCount; i++)
            {
                string name = i.ToString();

                VertexBehaviour behaviour = verticesBehaviour.Find(p => p.name == name);
                behaviour.Init(variables.clrNormalVertex, variables.clrCurrentVertex, variables.clrVisitedVertex);

                vertices.Add(new GraphVertex(name, behaviour));
            }
            #endregion

            #region 设置边
            vertices[0].AddNext(vertices[1]);//1-2
            vertices[0].AddNext(vertices[2]);//1-3
            vertices[0].AddNext(vertices[4]);//1-5

            vertices[1].AddNext(vertices[3]);//2-4
            vertices[1].AddNext(vertices[4]);//2-5

            vertices[2].AddNext(vertices[4]);//3-5
            vertices[2].AddNext(vertices[5]);//3-6

            vertices[3].AddNext(vertices[4]);//4-5
            vertices[3].AddNext(vertices[7]);//4-8

            vertices[4].AddNext(vertices[5]);//5-6

            vertices[5].AddNext(vertices[6]);//6-7
            vertices[5].AddNext(vertices[8]);//6-9

            vertices[6].AddNext(vertices[3]);//7-4
            vertices[6].AddNext(vertices[4]);//7-5
            vertices[6].AddNext(vertices[7]);//7-8

            vertices[8].AddNext(vertices[6]);//9-7
            vertices[8].AddNext(vertices[7]);//9-8
            #endregion

            graph = new Graph();
            for (int i = 0; i < vertices.Count; i++)
            {
                graph.AddVertex(vertices[i]);
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void ResetAll()
        {
            visited.Clear();
            currentVertex = null;
            step = TraverseStep.Init;
            undoList.Clear();

            graph.ResetAllVertex();
            variables.ResetAll();

            OnReset();
        }

        /// <summary>
        /// 是否遍历结束
        /// </summary>
        /// <returns></returns>
        public bool IsTraverseEnd()
        {
            return step == TraverseStep.End;
        }

        /// <summary>
        /// 
        /// </summary>
        public void setStart()
        {
            step = TraverseStep.Init;
        } 

        #region 遍历流程控制

        /// <summary>
        /// 执行步骤
        /// </summary>
        public void DoStep()
        {
            RecordUndo();
            variables.RecordUndo();
            
            switch (step)
            {
                case TraverseStep.Init:
                    Init();
                    break;
                case TraverseStep.SetCurrent:
                    SetCurrent();
                    break;
                case TraverseStep.ValidateCurrent:
                    ValidateCurrent();
                    break;
                case TraverseStep.ValidateInput:
                    ValidateInput();
                    break;
            }
        }

        /// <summary>
        /// 遍历初始化
        /// </summary>
        private void Init()
        {
            GraphVertex firstVertex = graph.GetVertex("1");

            //设置提示
            variables.SetStepHint("初始化，从顶点1开始遍历");

            //设置List
            AddListItem(firstVertex);

            //设置next
            variables.SetNext(firstVertex.name);

            //设置visited
            AddVisitedVertex(firstVertex);

            step = TraverseStep.SetCurrent;
        }

        /// <summary>
        /// 设置当前顶点
        /// </summary>
        private void SetCurrent()
        {
            //设置当前顶点状态为已访问状态
            if (currentVertex != null)
            {
                currentVertex.SetState(GraphVertex.VertexState.Visited);
            }

            //列表中还有值，则继续设置
            if (!IsListEmpty())
            {
                //修改提示，由子类决定
                SetSelectVertexHint();

                //跳转至下一个顶点
                currentVertex = GetNextVertexInList();

                //设置顶点为当前访问状态
                currentVertex.SetState(GraphVertex.VertexState.Current);

                //修改当前点text
                variables.SetCurrentVertexText(currentVertex.name);

                step = TraverseStep.ValidateCurrent;
            }
            //列表空了则结束
            else
            {
                //提示
                variables.SetStepHint("List为空，算法结束");

                //修改next
                variables.SetCurrentVertexText("");

                //修改执行按键的文字
                variables.SetRunButtonText("结束");

                step = TraverseStep.End;
            }
        }

        /// <summary>
        /// 验证当前顶点是否有可达到的邻接点
        /// </summary>
        private void ValidateCurrent()
        {
            //判断当前点是否有可到达且未访问的邻接点
            //如果有下一个，则提示输入
            if (currentVertex.HasVisitableNext())
            {
                variables.SetStepHint(string.Format("顶点{0:G}有可访问的邻接点，请输入下一个访问的顶点", currentVertex.name));
                variables.ToggleInputInteractable(true);
                step = TraverseStep.ValidateInput;
            }
            //如果没有，则删除list最后节点并设置当前点
            else
            {
                variables.SetStepHint(string.Format("顶点{0:G}没有可访问的邻接点，从List中删除", currentVertex.name));
                RemoveListItem();
                step = TraverseStep.SetCurrent;
            }
        }
        /// <summary>
        /// 验证输入有效性
        /// </summary>
        private void ValidateInput()
        {
            string inputVertexName = variables.GetInputValue();
            bool wrongResult = false;

            if (inputVertexName == "")
            {
                variables.SetWrongHint("请输入顶点");
            }
            else if (!Regex.IsMatch(inputVertexName, "[1-9]"))
            {
                variables.SetWrongHint("输入无效，请输入数字1-9表示下一个遍历的顶点");
                wrongResult = true;
            }
            else
            {
                GraphVertex nextVertex = graph.GetVertex(inputVertexName);
                
                //如果该顶点是当前顶点
                if (nextVertex.state == GraphVertex.VertexState.Current)
                {
                    variables.SetWrongHint(string.Format("顶点{0:G}是当前遍历到的顶点", nextVertex.name));
                    wrongResult = true;
                }
                //如果不是当前顶点的邻接点
                else if (!currentVertex.IsNext(nextVertex))
                {
                    variables.SetWrongHint(string.Format("顶点{0:G}不是顶点{1:G}的邻接点", nextVertex.name, currentVertex.name));
                    wrongResult = true;
                }
                //如果该顶点已访问
                else if (nextVertex.state == GraphVertex.VertexState.Visited)
                {
                    variables.SetWrongHint(string.Format("顶点{0:G}已访问过", nextVertex.name));
                    wrongResult = true;
                }                
                //顶点输入有效
                else
                {
                    //设置提示
                    variables.SetStepHint(string.Format("访问顶点{0:G}", nextVertex.name));

                    //清空输入框
                    variables.ClearInput();

                    //禁用输入框
                    variables.ToggleInputInteractable(false);

                    //修改颜色
                    nextVertex.SetState(GraphVertex.VertexState.Visited);

                    //修改list
                    AddListItem(nextVertex);

                    //修改next
                    variables.SetNext((visited.Count + 1).ToString());

                    //修改visited
                    AddVisitedVertex(nextVertex);

                    //由子类决定下一步
                    OnValidateInputComplete();
                }
            }

            //记录错误数
            if (wrongResult)
                OnWrongCount();
        }

        #endregion

        protected abstract bool IsListEmpty();//遍历列表是否为空
        protected abstract void OnReset();//重置
        protected abstract GraphVertex GetNextVertexInList();//获取遍历列表中的下一个节点
        protected abstract void SetSelectVertexHint();//设置选择节点提示
        protected abstract void OnValidateInputComplete();//验证输入
        protected abstract void AddListItem(GraphVertex vert);//添加到List
        protected abstract void RemoveListItem();//删除List中的元素
        protected abstract void OnWrongCount();//记录错误数
        protected abstract void OnRecordUndo(ArrayList arr);//记录撤销
        protected abstract void OnUndo(ArrayList arr);//撤销

        /// <summary>
        /// 添加访问点
        /// </summary>
        /// <param name="vert"></param>
        protected void AddVisitedVertex(GraphVertex vert)
        {
            visited.Add(vert);
            variables.SetVisited(visited.Count - 1, vert.name);
        }

        /// <summary>
        /// 记录撤销
        /// </summary>
        protected void RecordUndo()
        {
            ArrayList arr = new ArrayList();

            //0 记录每个点的状态
            int vertCount = graph.GetVertexCount();
            List<GraphVertex.VertexState> vertState = new List<GraphVertex.VertexState>();
            for (int i = 0; i < vertCount; i++)
            {
                vertState.Add(graph.GetVertex(i).GetState());
            }
            arr.Add(vertState);

            //1 记录访问列表状态
            List<GraphVertex> preVisited = new List<GraphVertex>(visited);
            arr.Add(preVisited);

            //2 记录当前顶点
            GraphVertex preCurrent = currentVertex == null ? null : currentVertex.Clone() as GraphVertex;
            arr.Add(preCurrent);

            //3 记录步骤枚举
            arr.Add(step);

            OnRecordUndo(arr);

            undoList.Add(arr);
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public void Undo()
        {
            //Debug.Log("===Undo===");
            ArrayList arr = undoList[undoList.Count - 1];

            //恢复每个点的状态
            int vertCount = graph.GetVertexCount();
            List<GraphVertex.VertexState> vertState = arr[0] as List<GraphVertex.VertexState>;
            for (int i = 0; i < vertCount; i++)
            {
                graph.GetVertex(i).SetState(vertState[i]);
                //Debug.Log(graph.GetVertex(i).name + " - " + graph.GetVertex(i).GetState());
            }

            //恢复访问列表状态
            List<GraphVertex> preVisited = arr[1] as List<GraphVertex>;
            visited = preVisited;

            //恢复当前顶点
            GraphVertex preCurrent = arr[2] as GraphVertex;
            currentVertex = preCurrent == null ? null : graph.GetVertex(preCurrent.name);

            //恢复枚举
            TraverseStep preStep = (TraverseStep)arr[3];
            step = preStep;

            OnUndo(arr);

            undoList.RemoveAt(undoList.Count - 1);
        }
    }
}