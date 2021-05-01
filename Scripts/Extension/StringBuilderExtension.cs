using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WJF
{
	public static class StringBuilderExtension
	{	
        public static void Clear(this StringBuilder sb)
        {
            sb.Remove(0, sb.Length);
        }
	}
}