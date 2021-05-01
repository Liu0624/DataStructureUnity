using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.CommonUtility.UI;
using DG.Tweening;

namespace WJF
{
    public partial class DjPanel
    {
        [Header("Graph")]
        public Color clrBtnSelected;
        public Color clrBtnUnselected;
        public Color clrBtnTextSelected;
        public Color clrBtnTextUnselected;

        private Transform digraphParent;
        private Transform undigraphParent;
        private Image imgDigraphSelection;
        private Image imgUndigraphSelection;

        private Coroutine digraphWorkCoroutine;
        private Coroutine UndigraphWorkCoroutine;

        //有向图
        private Transform digCircle2;
        private Transform digCircle3;
        private Transform digCircle4;
        private Transform digCircle5;
        private Text talk1, talk2, talk3, talk4;
        private Transform talkObject1, talkObject2, talkObject3, talkObject4;

        // 文字出现
       
        private string writerText5 = DjText.tip9;

        private Transform line15, line12, line52, line54, line53, line521, line531, line23, line43;

        private List<Transform> talkList = new List<Transform>();
        private List<Transform> digraphObject = new List<Transform>();
        private List<Transform> undigraphObject = new List<Transform>();
        private float showTime = 4.0f;
        private float dismissTime = 1.0f;

        private bool isStartWork = false;

        // 无向图
        private Transform lineBD, lineAB, lineAC, lineCD, lineBE, lineDE, lineDF, lineEG, lineFG, lineCF;
        private Transform circleB, circleC, circleD, circleE, circleF, circleG;
        private Button btnUndigraphReplay;
        /// <summary>
        /// 初始化图的显示
        /// </summary>
        private void InitGraph()
        {
            digraphParent = graphParent.Find("Digraph");
            undigraphParent = graphParent.Find("Undigraph");
            imgDigraphSelection = graphParent.gameObject.GetComponentInChildrenByName<Image>("BtnDigraph");
            imgUndigraphSelection = graphParent.gameObject.GetComponentInChildrenByName<Image>("BtnUndigraph");

            RegisterEvent(imgDigraphSelection.gameObject, EventType.Click_Left, p => SwitchGraph(GraphType.Digraph));
            RegisterEvent(imgUndigraphSelection.gameObject, EventType.Click_Left, p => SwitchGraph(GraphType.Undigraph));
            
            //有向图初始化
            initView();
            //无向图初始化
            UndigraphInitView();
        }

        /// <summary>
        /// 切换选择按键
        /// </summary>
        /// <param name="type"></param>
        private void SwitchSelectionButton(GraphType type)
        {
            //切换颜色
            imgDigraphSelection.color = type == GraphType.Digraph ? clrBtnSelected : clrBtnUnselected;
            imgUndigraphSelection.color = type == GraphType.Undigraph ? clrBtnSelected : clrBtnUnselected;

            //切换文字颜色
            imgDigraphSelection.GetComponentInChildren<Text>().color = type == GraphType.Digraph ? clrBtnTextSelected : clrBtnTextUnselected;
            imgUndigraphSelection.GetComponentInChildren<Text>().color = type == GraphType.Undigraph ? clrBtnTextSelected : clrBtnTextUnselected;

            //调整层级
            int selectedSiblingIndex = graphParent.childCount - 3;
            int unselectedSiblingIndex = graphParent.childCount - 1;
            imgDigraphSelection.transform.SetSiblingIndex(type == GraphType.Digraph ? selectedSiblingIndex : unselectedSiblingIndex);
            imgUndigraphSelection.transform.SetSiblingIndex(type == GraphType.Undigraph ? selectedSiblingIndex : unselectedSiblingIndex);

            //切换可用性
            UIUtility.ToggleRaycast(imgDigraphSelection.gameObject, !(type == GraphType.Digraph));
            UIUtility.ToggleRaycast(imgUndigraphSelection.gameObject, !(type == GraphType.Undigraph));
        }

