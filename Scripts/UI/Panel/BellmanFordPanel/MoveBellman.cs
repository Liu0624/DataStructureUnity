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
    public partial class BellmanFordPanel
    {
        GameObject labels;
        GameObject[] djLabels = new GameObject[5];
        GameObject[] flyodLabels = new GameObject[4];
        GameObject[] bellManLabels = new GameObject[7];

        GameObject moxing;

        //这里是djpanel下的几个相机，初始化时候需要先隐藏
        public GameObject camera21, camera22, camera23, camera24, camera25;

        //floyd下的四个相机
        public GameObject camera31, camera32, camera33, camera34;

        //bellman下的7个相机
        GameObject[] cameras = new GameObject[7];
        public static string[] housesName = new string[] { "A", "B", "C", "D", "E", "F", "G" };
        GameObject[] houses = new GameObject[7];


        //4只羊
        GameObject sheep1;
        GameObject sheep2;
        GameObject sheep3;
        GameObject sheep4;

        //四个羊的动画控制器
        Animator s1;
        Animator s2;
        Animator s3;
        Animator s4;

        private Transform jieshao1,tipHint;
        private Button closed;


        // Use this for initialization
        private void InitBellmanCarmeras()
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

            GameObject house = GameObject.Find("houses");
            houses[0] = house.transform.Find("houseA").gameObject;
            houses[1] = house.transform.Find("houseB").gameObject;
            houses[2] = house.transform.Find("houseC").gameObject;
            houses[3] = house.transform.Find("houseD").gameObject;
            houses[4] = house.transform.Find("houseE").gameObject;
            houses[5] = house.transform.Find("houseF").gameObject;
            houses[6] = house.transform.Find("houseG").gameObject;

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


            SetHouseNotEnable();
            chooseBellmanCamera(cameras[0],bellManLabels[0]);

        }

        private void initTipText(){
            jieshao1 = transform.Find("jieshaobg");
            closed = jieshao1.gameObject.GetComponentInChildrenByName<Button>("closebtn");

            jieshao1.gameObject.SetActive(true);

            closed.onClick.AddListener(() =>
            {
                jieshao1.gameObject.SetActive(false);
            });

        }


        //void Update()
        //{
            
        //    BellManQuit();
        //}

        public void BellManQuit()
        {
            PanelActivator.MessageBox("返回后本模块学习记录将清空，确认返回吗？", () =>
            {
                Statistics.setScore(quitWrongVI, SysDefine.Statistics.Bellman_Vertex_Input);
                Statistics.setScore(quitWrongDI, SysDefine.Statistics.Bellman_Dis_Input);

                SetCamerasNotEnable();
                ShowPanel<MainPanel>();
            }, () => { });
        }



        public void Control()
        {
            if (Input.GetKey(KeyCode.A))
            {
                chooseBellmanCamera(cameras[0], bellManLabels[0]);

                Invoke("CreateHouse", 0.01f);
            }
            if (Input.GetKey(KeyCode.B))
            {
                chooseBellmanCamera(cameras[1], bellManLabels[1]);

                Invoke("CreateHouse", 0.1f);
            }
            if (Input.GetKey(KeyCode.C))
            {
                chooseBellmanCamera(cameras[2], bellManLabels[2]);

                Invoke("CreateHouse", 0.1f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                chooseBellmanCamera(cameras[3], bellManLabels[3]);

                Invoke("CreateHouse", 0.1f);
            }
            if (Input.GetKey(KeyCode.E))
            {
                chooseBellmanCamera(cameras[4], bellManLabels[4]);

                Invoke("CreateHouse", 0.1f);
            }
            if (Input.GetKey(KeyCode.F))
            {
                chooseBellmanCamera(cameras[5], bellManLabels[5]);

                Invoke("CreateHouse", 0.1f);
            }
            if (Input.GetKey(KeyCode.G))
            {
                chooseBellmanCamera(cameras[6], bellManLabels[6]);

                Invoke("CreateHouse", 0.1f);
            }

     
        }


        private void SetCamerasNotEnable()
        {
            foreach (var camera in cameras)
            {
                camera.SetActive(false);
            }


        }

        private void SetHouseNotEnable()
        {
            for (int i = 1; i < 7; ++i)
            {
                houses[i].SetActive(false);
            }
        }

        private void CreateHouse()
        {
            string nameHouse = "";

            for (int i = 0; i < 7; ++i)
            {
                if (cameras[i].activeSelf == true)
                {
                    nameHouse = housesName[i];
                    break;
                }
            }

            if (nameHouse != "" && GameObject.Find("houses").transform.Find(nameHouse) == null)
            {
                switch (nameHouse)
                {
                    case "A":
                        houses[0].SetActive(true);
                        break;
                    case "B":
                        houses[1].SetActive(true);
                        break;
                    case "C":
                        houses[2].SetActive(true);
                        break;
                    case "D":
                        houses[3].SetActive(true);
                        break;
                    case "E":
                        houses[4].SetActive(true);
                        break;
                    case "F":
                        houses[5].SetActive(true);
                        break;
                    case "G":
                        houses[6].SetActive(true);
                        break;
                }
            }
        }

        private void chooseBellmanCamera(GameObject camera,GameObject label)
        {
            SetCamerasNotEnable();
            camera.SetActive(true);

            for(int i =0; i < 7; ++i)
            {
                bellManLabels[i].SetActive(false);
            }

            label.SetActive(true);
        }
    }
}