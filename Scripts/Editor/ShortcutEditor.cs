using UnityEngine;
using UnityEditor;
using System.Linq;
using WJF_CodeLibrary.CommonUtility.Sort;

namespace WJF
{
	public class ShortcutEditor
	{
        #region 创建线段

        private enum CircleType { Normal, Arrow }

        [MenuItem("Shortcut/ConnnectNormalCircle &0", false, 1)]
        private static void ConnectNormalCirle()
        {
            ConnectCirle(CircleType.Normal);
        }

        [MenuItem("Shortcut/ConnnectArrowlCircle &9", false, 1)]
        private static void ConnnectArrowlCircle()
        {
            ConnectCirle(CircleType.Arrow);
        }

        private static void ConnectCirle(CircleType type)
        {
            Transform[] targets = Selection.transforms;

            if (targets.Length <= 1)
            {
                return;
            }

            SortUtility.Bubble(targets, (x, y) =>
            {
                return x.GetSiblingIndex() > y.GetSiblingIndex();
            });

            for (int i = 0; i < targets.Length - 1; i++)
            {
                Transform v1 = targets[i];
                Transform v2 = targets[i + 1];
                CreateLine(v1, v2, type);
            }

            if (targets.Length > 2)
                CreateLine(targets[targets.Length - 1], targets[0], type);
        }

        private static void CreateLine(Transform v1, Transform v2, CircleType type)
        {
            string resourceName = type == CircleType.Normal ? "NormalLine" : "ArrowLine";
            GameObject linePattern = Resources.Load("Prefab/WJF/Shortcut/" + resourceName) as GameObject;
            GameObject line = Object.Instantiate(linePattern);
            Undo.RegisterCreatedObjectUndo(line, "CreateLine");

            line.transform.SetParent(v1.parent);
            line.transform.SetAsFirstSibling();
            line.transform.position = v1.position;

            Vector3 dir = v2.position - v1.position;
            line.transform.up = -dir;

            float dist = Vector3.Distance(v1.position, v2.position);
            line.GetComponent<RectTransform>().sizeDelta = new Vector2(type == CircleType.Normal ? 2f : 10f, dist);

            line.name = v1.name + "-" + v2.name;
        }

        #endregion

        #region 翻转

        [MenuItem("Shortcut/ReverseLine &8", false, 1)]
        private static void ReverseLine()
        {
            Transform target = Selection.activeTransform;
            RectTransform rt = target.GetComponent<RectTransform>();
            Undo.RecordObject(rt, "Reverse");

            rt.pivot = new Vector2(0.5f, rt.pivot.y == 0 ? 1 : 0);
            rt.localScale = new Vector3(1, rt.localScale.y * -1, 1);
        }

        #endregion
    }
}