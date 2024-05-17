using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Le0der.Toolbox
{
    public class TimelineHelper
    {
        /// <summary>
        /// 创建Timeline控制器
        /// </summary>
        /// <param name="director">PlayableDirector 组件</param>
        public static TimelineDirector CreateTimelineDirector(PlayableDirector director)
        {
            Debug.Assert(null != director, "null is director");
            return director.GetComponent<TimelineDirector>() ?? director.gameObject.AddComponent<TimelineDirector>();
        }

        /// <summary>
        /// 创建Timeline控制器
        /// </summary>
        /// <param name="directorPath">PlayableDirector 路径</param>
        public static TimelineDirector CreateTimelineDirector(string directorPath)
        {
            var director = GameObject.Find(directorPath);
            Debug.Assert(null != director, "null is directorPath");
            return director.GetComponent<TimelineDirector>() ?? director.gameObject.AddComponent<TimelineDirector>();
        }
    }
}