using UnityEngine;

namespace WJF_CodeLibrary.Singleton
{
	public class MonoSingleton<T> : MonoBehaviour where T : Component
	{
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject();
                        instance = go.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

		public virtual void Clear()
		{
			instance = null;
		}
	}
}