using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Le0derToolkit.Toolbox
{
    // 简单工具类，不需要提前放到场景之中
    public class CoroutineToolkit : MonoBehaviour
    {
        private static CoroutineToolkit m_instance;
        protected static CoroutineToolkit instance
        {
            get
            {
                if (m_instance == null)
                {
                    GameObject o = new GameObject("CoroutineToolkit");
                    DontDestroyOnLoad(o);
                    m_instance = o.AddComponent<CoroutineToolkit>();
                    instance.coroutineDict = new Dictionary<string, Coroutine>();
                }
                return m_instance;
            }
        }

        private Dictionary<string, Coroutine> coroutineDict;

        // 启动协程，并保存协程的引用
        public static void StartCoroutine(string coroutineName, IEnumerator routine)
        {
            if (!string.IsNullOrWhiteSpace(coroutineName))
            {
                if (instance.coroutineDict.ContainsKey(coroutineName))
                {
                    Debug.LogWarning("Coroutine with name " + coroutineName + " is already running.");
                    return;
                }

                instance.coroutineDict[coroutineName] = instance.StartCoroutine(instance.ExecuteCoroutine(coroutineName, routine));
            }
            else
            {
                instance.StartCoroutine(routine);
            }
        }

        // 停止指定名称的协程
        public static new void StopCoroutine(string coroutineName)
        {
            if (instance.coroutineDict.ContainsKey(coroutineName))
            {
                instance.StopCoroutine(instance.coroutineDict[coroutineName]);
                instance.coroutineDict.Remove(coroutineName);
            }
        }

        // 执行协程
        private IEnumerator ExecuteCoroutine(string coroutineName, IEnumerator routine)
        {
            yield return routine;
            coroutineDict.Remove(coroutineName);
        }
    }
}