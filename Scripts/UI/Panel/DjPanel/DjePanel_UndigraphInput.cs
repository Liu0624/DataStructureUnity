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

        private InputField[] vertexNameInput, vertexValueInput;
        private HorizontalLayoutGroup vertexNameLayout, vertexValueLayout;
        private Button btnUndigraphNext, btnUndigraphReset;
        private Text sValue, guideText;
        private int stepNumberUndigraph = 1;

        private bool isUndigraphReset = false;
        private int[] unDigraphWrongCountList = { 0, 0, 0, 0, 0, 0 };

        private List<List<string>> valueList = new List<List<string>>() {
            new List<string>() { "5", "2", "m", "m", "m",  "m" },
            new List<string>() { "5", "2", "8", "m", "10",  "m" },
            new List<string>() { "5", "2", "6", "11", "10", "m" },
            new List<string>() { "5", "2", "6",  "7", "8",  "m" },
            new List<string>() { "5", "2", "6", "7", "8", "14" },
            new List<string>() { "5", "2", "6", "7", "8","11" }};

        //private readonly string tip2 = "第一步\n请输入A到各个顶点的初始距离，不能直达的用0表示! \n点击下一步";

        private readonly string tip3 = "距离C路径是2最短\n首先遍历C点,将C加入集合S";

        private readonly string tip4 = "第二步,现在可以经过C去别的顶点了,请更新A到剩余顶点的最短路径值\nA -> C -> D = 8\nA -> C -> F = 10\nE和G还是到不了,继续下一步";

        private readonly string tip5 = "距离B点路径为5最短\n接下来将B点加入集合S";

        private readonly string tip6 = "第三步,我们现在可以经过顶点B去其他顶点了,请更新A到剩余顶点的最短路径值\nA -> B -> D = 6\nA -> B -> E = 11\n只剩下G还没有路到达了,继续往下走";

        private readonly string tip7 = "距离D路径为6是最短距离\n所以我们将D加入集合S。";

        private readonly string tip8 = "第四步,现在可以经过顶点D去其他顶点了,请更新A到剩余顶点的最短路径值\nA -> B -> D -> E = 7\nA -> B -> D -> F = 8\n可惜G还是没有路线,下一步下一步";

        private readonly string tip9 = "在集合L剩下的点中距离E距离为7为最短的距离,\n我们将E加入集合S。";

        private readonly string tip10 = "第五步,现在顶点E也已经到了,请更新A到剩余顶点的最短路径值\nA -> B -> D -> E -> G = 14\n现在所有点都到达了,但不一定都是最近的路,下一步";

        private readonly string tip11 = "在集合L中距离F最近,我们将F也拉入集合S。";

        private readonly string tip12 = "第六步,走到顶点F之后发现F到G点距离是3,所以从A到G最短路径就是11了\n最后再更新一下距离表信息！\n然后走完我们的最后一步吧 -------->";

        private readonly string tip13 = "将最后的顶点G加入集合S,整个求解过程就都完成了~~\n小伙伴们看懂了么？";

        /// 初始化矩阵部分
        private void InitUndigraphInput()
        {

            btnUndigraphNext = gameObject.GetComponentInChildrenByName<Button>("nextStep");
            btnUndigraphReset = gameObject.GetComponentInChildrenByName<Button>("BtnReset");

            sValue = gameObject.GetComponentInChildrenByName<Text>("s");
            guideText = gameObject.GetComponentInChildrenByName<Text>("guideText");


            vertexNameLayout = gameObject.GetComponentInChildrenByName<HorizontalLayoutGroup>("vertex");
            vertexValueLayout = gameObject.GetComponentInChildrenByName<HorizontalLayoutGroup>("vertexValue");

            vertexNameInput = vertexNameLayout.GetComponentsInChildren<InputField>();
            vertexValueInput = vertexValueLayout.GetComponentsInChildren<InputField>();


            btnUndigraphNext.onClick.AddListener(() => startUndigraphWork(OnNextStepClicked()));
            btnUndigraphReset.onClick.AddListener(() => OnResetClicked());

            ResetUnDigraphLayout();
        }

        /// <summary>
        /// 重置矩阵输入
        /// </summary>
        private void ResetUnDigraphLayout()
        {
            //重置顶点框
            foreach (var input in vertexNameInput)
            {
                input.enabled = false;
                input.transform.Find("Text").GetComponent<Text>().text = input.name;
                Image img = input.gameObject.GetComponentInChildrenByName<Image>(wrongHintName);
                img.color = ColorCalculateUtility.GetFadeColor(img.color, 0f);
            }

            ////重置顶点距离输入框
            foreach (var input in vertexValueInput)
            {
                input.text = "";
                input.enabled = true;
                Image img = input.gameObject.GetComponentInChildrenByName<Image>(wrongHintName);
                img.color = ColorCalculateUtility.GetFadeColor(img.color, 0f);
            }
            //重置错误计数
            for (int i = 0; i < 6; i++)
                unDigraphWrongCountList[i] = 0;
        }

        private IEnumerator showText(string hh)
        {
            guideText.text = "";
            foreach (char c in hh)
            {
                if (isUndigraphReset)
                {
                    break;
                }
                guideText.text += c;
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void enabledBtnState(bool state)
        {
            btnUndigraphNext.interactable = state;
            btnUndigraphReset.interactable = state;
        }

        /// <summary>
        /// 重置按钮点击事件
        /// </summary>
        private void OnResetClicked()
        {
            PanelActivator.MessageBox("重置后将清空最短路径查找过程，是否继续？",
                () =>
                {
                    ResetUndigraphPage();
                }, () => { });
        }

        private void ResetUndigraphPage()
        {
            if (UndigraphWorkCoroutine != null)
            {
                StopCoroutine(UndigraphWorkCoroutine);
            }
            isUndigraphReset = false;
            guideText.text = "";
            sValue.text = "{ A(0) }";
            ResetUdigraphCircleAndLine();
            ResetUnDigraphLayout();
            btnUndigraphNext.gameObject.SetActive(true);
            stepNumberUndigraph = 1;
            enabledBtnState(true);
        }

        //下一步按钮点击事件
        private IEnumerator OnNextStepClicked()
        {
            if (isInputComplete(vertexValueInput, stepNumberUndigraph, false, "A到顶点") && isInputCorrect(vertexValueInput, valueList[stepNumberUndigraph - 1], stepNumberUndigraph, false, "A到顶点"))
            {
                guideText.text = "";
                enabledBtnState(false);
                switch (stepNumberUndigraph)
                {
                    case 1:
                        Coroutine ac = StartCoroutine(AtoC());
                        yield return new WaitForSeconds(1.5f);
                        guideText.text = tip3;
                        disableView("C");
                        sValue.text = "{ A(0),C(2) }";
                        yield return new WaitForSeconds(2);
                        StopCoroutine(ac);
                        stepNumberUndigraph = 2;
                        break;
                    case 2:
                        Coroutine ab = StartCoroutine(AtoB());
                        yield return new WaitForSeconds(1.5f);
                        guideText.text = tip5;
                        disableView("B");
                        sValue.text = "{ A(0),C(2),B(5) }";
                        yield return new WaitForSeconds(2);
                        StopCoroutine(ab);
                        stepNumberUndigraph = 3;
                        break;
                    case 3:
                        Coroutine ad = StartCoroutine(AtoD());
                        yield return new WaitForSeconds(1.5f);
                        guideText.text = tip7;
                        disableView("D");
                        sValue.text = "{ A(0),C(2),B(5),D(6) }";
                        yield return new WaitForSeconds(2);
                        StopCoroutine(ad);
                        stepNumberUndigraph = 4;
                        break;
                    case 4:
                        Coroutine ae = StartCoroutine(AtoE());
                        yield return new WaitForSeconds(1.5f);
                        guideText.text = tip9;
                        disableView("E");
                        sValue.text = "{ A(0),C(2),B(5),D(6),E(7) }";
                        yield return new WaitForSeconds(2);
                        StopCoroutine(ae);
                        stepNumberUndigraph = 5;
                        break;
                    case 5:
                        Coroutine af = StartCoroutine(AtoF());
                        yield return new WaitForSeconds(1.5f);
                        guideText.text = tip11;
                        disableView("F");
                        sValue.text = "{ A(0),C(2),B(5),D(6),F(8) }";
                        yield return new WaitForSeconds(2);
                        StopCoroutine(af);
                        stepNumberUndigraph = 6;
                        break;
                    case 6:
                        Coroutine ag = StartCoroutine(AtoG());
                        yield return new WaitForSeconds(1.5f);
                        guideText.text = tip13;
                        disableView("G");
                        sValue.text = "{ A(0),C(2),B(5),D(6),E(7),F(8),G(11) }";
                        yield return new WaitForSeconds(2);
                        StopCoroutine(ag);
                        stepNumberUndigraph = 7;
                        break;
                }
                enabledBtnState(true);
            }

            if (stepNumberUndigraph == 7)
            {
                btnUndigraphNext.gameObject.SetActive(false);
            }
        }

        private void disableView(String vertexName)
        {
            foreach (var input in vertexNameInput)
            {
                if (isUndigraphReset)
                {
                    break;
                }
                if (input.name.Equals(vertexName))
                {
                    Image img = input.transform.Find(wrongHintName).GetComponent<Image>();
                    img.color = Color.red;
                    img.DOFade(0.5f, 0.25f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.Linear);
                    input.enabled = false;
                }
            }

            foreach (var input in vertexValueInput)
            {
                if (isUndigraphReset)
                {
                    break;
                }
                if (input.name.Equals(vertexName + "Value"))
                {
                    Image img = input.transform.Find(wrongHintName).GetComponent<Image>();
                    img.color = Color.red;
                    img.DOFade(0.5f, 0.25f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.Linear);
                    input.enabled = false;
                }
            }

        }

        private void UndigraphFillAnswer(int step)
        {
            switch (step)
            {
                case 1:
                    for (int i = 0; i < 6; i++)
                    {
                        vertexValueInput[i].text = valueList[0][i];
                    }
                    break;
                case 2:
                    for (int i = 2; i < 6; i++)
                    {
                        vertexValueInput[i].text = valueList[1][i];
                    }
                    break;
                case 3:
                    for (int i = 2; i < 6; i++)
                    {
                        vertexValueInput[i].text = valueList[2][i];
                    }
                    break;
                case 4:
                    for (int i = 3; i < 6; i++)
                    {
                        vertexValueInput[i].text = valueList[3][i];
                        print(vertexValueInput[i].transform.name);
                    }
                    break;
                case 5:
                    print(vertexValueInput[5].text);
                    vertexValueInput[5].text = valueList[4][5];
                    print(vertexValueInput[5].transform.name);

                    break;
                case 6:
                    vertexValueInput[5].text = valueList[5][5];
                    print(vertexValueInput[5].transform.name);
                    break;

            }
        }
    }
}