using UnityEngine;
using UnityEngine.UI;
using WJF_CodeLibrary.CommonUtility.UI;
using WJF_CodeLibrary.Extension;

namespace WJF_CodeLibrary.UIFramework
{
    public class UIMaskManager
    {
        private Transform pnlMask;//遮罩面板
        private Transform canvasTF;//UI物体

        #region 单例

        private static UIMaskManager instance;
        public static UIMaskManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new UIMaskManager();
                return instance;
            }
        }

        #endregion

        public UIMaskManager()
        {
            //获取UI物体
            canvasTF = Object.FindObjectOfType<Canvas>().transform;

            //获取遮罩面板，设置父物体
            pnlMask = canvasTF.GetChild("MaskPanel");

            if (pnlMask == null)
            {
                pnlMask = CreateMaskFromPrefab();
            }

            Transform parent = canvasTF.GetChild(UIDefine.PopupNode);
            if (parent != null)
                pnlMask.SetParent(parent);
            pnlMask.gameObject.SetActive(false);
        }

        /// <summary>
        /// 设置遮罩
        /// </summary>
        /// <param name="displayTransform">需要在遮罩前显示的面板</param>
        public void SetMask(Transform displayTransform, UIMaskType type)
        {
            //设置遮罩在最后一个，再设置面板再最后一个，保证遮罩在面板后
            pnlMask.SetAsLastSibling();
            displayTransform.SetAsLastSibling();

            //设置并显示/隐藏遮罩
            switch (type)
            {
                case UIMaskType.None:
                    pnlMask.gameObject.SetActive(false);
                    break;
                case UIMaskType.Normal:
                    Image mask = pnlMask.GetComponent<Image>();
                    mask.color = new Color(UIDefine.MaskNormalColorRGB, UIDefine.MaskNormalColorRGB, UIDefine.MaskNormalColorRGB, UIDefine.MaskNormalColorAlpha);
                    pnlMask.gameObject.SetActive(true);
                    break;
            }
        }

        /// <summary>
        /// 取消遮罩
        /// </summary>
        public void CancelMask()
        {
            pnlMask.gameObject.SetActive(false);
        }

        /// <summary>
        /// 销毁事件
        /// </summary>
        public void OnDestroy()
        {
            if (instance != null)
            {
                instance = null;
            }
        }

        /// <summary>
        /// 从预制体中创建遮罩界面
        /// </summary>
        /// <returns></returns>
        private Transform CreateMaskFromPrefab()
        {
            GameObject prefab = Resources.Load<GameObject>(UIDefine.MaskPrefabPath);

            if (prefab == null)
            {
                Debug.LogError("The mask prefab is not found");
                return null;
            }

            GameObject clone = Object.Instantiate(prefab);
            clone.transform.SetParent(canvasTF);

            RectTransform rtClone = clone.GetComponent<RectTransform>();
            UIUtility.TileRectTransform(rtClone);

            return clone.transform;
        }
    }
}