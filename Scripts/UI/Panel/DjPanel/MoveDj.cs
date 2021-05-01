using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.CommonUtility.UI;
using WJF_CodeLibrary.CommonUtility.Sort;
using DG.Tweening;
using System.Text.RegularExpressions;
using System;
using System.Text;
using WJF_CodeLibrary.CommonUtility;


namespace WJF
{
    public partial class DjPanel
    {
        GameObject labels;
        GameObject[] djLabels = new GameObject[5];
        GameObject[] flyodLabels = new GameObject[4];
        GameObject[] bellManLabels = new GameObject[7];

        //这里是djpanel下的几个相机，初始化时候需要先隐藏
        public GameObject camera21, camera22, camera23, camera24, camera25;

        //floyd下的四个相机
        public GameObject camera31, camera32, camera33, camera34;

        //bellman下的7个镜头
        GameObject[] cameras = new GameObject[7];

        ////4只羊
        //GameObject sheep1;
        //GameObject sheep2;
        //GameObject sheep3;
        //GameObject sheep4;

        //四个羊的动画控制器
        Animator s1;
        Animator s2;
        Animator s3;
        Animator s4;

        private Transform jieshao1;

        private Button closed;
        private TextMesh text1, text2, text3, text4, text5;


        public void DjQuit()
        {
            PanelActivator.MessageBox("返回后本模块学习记录将清空，确认返回吗？", () =>
            {
                Statistics.setScore(quitWrong, SysDefine.Statistics.DisInDigraph);
                SetCamerasNotEnable();
                ShowPanel<MainPanel>();
            }, () => { });
        }

        // Use this for initialization
        private void InitDjCarmeras()
        {
            camera21 = GameObject.Find("target").transform.Find("djMainCamera").gameObject;
            camera22 = GameObject.Find("target").transform.Find("2/2Camera").gameObject;
            camera23 = GameObject.Find("target").transform.Find("3/3Camera").gameObject;
            camera24 = GameObject.Find("target").transform.Find("4/4Camera").gameObject;
            camera25 = GameObject.Find("target").transform.Find("5/5Camera").gameObject;

            camera31 = GameObject.Find("cameras").transform.Find("site1").gameObject;
            camera32 = GameObject.Find("cameras").transform.Find("site2").gameObject;
            camera33 = GameObject.Find("cameras").transform.Find("site3").gameObject;
            camera34 = GameObject.Find("cameras").transform.Find("site4").gameObject;

            cameras[0] = GameObject.Find("BellmanCameras").transform.Find("cameraA").gameObject;
            cameras[1] = GameObject.Find("BellmanCameras").transform.Find("cameraB").gameObject;
            cameras[2] = GameObject.Find("BellmanCameras").transform.Find("cameraC").gameObject;
            cameras[3] = GameObject.Find("BellmanCameras").transform.Find("cameraD").gameObject;
            cameras[4] = GameObject.Find("BellmanCameras").transform.Find("cameraE").gameObject;
            cameras[5] = GameObject.Find("BellmanCameras").transform.Find("cameraF").gameObject;
            cameras[6] = GameObject.Find("BellmanCameras").transform.Find("cameraG").gameObject;

            InitLabels();

            SetCamerasNotEnable();

            chooseCamera(camera21, djLabels[0]);


            initText();

            CallbackUtility.ExecuteDelay(0f, () =>
            {
                //显示初始提示tip1弹框
                jieshao1.gameObject.SetActive(true);
            });

        }

        private void InitLabels()
        {
            FindLabels();
        }

        private void initText()
        {
            jieshao1 = transform.Find("jieshaobg");
            closed = jieshao1.gameObject.GetComponentInChildrenByName<Button>("closebtn");

            closed.onClick.AddListener(() =>
            {
                jieshao1.gameObject.SetActive(false);
                PanelActivator.MessageBox(DjText.tip2);
            });


            text5 = GameObject.Find("target").transform.Find("5").gameObject.GetComponentInChildrenByName<TextMesh>("5site");

        }

        /// <summary>
        /// 将相机全部关闭
        /// </summary>
        private void SetCamerasNotEnable()
        {
            foreach (var camera in cameras)
            {
                camera.SetActive(false);
            }

            camera21.SetActive(false);
            camera22.SetActive(false);
            camera23.SetActive(false);
            camera24.SetActive(false);
            camera25.SetActive(false);

            camera31.SetActive(false);
            camera32.SetActive(false);
            camera33.SetActive(false);
            camera34.SetActive(false);

        }

        /// <summary>
        /// 将某个相机激活
        /// </summary>
        /// <param name="camera"></param>
        private void chooseCamera(GameObject camera, GameObject label)
        {
            camera21.SetActive(false);
            camera22.SetActive(false);
            camera23.SetActive(false);
            camera24.SetActive(false);
            camera25.SetActive(false);
            camera.SetActive(true);

            for (int i = 0; i < 5; ++i)
                djLabels[i].SetActive(false);

            label.SetActive(true);
        }

        private void FindLabels()
        {
            labels = GameObject.Find("Labels");

            djLabels[0] = labels.transform.Find("LabelsDj").gameObject;
            djLabels[1] = labels.transform.Find("LabelDj2").gameObject;
            djLabels[2] = labels.transform.Find("LabelDj3").gameObject;
            djLabels[3] = labels.transform.Find("LabelDj4").gameObject;
            djLabels[4] = labels.transform.Find("LabelDj5").gameObject;

            djLabels[0].SetActive(false);
            djLabels[1].SetActive(false);
            djLabels[2].SetActive(false);
            djLabels[3].SetActive(false);
            djLabels[4].SetActive(false);

            flyodLabels[0] = labels.transform.Find("Floyd1").gameObject;
            flyodLabels[1] = labels.transform.Find("Floyd2").gameObject;
            flyodLabels[2] = labels.transform.Find("Floyd3").gameObject;
            flyodLabels[3] = labels.transform.Find("Floyd4").gameObject;

            flyodLabels[0].SetActive(false);
            flyodLabels[1].SetActive(false);
            flyodLabels[2].SetActive(false);
            flyodLabels[3].SetActive(false);

            bellManLabels[0] = labels.transform.Find("BellMan1").gameObject;
            bellManLabels[1] = labels.transform.Find("BellMan2").gameObject;
            bellManLabels[2] = labels.transform.Find("BellMan3").gameObject;
            bellManLabels[3] = labels.transform.Find("BellMan4").gameObject;
            bellManLabels[4] = labels.transform.Find("BellMan5").gameObject;
            bellManLabels[5] = labels.transform.Find("BellMan6").gameObject;
            bellManLabels[6] = labels.transform.Find("BellMan7").gameObject;

            bellManLabels[0].SetActive(false);
            bellManLabels[1].SetActive(false);
            bellManLabels[2].SetActive(false);
            bellManLabels[3].SetActive(false);
            bellManLabels[4].SetActive(false);
        }
    }
}