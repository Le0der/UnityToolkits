/****************************************************************************
 * Copyright (c) 2023 朱乐峰 （填写自己的 github id/或者昵称)
 * Copyright (c) 2023.4 朱乐峰  (要注明最后维护者)
 
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Le0derToolkit.Toolbox
{
    /// <summary>
    /// 网络请求工具
    /// </summary>
    public class NetworkToolkit : MonoSingleton<NetworkToolkit>
    {
        // public void GetRequset(string url, UnityAction<bool, string> onRequestEnd)
        // {
        //     StartCoroutine(IGetRequest(url, onRequestEnd));
        // }

        // public void PostRequest(string url, WWWForm form, UnityAction<bool, string> onRequestEnd)
        // {
        //     StartCoroutine(IPostRequest(url, form, onRequestEnd));
        // }

        // public void GetRequset(string url, UnityAction<bool, byte[]> onRequestEnd)
        // {
        //     StartCoroutine(IGetRequestFile(url, onRequestEnd));
        // }

        // public void PostRequest(string url, WWWForm form, UnityAction<bool, byte[]> onRequestEnd)
        // {
        //     StartCoroutine(IPostRequestFile(url, form, onRequestEnd));
        // }


        public IEnumerator IGetRequest(string url, UnityAction<bool, string> onRequestEnd)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();
                OnRequestEnd(webRequest, onRequestEnd);
            }
        }

        public IEnumerator IGetRequest(string url, UnityAction<bool, Sprite> onRequestEnd)
        {
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return webRequest.SendWebRequest();
                OnRequestEnd(webRequest, onRequestEnd);
            }
        }

        public IEnumerator IPostRequest(string url, WWWForm form, UnityAction<bool, string> onRequestEnd)
        {
            if (string.IsNullOrEmpty(url) || form == null)
            {
                yield break;
            }
            using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
            {
                yield return webRequest.SendWebRequest();
                OnRequestEnd(webRequest, onRequestEnd);
            }
        }

        public IEnumerator IGetRequestFile(string url, UnityAction<bool, byte[]> onRequestEnd)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();
                OnRequestEnd(webRequest, onRequestEnd);
            }
        }

        public IEnumerator IPostRequestFile(string url, WWWForm form, UnityAction<bool, byte[]> onRequestEnd)
        {
            if (string.IsNullOrEmpty(url) || form == null)
            {
                yield break;
            }
            using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
            {
                yield return webRequest.SendWebRequest();
                OnRequestEnd(webRequest, onRequestEnd);
            }
        }


        private void OnRequestEnd(UnityWebRequest webRequest, UnityAction<bool, string> onRequestEnd)
        {
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                if (onRequestEnd != null)
                    onRequestEnd.Invoke(false, webRequest.error);
                Debug.LogError(webRequest.error);
            }
            else
            {
                if (onRequestEnd != null)
                    onRequestEnd.Invoke(true, webRequest.downloadHandler.text);
            }
        }

        private void OnRequestEnd(UnityWebRequest webRequest, UnityAction<bool, byte[]> onRequestEnd)
        {
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                if (onRequestEnd != null)
                    onRequestEnd.Invoke(false, TryConverStringToBytes(webRequest.error));
                Debug.LogError(webRequest.error);
            }
            else
            {
                if (onRequestEnd != null)
                    onRequestEnd.Invoke(true, webRequest.downloadHandler.data);
            }
        }

        private void OnRequestEnd(UnityWebRequest webRequest, UnityAction<bool, Sprite> onRequestEnd)
        {
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                if (onRequestEnd != null)
                    onRequestEnd.Invoke(false, null);
                Debug.LogError(webRequest.error);
            }
            else
            {
                try
                {
                    // 获取加载的图像数据
                    Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

                    // 创建 Sprite 对象
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

                    onRequestEnd?.Invoke(true, sprite);
                }
                catch (System.Exception e)
                {
                    onRequestEnd?.Invoke(false, null);
                    Debug.LogError($"Unable to load Sprite due to: {e.Message}; StackTrace: {e.StackTrace}");
                }

            }
        }

        /// <summary>
        /// 字符串转bytes数组
        /// </summary>
        /// <param name="str">需要转换的字符串</param>
        /// <returns>转换后的数组</returns>
        private static byte[] TryConverStringToBytes(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;

            byte[] bytes = Encoding.ASCII.GetBytes(str);
            return bytes;
        }
    }
}