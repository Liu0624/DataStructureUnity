using System;
using UnityEngine;
using UnityEngine.UI;

namespace WJF_CodeLibrary.CommonUtility.UI
{
    /// <summary>
    /// 文本框属性
    /// </summary>
    public static partial class UIUtility
    {
        /// <summary>
        /// 获取文本框宽度
        /// </summary>
        /// <param name="text">文本框</param>
        /// <returns>文本框宽度</returns>
        public static float GetTextWidth(Text text)
        {
            return text.rectTransform.rect.width;
        }

        /// <summary>
        /// 获取文本框高度
        /// </summary>
        /// <param name="text">文本框</param>
        /// <returns>文本框高度</returns>
        public static float GetTextHeight(Text text)
        {
            return text.rectTransform.rect.height;
        }
    }

    /// <summary>
    /// text文字过长时设置省略后缀
    /// </summary>
    public static partial class UIUtility
    {
        /// <summary>
        /// 截取文字并放入文本框中，文本框宽度自动获取
        /// </summary>
        /// <param name="text">文本框</param>
        /// <param name="input">目标字符串</param>
        /// <param name="suffix">省略后缀</param>
        public static void SetStrippedText(Text text, string input, string suffix = "...")
        {
            int maxWidth = (int)GetTextWidth(text);
            SetStrippedText(text, input, maxWidth, suffix);
        }

        /// <summary>
        /// 截取文字并放入文本框中
        /// </summary>
        /// <param name="text">文本框</param>
        /// <param name="input">目标字符串</param>
        /// <param name="maxWidth">文本框宽度</param>
        /// <param name="suffix">省略后缀</param>
        public static void SetStrippedText(Text text, string input, int maxWidth, string suffix = "...")
        {
            int len = CalculateLengthOfText(text, input);

            //截断text的长度，如果总长度大于限制的最大长度，
            //那么先根据最大长度减去后缀长度的值拿到字符串，在拼接上后缀
            if (len > maxWidth)
            {
                text.text = StripLength(text, input, maxWidth - CalculateLengthOfText(text, suffix)) + suffix;
            }
            else
            {
                text.text = input;
            }
        }

        /// <summary>
        /// 根据maxWidth来截断input拿到子字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxWidth"></param>
        /// <returns></returns>
        private static string StripLength(Text text, string input, int maxWidth)
        {
            int totalLength = 0;
            Font myFont = text.font;
            myFont.RequestCharactersInTexture(input, text.fontSize, text.fontStyle);

            CharacterInfo characterInfo = new CharacterInfo();
            char[] arr = input.ToCharArray();
            int i = 0;

            foreach (char c in arr)
            {
                myFont.GetCharacterInfo(c, out characterInfo, text.fontSize);

                int newLength = totalLength + characterInfo.advance;
                if (newLength > maxWidth)
                {
                    if (Mathf.Abs(newLength - maxWidth) > Mathf.Abs(maxWidth - totalLength))
                    {
                        break;
                    }
                    else
                    {
                        totalLength = newLength;
                        i++;
                        break;
                    }
                }

                totalLength += characterInfo.advance;
                i++;
            }

            return input.Substring(0, i);
        }

        /// <summary>
        /// 计算字符串在指定text控件中的长度
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static int CalculateLengthOfText(Text text, string message)
        {
            int totalLength = 0;
            Font myFont = text.font;

            myFont.RequestCharactersInTexture(message, text.fontSize, text.fontStyle);

            CharacterInfo characterInfo = new CharacterInfo();
            char[] arr = message.ToCharArray();

            foreach (char c in arr)
            {
                myFont.GetCharacterInfo(c, out characterInfo, text.fontSize);
                totalLength += characterInfo.advance;
            }

            return totalLength;
        }
    }

    /// <summary>
    /// RectTransform属性设置
    /// </summary>
    public static partial class UIUtility
    {
        /// <summary>
        /// 锚点设置到四个角
        /// </summary>
        /// <param name="t"></param>       
        [Obsolete("不要使用，该方法有错误")]
        public static void AnchorsToCorners(RectTransform t)
        {
            RectTransform pt = t.parent as RectTransform;

            if (t == null || pt == null) return;

            Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
                                                t.anchorMin.y + t.offsetMin.y / pt.rect.height);
            Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
                                                t.anchorMax.y + t.offsetMax.y / pt.rect.height);

            t.anchorMin = newAnchorsMin;
            t.anchorMax = newAnchorsMax;
            t.offsetMin = t.offsetMax = new Vector2(0, 0);
        }

        /// <summary>
        /// 可将子物体尺寸完整覆盖父物体
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="size"></param>
        [Obsolete("不要使用，该方法有错误")]
        public static void SetInsetAndSizeFromParentEdge(RectTransform rt, Vector2 size)
        {
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, size.x);
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, size.x);
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, size.y);
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, size.y);
        }

        /// <summary>
        /// 铺满Canvas，锚点定位到四个角
        /// </summary>
        /// <param name="rt"></param>
        public static void TileRectTransform(RectTransform rt)
        {
            //Vector2 size = rt.GetComponentInParent<CanvasScaler>().referenceResolution;

            //SetInsetAndSizeFromParentEdge(rt, size);
            //AnchorsToCorners(rt);

            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }

    /// <summary>
    /// 交互性设置
    /// </summary>
    public static partial class UIUtility
    {
        public static void ToggleRaycast(GameObject go, bool state, Func<MaskableGraphic, bool> filter = null)
        {
            MaskableGraphic[] interactables = go.GetComponentsInChildren<MaskableGraphic>(true);
            foreach (var item in interactables)
            {
                if (filter != null)
                {
                    if (filter(item))
                        item.raycastTarget = state;
                }                    
                else
                {
                    item.raycastTarget = state;
                }
            }
        }
    }
}