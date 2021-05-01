using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Text;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.CommonUtility;

namespace WJF
{
	public partial class VertexEdgePanel
	{
        private Transform matrixInputParent;
        private InputField[] inputs;

        private Dictionary<GraphType, Dictionary<string, StringBuilder>> matrixRecord;

        private const string inverseStr = "#";
        private const string reachableStr = "1";
        private const string unreachableStr = "0";

        /// <summary>
        /// 初始化矩阵部分
        /// </summary>
		private void InitMatrix()
        {
            matrixInputParent = matrixParent.Find("MatrixInputParent");

            //初始化字典
            matrixRecord = new Dictionary<GraphType, Dictionary<string, StringBuilder>>();
            foreach (GraphType type in Enum.GetValues(typeof(GraphType)))
            {
                matrixRecord.Add(type, new Dictionary<string, StringBuilder>());
            }

            //输入框注册事件，并将输入框名字作为字典中的值
            inputs = matrixInputParent.GetComponentsInChildren<InputField>();
            foreach (var input in inputs)
            {
                input.onValueChanged.AddListener(value => OnMatrixInputValueChanged(input, value));

                foreach (var key in matrixRecord.Keys)
                {
                    matrixRecord[key].Add(input.name, new StringBuilder());
                }
            }

            RegisterEvent("BtnResetMatrix", EventType.Click_Left, p => OnResetMatrixClicked());
        }

        /// <summary>
        /// 重置矩阵输入
        /// </summary>
        private void ResetMatrix()
        {
            foreach (var type in matrixRecord.Keys)
            {
                foreach (var input in matrixRecord[type].Keys) 
                {
                    matrixRecord[type][input].Clear();
                }
            }

            //重置错误提示
            foreach (var input in inputs)
            {
                Image img = input.gameObject.GetComponentInChildrenByName<Image>(wrongHintName);
                img.color = ColorCalculateUtility.GetFadeColor(img.color, 0f);
            }
        }

        /// <summary>
        /// 切换矩阵输入
        /// </summary>
        /// <param name="type"></param>
        private void SwitchMatrix(GraphType type)
        {
            foreach (var input in inputs)
            {
                print(input);
                //无向图隐藏并禁用相反方向的输入框
                if (input.name.Contains(inverseStr))
                {
                    bool isUndigraph = type == GraphType.Undigraph;

                    Color clr = input.image.color;
                    clr.a = isUndigraph ? 0f : 1f;
                    input.image.color = clr;

                    input.interactable = !isUndigraph;
                }
                
                input.text = matrixRecord[type][input.name].ToString();
            }
        }

        /// <summary>
        /// 矩阵输入事件
        /// </summary>
        /// <param name="input"></param>
        /// <param name="value"></param>
        private void OnMatrixInputValueChanged(InputField input, string value)
        {
            if (!string.IsNullOrEmpty(value) && int.Parse(value) > 1)
            {
                value = reachableStr;
                input.text = value;
                return;
            }

            //记录输入的值
            matrixRecord[graphType][input.name].Clear();
            matrixRecord[graphType][input.name].Append(value);

            //如果是无向图，则要在相反的元素中填入相同值
            if (!input.name.Contains(inverseStr) && graphType == GraphType.Undigraph)
            {
                InputField inverseInput = matrixInputParent.Find(inverseStr + input.name).GetComponent<InputField>();
                inverseInput.text = value;
            }

            //隐藏错误提示
            Image img = input.gameObject.GetComponentInChildrenByName<Image>(wrongHintName);
            img.color = ColorCalculateUtility.GetFadeColor(img.color, 0f);
        }

        /// <summary>
        /// 矩阵重置按键事件
        /// </summary>
        private void OnResetMatrixClicked()
        {
            string msg = string.Format("重置后将清空已输入的{0:G}邻接矩阵，是否继续？", GetGraphChineseName(graphType));
            PanelActivator.MessageBox(msg,
                () =>
                {
                    foreach (var input in inputs)
                    {
                        input.text = "";
                    }
                }, () => { });
        }

        /// <summary>
        /// 验证矩阵输入
        /// </summary>
        /// <returns></returns>
        private List<Transform> ValidateMatrix()
        {
            List<Transform> result = new List<Transform>();

            foreach (var input in inputs)
            {
                string edgeName = input.name;
                bool isInverse = edgeName.Contains(inverseStr);

                //无向图无需检查翻转的边
                if (isInverse && graphType == GraphType.Undigraph)
                    continue;

                //如果边存在则判断是否填写1；如果边不存在则判断是否填写0
                bool validation = IsEdgeExist(graphType, edgeName) ? input.text == reachableStr : input.text == unreachableStr;
                if (!validation)
                    result.Add(input.transform);
            }

            //记录错误数
            if (result.Count > 0)
            {
                string title = "";
                switch (graphType)
                {
                    case GraphType.Digraph:
                        title = SysDefine.Statistics.MatrixInDigraph;
                        break;
                    case GraphType.Undigraph:
                        title = SysDefine.Statistics.MatrixInUndigraph;
                        break;
                }
                Statistics.AddWrong(title);
            }

            return result;
        }

        /// <summary>
        /// 给出矩阵答案
        /// </summary>
        private void FillMatrixAnswer()
        {
            foreach (var input in inputs)
            {
                string edgeName = input.name;
                input.text = IsEdgeExist(graphType, edgeName) ? reachableStr : unreachableStr;
            }
        }
	}
}