using UnityEngine;

namespace WJF_CodeLibrary.CommonUtility
{
    public static class ColorCalculateUtility
    {
        /// <summary>
        /// 获得颜色，可根据颜色面板上的颜色值(0~255)获得正确的颜色对象(颜色值在0~1)
        /// </summary>
        /// <param name="r">R（0~255）</param>
        /// <param name="g">G（0~255）</param>
        /// <param name="b">B（0~255）</param>
        /// <param name="a">A（0~255）</param>
        /// <returns>正确的颜色对象</returns>
        public static Color GetColor(float r, float g, float b, float a = 255f)
        {
            r /= 255f;
            g /= 255f;
            b /= 255f;
            a /= 255f;
            return new Color(r, g, b, a);
        }

        /// <summary>
        /// 改变指定颜色的透明
        /// </summary>
        /// <param name="color">颜色对象</param>
        /// <param name="fade">透明度</param>
        /// <returns>改变结果</returns>
        public static Color GetFadeColor(Color color, float fade)
        {
            Color c = color;
            c.a = fade;
            return c;
        }

        /// <summary>
        /// 颜色对象转换为十六进制字符串，使用时可能需要在前面添加'#'
        /// </summary>
        /// <param name="color">颜色对象</param>
        /// <returns>十六进制字符串</returns>
        public static string ColorToHex(Color color)
        {
            int r = Mathf.RoundToInt(color.r * 255.0f);
            int g = Mathf.RoundToInt(color.g * 255.0f);
            int b = Mathf.RoundToInt(color.b * 255.0f);
            int a = Mathf.RoundToInt(color.a * 255.0f);
            string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
            return hex;
        }

        /// <summary>
        /// 十六进制字符串转换为颜色对象
        /// </summary>
        /// <param name="hex">颜色的十六进制字符串，不包括前面的'#'</param>
        /// <returns>颜色对象</returns>
        public static Color HexToColor(string hex)
        {
            byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            float r = br / 255f;
            float g = bg / 255f;
            float b = bb / 255f;
            float a = cc / 255f;
            return new Color(r, g, b, a);
        }
    }
}