        /// <summary>
        /// 切换图的显示
        /// </summary>
        /// <param name="type"></param>
        private void SwitchGraphDisplay(GraphType type)
        {
            digraphParent.gameObject.SetActive(type == GraphType.Digraph);
            undigraphParent.gameObject.SetActive(type == GraphType.Undigraph);
         
            switch (type)
            {
                case GraphType.Digraph:
                    undigraphInputParent.gameObject.SetActive(false);
                    digraphInputParent.gameObject.SetActive(true);
                    break;
                case GraphType.Undigraph:
                    undigraphInputParent.gameObject.SetActive(true);
                    digraphInputParent.gameObject.SetActive(false);
                    break;
            }

        }

        private void initView()
        {
            digCircle5 = digraphParent.Find("DigCircle5");
            digCircle4 = digraphParent.Find("DigCircle4");
            digCircle2 = digraphParent.Find("DigCircle2");
            digCircle3 = digraphParent.Find("DigCircle3");

            line15 = digraphParent.Find("Line1-5");
            line12 = digraphParent.Find("Line1-2");
            line52 = digraphParent.Find("Line5-2");
            line54 = digraphParent.Find("Line5-4");
            line53 = digraphParent.Find("Line5-3");
            line521 = digraphParent.Find("Line5-2-1");
            line531 = digraphParent.Find("Line5-3-1");
            line23 = digraphParent.Find("Line2-3");
            line43 = digraphParent.Find("Line4-3");

            digraphObject.Add(digCircle5);
            digraphObject.Add(digCircle4);
            digraphObject.Add(digCircle3);
            digraphObject.Add(digCircle2);


            digraphObject.Add(line15);
            digraphObject.Add(line12);
            digraphObject.Add(line52);
            digraphObject.Add(line54);
            digraphObject.Add(line53);
            digraphObject.Add(line521);
            digraphObject.Add(line531);
            digraphObject.Add(line23);
            digraphObject.Add(line43);

            //对话框初始化
            talkObject1 = digraphParent.Find("talk1");
            talkObject1.gameObject.SetActive(false);
            talk1 = talkObject1.gameObject.GetComponentInChildrenByName<Text>("Text");

            talkObject2 = digraphParent.Find("talk2");
            talkObject2.gameObject.SetActive(false);
            talk2 = talkObject2.gameObject.GetComponentInChildrenByName<Text>("Text");

            talkObject3 = digraphParent.Find("talk3");
            talkObject3.gameObject.SetActive(false);
            talk3= talkObject3.gameObject.GetComponentInChildrenByName<Text>("Text");

            talkObject4 = digraphParent.Find("talk4");
            talkObject4.gameObject.SetActive(false);
            talk4 = talkObject4.gameObject.GetComponentInChildrenByName<Text>("Text");
        }

        private void UndigraphInitView()
        {
            circleB = undigraphParent.Find("B");
            circleC = undigraphParent.Find("C");
            circleD = undigraphParent.Find("D");
            circleE = undigraphParent.Find("E");
            circleF = undigraphParent.Find("F");
            circleG = undigraphParent.Find("G");

            undigraphObject.Add(circleB);
            undigraphObject.Add(circleC);
            undigraphObject.Add(circleD);
            undigraphObject.Add(circleE);
            undigraphObject.Add(circleF);
            undigraphObject.Add(circleG);

            lineBD = undigraphParent.Find("LineBD");
            lineAB = undigraphParent.Find("LineAB");
            lineAC = undigraphParent.Find("LineAC");
            lineCD = undigraphParent.Find("LineCD");
            lineBE = undigraphParent.Find("LineBE");
            lineDE = undigraphParent.Find("LineDE");
            lineDF = undigraphParent.Find("LineDF");
            lineEG = undigraphParent.Find("LineEG");
            lineFG = undigraphParent.Find("LineFG");
            lineCF = undigraphParent.Find("LineCF");

            undigraphObject.Add(lineBD);
            undigraphObject.Add(lineAB);
            undigraphObject.Add(lineAC);
            undigraphObject.Add(lineCD);
            undigraphObject.Add(lineBE);
            undigraphObject.Add(lineDE);
            undigraphObject.Add(lineDF);
            undigraphObject.Add(lineEG);
            undigraphObject.Add(lineFG);
            undigraphObject.Add(lineCF);

        }
        //有向图 初始化动画开始，控制左侧对话框的展示
        
        private void startDigraphWork(IEnumerator routine)
        {
            digraphWorkCoroutine = StartCoroutine(routine);
        }

