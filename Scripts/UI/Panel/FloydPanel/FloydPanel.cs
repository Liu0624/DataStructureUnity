using UnityEngine;
using WJF_CodeLibrary.UIFramework;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.CommonUtility.UI;

namespace WJF
{
    public partial class FloydPanel : UIPanelBase
    {
        public int quitWrongMat;
        public int quitWrongFill;
        public int quitWrongRun;

        private Transform graphParent;
        private Transform programWritingParent;
        private Transform matrixInputLayout;

        private Button btnSubmit;

        public RectTransform rect;
        public GameObject bg;
        public GameObject hint;
        bool isShow = false;


        public Button btnStart;
        public Button btnFull;
        public Button btnNoFull;
        public Button btnBack;



        public override void Init()
        {
            base.Init();

            CurrentUIType.panelType = UIPanelType.Normal;
            CurrentUIType.showType = UIShowType.Relevant;

            graphParent = transform.Find("GraphParent");

            programWritingParent = transform.Find("ProgramWritingParent");
            matrixInputLayout = gameObject.GetComponentInChildrenByName<Transform>("inputLayout");
            btnSubmit = gameObject.GetComponentInChildrenByName<Button>("BtnSubmit");

            btnStart.onClick.AddListener(() => { OnBtnStartClick(); });
            btnFull.onClick.AddListener(() => { OnBtnFullClick(); });
            btnNoFull.onClick.AddListener(() => { OnBtnNoFullClick(); });
            btnBack.onClick.AddListener(() => { OnBtnBackClick(); });


            InitProgramWriting();
            InitWrong();
            InitCarmeras();
            initTipText();
            

            btnSubmit.onClick.AddListener(() => OnSubmitClicked());
            RegisterEvent("BtnPreviousPanel", EventType.Click_Left, p => UICommonEvent.Return());
            RegisterEvent("BtnNextPanel", EventType.Click_Left, p => UICommonEvent.Next<BellmanFordPanel>());
            RegisterEvent("BtnQuit", EventType.Click_Left, p => UICommonEvent.Quit());
        }

        private void InitWrong()
        {
            quitWrongMat = Statistics.GetWrongCount(SysDefine.Statistics.MatrixFilling);
            quitWrongFill = Statistics.GetWrongCount(SysDefine.Statistics.ProgramFilling);
            quitWrongRun = Statistics.GetWrongCount(SysDefine.Statistics.ProgramRun);

            Statistics.ResetWrong(SysDefine.Statistics.MatrixFilling);
            Statistics.ResetWrong(SysDefine.Statistics.ProgramFilling);
            Statistics.ResetWrong(SysDefine.Statistics.ProgramRun);
        }

        protected override void OnShow()
        {
            base.OnShow();

            ResetPanel();
            OnShowProgramWriting();
        }

        /// <summary>
        /// 重置面板
        /// </summary>
        private void ResetPanel()
        {
            cntProgramInputWrong = 0;
            cntMatrixInputWrong = 0;
            runInputWrongList[0] = 0;
            runInputWrongList[1] = 0;
            runInputWrongList[2] = 0;
            runInputWrongList[3] = 0;
            runStepNumber = 1;
            isProgramInputSubmit = false;
            isProgramRunComplete = false;
            SetStepHint("");

            InputField[] inputs = GetComponentsInChildren<InputField>(true);
            foreach (var input in inputs)
            {
                input.text = "";
            }

            UIUtility.ToggleRaycast(tglStructure.gameObject, true);
            UIUtility.ToggleRaycast(tglFunction.gameObject, true);
        }


        /// <summary>
        /// 提交按键
        /// </summary>
        private void OnSubmitClicked()
        {
            SubmitProgramWriting();
        }

        /// <summary>
        /// 提交程序填空
        /// </summary>
        private void SubmitProgramWriting()
        {


            if (tglStructure.isOn)
            {
                onMatrixInputSubmit();
            }
            else
            {
                //如果程序输入未提交，则检测程序输入
                if (!isProgramInputSubmit)
                {
                    OnProgramWritingSubmit();
                }
                //否则检查运行输入
                else
                {
                    if (!isProgramRunComplete)
                    {
                        PanelActivator.MessageBox("请完成程序运行的输入。", null);
                    }
                    else
                    {
                        OnProgramWritingComplete();
                    }
                }
            }
        }

        /// <summary>
        /// 程序填写完成
        /// </summary>
        private void OnProgramWritingComplete()
        {
            PanelActivator.MessageBox("Floyd算法学习完毕\n即将返回主页进入下一阶段的学习。", () =>
            {
                SetCamerasNotEnable();
                Statistics.setComplete(SysDefine.Statistics.MatrixFilling);
                Statistics.setComplete(SysDefine.Statistics.ProgramFilling);
                Statistics.setComplete(SysDefine.Statistics.ProgramRun);
                ShowPanel<MainPanel>();
            });
        }


        private void Update()
        {
            isShow = RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition);
            if (rect.gameObject.activeSelf)
            {
                bg.SetActive(isShow);
                hint.SetActive(!isShow);
            }
        }

        void OnBtnStartClick()
        {
            UIManager.Instance.ShowUIPanel<StartPanel>("StartPanel");
        }

        void OnBtnFullClick()
        {
            Screen.fullScreen = true;
            btnFull.gameObject.SetActive(false);
            btnNoFull.gameObject.SetActive(true);

        }

        void OnBtnNoFullClick()
        {
            Screen.fullScreen = false;
            btnFull.gameObject.SetActive(true);
            btnNoFull.gameObject.SetActive(false);

        }

        void OnBtnBackClick()
        {
            FloydQuit();
            // UIManager.Instance.Return();
        }
    }
}