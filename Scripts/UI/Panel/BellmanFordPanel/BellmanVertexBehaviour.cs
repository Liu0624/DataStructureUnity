using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using WJF_CodeLibrary.Extension;
using DG.Tweening;

namespace WJF
{
    public class BellmanVertexBehaviour : MonoBehaviour
    {
        [HideInInspector]
        public Color clrNormalVertex = Color.yellow;//普通状态的顶点颜色
        [HideInInspector]
        public Color clrCurrentVertex = Color.yellow;//当前顶点的颜色
        [HideInInspector]
        public Color clrVisitedVertex = Color.white;//已访问过的顶点颜色

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
        public void SetColor(BellmanGraphVertex.VertexState state)
        {
            //Color imgColor;
            Color textColor;

            switch (state)
            {
                case BellmanGraphVertex.VertexState.Normal:
                    //imgColor = clrNormalVertex;
                    textColor = Color.yellow;
                    Debug.Log(clrNormalVertex + "+++++");
                    break;
                case BellmanGraphVertex.VertexState.Current:
                    //imgColor = clrCurrentVertex;
                    textColor = Color.yellow;
                    Debug.Log(clrNormalVertex + "===");
                    break;
                case BellmanGraphVertex.VertexState.Visited:
                    //imgColor = clrVisitedVertex;
                    //textColor = Color.white;
                    textColor = new Color(1f,137f/255f,0,1);
                    Debug.Log(clrNormalVertex + "---");
                    break;
                default:
                    //imgColor = Color.clear;
                    textColor = Color.clear;
                    Debug.Log("fdfdfd");
                    break;
            }

            //img.DOColor(imgColor, 0.5f).SetEase(Ease.Linear);
            text.DOColor(textColor, 0.5f).SetEase(Ease.Linear);
        }
    }
}