using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WJF_CodeLibrary.UIFramework;

namespace WJF
{

    public class RightPanel : UIPanelBase
    {
        public RectTransform rect;
        public GameObject bg;
        public GameObject hint;
        bool isShow = false;


        public Button btnStart;
        public Button btnFull;
        public Button btnNoFull;
        public Button btnBack;


        private GameObject canvas;


        public override void Init()
        {
            base.Init();

            CurrentUIType.panelType = UIPanelType.Normal;

            btnStart.onClick.AddListener(() => { OnBtnStartClick(); });
            btnFull.onClick.AddListener(() => { OnBtnFullClick(); });
            btnNoFull.onClick.AddListener(() => { OnBtnNoFullClick(); });
            btnBack.onClick.AddListener(() => { OnBtnBackClick(); });

            canvas = UIManager.Instance.GetCurrentCanvas();
        }


        private void Update()
        {
            transform.SetAsLastSibling();
            if (canvas.transform.Find("Normal/StartPanel(Clone)").gameObject.activeSelf ||
            canvas.transform.Find("Normal/MainPanel(Clone)").gameObject.activeSelf)
            {
                bg.SetActive(false);
                hint.SetActive(false);
                return;
            }
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
            Transform djGo = canvas.transform.Find("Normal/DjPanel(Clone)");
            Transform floydGo = canvas.transform.Find("Normal/FloydPanel(Clone)");
            Transform bellmanGo = canvas.transform.Find("Normal/BellmanFordPanel(Clone)");
            Transform veGo = canvas.transform.Find("Normal/VertexEdgePanel(Clone)");
            Transform storeGo = canvas.transform.Find("Normal/StorageStructurePanel(Clone)");
            Transform traGo = canvas.transform.Find("Normal/TraversePanel(Clone)");


            if (djGo != null)
            {
                if (djGo.gameObject.activeSelf)
                    canvas.transform.Find("Normal/DjPanel(Clone)").GetComponent<DjPanel>().DjQuit();
            }

            if (floydGo != null)
            {
                if (floydGo.gameObject.activeSelf)
                    canvas.transform.Find("Normal/FloydPanel(Clone)").GetComponent<FloydPanel>().FloydQuit();
            }

            if (bellmanGo != null)
            {
                if (bellmanGo.gameObject.activeSelf)
                    canvas.transform.Find("Normal/BellmanFordPanel(Clone)").GetComponent<BellmanFordPanel>().BellManQuit();
            }

            if (veGo != null)
            {
                if (veGo.gameObject.activeSelf)
                    canvas.transform.Find("Normal/VertexEdgePanel(Clone)").GetComponent<VertexEdgePanel>().VeQuit();
            }

            if (storeGo != null)
            {
                if (storeGo.gameObject.activeSelf)
                    canvas.transform.Find("Normal/StorageStructurePanel(Clone)").GetComponent<StorageStructurePanel>().StoreQuit();
            }

            if (traGo != null)
            {
                if (traGo.gameObject.activeSelf)
                    canvas.transform.Find("Normal/TraversePanel(Clone)").GetComponent<TraversePanel>().TravelQuit();
            }



            // UIManager.Instance.Return();
        }
    }
}