        private void startUndigraphWork(IEnumerator routine)
        {
            UndigraphWorkCoroutine = StartCoroutine(routine);
        }

        //遍历的第一趟操作1到5
        private IEnumerator startFirstTime()
        {
            //路径显示
            talk1.text = "";
            talkObject2.gameObject.SetActive(false);
            talkObject3.gameObject.SetActive(false);
            talkObject4.gameObject.SetActive(false);

            StartCoroutine(Slowly(line12));
            yield return new WaitForSeconds(dismissTime);
            StartCoroutine(Slowly(line15));
            yield return new WaitForSeconds(dismissTime);

            StartCoroutine(TwinkleTwo(line15));
            line15.GetComponent<Image>().color = Color.yellow;
            line12.GetComponent<CanvasGroup>().alpha = 0.0f;
            line15.GetComponent<CanvasGroup>().alpha = 0.0f;
            
            yield return new WaitForSeconds(1.0f);

            StartCoroutine(Slowly(digCircle5));
            yield return new WaitForSeconds(dismissTime);
            talkObject1.gameObject.SetActive(true);

            yield return new WaitForSeconds(dismissTime);
            secondTime.gameObject.SetActive(true);
            secondInputs[3].transform.Find(placeholderName).GetComponent<Text>().text = alreadyFind;
            secondInputs[3].enabled = false;
            enableSubmitBtnState(true);
            chooseCamera(camera25,djLabels[4]);
            //LabelControl.Instance.SwitchLabels();
            PanelActivator.MessageBox(DjText.tip3);
        }

        // 1到4
        private IEnumerator startSecondTime()
        {
            talk2.text = "";
            talkObject1.gameObject.SetActive(false);
            talkObject3.gameObject.SetActive(false);
            talkObject4.gameObject.SetActive(false);

            StartCoroutine(Slowly(line12));
            yield return new WaitForSeconds(dismissTime);

            StartCoroutine(Slowly(line52));
            yield return new WaitForSeconds(dismissTime);

            StartCoroutine(Slowly(line53));
            yield return new WaitForSeconds(dismissTime);
            StartCoroutine(Slowly(line54));
            yield return new WaitForSeconds(dismissTime);

            StartCoroutine(TwinkleList(new List<Transform>() { line15, line54 }));
            line54.GetComponent<Image>().color = Color.yellow;
            line12.GetComponent<CanvasGroup>().alpha = 0.0f;
            line53.GetComponent<CanvasGroup>().alpha = 0.0f;
            if (line52.GetComponent<Image>().color != Color.black)
                line52.GetComponent<CanvasGroup>().alpha = 0.0f;
            yield return new WaitForSeconds(dismissTime);

            StartCoroutine(Slowly(digCircle4));
            yield return new WaitForSeconds(dismissTime);
            talkObject2.gameObject.SetActive(true);
            yield return new WaitForSeconds(dismissTime);
            
           
            threeTime.gameObject.SetActive(true);

            threeInputs[2].transform.Find(placeholderName).GetComponent<Text>().text = alreadyFind;
            threeInputs[2].enabled = false;
            threeInputs[3].transform.Find(placeholderName).GetComponent<Text>().text = alreadyFind;
            threeInputs[3].enabled = false;
            enableSubmitBtnState(true);
            chooseCamera(camera24,djLabels[3]);
            //LabelControl.Instance.SwitchLabels();
            PanelActivator.MessageBox(DjText.tip5);
            
        }

