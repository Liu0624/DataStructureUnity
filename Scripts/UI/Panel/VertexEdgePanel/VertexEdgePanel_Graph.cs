using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using WJF_CodeLibrary.CommonUtility.UI;
using DG.Tweening;

namespace WJF
{
	public partial class VertexEdgePanel
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

        private Coroutine tweenCoroutine;

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
            Transform moveObject = null;

            switch (type)
            {
                case GraphType.Digraph:
                    moveObject = digraphParent.Find("MoveObject");
                    break;
                case GraphType.Undigraph:
                    moveObject = undigraphParent.Find("MoveObject");
                    break;
            }

            //digraphParent.gameObject.SetActive(type == GraphType.Digraph);
            if (type != GraphType.Digraph)
            {
                digraphParent.gameObject.SetActive(false);
            }

            undigraphParent.gameObject.SetActive(type == GraphType.Undigraph);

            if (tweenCoroutine != null)
                StopCoroutine(tweenCoroutine);

            tweenCoroutine = StartCoroutine(MoveTween(moveObject, type));
        }

        /// <summary>
        /// 动画协程
        /// </summary>
        /// <param name="moveObject"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private IEnumerator MoveTween(Transform moveObject, GraphType type)
        {
            moveObject.DOKill();

            float delay = 0.5f;
            float moveTime = 5f;
            float scaleTime = 1.5f;
            Vector3 scaleValue = Vector3.zero;
            float originScaleOffset = 0.001f;
            string[] edges = GetEdges(type);

            Transform firstVertex = moveObject.parent.Find(edges[0][0].ToString());
            moveObject.position = firstVertex.position;
            moveObject.localScale = Vector3.one * originScaleOffset;

            while (true)
            {
                for (int i = 0; i < edges.Length; i++)
                {
                    string v1Name = edges[i][0].ToString();
                    string v2Name = edges[i][1].ToString();
                    Transform v1 = moveObject.parent.Find(v1Name);
                    Transform v2 = moveObject.parent.Find(v2Name);
                    Vector3 dir = v2.position - v1.position;
                    int idx = i;

                    yield return new WaitForSeconds(delay);

                    //移动
                    moveObject.position = v1.position;
                    moveObject.DOMove(v2.position, moveTime).SetEase(Ease.InOutQuad);

                    //缩放
                    //区分飞机和汽车
                    switch (type)
                    {
                        case GraphType.Digraph://飞机
                            moveObject.up = dir;
                            scaleValue = Vector3.one;
                            moveObject.localScale = Vector3.zero;
                            break;
                        case GraphType.Undigraph://汽车
                            bool isGoRight = v2.position.x > v1.position.x;
                            scaleValue = new Vector3(isGoRight ? 1 : -1, 1, 1);
                            moveObject.localScale = scaleValue * originScaleOffset;
                            moveObject.right = dir * (isGoRight ? 1 : -1);
                            break;
                    }

                    //放大
                    moveObject.DOScale(scaleValue, scaleTime);

                    //缩小
                    moveObject.DOScale(scaleValue * originScaleOffset, scaleTime).SetDelay(moveTime - scaleTime);

                    yield return new WaitForSeconds(moveTime);
                }
            }
        }
	}
}