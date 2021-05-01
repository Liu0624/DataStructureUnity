using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.CommonUtility.UI;
using System.Text;
using System;
using System.Linq;
using WJF_CodeLibrary.CommonUtility;

namespace WJF
{
	public partial class VertexEdgePanel
	{
        private InputField inputVertex;
        private Transform edgeInputParent;

        private List<GameObject> edgeItemPool;
        private Dictionary<GraphType, StringBuilder> vertexRecord;
        private Dictionary<GraphType, List<StringBuilder>> edgeRecord;

        /// <summary>
        /// 初始化集合
        /// </summary>
        private void InitCollection()
        {
            inputVertex = collectionParent.gameObject.GetComponentInChildrenByName<InputField>("VertexInput");
            edgeInputParent = collectionParent.Find("EdgeInputParent");
            //btnAddEdge = edgeInputParent.GetComponentInChildren<Button>();

            edgeItemPool = new List<GameObject>();
            foreach (Transform child in edgeInputParent)
            {
                if (child.name != "Add")
                {
                    edgeItemPool.Add(child.gameObject);

                    //注册输入事件
                    InputField[] inputs = child.GetComponentsInChildren<InputField>();
                    foreach (var input in inputs)
                    {
                        input.onValueChanged.AddListener(value => OnEdgeInputValueChanged(input, value));
                    }
                }   
            }

            vertexRecord = new Dictionary<GraphType, StringBuilder>();
            edgeRecord = new Dictionary<GraphType, List<StringBuilder>>();

            //初始化两个集合
            foreach (GraphType type in Enum.GetValues(typeof(GraphType)))
            {
                vertexRecord.Add(type, new StringBuilder());

                int edgesCount = GetEdges(type).Length;
                edgeRecord.Add(type, new List<StringBuilder>());
                for (int i = 0; i < edgesCount; i++)
                {
                    edgeRecord[type].Add(new StringBuilder(SysDefine.NoData + SysDefine.NoData));
                }
            }

            inputVertex.onValueChanged.AddListener(OnVertexInputValueChanged);
            //btnAddEdge.onClick.AddListener(OnAddEdgeClicked);

            RegisterEvent("BtnResetCollection", EventType.Click_Left, p => OnResetCollectionClicked());
        }

        /// <summary>
        /// 重置集合
        /// </summary>
        private void ResetCollection()
        {
            //重置顶点的输入
            inputVertex.text = "";

            //重置顶点的错误提示
            Image imgInputVertexWrongHint = inputVertex.transform.Find(wrongHintName).GetComponent<Image>();
            imgInputVertexWrongHint.color = ColorCalculateUtility.GetFadeColor(imgInputVertexWrongHint.color, 0f);

            //重置边的输入和错误提示
            ClearEdgeInput();
            edgeItemPool.ForEach(go =>
            {
                Image img = go.GetComponentInChildrenByName<Image>(wrongHintName);
                img.color = ColorCalculateUtility.GetFadeColor(img.color, 0f);
            });

            //重置记录
            foreach (GraphType type in Enum.GetValues(typeof(GraphType)))
            {
                vertexRecord[type].Clear();

                for (int i = 0; i < edgeRecord[type].Count; i++)
                {
                    edgeRecord[type][i].Clear();
                    edgeRecord[type][i].Append(SysDefine.NoData + SysDefine.NoData);
                }
            }
        }

        /// <summary>
        /// 切换填写的内容
        /// </summary>
        private void SwitchCollection(GraphType type)
        {
            //切换顶点集合输入
            inputVertex.text = vertexRecord[type].ToString();

            //切换边的集合输入
            //foreach (var item in edgeItemPool)
            //    item.SetActive(false);

            //先清空边的输入
            ClearEdgeInput();

            List<StringBuilder> collection = edgeRecord[type];
            for (int i = 0; i < collection.Count; i++)
            {
                GameObject item = edgeItemPool[i];

                //修改括号
                Text textBracket = item.GetComponentInChildrenByName<Text>("Bracket");
                textBracket.text = graphType == GraphType.Digraph ? "<　　,　　>" : "(　　,　　)";

                //填充数据
                InputField[] inputs = item.GetComponentsInChildren<InputField>();
                foreach (var input in inputs)
                {
                    int index = int.Parse(input.name[1].ToString()) - 1;
                    string value = collection[i][index].ToString();
                    input.text = value == SysDefine.NoData ? "" : value;
                }
            }

        }


        /// <summary>
        /// 顶点输入事件
        /// </summary>
        /// <param name="value"></param>
        private void OnVertexInputValueChanged(string value)
        {                        
            vertexRecord[graphType].Clear();
            vertexRecord[graphType].Append(value);

            //隐藏错误提示
            Image img = inputVertex.gameObject.GetComponentInChildrenByName<Image>(wrongHintName);
            img.color = ColorCalculateUtility.GetFadeColor(img.color, 0f);
        }

