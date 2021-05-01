using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WJF_CodeLibrary.Extension;
using UnityEngine.UI;
using System;
using DG.Tweening;
using WJF_CodeLibrary.CommonUtility;

namespace WJF
{
	public class AdjacencyItem : MonoBehaviour
	{	
        public enum ItemType { Vertex, Adjacency }//节点类型：顶点、邻接点
        public ItemType type;

        [HideInInspector]
        public AdjacencyItem rear;//前一个
        [HideInInspector]
        public AdjacencyItem next;//下一个
        [HideInInspector]
        public GameObject arrowGo;//箭头物体

        private Button btnAdd;
        private Button btnRemove;
        private InputField inputNum;
        private Text textWeight;

        private GameObject adjacencyPattern;
        private GameObject arrowPattern;

        private AdjacencyGroup group;

        private bool isHoverIn;//标记是否鼠标悬浮在物体上

        public int num { private set; get; }
        public string vertexName { private set; get; }
        public int weight { private set; get; }
        public bool validation { private set; get; }

        private const float itemInterval = 100f;
        private const float originAdjPosX = 120f;
        private const float originArrowPosX = 80f;

        private readonly Color clrWrong = new Color(255 / 255f, 160 / 255f, 160 / 255f);

        private int MaxVerticesCount
        {
            get
            {
                return AdjacencyInfo.Instance.vertices.Count;
            }
        }
        
        private void Start() 
		{
            btnAdd = gameObject.GetComponentInChildrenByName<Button>("Add");
            btnRemove = gameObject.GetComponentInChildrenByName<Button>("Remove");

            adjacencyPattern = Storage.GetPattern("AdjacencyPattern");
            arrowPattern = Storage.GetPattern("ArrowPattern");

            group = GetComponentInParent<AdjacencyGroup>();

            //区分设置
            switch (type)
            {
                case ItemType.Vertex:
                    string numStr = gameObject.GetComponentInChildrenByName<Text>("Num").text;
                    num = int.Parse(numStr);
                    vertexName = transform.Find("VertexName").GetComponentInChildren<Text>().text;
                    group.Add(this);
                    break;
                case ItemType.Adjacency:
                    textWeight = transform.Find("Weight").GetComponentInChildren<Text>();
                    inputNum = gameObject.GetComponentInChildrenByName<InputField>("Num");
                    inputNum.onValueChanged.AddListener(OnAdjNumValueChanged);
                    num = -1;
                    weight = -1;
                    break;
            }

            next = null;
            validation = false;

            btnAdd.gameObject.SetActive(false);
            btnRemove.gameObject.SetActive(false);

            btnAdd.onClick.AddListener(OnAdd);
            btnRemove.onClick.AddListener(OnRemove);

            EventTriggerListener.Get(gameObject).onEnter = OnHoverIn;
            EventTriggerListener.Get(gameObject).onExit = OnHoverOut;
        }

        /// <summary>
        /// 设置标记邻接点的文本框
        /// </summary>
        /// <param name="isEmpty"></param>
        public void SetNextText(bool isEmpty)
        {
            transform.Find("Next").GetComponentInChildren<Text>().text = isEmpty ? "" : "^";
        }

        /// <summary>
        /// 开关输入
        /// </summary>
        /// <param name="state"></param>
        public void ToggleInput(bool state)
        {
            if (type == ItemType.Vertex)
                return;

            inputNum.interactable = state;
        }

        /// <summary>
        /// 添加节点事件
        /// </summary>
        private void OnAdd()
        {
            //修改当前的next节点显示
            SetNextText(true);

            //禁用当前邻接点的输入
            ToggleInput(false);

            //隐藏添加删除按键
            btnAdd.gameObject.SetActive(false);
            btnRemove.gameObject.SetActive(false);

            //生成箭头
            GameObject arrow = Instantiate(arrowPattern);
            arrow.transform.SetParent(transform.parent, false);

            float arrowPatternPosX = arrowPattern.GetComponent<RectTransform>().anchoredPosition.x;
            float arrowPosX = originArrowPosX + itemInterval * (group.GetItemsCount() - 1);
            arrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPosX, 0f);

            arrow.SetActive(true);

            //最后生成邻接点
            GameObject adjItemGo = Instantiate(adjacencyPattern);

