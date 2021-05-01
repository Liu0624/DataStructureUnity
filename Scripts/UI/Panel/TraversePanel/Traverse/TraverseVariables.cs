using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using DG.Tweening;
using System;
using WJF_CodeLibrary.CommonUtility;

namespace WJF
{
	public class TraverseVariables : MonoBehaviour
	{
        public Color clrNormalVertex;//普通状态的顶点颜色
        public Color clrCurrentVertex;//当前顶点的颜色
        public Color clrVisitedVertex;//已访问过的顶点颜色

        private Text textGraphLegend;//遍历方法的text
        private Text textStepHint;//步骤提示text
        private Text textCurrentVertex;//当前顶点text
        private Transform traverseListParent;//list父物体
        private InputField inputVertex;//下一个顶点输入框
        private Text textWrongHint;//错误提示text
        private Text textNext;//下一个序号的text
        private Transform visitedParent;//已访问列表父物体
        private Button btnRun;//执行
        private Button btnUndo;//撤销
        private Button btnReset;//重置
        private Button btnSwitch;//切换遍历方法

        private List<ArrayList> undoList;

        private const float tweenDuration = 0.5f;
        private readonly Color clrListItem = new Color(230 / 255f, 82 / 255f, 179 / 255f, 1f);

        public void Init()
        {
            textGraphLegend = gameObject.GetComponentInChildrenByName<Text>("Legend");
            textStepHint = transform.Find("StepHintParent").GetComponentInChildren<Text>();
            textCurrentVertex = gameObject.GetComponentInChildrenByName<Text>("Right/TextCurrentVertex");
            traverseListParent = transform.Find("Right/ListParent/List");
            inputVertex = GetComponentInChildren<InputField>();
            textWrongHint = gameObject.GetComponentInChildrenByName<Text>("InputWrongHint");
            textNext = gameObject.GetComponentInChildrenByName<Text>("Right/NextParent/NextText");
            visitedParent = transform.Find("Right/VisitedParent/Visited");
            btnRun = gameObject.GetComponentInChildrenByName<Button>("Right/BtnRun");
            btnUndo = gameObject.GetComponentInChildrenByName<Button>("Right/BtnUndo");
            btnReset = gameObject.GetComponentInChildrenByName<Button>("Right/BtnReset");
            btnSwitch = gameObject.GetComponentInChildrenByName<Button>("BtnSwitch");

            undoList = new List<ArrayList>();

            inputVertex.onValueChanged.AddListener(value =>
            {
                SetWrongHint("");
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
                onRun();
            });
        }

