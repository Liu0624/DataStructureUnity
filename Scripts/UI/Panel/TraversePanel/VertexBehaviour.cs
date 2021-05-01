using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using DG.Tweening;

namespace WJF
{
	public class VertexBehaviour : MonoBehaviour
	{
        [HideInInspector]
        public Color clrNormalVertex;//普通状态的顶点颜色
        [HideInInspector]
        public Color clrCurrentVertex;//当前顶点的颜色
        [HideInInspector]
        public Color clrVisitedVertex;//已访问过的顶点颜色

        private Image img;
        private Text text;
        private Dictionary<string, GameObject> arrows;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="clrNormalVertex"></param>
        /// <param name="clrCurrentVertex"></param>
        /// <param name="clrVisitedVertex"></param>
		public void Init(Color clrNormalVertex, Color clrCurrentVertex, Color clrVisitedVertex) 
		{
            this.clrNormalVertex = clrNormalVertex;
            this.clrCurrentVertex = clrCurrentVertex;
            this.clrVisitedVertex = clrVisitedVertex;

            img = GetComponent<Image>();
            text = GetComponentInChildren<Text>();

            //获取对应的边
            arrows = new Dictionary<string, GameObject>();
            Transform edgeParent = transform.parent.GetBrother("Edge");
            foreach (Transform edge in edgeParent)
            {
                if (edge.name[0].ToString() == name)
                {
                    arrows.Add(edge.name[1].ToString(), edge.gameObject);
                }
            }
		}

        /// <summary>
        /// 设置顶点颜色
        /// </summary>
        /// <param name="state"></param>
        public void SetColor(GraphVertex.VertexState state)
        {
            Color imgColor;
            Color textColor;

            switch (state)
            {
                case GraphVertex.VertexState.Normal:
                    imgColor = clrNormalVertex;
                    textColor = Color.black;
                    break;
                case GraphVertex.VertexState.Current:
                    imgColor = clrCurrentVertex;
                    textColor = Color.black;
                    break;
                case GraphVertex.VertexState.Visited:
                    imgColor = clrVisitedVertex;
                    textColor = Color.white;
                    break;
                default:
                    imgColor = Color.clear;
                    textColor = Color.clear;
                    break;
            }

            img.DOColor(imgColor, 0.5f).SetEase(Ease.Linear);
            text.DOColor(textColor, 0.5f).SetEase(Ease.Linear);
        }
	}
}