        // 1到2
        private IEnumerator startThreeTime()
        {
            talk3.text = "";
            talkObject1.gameObject.SetActive(false);
            talkObject2.gameObject.SetActive(false);
            talkObject4.gameObject.SetActive(false);

            StartCoroutine(Slowly(line12));
            yield return new WaitForSeconds(dismissTime);

            StartCoroutine(Slowly(line52));
            yield return new WaitForSeconds(dismissTime);

            StartCoroutine(Slowly(line53));
            yield return new WaitForSeconds(dismissTime);

            StartCoroutine(Slowly(line43));
            yield return new WaitForSeconds(dismissTime);

            StartCoroutine(TwinkleList(new List<Transform>() { line15, line52 }));
            yield return new WaitForSeconds(dismissTime);

            line52.GetComponent<Image>().color = Color.yellow;
            line12.GetComponent<CanvasGroup>().alpha = 0.0f;
            line53.GetComponent<CanvasGroup>().alpha = 0.0f;
            line43.GetComponent<CanvasGroup>().alpha = 0.0f;
            yield return new WaitForSeconds(dismissTime);
            StartCoroutine(Slowly(digCircle2));
            yield return new WaitForSeconds(dismissTime);
            talkObject3.gameObject.SetActive(true);
            //结论
            
            
            fourTime.gameObject.SetActive(true);
            fourInputs[0].transform.Find(placeholderName).GetComponent<Text>().text = alreadyFind;
            fourInputs[0].enabled = false;
            fourInputs[2].transform.Find(placeholderName).GetComponent<Text>().text = alreadyFind;
            fourInputs[2].enabled = false;
            fourInputs[3].transform.Find(placeholderName).GetComponent<Text>().text = alreadyFind;
            fourInputs[3].enabled = false;
            enableSubmitBtnState(true);
            chooseCamera(camera22,djLabels[1]);
            //LabelControl.Instance.SwitchLabels();
           PanelActivator.MessageBox(DjText.tip7);

        }


        private IEnumerator startFourTime()
        {
            //1 到3
            talk4.text = "";
            talkObject1.gameObject.SetActive(false);
            talkObject2.gameObject.SetActive(false);
            talkObject3.gameObject.SetActive(false);

            StartCoroutine(Slowly(line23));
            yield return new WaitForSeconds(dismissTime);

            StartCoroutine(Slowly(line53));
            yield return new WaitForSeconds(dismissTime);

            StartCoroutine(Slowly(line43));
            yield return new WaitForSeconds(dismissTime);

            StartCoroutine(TwinkleList(new List<Transform>() { line15, line52, line23 }));
            yield return new WaitForSeconds(dismissTime);

            line23.GetComponent<Image>().color = Color.yellow;
            line53.GetComponent<CanvasGroup>().alpha = 0.0f;
            line43.GetComponent<CanvasGroup>().alpha = 0.0f;
            yield return new WaitForSeconds(dismissTime);
            StartCoroutine(Slowly(digCircle3));
            yield return new WaitForSeconds(dismissTime);

            talkObject4.gameObject.SetActive(true);

            //结论
            
            chooseCamera(camera23,djLabels[2]);
            //LabelControl.Instance.SwitchLabels();
            //PanelActivator.MessageBox(DjText.tip9);

            //yield return new WaitForSeconds(1.0f);
            PanelActivator.MessageBox("安全抵达区域3，目前这片草场上所有的藏羚羊已经全部都找到了！点击提交回到主界面！", () =>{
                talk4.text = "";
                talkObject4.gameObject.SetActive(false);
                enableSubmitBtnState(true);
            });

            
        }

        //无向图
        //A到C
        private IEnumerator AtoC()
        {
            //A到C
            startUndigraphWork(Slowly(lineAB));
            startUndigraphWork(Slowly(lineAC));
            yield return new WaitForSeconds(dismissTime);
            startUndigraphWork(TwinkleTwo(lineAC));
            lineAC.GetComponent<Image>().color = Color.green;
            yield return new WaitForSeconds(dismissTime);
            startUndigraphWork(Slowly(circleC));
        }
        private IEnumerator AtoB()
        {
            //A到B
            startUndigraphWork(Slowly(lineCD));
            startUndigraphWork(Slowly(lineCF));
            yield return new WaitForSeconds(dismissTime);
            startUndigraphWork(TwinkleTwo(lineAB));
            lineAB.GetComponent<Image>().color = Color.green;
            yield return new WaitForSeconds(dismissTime);
            startUndigraphWork(Slowly(circleB));
        }
        private IEnumerator AtoD()
        {
            //A到D
            startUndigraphWork(Slowly(lineBE));
            startUndigraphWork(Slowly(lineBD));
            yield return new WaitForSeconds(dismissTime);
            startUndigraphWork(TwinkleList(new List<Transform>() { lineAB, lineBD }));
            lineBD.GetComponent<Image>().color = Color.green;
            yield return new WaitForSeconds(dismissTime);
            startUndigraphWork(Slowly(circleD));
        }
        private IEnumerator AtoE()
        {
            //A到E
            startUndigraphWork(Slowly(lineBE));
            startUndigraphWork(Slowly(lineDE));
            yield return new WaitForSeconds(dismissTime);
            startUndigraphWork(TwinkleList(new List<Transform>() { lineAB, lineBD, lineDE }));
            lineDE.GetComponent<Image>().color = Color.green;
            yield return new WaitForSeconds(dismissTime);
            startUndigraphWork(Slowly(circleE));
        }
        private IEnumerator AtoF()
        {
            //A到F
            startUndigraphWork(Slowly(lineDF));
            startUndigraphWork(TwinkleList(new List<Transform>() { lineAB, lineBD, lineDF }));
            lineDF.GetComponent<Image>().color = Color.green;
            yield return new WaitForSeconds(dismissTime);
            startUndigraphWork(Slowly(circleF));
        }
        private IEnumerator AtoG()
        {
            //A到G
            startUndigraphWork(Slowly(lineEG));
            startUndigraphWork(Slowly(lineFG));
            yield return new WaitForSeconds(dismissTime);
            startUndigraphWork(TwinkleList(new List<Transform>() { lineAB, lineBD, lineDF, lineFG }));
            lineFG.GetComponent<Image>().color = Color.green;
            yield return new WaitForSeconds(dismissTime);
            startUndigraphWork(Slowly(circleG));
        }



