using UnityEngine;


namespace Patterns
{

    /// <summary>
    /// An singleton abstract component pattern
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonComponent<T> : MonoBehaviour
        where T : SingletonComponent<T>
    {
        #region instance part
        private static T m_pInstance = null;

        public static T Instance
        {
            get
            {
                if (m_pInstance == null)
                    m_pInstance = FindObjectOfType<T>();
                if (m_pInstance == null)
                {
                    GameObject pInstance = new GameObject(typeof(T).ToString());
                    if(Application.isPlaying)
                        GameObject.DontDestroyOnLoad(pInstance);
                    m_pInstance = pInstance.AddComponent<T>();
                }
                return m_pInstance;
            }
        }

        public virtual void Awake()
        {
#if UNITY_EDITOR
            if (m_pInstance != null)
                Debug.LogError(name + " already initialized", this);
#endif
            m_pInstance = (T)this;
        }

        public static bool HasInstance()
        {
            return m_pInstance != null;
        }
        #endregion
    }



    /// <summary>
    /// A class to manager all singleton classes. yet testing
    /// </summary>
    public static class SingletonManager
    {
        #region management part

        /// <summary>
        /// stores a singleton intance
        /// </summary>
        /// <param name="comp"></param>
        public static void Add(Object comp)
        {
            //m_pInstanceList.Add(comp);
        }
        /// <summary>
        /// removes a singleton intance
        /// </summary>
        /// <param name="comp"></param>
        public static void Remove(Object comp)
        {
            //m_pInstanceList.Remove(comp);
        }

        /// <summary>
        /// clear a singleton intances
        /// </summary>
        public static void Clear()
        {
            //for (int i = m_pInstanceList.Count - 1; i >= 0; i--)
            //    GameObject.Destroy(m_pInstanceList[i]);
            //m_pInstanceList.Clear();
        }
        #endregion
    }

    /// <summary>
    /// simple singleton method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonClass<T>
        where T : SingletonClass<T>, new()
    {
        protected static T m_pInstance = null;
        public static T GetInstance()
        {
            if (m_pInstance == null)
            {
                m_pInstance = new T();
            }
            return m_pInstance;
        }
        public static T Instance
        {
            get { return GetInstance(); }
        }
        public static bool HasInstance()
        {
            return m_pInstance != null;
        }
    }
}