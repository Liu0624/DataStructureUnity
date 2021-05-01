using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.CommonUtility.UI;
using WJF_CodeLibrary.CommonUtility.Sort;
using DG.Tweening;
using System.Text.RegularExpressions;


namespace WJF
{
    public partial class FloydPanel
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

        //bellman下的7个镜头
        GameObject[] cameras = new GameObject[7];

        //文案打印每两个字直接出现的的速度
        private const float tweenDuration = 0.5f;

        private Text tipHintText;

        //4只羊
        GameObject sheep1;
        GameObject sheep2;
        GameObject sheep3;
        GameObject sheep4;
        GameObject sheep5;
        GameObject sheep6;
        GameObject sheep7;
        GameObject sheep8;

        //四个羊的动画控制器
        Animator s1;
        Animator s2;
        Animator s3;
        Animator s4;
        Animator s5;
        Animator s6;
        Animator s7;
        Animator s8;

        private Transform jieshao1,tipHint;
        private Button closed;

        //void Update()
        //{
        //    if (Input.GetKey(KeyCode.Q))
        //    {
        //        FloydQuit();
        //    }
        //}

        public void FloydQuit()
        {
            PanelActivator.MessageBox("返回后本模块学习记录将清空，确认返回吗？", () =>
            {
                Statistics.setScore(quitWrongMat, SysDefine.Statistics.MatrixFilling);
                Statistics.setScore(quitWrongFill, SysDefine.Statistics.ProgramFilling);
                Statistics.setScore(quitWrongRun, SysDefine.Statistics.ProgramRun);

                SetCamerasNotEnable();
                ShowPanel<MainPanel>();
            }, () => { });
        }


        // Use this for initialization
        private void InitCarmeras()
        {
            camera21 = GameObject.Find("target").transform.Find("djMainCamera").gameObject;
            camera22 = GameObject.Find("target").transform.Find("2/2Camera").gameObject;
            camera23 = GameObject.Find("target").transform.Find("3/3Camera").gameObject;
            camera24 = GameObject.Find("target").transform.Find("4/4Camera").gameObject;
            camera25 = GameObject.Find("target").transform.Find("5/5Camera").gameObject;

            cameras[0] = GameObject.Find("BellmanCameras").transform.Find("cameraA").gameObject;
            cameras[1] = GameObject.Find("BellmanCameras").transform.Find("cameraB").gameObject;
            cameras[2] = GameObject.Find("BellmanCameras").transform.Find("cameraC").gameObject;
            cameras[3] = GameObject.Find("BellmanCameras").transform.Find("cameraD").gameObject;
            cameras[4] = GameObject.Find("BellmanCameras").transform.Find("cameraE").gameObject;
            cameras[5] = GameObject.Find("BellmanCameras").transform.Find("cameraF").gameObject;
            cameras[6] = GameObject.Find("BellmanCameras").transform.Find("cameraG").gameObject;

            camera31 = GameObject.Find("cameras").transform.Find("site1").gameObject;
            camera32 = GameObject.Find("cameras").transform.Find("site2").gameObject;
            camera33 = GameObject.Find("cameras").transform.Find("site3").gameObject;
            camera34 = GameObject.Find("cameras").transform.Find("site4").gameObject;


            moxing = GameObject.Find("moxing");
            sheep1 = moxing.transform.Find("sheep1").gameObject;
            sheep2 = moxing.transform.Find("sheep2").gameObject;
            sheep3 = moxing.transform.Find("sheep3").gameObject;
            sheep4 = moxing.transform.Find("sheep4").gameObject;
            sheep5 = moxing.transform.Find("sheep5").gameObject;
            sheep6 = moxing.transform.Find("sheep6").gameObject;
            sheep7 = moxing.transform.Find("sheep7").gameObject;
            sheep8 = moxing.transform.Find("sheep8").gameObject;


            s1 = sheep1.GetComponent<Animator>();
            s2 = sheep2.GetComponent<Animator>();
            s3 = sheep3.GetComponent<Animator>();
            s4 = sheep4.GetComponent<Animator>();
            s5 = sheep5.GetComponent<Animator>();
            s6 = sheep6.GetComponent<Animator>();
            s7 = sheep7.GetComponent<Animator>();
            s8 = sheep8.GetComponent<Animator>();

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

            SetCamerasNotEnable();

            chooseCamera(camera31,flyodLabels[0]);
        }


        private void initTipText(){
            tipHint = transform.Find("TipHint");
            tipHintText = tipHint.gameObject.GetComponentInChildrenByName<Text>("TipText");
        
            jieshao1 = transform.Find("jieshaobg");
            closed = jieshao1.gameObject.GetComponentInChildrenByName<Button>("closebtn");

            jieshao1.gameObject.SetActive(true);
            tipHint.gameObject.SetActive(false);

            closed.onClick.AddListener(() =>
            {
                jieshao1.gameObject.SetActive(false);
                tipHint.gameObject.SetActive(true);
            });

        }


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