            AdjacencyItem adjItem = adjItemGo.GetComponent<AdjacencyItem>();
            next = adjItem;
            adjItem.rear = this;
            adjItem.arrowGo = arrow;

            adjItemGo.transform.SetParent(transform.parent, false);

            float adjPatternPosX = adjacencyPattern.GetComponent<RectTransform>().anchoredPosition.x;
            float adjItemPosX = originAdjPosX + itemInterval * (group.GetItemsCount() - 1);
            adjItemGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(adjItemPosX, 0f);

            adjItemGo.SetActive(true);

            //添加到组
            group.Add(adjItem);
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        private void OnRemove()
        {
            //显示上一个next节点为空
            rear.SetNextText(false);

            //开启上一个邻接点的输入
            rear.ToggleInput(true);

            group.Remove(this);
            Destroy(arrowGo);
            Destroy(gameObject);
        }

        /// <summary>
        /// 悬浮事件
        /// </summary>
        /// <param name="go"></param>
        private void OnHoverIn(GameObject go)
        {
            //如果有下一个则不显示
            if (next != null)
                return;
            
            isHoverIn = true;

            //只有第一个不显示删除
            if (type != ItemType.Vertex)
                btnRemove.gameObject.SetActive(true);

            //控制添加按钮的显示
            //如果是邻接点，则判断对错
            if (type == ItemType.Adjacency)
            {
                if (!validation)
                {
                    btnAdd.gameObject.SetActive(false);
                    return;
                }                    
            }

            //不能超过顶点个数
            if (group.GetItemsCount() != MaxVerticesCount)
                btnAdd.gameObject.SetActive(true);
        }

        /// <summary>
        /// 退出悬浮
        /// </summary>
        /// <param name="go"></param>
        private void OnHoverOut(GameObject go)
        {
            btnAdd.gameObject.SetActive(false);
            btnRemove.gameObject.SetActive(false);

            isHoverIn = false;
        }

        /// <summary>
        /// 输入事件
        /// </summary>
        /// <param name="value"></param>
        private void OnAdjNumValueChanged(string value)
        {
            if (value == string.Empty)
            {
                weight = -1;
                num = -1;
                validation = false;
                textWeight.text = "";
                HighlightWrongOff();
                OnHoverIn(null);
            }
            else
            {
                int inputNum = int.Parse(value);
                weight = AdjacencyInfo.Instance.GetWeight(group.GetRootVertexNum(), inputNum);
                textWeight.text = weight.ToString();

                //如果输入错误 ||　当前正确的已输入过，则错误
                if (weight == -1 || (num != inputNum && group.ContainsAdjNode(inputNum)))
                {
                    validation = false;
                    HighlightWrongOn();
                    num = -1;
                }
                else
                {
                    validation = true;
                    HighlightWrongOff();
                    num = inputNum;
                }

                //如果悬浮状态，则刷新
                if (isHoverIn)
                {
                    OnHoverIn(null);
                }
            }
        }
                
        /// <summary>
        /// 高亮错误
        /// </summary>
        private void HighlightWrongOn()
        {
            HighlightWrongOff();

            Image inputWrong = inputNum.gameObject.GetComponentInChildrenByName<Image>("WrongHint");
            Image weightWrong = textWeight.gameObject.GetBrother("WrongHint").GetComponent<Image>();
            float targetFade = 0.8f;
            float duration = 0.25f;

            inputWrong.DOFade(targetFade, duration ).SetEase(Ease.Linear).SetLoops(3, LoopType.Yoyo);
            weightWrong.DOFade(targetFade, duration).SetEase(Ease.Linear).SetLoops(3, LoopType.Yoyo);
        }

        /// <summary>
        /// 关闭高亮
        /// </summary>
        private void HighlightWrongOff()
        {
            Image inputWrong = inputNum.gameObject.GetComponentInChildrenByName<Image>("WrongHint");
            Image weightWrong = textWeight.gameObject.GetBrother("WrongHint").GetComponent<Image>();

            inputWrong.color = ColorCalculateUtility.GetFadeColor(inputWrong.color, 0f);
            weightWrong.color = ColorCalculateUtility.GetFadeColor(weightWrong.color, 0f);
        }
    }
}