        /// <summary>
        /// 注册后退事件
        /// </summary>
        /// <param name="onUndo"></param>
        public void RegisterUndoAction(Action onUndo)
        {
            btnUndo.onClick.RemoveAllListeners();
            btnUndo.onClick.AddListener(() => onUndo());
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
        /// 注册切换事件
        /// </summary>
        /// <param name="onSwitch"></param>
        public void RegisterSwitchAction(Action onSwitch)
        {
            btnSwitch.onClick.RemoveAllListeners();
            btnSwitch.onClick.AddListener(() => onSwitch());
        }

        /// <summary>
        /// 等待动画
        /// </summary>
        public void WaitForTween()
        {
            //禁用所有按键
            btnRun.interactable = false;
            btnUndo.interactable = false;
            btnReset.interactable = false;
            btnSwitch.interactable = false;

            CallbackUtility.ExecuteDelay(tweenDuration, () =>
            {
                //打开所有按键
                btnRun.interactable = true;
                btnUndo.interactable = undoList.Count != 0;
                btnReset.interactable = true;
                btnSwitch.interactable = true;
            });
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
        /// 设置错误提示
        /// </summary>
        /// <param name="hint"></param>
        public void SetWrongHint(string hint)
        {
            textWrongHint.text = hint;

            textWrongHint.DOKill();
            
            if (hint != "")
            {
                textWrongHint.color = ColorCalculateUtility.GetFadeColor(textWrongHint.color, 0.4f);
                textWrongHint.DOFade(1f, 0.25f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.Linear);
            }

            textWrongHint.gameObject.GetBrother("Bubble").SetActive(hint != "");
        }

        /// <summary>
        /// 获取输入值
        /// </summary>
        /// <returns></returns>
        public string GetInputValue()
        {
            return inputVertex.text;
        }

        /// <summary>
        /// 开关输入
        /// </summary>
        /// <param name="state"></param>
        public void ToggleInputInteractable(bool state)
        {
            inputVertex.interactable = state;
        }

        /// <summary>
        /// 清空输入
        /// </summary>
        public void ClearInput()
        {
            inputVertex.text = "";
        }

        /// <summary>
        /// 设置Next显示
        /// </summary>
        /// <param name="value"></param>
        public void SetNext(string value)
        {
            textNext.text = value;
        }

        /// <summary>
        /// 设置访问列表显示
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetVisited(int index, string value)
        {
            Transform child = visitedParent.Find(index.ToString());

            child.GetComponentInChildren<Text>().text = value;

            Image img = child.GetComponent<Image>();
            float fade = value == "" ? 0f : 1f;
            img.color = ColorCalculateUtility.GetFadeColor(img.color, fade);
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
            SetStepHint("点击“开始”按键进行遍历");
            SetCurrentVertexText("");
            SetNext("");
            SetWrongHint("");
            inputVertex.text = "";
            inputVertex.interactable = false;

            for (int i = 0; i < traverseListParent.childCount; i++)
            {
                SetListItemDisplay(i, "");
            }

            for (int i = 0; i < visitedParent.childCount; i++)
            {
                SetVisited(i, "");
            }

            undoList.Clear();
            btnUndo.interactable = false;
        }

        /// <summary>
        /// 记录后退
        /// </summary>
        public void RecordUndo()
        {
            ArrayList arr = new ArrayList();

            //0 记录提示
            arr.Add(textStepHint.text);

            //1 记录当前顶点
            arr.Add(textCurrentVertex.text);

            //2 记录输入框交互状态
            arr.Add(inputVertex.interactable);

            //3 记录List中每个text
            List<string> listText = new List<string>();
            for (int i = 0; i < traverseListParent.childCount; i++)
            {
                Transform child = traverseListParent.GetChild(i);
                string text = child.GetComponentInChildren<Text>().text;
                listText.Add(text);
            }
            arr.Add(listText);

            //4 记录next
            arr.Add(textNext.text);

            //5 记录visited中每个text
            List<string> visitedText = new List<string>();
            for (int i = 0; i < visitedParent.childCount; i++)
            {
                Transform child = visitedParent.Find(i.ToString());
                string text = child.GetComponentInChildren<Text>().text;
                visitedText.Add(text);
            }
            arr.Add(visitedText);

            ////6 记录执行按键的文字
            //arr.Add(btnRun.GetComponentInChildren<Text>().text);

            undoList.Add(arr);
        }

        /// <summary>
        /// 后退
        /// </summary>
        public void Undo()
        {
            ArrayList arr = undoList[undoList.Count - 1];

            //恢复提示
            SetStepHint(arr[0] as string);

            //恢复当前顶点
            textCurrentVertex.text = arr[1] as string;

            //恢复输入框交互状态
            inputVertex.interactable = (bool)arr[2];

            //恢复list
            List<string> listText = arr[3] as List<string>;
            for (int i = 0; i < listText.Count; i++)
            {
                SetListItemDisplay(i, listText[i]);
            }

            //恢复next
            textNext.text = arr[4] as string;

            //恢复visited
            List<string> visitedText = arr[5] as List<string>;
            for (int i = 0; i < visitedText.Count; i++)
            {
                SetVisited(i, visitedText[i]);
            }

            ////恢复执行按键文字
            //SetRunButtonText(arr[6] as string);
            //根据步骤设置运行按键
            SetRunButtonText(undoList.Count == 1 ? "开始" : "执行");

            //直接清空输入错误提示
            SetWrongHint("");

            undoList.RemoveAt(undoList.Count - 1);

            btnUndo.interactable = undoList.Count > 0;
        }
    }
}