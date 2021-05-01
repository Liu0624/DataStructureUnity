using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WJF_CodeLibrary.UIFramework;

namespace WJF
{
	public class Storage
	{
        private static Dictionary<string, GameObject> pattern = new Dictionary<string, GameObject>();

        public static GameObject GetPattern(string patternName)
        {
            if (!pattern.ContainsKey(patternName))
            {
                GameObject patternInstance = Resources.Load(UIDefine.UIPathRoot + "Pattern/" + patternName) as GameObject;
                pattern.Add(patternName, patternInstance);
            }

            return pattern[patternName];
        }
	}
}