using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Text;
using DG.Tweening;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.CommonUtility;

namespace WJF
{
    public partial class DjPanel
    {

        private InputField[] firstInputs, secondInputs, threeInputs, fourInputs;
        private Text textS1, textS2, textS3, textS4;
        private VerticalLayoutGroup firstTime, secondTime, threeTime, fourTime;
        private Text tip21, tip51, tip22, tip32, tip42, tip23, tip33, tip34;

        private Button btnSubmit, btnDigraphReset;

        private List<string> firstTimeList = new List<string>() { "10", "m", "m", "5" };
        private List<string> secondTimeList = new List<string>() { "8", "14", "7", "-1" };
        private List<string> threeTimeList = new List<string>() { "8", "13", "-1", "-1" };
        private List<string> fourTimeList = new List<string>() { "-1", "9", "-1", "-1" };

        public int setpNumberDigraph = 1;

        private readonly string alreadyFind = "已到达";

        private int[] digraphWrongCountList = { 0, 0, 0, 0 };
    
        private Coroutine oneToFive, oneToFour, oneToTwo, oneToThree, click5, click2, click3, click4;
       
        /// 初始化矩阵部分
        private void InitDigraphInput()
        {
            textS1 = gameObject.GetComponentInChildrenByName<Text>("s1");
            textS2 = gameObject.GetComponentInChildrenByName<Text>("s2");
            textS3 = gameObject.GetComponentInChildrenByName<Text>("s3");
            textS4 = gameObject.GetComponentInChildrenByName<Text>("s4");

            tip21 = gameObject.GetComponentInChildrenByName<Text>("tip21");
            tip51 = gameObject.GetComponentInChildrenByName<Text>("tip51");
            tip22 = gameObject.GetComponentInChildrenByName<Text>("tip22");
            tip32 = gameObject.GetComponentInChildrenByName<Text>("tip32");
            tip42 = gameObject.GetComponentInChildrenByName<Text>("tip42");
            tip23 = gameObject.GetComponentInChildrenByName<Text>("tip23");
            tip33 = gameObject.GetComponentInChildrenByName<Text>("tip33");
            tip34 = gameObject.GetComponentInChildrenByName<Text>("tip34");

            btnSubmit = gameObject.GetComponentInChildrenByName<Button>("BtnSubmitInput");
            btnDigraphReset = gameObject.GetComponentInChildrenByName<Button>("BtnResetInput");

            firstTime = gameObject.GetComponentInChildrenByName<VerticalLayoutGroup>("FirstTime");
            secondTime = gameObject.GetComponentInChildrenByName<VerticalLayoutGroup>("SecondTime");
            threeTime = gameObject.GetComponentInChildrenByName<VerticalLayoutGroup>("ThreeTime");
            fourTime = gameObject.GetComponentInChildrenByName<VerticalLayoutGroup>("FourTime");

            firstInputs = firstTime.GetComponentsInChildren<InputField>();
            secondInputs = secondTime.GetComponentsInChildren<InputField>();
            threeInputs = threeTime.GetComponentsInChildren<InputField>();
            fourInputs = fourTime.GetComponentsInChildren<InputField>();

            ResetDigraphPage();
            btnSubmit.onClick.AddListener(() =>
            {
                OnSubmitInputClicked();
            });

            btnDigraphReset.onClick.AddListener(() =>
            {
                OnResetMatrixClicked();
            });
        }

        /// <summary>
        /// 重置有向图页面
        /// </summary>
        private void ResetDigraphPage()
        {

            if (digraphWorkCoroutine != null)
            {
                StopCoroutine(digraphWorkCoroutine);
            }

            InputField[] inputFields = digraphInputParent.GetComponentsInChildren<InputField>();
            ////重置所有输入框
            foreach (var input in inputFields)
            {
                input.text = "";
                Image img = input.gameObject.GetComponentInChildrenByName<Image>(wrongHintName);
                img.color = ColorCalculateUtility.GetFadeColor(img.color, 0f);
            }

            //隐藏路径集合以及其余步骤面板
            textS1.enabled = false;
            textS2.enabled = false;
            textS3.enabled = false;
            textS4.enabled = false;

            secondTime.gameObject.SetActive(false);
            threeTime.gameObject.SetActive(false);
            fourTime.gameObject.SetActive(false);

            tip21.gameObject.SetActive(false);
            tip51.gameObject.SetActive(false);
            tip22.gameObject.SetActive(false);
            tip32.gameObject.SetActive(false);
            tip42.gameObject.SetActive(false);
            tip23.gameObject.SetActive(false);
            tip33.gameObject.SetActive(false);
            tip34.gameObject.SetActive(false);

            for (int i = 0; i < 4; i++)
                digraphWrongCountList[i] = 0;
            //有向图重置
            ResetDigraphCircleAndLine();
            setpNumberDigraph = 1;

            //恢复按钮点击状态
            enableSubmitBtnState(true);
        }


