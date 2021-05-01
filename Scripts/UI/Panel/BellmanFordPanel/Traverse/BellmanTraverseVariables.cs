using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using DG.Tweening;
using System;
using WJF_CodeLibrary.CommonUtility;
using System.Text.RegularExpressions;

namespace WJF
{
    public class BellmanTraverseVariables : MonoBehaviour
    {
        public Color clrNormalVertex;//普通状态的顶点颜色
        public Color clrCurrentVertex;//当前顶点的颜色
        public Color clrVisitedVertex;//已访问过的顶点颜色

        private Text textGraphLegend;//遍历方法的text
        private Text textStepHint;//步骤提示text
        private Text textCurrentVertex;//当前顶点text
        private Transform traverseListParent;//list父物体
        private InputField inputVertex;//下一个顶点输入框
        private InputField inputVertexDis;//下一个顶点距离权值输入框
        private Text textVertexWrongHint;//错误提示text
        private Text textDisWrongHint;//错误提示text
        private Text textFormChangeTip;//右侧表格更新提示text
        public readonly int formValueMax = 99999;

        private Button btnRun;//执行
        private Button btnReset;//重置
        private Transform formParent;
        private Text[] formTextList;

        private const float tweenDuration = 0.5f;
        private readonly Color clrListItem = new Color(230 / 255f, 82 / 255f, 179 / 255f, 1f);

        public void Init()
        {
            textGraphLegend = gameObject.GetComponentInChildrenByName<Text>("Legend");
            textStepHint = transform.Find("StepHintParent").GetComponentInChildren<Text>();
            textCurrentVertex = gameObject.GetComponentInChildrenByName<Text>("TextCurrentVertex");
            traverseListParent = transform.Find("ListParent/List");
            formParent = transform.Find("FormParent");


            inputVertex = gameObject.GetComponentInChildrenByName<InputField>("nextInputParent/InputVertex");
            inputVertexDis = gameObject.GetComponentInChildrenByName<InputField>("nextDisInputParent/InputVertex111");

            textVertexWrongHint = gameObject.GetComponentInChildrenByName<Text>("nextInputParent/InputWrongHint");
            textDisWrongHint = gameObject.GetComponentInChildrenByName<Text>("nextDisInputParent/InputWrongHint111");
            textFormChangeTip = gameObject.GetComponentInChildrenByName<Text>("FormValueTip");

            btnRun = gameObject.GetComponentInChildrenByName<Button>("BtnRun");
            btnReset = gameObject.GetComponentInChildrenByName<Button>("BtnReset");

            formTextList = formParent.GetComponentsInChildren<Text>();

            // 顶点输入框监听事件
            inputVertex.onValueChanged.AddListener(value =>
            {
                SetVertexWrongHint("");
            });

            // 权值输入框监听事件
            inputVertexDis.onValueChanged.AddListener(value =>
            {
                SetDisWrongHint("");
            });

        }

        /// <summary>
        /// 注册运行事件
        /// </summary>
        /// <param name="onRun"></param>
        public void RegisterRunAction(Action onRun)
        {
            btnRun.onClick.RemoveAllListeners();
            btnRun.onClick.AddListener(() =>
            {
                //置空表格更改提示文案
                SetFormChangeTip("");
                onRun();
            });
        }

        /// <summary>
        /// 注册重置事件
        /// </summary>
        /// <param name="onReset"></param>
        public void RegisterResetAction(Action onReset)
        {
            btnReset.onClick.RemoveAllListeners();
            btnReset.onClick.AddListener(() => onReset());
        }

        /// <summary>
        /// 等待动画
        /// </summary>
        public void WaitForTween()
        {
            //禁用所有按键
            btnRun.interactable = false;
            btnReset.interactable = false;

            CallbackUtility.ExecuteDelay(tweenDuration, () =>
            {
                //打开所有按键
                btnRun.interactable = true;
                btnReset.interactable = true;
            });
        }

        public void SetFormChangeTip(string tipText)
        {
            textFormChangeTip.text = tipText;
        }

        /// <summary>
        /// 高亮错误
        /// </summary>
        /// <param name="input"></param>
        private void HighlightWrongOn(Text input)
        {
            Image wrongHint = input.gameObject.GetComponentInChildrenByName<Image>("WrongHint");
            if (wrongHint != null)
            {
                FadeComponent(wrongHint);
                wrongHint.DOFade(0.5f, 0.25f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.Linear);
            }
        }

