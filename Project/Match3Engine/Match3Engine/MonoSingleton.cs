using UnityEngine;

namespace Match3Engine
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        private static bool wasDestryed;
        
        public static T Instance
        {
            get
            {
                if (wasDestryed)
                    return null;

                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType(typeof(T)) as T;

                    if (instance == null)
                    {
                        GameObject gameObject = new GameObject(typeof(T).Name);
                        GameObject.DontDestroyOnLoad(gameObject);

                        instance = gameObject.AddComponent(typeof(T)) as T;
                    }
                }

                return instance;
            }
        }
        
        protected virtual void OnDestroy()
        {
            if (instance)
                Destroy(instance);

            instance = null;
            wasDestryed = true;
        }
    }
}
