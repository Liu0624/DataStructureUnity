using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.CommonUtility.UI;
using WJF_CodeLibrary.CommonUtility.Sort;
using DG.Tweening;
using System.Text.RegularExpressions;
using WJF_CodeLibrary.CommonUtility;

namespace WJF
{
    public partial class FloydPanel
    {
        public Color clrToggleSelected;
        public Color clrToggleUnselected;
        public Color clrToggleTextSelected;
        public Color clrToggleTextUnselected;

        private Toggle tglStructure;
        private Toggle tglFunction;
        private Toggle tglRun;
        private ScrollRect scroll;
        private GameObject structureContent;
        private GameObject functionContent;
        private GameObject runContent1;
        private GameObject runContent2;
        private GameObject runContent3;
        private GameObject runContent4;
        private GameObject runContent5;

        private Transform struceContentView;

        private List<GameObject> runContentList = new List<GameObject>();

        private InputField[] programInputs;

        //邻接矩阵输入框
        private InputField[] matrixInputs;

        private InputField[] runInputs1, runInputs2, runInputs3, runInputs4;

        private List<InputField[]> runInputList = new List<InputField[]>();
        private Button btnRunExecute;

        private bool isProgramInputSubmit;
        private bool isProgramRunComplete;
        private int cntProgramInputWrong;

        private int cntMatrixInputWrong;


        private int cntRunInputWrong1, cntRunInputWrong2, cntRunInputWrong3, cntRunInputWrong4;

        private List<int> runInputWrongList = new List<int>() { 0, 0, 0, 0 };
        private List<InputField> wrongProgramInputs;

        private List<InputField> wrongmatrixInputs;
        private List<InputField> wrongRunInputs1, wrongRunInputs2, wrongRunInputs3, wrongRunInputs4;

        private int runStepNumber = 1;

        /// <summary>
        /// 程序填空答案
        /// </summary>
        private readonly string[] inputAnwser = new string[]
        {
            "max", "e[t1][t2]=t3;", "e[i][k]+e[k][j]", "e[i][j]=e[i][k]+e[k][j];"
        };

        /// <summary>
        /// 运行填空边的答案
        /// </summary>
        private readonly string[] edgeAnswer1 = new string[]
        {
            "9", "7", "11"
        };

        private readonly string[] edgeAnswer2 = new string[]
        {
            "5", "10"
        };

        private readonly string[] edgeAnswer3 = new string[]
        {
            "10", "4"
        };

        private readonly string[] edgeAnswer4 = new string[]
        {
            "9", "6", "8"
        };

        private readonly string[,] matrixInputAnswer = new string[,]
        {
            {"0", "2", "6","4"},
            {"m", "0", "3","m"},
            {"7", "m", "0","1"},
            {"5", "m", "12","0"}
        };

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitProgramWriting()
        {
            tglStructure = programWritingParent.gameObject.GetComponentInChildrenByName<Toggle>("ToggleStructure");
            tglFunction = programWritingParent.gameObject.GetComponentInChildrenByName<Toggle>("ToggleFunction");
            tglRun = programWritingParent.gameObject.GetComponentInChildrenByName<Toggle>("ToggleRun");
            scroll = programWritingParent.GetComponentInChildren<ScrollRect>();
            structureContent = scroll.content.Find("StructureContent").gameObject;
            functionContent = scroll.content.Find("FunctionContent").gameObject;

            struceContentView = programWritingParent.gameObject.GetComponentInChildrenByName<Transform>("struceContent");

            runContent1 = scroll.content.Find("RunContent1").gameObject;
            runContent2 = scroll.content.Find("RunContent2").gameObject;
            runContent3 = scroll.content.Find("RunContent3").gameObject;
            runContent4 = scroll.content.Find("RunContent4").gameObject;
            runContent5 = scroll.content.Find("RunContent5").gameObject;
            runContentList.Add(runContent1);
            runContentList.Add(runContent2);
            runContentList.Add(runContent3);
            runContentList.Add(runContent4);
            runContentList.Add(runContent5);

            showInputContent(runContent1);

            programInputs = functionContent.GetComponentsInChildren<InputField>();
            matrixInputs = matrixInputLayout.GetComponentsInChildren<InputField>();

            runInputs1 = runContent1.GetComponentsInChildren<InputField>();
            runInputs2 = runContent2.GetComponentsInChildren<InputField>();
            runInputs3 = runContent3.GetComponentsInChildren<InputField>();
            runInputs4 = runContent4.GetComponentsInChildren<InputField>();
            runInputList.Add(runInputs1);
            runInputList.Add(runInputs2);
            runInputList.Add(runInputs3);
            runInputList.Add(runInputs4);

            btnRunExecute = programWritingParent.gameObject.GetComponentInChildrenByName<Button>("BtnRunExecute");

            SortUtility.Bubble(programInputs, (x, y) => x.transform.GetSiblingIndex() > y.transform.GetSiblingIndex());
            SortUtility.Bubble(runInputs1, (x, y) => x.transform.GetSiblingIndex() > y.transform.GetSiblingIndex());

            wrongProgramInputs = new List<InputField>();
            wrongmatrixInputs = new List<InputField>();
            wrongRunInputs1 = new List<InputField>();
            wrongRunInputs2 = new List<InputField>();
            wrongRunInputs3 = new List<InputField>();
            wrongRunInputs4 = new List<InputField>();

            tglStructure.onValueChanged.AddListener(state => OnToggleValueChanged(tglStructure, state));
            tglFunction.onValueChanged.AddListener(state => OnToggleValueChanged(tglFunction, state));
            tglRun.onValueChanged.AddListener(state => OnToggleValueChanged(tglRun, state));

            foreach (var input in programInputs)
            {
                input.onValueChanged.AddListener(value => HighlightWrongInputOff(input));
            }

            foreach (var input in matrixInputs)
            {
                input.onValueChanged.AddListener(value => HighlightWrongInputOff(input));
            }

            foreach (InputField[] inputs in runInputList)
            {
                foreach (var input in inputs)
                {
                    input.onValueChanged.AddListener(value => OnRunInputValueChanged(input));
                }
            }

            //执行按钮执行逻辑
            btnRunExecute.onClick.AddListener(OnRunExecute);
        }

