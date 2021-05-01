using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.UIFramework;

namespace WJF
{

    // 实验结果分析panel
    public class ResultPanel : UIPanelBase
    {
        public Color clrBtnNormal;//普通颜色
        public Color clrBtnSelected;//选中颜色

        private enum SwitchType { Score, Detail }
        private SwitchType type;

        private GameObject scoreParent;
        private GameObject detailParent;

        private Button btnScore;//点击查看分数
        private Button btnDetail;//点击查看做题详情

        private Button backHome;//返回
        private Button sendMessageBtn;//提交

        private GameObject on1, on2, off1, off2;//左侧按钮背景，显示切换效果

        private Text button1Text, button2Text;//左侧两个按钮，为了设置点击变色

        private Text scoreText;//最终得分

        private ScrollRect scroll;

        private GameObject numberStep;

        private List<GameObject> numberList = new List<GameObject>();

        private List<ResultItem> itemList = new List<ResultItem>();

        public override void Init()
        {
            base.Init();

            CurrentUIType.panelType = UIPanelType.Normal;
            CurrentUIType.showType = UIShowType.Relevant;

            scoreParent = gameObject.GetChild("right1");
            detailParent = gameObject.GetChild("right2");

            on1 = gameObject.GetChild("menu/on1");
            on2 = gameObject.GetChild("menu/on2");
            off1 = gameObject.GetChild("menu/off1");
            off2 = gameObject.GetChild("menu/off2");

            btnScore = gameObject.GetComponentInChildrenByName<Button>("menu/Button1");
            btnDetail = gameObject.GetComponentInChildrenByName<Button>("menu/Button2");
            backHome = gameObject.GetComponentInChildrenByName<Button>("back");
            sendMessageBtn = gameObject.GetComponentInChildrenByName<Button>("send");

            button1Text = btnScore.gameObject.GetComponentInChildrenByName<Text>("Text");
            button2Text = btnDetail.gameObject.GetComponentInChildrenByName<Text>("Text");
            scoreText = scoreParent.gameObject.GetComponentInChildrenByName<Text>("score");

            btnScore.onClick.AddListener(() => SwitchDisplay(SwitchType.Score));
            btnDetail.onClick.AddListener(() => SwitchDisplay(SwitchType.Detail));

            backHome.onClick.AddListener(() =>
            {
                PanelActivator.MessageBox("确定返回主页面?", () =>
                {
                    ShowPanel<MainPanel>();
                    //HidePanel<ResultPanel>();
                }, () => { });
            });

            sendMessageBtn.onClick.AddListener(() =>
            {
                PanelActivator.MessageBox("确定提交结果吗？", () =>
                {
                    //发送实验数据到服务器
                    sendData();
                   // HidePanel<ResultPanel>();
                }, () => { });
            });

            scroll = transform.Find("right2").GetComponentInChildren<ScrollRect>();


            //初始化12个条目
            for (int i = 1; i <= 12; i++)
            {
                string name = i.ToString();
                numberStep = scroll.content.Find(name).gameObject;
                numberList.Add(numberStep);
            }

            //构造12条答题记录
            initData();

            //计算最终得分
            initTotalScore();

            //显示文案到页面
            initView();

            
        }

        private void sendData()
        {
            Statistics.endTime = Communication.Instance.GetTimeStamp();
            Communication.Instance.SendData(1, (int)initTotalScore(), Statistics.startTime, Statistics.endTime, Statistics.GetResultJsonStr(), null, null);
        }

        private double initTotalScore()
        {
            double score = 0;


            foreach (ResultItem item in itemList)
            {
                if (item.isComplete)
                {
                    score += item.score;
                }
            }

            scoreText.text = score + "";

            return score;
        }

        private void initView()
        {
            for (int i = 0; i < 12; i++)
            {
                Text[] textList = numberList[i].GetComponentsInChildren<Text>();

                ResultItem item = itemList[i];

                for (int k = 0; k < textList.Length; k++)
                {
                    if (k < 4)
                    {
                        textList[0].text = item.name;
                        textList[1].text = item.isComplete ? item.status : item.defaltStatus;
                        textList[2].text = item.isComplete ? item.scoreText : item.defaltScoreText;
                        textList[3].text = item.isComplete ? item.tip : item.defaltTip;
                    }
                }
            }
        }