        //打字效果
        private IEnumerator TypewriterEffect(Transform ss, string hh)
        {
            Text tt = ss.GetComponent<Text>();
            foreach (char c in hh)
            {

                if (!isStartWork)
                {
                    break;
                }
                tt.text += c;
                yield return new WaitForSeconds(0.1f);
            }
        }

        //渐变出现
        private IEnumerator Slowly(Transform tt)
        {
            float ww = 0.01f;
            CanvasGroup canvasGroup = tt.GetComponent<CanvasGroup>();
            while (1.0f != canvasGroup.alpha)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1.0f, 4.0f * ww);
                if (Mathf.Abs(1.0f - canvasGroup.alpha) <= 0.01f)
                {
                    canvasGroup.alpha = 1.0f;
                }
                yield return new WaitForSeconds(ww);
                ww += 0.01f;
            }

        }

        
        //使物体闪烁3次
        private IEnumerator TwinkleTwo(Transform graphObject)
        {
            int temp = 0;
            bool flag = true;
            while (temp <= 3)
            {
                if (flag)
                {
                    graphObject.gameObject.SetActive(false);
                    flag = false;
                }
                else
                {
                    graphObject.gameObject.SetActive(true);
                    flag = true;
                }
                yield return new WaitForSeconds(0.3f);
                temp += 1;
            }

        }

        //使多个物体闪烁3次
        private IEnumerator TwinkleList(List<Transform> graphObjectList)
        {
            int temp = 0;
            bool flag = true;
            while (temp <= 3)
            {
                if (flag)
                {
                    foreach (Transform graphObject in graphObjectList)
                    {
                        graphObject.gameObject.SetActive(false);
                    }
                    flag = false;
                }
                else
                {
                    foreach (Transform graphObject in graphObjectList)
                    {
                        graphObject.gameObject.SetActive(true);
                    }
                    flag = true;
                }
                yield return new WaitForSeconds(0.3f);
                temp += 1;
            }

        }

        //恢复初始状态
        public void ResetDigraphCircleAndLine()
        {
            CanvasGroup canvasGroup;
            
            talkObject1.gameObject.SetActive(false);
            talkObject2.gameObject.SetActive(false);
            talkObject3.gameObject.SetActive(false);
            talkObject4.gameObject.SetActive(false);
    
            foreach (Transform t in digraphObject)
            {
                canvasGroup = t.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                }
            }
        }

         public void ResetUdigraphCircleAndLine()
        {
            CanvasGroup canvasGroup;
            if (UndigraphWorkCoroutine != null)
            {
                StopCoroutine(UndigraphWorkCoroutine);
            }

            foreach (Transform t in undigraphObject)
            {
                canvasGroup = t.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                }
            }
        }

        private void startDelay()
        {
            //startDigraphWork(AnimationTalkStart());
        }

        
    }
}