        #region 移动位置
        private void MoveToSite1()
        {
            sheep1.transform.localPosition = new Vector3(-2246.353f, -230.7f, 18101.35f);
            sheep1.transform.localRotation = Quaternion.Euler(-1.474f, -61.793f, 7.893001f);

            sheep2.transform.localPosition = new Vector3(-2227.009f, -220f, 18182.44f);
            sheep2.transform.localRotation = Quaternion.Euler(-4.924f, -81.562f, -1.593f);

            sheep3.transform.localPosition = new Vector3(-2276.269f, -219.4f, 18201.29f);
            sheep3.transform.localRotation = Quaternion.Euler(-15.539f, -33.06f, 4.358f);

            sheep4.transform.localPosition = new Vector3(-2366.19f, -196.3f, 18241.61f);
            sheep4.transform.localRotation = Quaternion.Euler(-4.992f, -223.754f, 7.893001f);

            sheep5.transform.localPosition = new Vector3(-2430.268f, -193.8999f, 18218.98f);
            sheep5.transform.localRotation = Quaternion.Euler(- 4.733f, -231.649f, 3.683f);

            sheep6.transform.localPosition = new Vector3(-2423.509f, -199.9499f, 18279.71f);
            sheep6.transform.localRotation = Quaternion.Euler(5.946f, -292.912f, -0.528f);

            sheep7.transform.localPosition = new Vector3(-2439.343f, -197.8828f, 18241.04f);
            sheep7.transform.localRotation = Quaternion.Euler(-5.497f, -271.642f, 4.586f);

            sheep8.transform.localPosition = new Vector3(-2389.059f, -199.5939f, 18270.88f);
            sheep8.transform.localRotation = Quaternion.Euler(-2.996f, -262.942f, -0.765f);

            chooseCamera(camera31,flyodLabels[0]);
        }

        private void MoveToSite2()
        {
            sheep1.transform.localPosition = new Vector3(-8937f, -258.5f, 7685.9f);
            sheep1.transform.localRotation = Quaternion.Euler(3.478f, -410.243f, 2.707f);

            sheep2.transform.localPosition = new Vector3(-8880.8f, -255.5f, 7668.4f);
            sheep2.transform.localRotation = Quaternion.Euler(-1.524f, -414.355f, -1.487f);

            sheep3.transform.localPosition = new Vector3(-8936f, -249.8f, 7623.6f);
            sheep3.transform.localRotation = Quaternion.Euler(1.86f, -53.251f, -2.933f);

            sheep4.transform.localPosition = new Vector3(-8985.4f, -254f, 7651.8f);
            sheep4.transform.localRotation = Quaternion.Euler(0.418f, -88.826f, -0.626f);

            sheep5.transform.localPosition = new Vector3(-8988.3f, -240f, 7577.8f);
            sheep5.transform.localRotation = Quaternion.Euler(1.412f, -438.607f, -5.756f);

            sheep6.transform.localPosition = new Vector3(-8869f, -258.9f, 7699.4f);
            sheep6.transform.localRotation = Quaternion.Euler(3.838f, -10.251f, -0.924f);

            sheep7.transform.localPosition = new Vector3(-8995.8f, -245.9f, 7604.7f);
            sheep7.transform.localRotation = Quaternion.Euler(-5.065f, -432.955f, -6.092f);

            sheep8.transform.localPosition = new Vector3(-8877f, -238.7f, 7583.1f);
            sheep8.transform.localRotation = Quaternion.Euler(7.555f, -421.08f, -0.424f);

            chooseCamera(camera32, flyodLabels[1]);

           PanelActivator.MessageBox(FloydText.ltal2);
        }

        private void MoveToSite3()
        {
            sheep1.transform.localPosition = new Vector3(302.2f, -36.3f, -1658.3f);
            sheep1.transform.localRotation = Quaternion.Euler(2.872f, -160.43f, -0.037f);

            sheep2.transform.localPosition = new Vector3(352.2f, -40.6f, -1622.2f);
            sheep2.transform.localRotation = Quaternion.Euler(-0.345f, -539.34f, 2.101f);

            sheep3.transform.localPosition = new Vector3(319.1f, -35.2f, -1726.6f);
            sheep3.transform.localRotation = Quaternion.Euler(-3.248f, -514.892f, -1.227f);

            sheep4.transform.localPosition = new Vector3(305f, -38.2f, -1624.4f);
            sheep4.transform.localRotation = Quaternion.Euler(-0.388f, -153.589f, -0.645f);

            sheep5.transform.localPosition = new Vector3(343f, -39.3f, -1658.8f);
            sheep5.transform.localRotation = Quaternion.Euler(-3.336f, -191.693f, 0.962f);

            sheep6.transform.localPosition = new Vector3(283.4f, -38.2f, -1672.8f);
            sheep6.transform.localRotation = Quaternion.Euler(-1.581f, -112.67f, -3.091f);

            sheep7.transform.localPosition = new Vector3(282.6f, -34.1f, -1728.2f);
            sheep7.transform.localRotation = Quaternion.Euler(-2.436f, -490.124f, -2.475f);

            sheep8.transform.localPosition = new Vector3(337.8f, -38.6f, -1589.6f);
            sheep8.transform.localRotation = Quaternion.Euler(4.901f, -510.048f, -1.512f);

            chooseCamera(camera33, flyodLabels[2]);
            PanelActivator.MessageBox(FloydText.ltal3);
        }