        private void initData()
        {

            ResultItem item1 = new ResultItem(SysDefine.Statistics.MatrixInDigraph, Statistics.isComplete(SysDefine.Statistics.MatrixInDigraph), 1, 3, Statistics.GetWrongCount(SysDefine.Statistics.MatrixInDigraph));
            ResultItem item2 = new ResultItem(SysDefine.Statistics.MatrixInUndigraph, Statistics.isComplete(SysDefine.Statistics.MatrixInUndigraph), 1, 3, Statistics.GetWrongCount(SysDefine.Statistics.MatrixInUndigraph));
            ResultItem item3 = new ResultItem(SysDefine.Statistics.AdjacencyList, Statistics.isComplete(SysDefine.Statistics.AdjacencyList), 1, 6, Statistics.GetWrongCount(SysDefine.Statistics.AdjacencyList));
            ResultItem item4 = new ResultItem(SysDefine.Statistics.AdjacencyListDigraph, Statistics.isComplete(SysDefine.Statistics.AdjacencyListDigraph), 1, 6, Statistics.GetWrongCount(SysDefine.Statistics.AdjacencyListDigraph));
            ResultItem item5 = new ResultItem(SysDefine.Statistics.DFS_Input, Statistics.isComplete(SysDefine.Statistics.DFS_Input), 1, 5, Statistics.GetWrongCount(SysDefine.Statistics.DFS_Input));
            ResultItem item6 = new ResultItem(SysDefine.Statistics.BFS_Input, Statistics.isComplete(SysDefine.Statistics.BFS_Input), 1, 5, Statistics.GetWrongCount(SysDefine.Statistics.BFS_Input));

            //DJ
            ResultItem item7 = new ResultItem(SysDefine.Statistics.DisInDigraph, Statistics.isComplete(SysDefine.Statistics.DisInDigraph), 1, 12, Statistics.GetWrongCount(SysDefine.Statistics.DisInDigraph));

            //Floyd
            ResultItem item8 = new ResultItem(SysDefine.Statistics.MatrixFilling, Statistics.isComplete(SysDefine.Statistics.MatrixFilling), 1, 3, Statistics.GetWrongCount(SysDefine.Statistics.MatrixFilling));
            ResultItem item9 = new ResultItem(SysDefine.Statistics.ProgramFilling, Statistics.isComplete(SysDefine.Statistics.ProgramFilling), 1, 3, Statistics.GetWrongCount(SysDefine.Statistics.ProgramFilling));
            ResultItem item10 = new ResultItem(SysDefine.Statistics.ProgramRun, Statistics.isComplete(SysDefine.Statistics.ProgramRun), 2, 24, Statistics.GetWrongCount(SysDefine.Statistics.ProgramRun));

            //bellman
            ResultItem item11 = new ResultItem(SysDefine.Statistics.Bellman_Vertex_Input, Statistics.isComplete(SysDefine.Statistics.Bellman_Vertex_Input), 3, 15, Statistics.GetWrongCount(SysDefine.Statistics.Bellman_Vertex_Input));
            ResultItem item12 = new ResultItem(SysDefine.Statistics.Bellman_Dis_Input, Statistics.isComplete(SysDefine.Statistics.Bellman_Dis_Input), 3, 15, Statistics.GetWrongCount(SysDefine.Statistics.Bellman_Dis_Input));

            itemList.Add(item1);
            itemList.Add(item2);
            itemList.Add(item3);
            itemList.Add(item4);
            itemList.Add(item5);
            itemList.Add(item6);
            itemList.Add(item7);
            itemList.Add(item8);
            itemList.Add(item9);
            itemList.Add(item10);
            itemList.Add(item11);
            itemList.Add(item12);
        }

        protected override void OnShow()
        {
            base.OnShow();
            SwitchDisplay(SwitchType.Score);
        }

        /// <summary>
        /// 切换显示
        /// </summary>
        /// <param name="type"></param>
        private void SwitchDisplay(SwitchType type)
        {
            scoreParent.SetActive(type == SwitchType.Score);
            detailParent.SetActive(type == SwitchType.Detail);

            SwitchButton(type);
        }

        /// <summary>
        /// 切换按键效果
        /// </summary>
        /// <param name="type"></param>
        private void SwitchButton(SwitchType type)
        {
            if (type == SwitchType.Score)
            {
                //1 打开 2 关闭
                on1.SetActive(true);
                on2.SetActive(false);
                off1.SetActive(false);
                off2.SetActive(true);

                //修改按钮1和2的文字颜色
                button1Text.color = new Color(252 / 255f, 255 / 255f, 0 / 255, 255 / 255f);
                button2Text.color = new Color(0 / 255f, 245 / 255f, 255 / 255, 255 / 255f);
            }
            else
            {
                //2 打开 1 关闭
                on1.SetActive(false);
                on2.SetActive(true);
                off1.SetActive(true);
                off2.SetActive(false);

                button1Text.color = new Color(0 / 255f, 245 / 255f, 255 / 255, 255 / 255f);
                button2Text.color = new Color(252 / 255f, 255 / 255f, 0 / 255, 255 / 255f);
            }
        }

    }
}