        /// <summary>
        /// 关闭高亮
        /// </summary>
        /// <param name="input"></param>
        private void HighlightWrongOff(Text input)
        {
            Image wrongHint = input.gameObject.GetComponentInChildrenByName<Image>("WrongHint");
            if (wrongHint != null)
            {
                wrongHint.DOKill();
                FadeComponent(wrongHint);
            }

        }

        /// <summary>
        /// 隐藏组件
        /// </summary>
        /// <param name="graphic"></param>
        private void FadeComponent(MaskableGraphic graphic)
        {
            Color c = graphic.color;
            c.a = 0f;
            graphic.color = c;
        }



        //设置某个顶点的父节点和距离值 F[v] D[v]
        public void SetVertexValueInForm(string vertexName, string fatherVertex, string disValue)
        {
            foreach (Text textValue in formTextList)
            {
                string name = textValue.name;
                if (name.Equals((vertexName + "Left")))
                {
                    textValue.text = fatherVertex;
                }
                if (name.Equals((vertexName + "Value")))
                {
                    textValue.text = disValue;
                }

                if (name.Equals(vertexName))
                {
                    HighlightWrongOn(textValue);
                }
                else if (!name.EndsWith("Left") && !name.EndsWith("Value"))
                {
                    HighlightWrongOff(textValue);
                }
            }
        }

        //在每次遍历结束时候重置表格颜色
        public void ResetFormChangeColor()
        {
            foreach (Text textValue in formTextList)
            {
                HighlightWrongOff(textValue);
            }
        }

        //获取右侧表格从初始点到某个顶点的最短路径值，D[v]
        public int GetVertexValueInForm(string vertexName)
        {
            foreach (Text textValue in formTextList)
            {
                if (textValue.name == (vertexName + "Value"))
                {
                    if (textValue.text != null)
                    {
                        if (Regex.IsMatch(textValue.text, "^[-\\+]?[\\d]*$"))
                        {
                            return int.Parse(textValue.text);
                        }
                        else
                        {
                            return formValueMax;
                        }
                    }
                }
            }
            return formValueMax;
        }

        //重置右侧表格
        public void ResetForm()
        {
            foreach (Text textValue in formTextList)
            {
                string name = textValue.name;
                if (name.EndsWith("Left"))
                {
                    if (textValue.name == "ALeft")
                    {
                        textValue.text = "A";
                    }
                    else
                    {
                        textValue.text = "--";
                    }
                }
                else if (name.EndsWith("Value"))
                {
                    if (textValue.name == "AValue")
                    {
                        textValue.text = "0";
                    }
                    else
                    {
                        textValue.text = "∞";
                    }
                }
            }
        }

        /// <summary>
        /// 设置遍历方法名
        /// </summary>
        /// <param name="value"></param>
        public void SetGraphLegend(string value)
        {
            textGraphLegend.text = value;
        }

        /// <summary>
        /// 设置步骤提示
        /// </summary>
        /// <param name="value"></param>
        public void SetStepHint(string value)
        {
            textStepHint.text = "";

            textStepHint.DOKill();
            textStepHint.DOText(value, tweenDuration).SetEase(Ease.Linear);

            WaitForTween();
        }

        /// <summary>
        /// 设置当前顶点文本
        /// </summary>
        /// <param name="value"></param>
        public void SetCurrentVertexText(string value)
        {
            textCurrentVertex.text = "当前顶点：" + value;
        }

        /// <summary>
        /// 设置List显示
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="tween"></param>
        public void SetListItemDisplay(int index, string value, bool tween = false)
        {
            Transform listItem = traverseListParent.GetChild(index);
            Image img = listItem.GetComponent<Image>();
            Text text = listItem.GetComponentInChildren<Text>();
            Transform lineL = listItem.Find("LineL");
            Transform lineR = listItem.Find("LineR");
            Image imgLineL = lineL == null ? null : lineL.GetComponent<Image>();
            Image imgLineR = lineR == null ? null : lineR.GetComponent<Image>();

            text.text = value;

            float targetFade = value == "" ? 0f : 1f;

            if (tween)
            {
                img.DOFade(targetFade, tweenDuration).SetEase(Ease.Linear);
                text.DOFade(targetFade, tweenDuration).SetEase(Ease.Linear);

                if (imgLineL != null)
                    imgLineL.DOFade(targetFade, tweenDuration).SetEase(Ease.Linear);
                if (imgLineR != null)
                    imgLineR.DOFade(targetFade, tweenDuration).SetEase(Ease.Linear);
            }
            else
            {
                img.color = ColorCalculateUtility.GetFadeColor(img.color, targetFade);
                text.color = ColorCalculateUtility.GetFadeColor(text.color, targetFade);

                if (imgLineL != null)
                    imgLineL.color = ColorCalculateUtility.GetFadeColor(imgLineL.color, targetFade);
                if (imgLineR != null)
                    imgLineR.color = ColorCalculateUtility.GetFadeColor(imgLineR.color, targetFade);
            }
        }

