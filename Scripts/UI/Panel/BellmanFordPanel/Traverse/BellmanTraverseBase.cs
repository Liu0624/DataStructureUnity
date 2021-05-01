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
    [RequireComponent(typeof(BellmanTraverseVariables))]
    public abstract class BellmanTraverseBase : MonoBehaviour
    {
        protected enum BellmanTraverseStep
        {
            Init = 0,//0.初始设置
            SetCurrent,//1.设置当前顶点
            ValidateCurrent,//2.判断是否有可到达的邻接点，有则等待输入，没有则删除列表中的当前点
            ValidateInput,//3.输入顶点并判断
            End,//结束
        }

        protected BellmanTraverseVariables variables;//界面控制
        protected BellmanGraph graph;//图
        protected List<BellmanGraphVertex> visited;//访问列表
        protected BellmanGraphVertex currentVertex;//当前顶点
        protected BellmanTraverseStep step;//遍历步骤

        private const int maxVerticesCount = 7;

        private int goThroughCount = 1;//贝尔曼福特算法遍历的次数，最多为顶点数减1

        protected bool formValueChanged = false;//标志每次遍历表格中是否有值改变

        //完整的边权值集合
        private Dictionary<string, string> disAnswerMap = new Dictionary<string, string>();

        public abstract string Name { get; }//遍历方法名

        protected virtual void Awake()
        {
            variables = GetComponent<BellmanTraverseVariables>();
            variables.Init();

            InitGraph();

            visited = new List<BellmanGraphVertex>();
            currentVertex = null;
            step = BellmanTraverseStep.Init;

            disAnswerMap.Add("AB", "6");
            disAnswerMap.Add("AC", "5");
            disAnswerMap.Add("AD", "5");
            disAnswerMap.Add("BE", "-1");
            disAnswerMap.Add("CB", "-2");
            disAnswerMap.Add("CE", "1");
            disAnswerMap.Add("DC", "-2");
            disAnswerMap.Add("DF", "-1");
            disAnswerMap.Add("EG", "3");
            disAnswerMap.Add("FG", "3");
        }

        /// <summary>
        /// 初始化图
        /// </summary>
        private void InitGraph()
        {
            //获取所有顶点的行为
            List<BellmanVertexBehaviour> verticesBehaviour = GetComponentsInChildren<BellmanVertexBehaviour>().ToList();

            if (verticesBehaviour.Count != maxVerticesCount)
            {
                Debug.Log("顶点行为类数量有错误");
                return;
            }

            //构造图
            #region 设置顶点
            List<BellmanGraphVertex> vertices = new List<BellmanGraphVertex>();
            for (int i = 1; i <= maxVerticesCount; i++)
            {
                string name = ((char)('A' + i - 1)).ToString();

                BellmanVertexBehaviour behaviour = verticesBehaviour.Find(p => p.name == name);
                behaviour.Init(variables.clrNormalVertex, variables.clrCurrentVertex, variables.clrVisitedVertex);
                BellmanGraphVertex graphVertex = new BellmanGraphVertex(name, behaviour);
                graphVertex.SetFather(new BellmanGraphVertex("--", behaviour));
                vertices.Add(graphVertex);
            }
            #endregion

            #region 设置边
            //A
            vertices[0].AddNext(vertices[1]);//A-B
            vertices[0].AddNext(vertices[2]);//A-C
            vertices[0].AddNext(vertices[3]);//A-D

            //B
            vertices[1].AddNext(vertices[4]);//B-E

            //C
            vertices[2].AddNext(vertices[1]);//C-B
            vertices[2].AddNext(vertices[4]);//C-E

            //D
            vertices[3].AddNext(vertices[2]);//D-C
            vertices[3].AddNext(vertices[5]);//D-F

            //E
            vertices[4].AddNext(vertices[6]);//E-G

            //F
            vertices[5].AddNext(vertices[6]);//F-G
            #endregion


            graph = new BellmanGraph();
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
            formValueChanged = false;
            step = BellmanTraverseStep.Init;

            graph.ResetAllVertex();
            variables.ResetAll();
            OnReset();
        }

        //在每次遍历之间重置
        public void ResetInStep()
        {
            formValueChanged = false;
            currentVertex = null;
            step = BellmanTraverseStep.Init;
            graph.ResetAllVertex();
            variables.ResetPage();
            OnReset();
        }

        /// <summary>
        /// 是否遍历结束
        /// </summary>
        /// <returns></returns>
        public bool IsTraverseEnd()
        {
            return step == BellmanTraverseStep.End;
        }

        #region 遍历流程控制

        /// <summary>
        /// 执行步骤
        /// </summary>
        public void DoStep()
        {
            switch (step)
            {
                case BellmanTraverseStep.Init:
                    Init();
                    break;
                case BellmanTraverseStep.SetCurrent:
                    SetCurrent();
                    break;
                case BellmanTraverseStep.ValidateCurrent:
                    ValidateCurrent();
                    break;
                case BellmanTraverseStep.ValidateInput:
                    ValidateInput();
                    break;
            }
        }

        /// <summary>
        /// 遍历初始化
        /// </summary>
        private void Init()
        {
            BellmanGraphVertex firstVertex = graph.GetVertex("A");

            //设置提示
            variables.SetStepHint("初始化，从顶点A开始遍历");

            //设置List
            AddListItem(firstVertex);

            //设置visited
            AddVisitedVertex(firstVertex);

            step = BellmanTraverseStep.SetCurrent;
        }

        /// <summary>
        /// 设置当前顶点
        /// </summary>
        private void SetCurrent()
        {
            //设置当前顶点状态为已访问状态
            if (currentVertex != null)
            {
                currentVertex.SetState(BellmanGraphVertex.VertexState.Visited);
            }

            //列表中还有值，则继续设置
            if (!IsListEmpty())
            {
                //修改提示，由子类决定
                SetSelectVertexHint();

                //跳转至下一个顶点
                currentVertex = GetNextVertexInList();

                //设置顶点为当前访问状态
                currentVertex.SetState(BellmanGraphVertex.VertexState.Current);

                //修改当前点text
                variables.SetCurrentVertexText(currentVertex.name);

                step = BellmanTraverseStep.ValidateCurrent;
            }
            //列表空了则结束
            else
            {
                if (goThroughCount >= 6)
                {
                    //提示
                    variables.SetStepHint("List为空，算法结束");

                    //修改next
                    variables.SetCurrentVertexText("");

                    //修改执行按键的文字
                    variables.SetRunButtonText("结束");

                    step = BellmanTraverseStep.End;
                }
                else
                {
                    //如果本次遍历表格中有值变化则进行下一次遍历，如果没有值变化可以提前结束遍历
                    if (formValueChanged)
                    {
                        //每次遍历完成后重置
                        ResetInStep();
                        //提示
                        variables.SetStepHint(string.Format("第{0:G}次遍历结束，点击开始进行第{1:G}次遍历", goThroughCount, goThroughCount + 1));

                        //修改执行按键的文字
                        variables.SetRunButtonText("开始");
                    }
                    else
                    {
                        PanelActivator.MessageBox("本次遍历没有路径值更新，可提前结束遍历！", () =>
                    {
                        //点击确认后操作
                        variables.SetStepHint("算法结束");

                        //修改next
                        variables.SetCurrentVertexText("");

                        //修改执行按键的文字
                        variables.SetRunButtonText("结束");

                        step = BellmanTraverseStep.End;

                    }, () =>
                     {
                         //点击取消后继续下一次遍历
                         //每次遍历完成后重置
                         ResetInStep();
                         //提示
                         variables.SetStepHint(string.Format("第{0:G}次遍历结束，点击开始进行第{1:G}次遍历", goThroughCount, goThroughCount + 1));

                         //修改执行按键的文字
                         variables.SetRunButtonText("开始");

                     });
                    }

                }
                goThroughCount++;
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
                variables.SetStepHint(string.Format("顶点{0:G}有可访问的邻接点，请输入下一个顶点并建保护站", currentVertex.name));
                variables.ToggleInputInteractable(true);
                step = BellmanTraverseStep.ValidateInput;
            }
            //如果没有，则删除list最后节点并设置当前点
            else
            {
                variables.SetStepHint(string.Format("顶点{0:G}没有可访问的邻接点，从List中删除", currentVertex.name));
                RemoveListItem();
                step = BellmanTraverseStep.SetCurrent;
            }
        }
        /// <summary>
        /// 验证输入有效性
        /// </summary>
        private void ValidateInput()
        {
            //用户输入的顶点名称
            string inputVertexName = variables.GetVertexInputValue();
            //用户输入的顶点权值
            string inputVertexDis = variables.GetDisInputValue();

            bool wrongResult = false;

            if (inputVertexName.Trim() == "")
            {
                variables.SetVertexWrongHint("请输入下个顶点");
                return;
            }

            if (inputVertexDis.Trim() == "")
            {
                variables.SetDisWrongHint("请输入到下个顶点的权值");
                return;
            }

            // 如果以下两处有错误直接拦截返回，不往下走
            if (!Regex.IsMatch(inputVertexName, "[B-G]"))
            {
                variables.SetVertexWrongHint("输入无效，请输入大写字母B-G表示下一个遍历的顶点");
                OnVertexWrongCount();

                int wrong111 = Statistics.GetWrongCount(SysDefine.Statistics.Bellman_Vertex_Input);
                Debug.Log(wrong111);
                
                return;
            }

            //基本信息已验证通过，验证其他方面是否正确
            BellmanGraphVertex nextVertex = graph.GetVertex(inputVertexName);

            //如果该顶点是当前顶点
            if (nextVertex.state == BellmanGraphVertex.VertexState.Current)
            {
                variables.SetVertexWrongHint(string.Format("顶点{0:G}是当前遍历到的顶点", nextVertex.name));
                wrongResult = true;
            }
            //如果不是当前顶点的邻接点
            else if (!currentVertex.IsNext(nextVertex))
            {
                variables.SetVertexWrongHint(string.Format("顶点{0:G}不是顶点{1:G}的邻接点", nextVertex.name, currentVertex.name));
                wrongResult = true;
            }
            else if (!disAnswerMap[currentVertex.name + inputVertexName].Equals(inputVertexDis))
            {
                variables.SetDisWrongHint("输入无效，请输入正确的顶点权值");
                OnDisWrongCount();
            }
            else
            {
                //设置提示
                variables.SetStepHint(string.Format("访问顶点{0:G}", nextVertex.name));

                //清空输入框
                variables.ClearInput();

                //禁用输入框
                variables.ToggleInputInteractable(false);

                //修改list，必须放到setstate之前，如果节点未被访问过才加入栈中
                AddListItem(nextVertex);

                //修改颜色
                nextVertex.SetState(BellmanGraphVertex.VertexState.Visited);

                //设置父节点
                nextVertex.SetFather(currentVertex);


                //由子类决定下一步
                OnValidateInputComplete(currentVertex.name, inputVertexName, inputVertexDis, variables);
            }

            //记录错误数
            if (wrongResult)
            {
                OnVertexWrongCount();
            }
        }

        #endregion

        protected abstract bool IsListEmpty();//遍历列表是否为空
        protected abstract void OnReset();//重置
        protected abstract BellmanGraphVertex GetNextVertexInList();//获取遍历列表中的下一个节点
        protected abstract void SetSelectVertexHint();//设置选择节点提示
        protected abstract void OnValidateInputComplete(string currentName, string nextName, string nextValue, BellmanTraverseVariables variables);//验证输入
        protected abstract void AddListItem(BellmanGraphVertex vert);//添加到List
        protected abstract void RemoveListItem();//删除List中的元素
        protected abstract void OnVertexWrongCount();//记录顶点错误数
        protected abstract void OnDisWrongCount();//记录权值错误数

        /// <summary>
        /// 添加访问点
        /// </summary>
        /// <param name="vert"></param>
        protected void AddVisitedVertex(BellmanGraphVertex vert)
        {
            visited.Add(vert);
        }

    }
}