        private void showInputContent(GameObject visiableObject)
        {
            runContent1.SetActive(false);
            runContent2.SetActive(false);
            runContent3.SetActive(false);
            runContent4.SetActive(false);
            runContent5.SetActive(false);

            visiableObject.SetActive(true);
            RefreshRunContent(false, runStepNumber);
        }


        //邻接矩阵输入提交检测
        private void onMatrixInputSubmit()
        {
            //记录错误
            wrongmatrixInputs.Clear();

            for (int i = 0; i < matrixInputs.Length; i++)
            {
                if (matrixInputs[i].text != matrixInputAnswer[int.Parse(matrixInputs[i].name.Substring(0, 1)) - 1, int.Parse(matrixInputs[i].name.Substring(1)) - 1])
                    wrongmatrixInputs.Add(matrixInputs[i]);
            }

            bool result = wrongmatrixInputs.Count == 0;

            if (!result)
                cntMatrixInputWrong++;
            else
                cntMatrixInputWrong = 0;

            if (result)
            {
                tglFunction.isOn = true;

                Statistics.setComplete(SysDefine.Statistics.MatrixFilling);
                //展示操作步骤提示
                SetStepHint(FloydText.tip3);
            }
            else
            {
                if (cntMatrixInputWrong >= 3)
                {
                    PanelActivator.MessageBox("填写错误超过3次，已给出正确答案。", () =>
                    {
                        //填充邻接矩阵正确答案
                        for (int i = 0; i < matrixInputs.Length; i++)
                        {
                            matrixInputs[i].text = matrixInputAnswer[int.Parse(matrixInputs[i].name.Substring(0, 1)) - 1, int.Parse(matrixInputs[i].name.Substring(1)) - 1];
                        }
                    });
                }
                else
                {
                    PanelActivator.MessageBox("填写有错误，已标记出错误项。", () =>
                    {
                        foreach (var input in wrongmatrixInputs)
                        {
                            HighlightWrongInputOn(input);
                        }
                    });
                }

                //记录错误数
                Statistics.AddWrong(SysDefine.Statistics.MatrixFilling);
            }
        }

        /// <summary>
        /// 切换到程序填空，打开结构界面
        /// </summary>
        private void OnShowProgramWriting()
        {
            tglStructure.isOn = true;

            //展示初始化提示弹框

            //展示操作步骤提示开始
            SetStepHint(FloydText.tip2);
        }