        /// <summary>
        /// 边输入事件
        /// </summary>
        /// <param name="input"></param>
        /// <param name="value"></param>
        private void OnEdgeInputValueChanged(InputField input, string value)
        {
            //隐藏错误提示
            Image img = input.gameObject.GetBrother(wrongHintName).GetComponent<Image>();
            img.color = ColorCalculateUtility.GetFadeColor(img.color, 0f);

            if (!input.isFocused)
                return;

            if (value == "")
                value = SysDefine.NoData;

            //记录输入
            int parentSiblingIndex = input.transform.parent.GetSiblingIndex();
            int index = int.Parse(input.name[1].ToString()) - 1;
            string replacedEdge = edgeRecord[graphType][parentSiblingIndex][index].ToString();
            edgeRecord[graphType][parentSiblingIndex].Replace(replacedEdge, value, index, 1);
        }

        /// <summary>
        /// 重置顶点和边
        /// </summary>
        private void OnResetCollectionClicked()
        {
            string msg = string.Format("重置后将清空已输入的{0:G}顶点与边的集合，是否继续？", GetGraphChineseName(graphType));
            PanelActivator.MessageBox(msg,
                () =>
                {
                    //清空顶点集合
                    inputVertex.text = "";

                    //清空边的集合
                    ClearEdgeInput();
                    foreach (var key in edgeRecord.Keys)
                    {
                        edgeRecord[key].ForEach(p =>
                        {
                            p.Clear();
                            p.Append(SysDefine.NoData + SysDefine.NoData);
                        });
                    }

                    //显示添加按键
                    //btnAddEdge.gameObject.SetActive(true);
                }, () => { });
        }

        /// <summary>
        /// 清空边的输入
        /// </summary>
        private void ClearEdgeInput()
        {
            edgeItemPool.ForEach(go =>
            {
                InputField[] inputs = go.GetComponentsInChildren<InputField>();
                foreach (var input in inputs)
                {
                    input.text = "";
                }
            });
        }

        /// <summary>
        /// 验证顶点与边的输入
        /// </summary>
        /// <returns>返回错误项</returns>
        private List<Transform> ValidateCollection()
        {
            List<Transform> result = new List<Transform>();

            //判断顶点
            bool vertexResult = true;

            //提取输入的字母
            string inputStr = inputVertex.text.ToUpper();
            List<string> inputLetter = new List<string>();
            for (int i = 0; i < inputStr.Length; i++)
            {
                char c = inputStr[i];
                if (char.IsLetter(c))
                    inputLetter.Add(c.ToString());
            }

            //判断输入的字母数量是否与顶点数相等
            if (inputLetter.Count != CurrentVertices.Length)
                vertexResult = false;

            //如果长度符合，再继续检查，否则不检查
            if (vertexResult)
            {
                List<string> lstVertices = CurrentVertices.ToList();
                foreach (var v in lstVertices)
                {
                    if (!inputLetter.Contains(v))
                    {
                        vertexResult = false;
                        break;
                    }
                }
            }

            //如果有错误，则添加至结果序列
            if (!vertexResult)
                result.Add(inputVertex.transform);

            //判断边的集合
            List<string> lstEdges = CurrentEdges.ToList();

            edgeItemPool.ForEach(item =>
            {
                string e1 = item.GetComponentInChildrenByName<InputField>("E1").text;
                string e2 = item.GetComponentInChildrenByName<InputField>("E2").text;
                string inputEdge = (e1 + e2).ToUpper();

                //如果存在则删除已检查的边
                if (lstEdges.Contains(inputEdge))
                {
                    lstEdges.Remove(inputEdge);
                }
                else
                {
                    if (graphType == GraphType.Undigraph)
                    {
                        string inverseEdge = (e2 + e1).ToUpper();
                        if (lstEdges.Contains(inverseEdge))
                        {
                            lstEdges.Remove(inverseEdge);
                        }
                        else
                        {
                            result.Add(item.transform);
                        }
                    }
                    else
                    {
                        result.Add(item.transform);
                    }
                }
            });

            //记录错误数
            if (result.Count > 0)
            {
                string title = "";
                switch (graphType)
                {
                    case GraphType.Digraph:
                        title = SysDefine.Statistics.CollectionInDigraph;
                        break;
                    case GraphType.Undigraph:
                        title = SysDefine.Statistics.CollectionInUndigraph;
                        break;
                }
                Statistics.AddWrong(title);
            }   

            return result;
        }

        /// <summary>
        /// 给出答案
        /// </summary>
        private void FillCollectionAnswer()
        {
            //填写顶点
            string strVertices = "";
            string[] currentVertices = CurrentVertices;

            for (int i = 0; i < currentVertices.Length; i++)
            {
                strVertices += currentVertices[i];
            }

            inputVertex.text = strVertices;

            //填写顶点
            string[] currentEdges = CurrentEdges;

            for (int i = 0; i < currentEdges.Length; i++)
            {
                string e1 = currentEdges[i][0].ToString();
                string e2 = currentEdges[i][1].ToString();

                GameObject item = edgeItemPool[i];
                item.GetComponentInChildrenByName<InputField>("E1").text = e1;
                item.GetComponentInChildrenByName<InputField>("E2").text = e2;
            }
        }
	}
}