        /// <summary>
        /// 渐隐list项
        /// </summary>
        /// <param name="index"></param>
        public void FadeOutListItem(int index)
        {
            Transform listItem = traverseListParent.GetChild(index);
            Image img = listItem.GetComponent<Image>();
            Text text = listItem.GetComponentInChildren<Text>();
            Transform lineL = listItem.Find("LineL");
            Transform lineR = listItem.Find("LineR");
            Image imgLineL = lineL == null ? null : lineL.GetComponent<Image>();
            Image imgLineR = lineR == null ? null : lineR.GetComponent<Image>();

            img.DOFade(0, tweenDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                SetListItemDisplay(index, "");
            });

            text.DOFade(0, tweenDuration).SetEase(Ease.Linear);

            if (imgLineL != null)
                imgLineL.DOFade(0f, tweenDuration).SetEase(Ease.Linear);
            if (imgLineR != null)
                imgLineR.DOFade(0f, tweenDuration).SetEase(Ease.Linear);
        }

        /// <summary>
        /// 设置顶点错误提示
        /// </summary>
        /// <param name="hint"></param>
        public void SetVertexWrongHint(string hint)
        {
            textVertexWrongHint.text = hint;

            textVertexWrongHint.DOKill();

            if (hint != "")
            {
                textVertexWrongHint.color = ColorCalculateUtility.GetFadeColor(textVertexWrongHint.color, 0.4f);
                textVertexWrongHint.DOFade(1f, 0.25f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.Linear);
            }

            textVertexWrongHint.gameObject.GetBrother("Bubble").SetActive(hint != "");
        }

        /// <summary>
        /// 设置顶点权值错误提示
        /// </summary>
        /// <param name="hint"></param>
        public void SetDisWrongHint(string hint)
        {
            textDisWrongHint.text = hint;
            textDisWrongHint.DOKill();

            if (hint != "")
            {
                textDisWrongHint.color = ColorCalculateUtility.GetFadeColor(textDisWrongHint.color, 0.4f);
                textDisWrongHint.DOFade(1f, 0.25f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.Linear);
            }
            textDisWrongHint.gameObject.GetBrother("Bubble").SetActive(hint != "");
        }

        /// <summary>
        /// 获取顶点输入值
        /// </summary>
        /// <returns></returns>
        public string GetVertexInputValue()
        {
            return inputVertex.text;
        }

        /// <summary>
        /// 获取距离下一顶点权值的输入值
        /// </summary>
        /// <returns></returns>
        public string GetDisInputValue()
        {
            return inputVertexDis.text;
        }


        /// <summary>
        /// 开关输入,包括顶点输入和权值输入
        /// </summary>
        /// <param name="state"></param>
        public void ToggleInputInteractable(bool state)
        {
            inputVertex.interactable = state;
            inputVertexDis.interactable = state;
        }

        /// <summary>
        /// 清空输入
        /// </summary>
        public void ClearInput()
        {
            inputVertex.text = "";
            inputVertexDis.text = "";
        }

        /// <summary>
        /// 设置运行按键文本
        /// </summary>
        /// <param name="value"></param>
        public void SetRunButtonText(string value)
        {
            btnRun.GetComponentInChildren<Text>().text = value;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void ResetAll()
        {
            ResetForm();
            ResetPage();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void ResetPage()
        {
            SetStepHint("点击“开始”按键进行遍历");
            SetCurrentVertexText("");
            SetVertexWrongHint("");
            SetDisWrongHint("");
            SetFormChangeTip("");
            ResetFormChangeColor();

            inputVertex.text = "";
            inputVertex.interactable = false;
            inputVertexDis.text = "";
            inputVertexDis.interactable = false;

            for (int i = 0; i < traverseListParent.childCount; i++)
            {
                SetListItemDisplay(i, "");
            }
        }
    }
}