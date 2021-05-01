using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJF
{
    public class LabelControl:MonoBehaviour
    {
        private GameObject[] djCameras = new GameObject[5];
        private GameObject[] flyodCameras = new GameObject[4];
        private GameObject[] bellCameras = new GameObject[7];

        private GameObject labels;
        private GameObject[] djLabels = new GameObject[5];
        private GameObject[] flyodLabels = new GameObject[4];
        private GameObject[] bellManLabels = new GameObject[7];

        private static LabelControl instance;
        public static LabelControl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LabelControl();
                }

                return instance;
            }
        }


        // Use this for initialization
        void Start()
        {
            FindCameras();

            FindLabels();


        }

        // Update is called once per frame
       

        private void FindCameras()
        {
            djCameras[0] = GameObject.Find("target").transform.Find("djMainCamera").gameObject;
            djCameras[1] = GameObject.Find("target").transform.Find("2/2Camera").gameObject;
            djCameras[2] = GameObject.Find("target").transform.Find("3/3Camera").gameObject;
            djCameras[3] = GameObject.Find("target").transform.Find("4/4Camera").gameObject;
            djCameras[4] = GameObject.Find("target").transform.Find("5/5Camera").gameObject;

            flyodCameras[0] = GameObject.Find("cameras").transform.Find("site1").gameObject;
            flyodCameras[1] = GameObject.Find("cameras").transform.Find("site2").gameObject;
            flyodCameras[2] = GameObject.Find("cameras").transform.Find("site3").gameObject;
            flyodCameras[3] = GameObject.Find("cameras").transform.Find("site4").gameObject;

            bellCameras[0] = GameObject.Find("BellmanCameras").transform.Find("cameraA").gameObject;
            bellCameras[1] = GameObject.Find("BellmanCameras").transform.Find("cameraB").gameObject;
            bellCameras[2] = GameObject.Find("BellmanCameras").transform.Find("cameraC").gameObject;
            bellCameras[3] = GameObject.Find("BellmanCameras").transform.Find("cameraD").gameObject;
            bellCameras[4] = GameObject.Find("BellmanCameras").transform.Find("cameraE").gameObject;
            bellCameras[5] = GameObject.Find("BellmanCameras").transform.Find("cameraF").gameObject;
            bellCameras[6] = GameObject.Find("BellmanCameras").transform.Find("cameraG").gameObject;
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

        public  void SwitchLabels()
        {
            GameObject camera = null;
            GameObject findGameObject = FindOnLabel();

            int flag = 0;

            for (int i = 0; i < 5; ++i)
            {
                if (djCameras[i].activeSelf)
                {
                    camera = djCameras[i];

                    if (djLabels[i] != findGameObject)
                    {
                        if (findGameObject != null)
                            findGameObject.SetActive(false);

                        djLabels[i].SetActive(true);
                    }

                    flag = 1;
                    break;
                }
            }

            if (flag == 0)
            {
                for (int i = 0; i < 4; ++i)
                {
                    if (flyodCameras[i].activeSelf)
                    {
                        camera = flyodCameras[i];

                        if (flyodLabels[i] != findGameObject)
                        {
                            if (findGameObject != null)
                                findGameObject.SetActive(false);
                            flyodLabels[i].SetActive(true);
                        }

                        flag = 2;
                        break;
                    }
                }
            }

            if (flag == 0)
            {
                for (int i = 0; i < 7; ++i)
                {
                    if (bellCameras[i].activeSelf)
                    {
                        camera = bellCameras[i];

                        if (bellManLabels[i] != findGameObject)
                        {
                            if (findGameObject != null)
                                findGameObject.SetActive(false);
                            bellManLabels[i].SetActive(true);
                        }

                        break;
                    }
                }
            }
        }

        private GameObject FindOnLabel()
        {
            for (int i = 0; i < 5; ++i)
            {
                if (djLabels[i].activeSelf)
                {
                    return djLabels[i];
                }
            }

            for (int i = 0; i < 4; ++i)
            {
                if (flyodLabels[i].activeSelf)
                {
                    return flyodLabels[i];
                }
            }

            for (int i = 0; i < 7; ++i)
            {
                if (bellManLabels[i].activeSelf)
                {
                    return bellManLabels[i];
                }
            }

            return null;
        }
    }
}

