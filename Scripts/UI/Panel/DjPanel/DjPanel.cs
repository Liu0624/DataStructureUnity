using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WJF_CodeLibrary.UIFramework;
using UnityEngine.UI;
using System;
using WJF_CodeLibrary.Extension;
using DG.Tweening;
using WJF_CodeLibrary.CommonUtility;

namespace WJF
{
    public partial class DjPanel : UIPanelBase
    {
        public int quitWrong = 0;
        private GraphType graphType;

        private Transform graphParent;
        private Transform digraphInputParent;
        private Transform undigraphInputParent;

        private readonly string wrongHintName = "WrongHint";
        private readonly string placeholderName = "Placeholder";


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
            digraphInputParent = transform.Find("DigraphInputParent");
            undigraphInputParent = transform.Find("UndigraphInputParent");
            digraphInputParent.gameObject.SetActive(false);

            InitGraph();
            InitDigraphInput();
            InitUndigraphInput();
            InitWrong();
            InitDjCarmeras();

            btnStart.onClick.AddListener(() => { OnBtnStartClick(); });
            btnFull.onClick.AddListener(() => { OnBtnFullClick(); });
            btnNoFull.onClick.AddListener(() => { OnBtnNoFullClick(); });
            btnBack.onClick.AddListener(() => { OnBtnBackClick(); });

        }

        protected override void OnShow()
        {
            base.OnShow();

            ResetPanel();
            SwitchGraph(GraphType.Digraph);
        }

        /// <summary>
        /// 重置界面
        /// </summary>
        private void ResetPanel()
        {
            InputField[] inputs = GetComponentsInChildren<InputField>(true);
            foreach (var input in inputs)
            {
                input.text = "";
            }

            ResetUndigraphPage();//重置无向图页面
            ResetDigraphPage();//重置有向图页面
        }

        /// <summary>
        /// 切换图的显示
        /// </summary>
        /// <param name="type"></param>
        private void SwitchGraph(GraphType type)
        {
            graphType = type;

            //切换图的显示
            SwitchGraphDisplay(type);

            //切换按钮效果
            SwitchSelectionButton(type);

        }

        private void InitWrong()
        {
            //
            quitWrong = Statistics.GetWrongCount(SysDefine.Statistics.DisInDigraph);
            Statistics.ResetWrong(SysDefine.Statistics.DisInDigraph);
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
            DjQuit();
            // UIManager.Instance.Return();
        }
    }
}