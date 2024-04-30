using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Le0derToolkit.Toolbox
{
    public class ConfigToolkit : Singleton<ConfigToolkit>
    {
        // 文件路径格式
        protected const string m_pathFormat = "{0}/Configs/{1}.json";

        // 加载方法格式
        protected const string m_loadFormat = "Load{0}Config";

        // 加载配置文件
        public void LoadConfig<T>(string name, UnityAction<T> onLoadEnd, UnityAction<string> onError = null) where T : ConfigBase
        {
            // 构造协程名称
            var coroutineName = string.Format(m_loadFormat, name);

            // 开始加载配置文件协程
            CoroutineToolkit.StartCoroutine(coroutineName, ILoadConfig(name, onLoadEnd, onError));
        }

        // 加载配置文件协程
        public IEnumerator ILoadConfig<T>(string name, UnityAction<T> onLoadEnd, UnityAction<string> onError = null) where T : ConfigBase
        {
            // 构造文件路径
            var url = string.Format(m_pathFormat, Application.streamingAssetsPath, name);

            // 发送网络请求加载配置文件
            yield return NetworkToolkit.Instance.IGetRequest(url, OnLoadConfigEnd);

            // 加载完成回调
            void OnLoadConfigEnd(bool success, string data)
            {
                if (success)
                {
                    try
                    {
                        // 解析配置文件数据
                        var configData = JsonToolkit.DeserializeObject<T>(data);
                        onLoadEnd?.Invoke(configData);
                    }
                    catch (Exception e)
                    {
                        onLoadEnd?.Invoke(null);
                        // 解析出错，回调错误信息
                        onError?.Invoke("解析配置文件出错：" + e.Message);
                        Debug.LogErrorFormat("解析配置文件出错，错误：{0}; 堆栈：{1}", e.Message, e.StackTrace);
                    }
                }
                else
                {
                    onLoadEnd?.Invoke(null);
                    // 加载失败，回调错误信息
                    onError?.Invoke("加载配置文件失败：" + data);
                    Debug.LogErrorFormat("加载配置文件失败，错误：{0}", data);
                }
            }
        }
    }

    public abstract class ConfigBase { }
}
