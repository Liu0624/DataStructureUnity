using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.UIFramework;

namespace WJF
{

    // 实验结果分析Itme
    public class ResultItem
    {

        public string name;//名字

        public double scoreWeight;//错误次数对应的分数比例
        public int wrongCount;//错误次数

        public string status;//答题状态
        public double score;//分数

        public string scoreText;//分值显示

        public int totalScore;//总分数
        public string tip;//描述

        public bool isComplete;//是否填写完成此步骤，如果直接跳过填写看结果此处是false

        //默认的初始化分数提示
        public string defaltScoreText;

        public string defaltStatus;

        public string defaltTip;

        public ResultItem(string name,bool complete, double scoreWeight,  int totalScore ,int wrongCount)
        {
            this.name = name;
            this.isComplete = complete;
            this.wrongCount = wrongCount;
            this.scoreWeight = scoreWeight;
            this.totalScore = totalScore;

            initData();
        }

        private void initData()
        {
            //计算得到的分数
            score = totalScore - wrongCount * scoreWeight;

            if (score < 0) score = 0;

            scoreText = "得分: " + score + "\n总分: " + totalScore;
            
            //设置默认的值，当用户没有做题直接看结果时候要展示默认设置
            defaltScoreText = "得分: 0\n总分: " + totalScore;
            defaltStatus = "错误";
            defaltTip = "未完成";

            //计算答题状态
            if (score == totalScore)
            {
                status = "正确";
            }
            else if (score == 0)
            {
                status = "错误";
            }
            else
            {
                status = "部分正确";
            }

            //计算描述
            tip = "答错" + wrongCount + "次";
        }
    }

}