        private void MoveToSite4()
        {
            sheep1.transform.localPosition = new Vector3(11702.4f, -151.5f, 11189.7f);
            sheep1.transform.localRotation = Quaternion.Euler(2.309f, -196.274f, -1.709f);

            sheep2.transform.localPosition = new Vector3(11756.3f, -156f, 11208.6f);
            sheep2.transform.localRotation = Quaternion.Euler(-3.527f, -532.843f, 2.051f);

            sheep3.transform.localPosition = new Vector3(11770f, -149.1f, 11144.8f);
            sheep3.transform.localRotation = Quaternion.Euler(-3.464f, -539.565f, 0.24f);

            sheep4.transform.localPosition = new Vector3(11720.4f, -147f, 11118.2f);
            sheep4.transform.localRotation = Quaternion.Euler(-5.21f, -169.951f, -0.511f);

            sheep5.transform.localPosition = new Vector3(11774f, -139.5f, 11076.2f);
            sheep5.transform.localRotation = Quaternion.Euler(-5.847f, -191.736f, 0.965f);

            sheep6.transform.localPosition = new Vector3(11782.7f, -133.4f, 11035.4f);
            sheep6.transform.localRotation = Quaternion.Euler(-5.887f, -183.693f, 0.489f);

            sheep7.transform.localPosition = new Vector3(11834.4f, -142.9f, 11151.5f);
            sheep7.transform.localRotation = Quaternion.Euler(-5.755f, -553.286f, 2.344f);

            sheep8.transform.localPosition = new Vector3(11823.9f, -136.6f, 11098.2f);
            sheep8.transform.localRotation = Quaternion.Euler(-4.642f, -533.252f, -3.357f);

            chooseCamera(camera34, flyodLabels[3]);
            PanelActivator.MessageBox(FloydText.ltal4);
        }

        #endregion

        private void Run()
        {
            sheep1.transform.Translate(Vector3.forward * Time.deltaTime * 20);
            sheep2.transform.Translate(Vector3.forward * Time.deltaTime * 20);
            sheep3.transform.Translate(Vector3.forward * Time.deltaTime * 20);
            sheep4.transform.Translate(Vector3.forward * Time.deltaTime * 20);
            sheep5.transform.Translate(Vector3.forward * Time.deltaTime * 20);
            sheep6.transform.Translate(Vector3.forward * Time.deltaTime * 20);
            sheep7.transform.Translate(Vector3.forward * Time.deltaTime * 20);
            sheep8.transform.Translate(Vector3.forward * Time.deltaTime * 20);

            s1.Play("run", 0, 0);
            s2.Play("run", 0, 0);
            s3.Play("run", 0, 0);
            s4.Play("run", 0, 0);
            s5.Play("run", 0, 0);
            s6.Play("run", 0, 0);
            s7.Play("run", 0, 0);
            s8.Play("run", 0, 0);
        }

        private void IdlePlay(Animator a)
        {
            a.Play("idle", 0, 0);
        }

        private void EatPlay(Animator a)
        {
            a.Play("eat", 0, 0);
        }

        private void SetAnimator(bool sheep1, bool sheep2, bool sheep3, bool sheep4)
        {
            s1.enabled = sheep1;
            s2.enabled = sheep2;
            s3.enabled = sheep3;
            s4.enabled = sheep4;
        }

        private void chooseCamera(GameObject camera, GameObject label)
        {
            camera31.SetActive(false);
            camera32.SetActive(false);
            camera33.SetActive(false);
            camera34.SetActive(false);
            camera.SetActive(true);

            for(int i = 0; i < 4; ++i)
            {
                flyodLabels[i].SetActive(false);
            }

            label.SetActive(true);
        }


        /// <summary>
        /// 设置步骤提示
        /// </summary>
        public void SetStepHint(string value)
        {
            tipHintText.text = "";

            tipHintText.DOKill();
            tipHintText.DOText(value, tweenDuration).SetEase(Ease.Linear);
        }
    }
}