        /// <summary>
        /// 重置按钮点击事件
        /// </summary>
        private void OnResetMatrixClicked()
        {
            PanelActivator.MessageBox("重置后将清空已输入的有向图的最短路径查找过程，是否继续？",
                () =>
                {
                    ResetDigraphPage();
                }, () => { });
        }

        /// <summary>
        /// 运行按钮点击事件
        /// </summary>
        private void OnSubmitInputClicked()
        {
            switch (setpNumberDigraph)
            {
                case 1:
                    OnSubmit1Clicked();
                    break;
                case 2:
                    OnSubmit2Clicked();
                    break;
                case 3:
                    OnSubmit3Clicked();
                    break;
                case 4:
                    OnSubmit4Clicked();
                    break;
                case 5:
                    PanelActivator.MessageBox("有向图最短路径已学习完成，请返回主菜单！", () =>
                    {
                        //Statistics.setComplete(SysDefine.Statistics.Bellman_Vertex_Input);
                        //Statistics.setComplete(SysDefine.Statistics.Bellman_Dis_Input);

                        SetCamerasNotEnable();
                        Statistics.setComplete(SysDefine.Statistics.DisInDigraph);
                        ShowPanel<MainPanel>();
                    }, () => { });
                    break;
                default:
                    return;
            }
        }

        //调整提交按钮的点击状态
        public void enableSubmitBtnState(bool state)
        {
            btnSubmit.interactable = state;
            btnDigraphReset.interactable = state;
        }

        private void OnSubmit1Clicked()
        {
            if (isInputComplete(firstInputs, 1) && isInputCorrect(firstInputs, firstTimeList, 1))
            {
                stopAllWork();
                enableSubmitBtnState(false);
                textS1.enabled = true;
                tip21.gameObject.SetActive(true);
                tip51.gameObject.SetActive(true);
                hightLightInput("51", firstInputs);

                setpNumberDigraph = 2;
                oneToFive = StartCoroutine(startFirstTime());
            }
        }

        private void OnSubmit2Clicked()
        {

            if (isInputComplete(secondInputs, 2) && isInputCorrect(secondInputs, secondTimeList, 2))
            {
                stopAllWork();
                enableSubmitBtnState(false);
                textS2.enabled = true;
                tip22.gameObject.SetActive(true);
                tip32.gameObject.SetActive(true);
                tip42.gameObject.SetActive(true);
                hightLightInput("42", secondInputs);

                setpNumberDigraph = 3;
                oneToFour = StartCoroutine(startSecondTime());
            }

        }

        private void OnSubmit3Clicked()
        {
            if (isInputComplete(threeInputs, 3) && isInputCorrect(threeInputs, threeTimeList, 3))
            {
                stopAllWork();
                textS3.enabled = true;
                enableSubmitBtnState(false);
                tip23.gameObject.SetActive(true);
                tip33.gameObject.SetActive(true);
                hightLightInput("23", threeInputs);

                setpNumberDigraph = 4;
                oneToTwo = StartCoroutine(startThreeTime());
            }
        }

        private void OnSubmit4Clicked()
        {
            if (isInputComplete(fourInputs, 4) && isInputCorrect(fourInputs, fourTimeList, 4))
            {
                stopAllWork();
                enableSubmitBtnState(false);
                textS4.enabled = true;
                tip34.gameObject.SetActive(true);
                hightLightInput("34", fourInputs);
                setpNumberDigraph = 5;
                oneToThree = StartCoroutine(startFourTime());
            }
        }

        private void stopAllWork()
        {
            if (oneToThree != null)
                StopCoroutine(oneToThree);
            if (oneToTwo != null)
                StopCoroutine(oneToTwo);
            if (oneToFour != null)
                StopCoroutine(oneToFour);
            if (oneToFive != null)
                StopCoroutine(oneToFive);
        }

        private void hightLightInput(string inputName, InputField[] inputFields)
        {
            foreach (var input in inputFields)
            {
                if (input.name.Equals(inputName))
                {
                    Image img = input.transform.Find(wrongHintName).GetComponent<Image>();
                    img.color = Color.yellow;
                    img.DOFade(0.5f, 0.25f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.Linear);
                }
            }
        }