        /// <summary>
        /// 选项改变事件
        /// </summary>
        /// <param name="tgl"></param>
        /// <param name="state"></param>
        private void OnToggleValueChanged(Toggle tgl, bool state)
        {
            //切换背景颜色
            tgl.image.color = state ? clrToggleSelected : clrToggleUnselected;

            //切换文字颜色
            tgl.GetComponentInChildren<Text>().color = state ? clrToggleTextSelected : clrToggleTextUnselected;

            //切换层级
            if (state)
                tgl.transform.SetAsFirstSibling();
            else
                tgl.transform.SetAsLastSibling();

            //切换效果
            if (tgl != tglRun)
            {
                UIUtility.ToggleRaycast(tgl.gameObject, !state);
            }
            else
            {
                //如果开启运行则禁用其他标签
                if (state)
                {
                    UIUtility.ToggleRaycast(tglStructure.gameObject, false);
                    UIUtility.ToggleRaycast(tglFunction.gameObject, false);

                    //切换到运行自动置顶
                    RefreshRunContent(true, runStepNumber);
                }
            }

            //如果在程序填写界面，则运行界面的几个面板都要隐藏
            if (tglFunction.isOn)
            {
                runContent1.SetActive(false);
                runContent2.SetActive(false);
                runContent3.SetActive(false);
                runContent4.SetActive(false);
                runContent5.SetActive(false);
            }

            structureContent.SetActive(tglStructure.isOn);
            functionContent.SetActive(tglFunction.isOn);
            runContent1.SetActive(tglRun.isOn);
            btnRunExecute.gameObject.SetActive(tglRun.isOn);

            //结构界面让用户输入临街矩阵
            scroll.gameObject.SetActive(!tglStructure.isOn);
            struceContentView.gameObject.SetActive(tglStructure.isOn);
        }

        /// <summary>
        /// 验证程序填空
        /// </summary>
        /// <returns></returns>
        private bool ValidateProgramWriting()
        {
            //记录错误
            wrongProgramInputs.Clear();

            for (int i = 0; i < programInputs.Length; i++)
            {
                if (programInputs[i].text != inputAnwser[i])
                    wrongProgramInputs.Add(programInputs[i]);
            }

            bool result = wrongProgramInputs.Count == 0;

            if (!result)
                cntProgramInputWrong++;
            else
                cntProgramInputWrong = 0;

            return result;
        }

        /// <summary>
        /// 高亮错误
        /// </summary>
        /// <param name="input"></param>
        private void HighlightWrongInputOn(InputField input)
        {
            Image wrongHint = input.gameObject.GetComponentInChildrenByName<Image>("WrongHint");
            FadeComponent(wrongHint);
            wrongHint.DOFade(0.5f, 0.25f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.Linear);
        }