        //检查是否全部输入完成
        private bool isInputComplete(InputField[] inputs, int stepNum, bool isDigraph = true, String tipText = "V1到顶点V")
        {
            //如果有未填写的，提示用户
            foreach (var input in inputs)
            {
                if (input.enabled && input.text == "")
                {
                    if (isDigraph)
                    {
                         //记录错误数
                        Statistics.AddWrong(SysDefine.Statistics.DisInDigraph);
                        digraphWrongCountList[stepNum - 1]++;
                        if (digraphWrongCountList[stepNum - 1] >= 3)
                        {
                            //有向图给出答案
                            FillAnswer(stepNum);
                            PanelActivator.MessageBox("填写错误超过3次，已给出正确答案。", () => { });
                        }
                        else
                        {
                            PanelActivator.MessageBox(tipText + input.name.Substring(0, 1) + "路径值不能为空，如无路径可达请用m(max)表示！", () =>
                            {
                                Image img = input.transform.Find(wrongHintName).GetComponent<Image>();
                                img.color = Color.yellow;
                                img.DOFade(0.0f, 0.25f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.Linear);
                            });
                        }
                    }
                    else
                    {
                        //记录错误数
                        Statistics.AddWrong(SysDefine.Statistics.DisInUndigraph);
                        unDigraphWrongCountList[stepNum - 1]++;
                        if (unDigraphWrongCountList[stepNum - 1] >= 3)
                        {
                            UndigraphFillAnswer(stepNum);
                            PanelActivator.MessageBox("填写错误超过3次，已给出正确答案。", () => { });
                        }
                        else
                        {
                            PanelActivator.MessageBox(tipText + input.name.Substring(0, 1) + "路径值不能为空，如无路径可达请用m(max)表示！", () =>
                            {
                                Image img = input.transform.Find(wrongHintName).GetComponent<Image>();
                                img.color = Color.yellow;
                                img.DOFade(0.0f, 0.25f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.Linear);

                            });
                        }
                    }
                    return false;
                }
            }
            return true;
        }

        //检查是否全部输入正确
        private bool isInputCorrect(InputField[] inputs, List<string> values, int stepNum, bool isDigraph = true, String tipText = "V1到顶点V")
        {
            //提示用户不是最短路径
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i].enabled && !inputs[i].text.ToString().Equals(values[i]))
                {
                    if (isDigraph)
                    {
                        //记录错误数
                        Statistics.AddWrong(SysDefine.Statistics.DisInDigraph);
                        digraphWrongCountList[stepNum - 1]++;
                        if (digraphWrongCountList[stepNum - 1] >= 3)
                        {
                            //有向图给出答案
                            FillAnswer(stepNum);
                            PanelActivator.MessageBox("填写错误超过3次，已给出正确答案。", () => { });
                        }
                        else
                        {
                            PanelActivator.MessageBox(tipText + inputs[i].name.Substring(0, 1) + "路径值不是最短路径，请检查其他路径是否可达！", () =>
                            {
                                Image img = inputs[i].transform.Find(wrongHintName).GetComponent<Image>();
                                img.color = Color.yellow;
                                img.DOFade(0.0f, 0.25f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.Linear);
                            });
                        }
                    }
                    else
                    {
                        //记录错误数
                        Statistics.AddWrong(SysDefine.Statistics.DisInUndigraph);
                        unDigraphWrongCountList[stepNum - 1]++;
                        if (unDigraphWrongCountList[stepNum - 1] >= 3)
                        {
                            UndigraphFillAnswer(stepNum);
                            PanelActivator.MessageBox("填写错误超过3次，已给出正确答案。", () => { });
                        }
                        else
                        {
                            PanelActivator.MessageBox(tipText + inputs[i].name.Substring(0, 1) + "路径值不是最短路径，请检查其他路径是否可达！", () =>
                            {
                                Image img = inputs[i].transform.Find(wrongHintName).GetComponent<Image>();
                                img.color = Color.yellow;
                                img.DOFade(0.0f, 0.25f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.Linear);

                            });
                        }
                    }

                    return false;
                }

            }

            return true;
        }

        private void FillAnswer(int stepNum)
        {
            switch (stepNum)
            {
                case 1:
                    for (int i = 0; i < 4; i++)
                    {
                        firstInputs[i].text = firstTimeList[i];
                    }
                    break;
                case 2:
                    for (int i = 0; i < 3; i++)
                    {
                        secondInputs[i].text = secondTimeList[i];
                    }
                    break;
                case 3:
                    for (int i = 0; i < 2; i++)
                    {
                        threeInputs[i].text = threeTimeList[i];
                    }
                    break;
                case 4:
                    fourInputs[1].text = fourTimeList[1];

                    break;
            }
        }
    }
}