        /// <summary>
        /// 关闭高亮
        /// </summary>
        /// <param name="input"></param>
        private void HighlightWrongInputOff(InputField input)
        {
            Image wrongHint = input.gameObject.GetComponentInChildrenByName<Image>("WrongHint");
            wrongHint.DOKill();
            FadeComponent(wrongHint);
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

        /// <summary>
        /// 程序填空提交
        /// </summary>
        private void OnProgramWritingSubmit()
        {
            bool validation = ValidateProgramWriting();

            if (validation)
            {
                isProgramInputSubmit = true;
                tglRun.isOn = true;

                Statistics.setComplete(SysDefine.Statistics.ProgramFilling);
                //展示操作步骤提示
                SetStepHint(FloydText.tip4);

                
               PanelActivator.MessageBox(FloydText.ltalk1);
                
            }
            else
            {
                if (cntProgramInputWrong >= 3)
                {
                    PanelActivator.MessageBox("填写错误超过3次，已给出正确答案。", () =>
                    {
                        for (int i = 0; i < programInputs.Length; i++)
                        {
                            programInputs[i].text = inputAnwser[i];
                        }
                    });
                }
                else
                {
                    PanelActivator.MessageBox("填写有错误，已标记出错误项。", () =>
                    {
                        foreach (var input in wrongProgramInputs)
                        {
                            HighlightWrongInputOn(input);
                        }
                    });
                }

                //记录错误数
                Statistics.AddWrong(SysDefine.Statistics.ProgramFilling);
            }
        }

        #region 运行框的逻辑

        /// <summary>
        /// 运行填空输入事件
        /// </summary>
        /// <param name="input"></param>
        /// <param name="value"></param>
        private void OnRunInputValueChanged(InputField input)
        {
            if (input.isFocused)
            {
                HighlightWrongInputOff(input);
            }
        }

        /// <summary>
        /// 点击了运行中的执行
        /// </summary>
        private void OnRunExecute()
        {
            InputField[] runInputs = runInputs1;
            string[] edgeAnswer = edgeAnswer1;
            List<InputField> wrongRunInputs = wrongRunInputs1;
            int runInputWrongNumber = runInputWrongList[runStepNumber - 1];

            switch (runStepNumber)
            {
                case 1:
                    runInputs = runInputs1;
                    edgeAnswer = edgeAnswer1;
                    wrongRunInputs = wrongRunInputs1;
                    break;
                case 2:
                    runInputs = runInputs2;
                    edgeAnswer = edgeAnswer2;
                    wrongRunInputs = wrongRunInputs2;
                    break;
                case 3:
                    runInputs = runInputs3;
                    edgeAnswer = edgeAnswer3;
                    wrongRunInputs = wrongRunInputs3;
                    break;
                case 4:
                    runInputs = runInputs4;
                    edgeAnswer = edgeAnswer4;
                    wrongRunInputs = wrongRunInputs4;
                    break;
                default:
                    break;
            }

            //取消所有错误高亮
            foreach (var input in runInputs)
                HighlightWrongInputOff(input);

            //检测有效性
            wrongRunInputs1.Clear();
            wrongRunInputs2.Clear();
            wrongRunInputs3.Clear();
            wrongRunInputs4.Clear();

            for (int i = 0; i < runInputs.Length; i++)
            {
                //检查对错
                string eValue = runInputs[i].text.ToString();
                if (eValue != edgeAnswer[i])
                {
                    wrongRunInputs.AddIfNotContains(runInputs[i]);
                }
            }

            bool validation = wrongRunInputs.Count == 0;

            if (validation)
            {
                runStepNumber++;
                if (runStepNumber == 2)
                {
                    //设置步骤提示
                   // SetStepHint(FloydText.tip6);

                    Run();
                    Invoke("MoveToSite2", 1.5f);
                }
                else if (runStepNumber == 3)
                {
                   // SetStepHint(FloydText.tip7);

                    Run();
                    Invoke("MoveToSite3", 1.5f);
                }
                else if (runStepNumber == 4)
                {
                   // SetStepHint(FloydText.tip8);
                    Run();
                    Invoke("MoveToSite4", 1.5f);
                }
                else if (runStepNumber == 5)
                {
                    //SetStepHint(FloydText.tip9);
                    MoveToSite1();
                    btnRunExecute.gameObject.SetActive(false);
                    isProgramRunComplete = true;

                    CallbackUtility.ExecuteDelay(1.5f, () =>
                    {
                        PanelActivator.MessageBox(FloydText.tip10, () => { });
                    });
                }
                showInputContent(runContentList[runStepNumber - 1]);
            }
            else
            {
                runInputWrongNumber++;
                runInputWrongList[runStepNumber - 1] = runInputWrongNumber;

                if (runInputWrongNumber >= 3)
                {
                    showRunResultTip("填写错误超过3次，已给出正确答案。");
                    FillEdgeInput(runInputs, edgeAnswer);
                }
                else
                {
                    showRunResultTip("填写有错误，已标记错误项。");
                    foreach (var input in wrongRunInputs)
                        HighlightWrongInputOn(input);
                }

                //记录错误数
                Statistics.AddWrong(SysDefine.Statistics.ProgramRun);
            }

        }

        /// <summary>
        /// 给出正确答案
        /// </summary>
        private void FillEdgeInput(InputField[] runInputs, string[] edgeAnswer)
        {
            for (int i = 0; i < runInputs.Length; i++)
            {
                runInputs[i].text = edgeAnswer[i].ToString();
                HighlightWrongInputOff(runInputs[i]);
            }
        }

        //显示提示框
        private void showRunResultTip(string resultTip)
        {
            PanelActivator.MessageBox(resultTip, () => { });
        }

        /// <summary>
        /// 刷新文本框
        /// </summary>
        /// <param name="isGoTop"></param>
        private void RefreshRunContent(bool isGoTop, int num)
        {

            string contentName = "RunContent" + num.ToString();
            Refresh(scroll.content.Find(contentName).GetComponent<RectTransform>(), () =>
            {
                Refresh(scroll.content.GetComponent<RectTransform>(), () =>
                {
                    scroll.verticalScrollbar.value = isGoTop ? 1f : 0f;
                });
            });
        }

        #endregion
    }
}