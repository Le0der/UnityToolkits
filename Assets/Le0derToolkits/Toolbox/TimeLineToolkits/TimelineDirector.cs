using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Le0derToolkit.Toolbox
{
    [RequireComponent(typeof(PlayableDirector))]
    public class TimelineDirector : MonoBehaviour
    {
        #region ENUM

        public enum Status
        {
            NULL,
            PLAYING,
            PAUSED,
            STOPPED,
        }

        public enum Direction
        {
            NULL,
            FORWARD,
            BACKWARD
        }

        #endregion

//        [SerializeField]
        private PlayableDirector m_playableDirector;

        [Range(0f, 1f)]
        public float PlaySpeed = 1f;

        /// <summary>
        /// 播放模式
        /// </summary>
        public WrapMode WrapMode = WrapMode.Once;

        /// <summary>
        /// 开始播放事件, 返回时 时间点，和触发时方向
        /// </summary>
        public Action<double, Direction> OnPlay;

        /// <summary>
        /// 暂停播放事件, 返回时 时间点，和触发时方向
        /// </summary>
        public Action<double, Direction> OnPause;

        /// <summary>
        /// 停止播放事件, 返回时 时间点，和触发时方向
        /// </summary>
        public Action<double, Direction> OnStop;

        /// <summary>
        /// 继续播放事件, 返回时 时间点，和触发时方向
        /// </summary>
        public Action<double, Direction> OnContinue;

        /// <summary>
        /// Timeline长度
        /// </summary>
        public double Duration { get; private set; } = -1f;

        /// <summary>
        /// 当前播放状态
        /// </summary>
        public Status CurrentPlayStatus { get; private set; } = Status.NULL;

        /// <summary>
        /// 当前播放方向
        /// </summary>
        public Direction CurrentPlayDirection { get; private set; } = Direction.NULL;

        /// <summary>
        /// 当前播放进度
        /// </summary>
        public double CurrentTime { get; private set; } = 0d;

        /// <summary>
        /// 播放开始时间点
        /// </summary>
        private double m_timeCache = -1f;

        /// <summary>
        /// 上次运行方向
        /// </summary>
        private Direction m_prePlayedDirectionCache = Direction.NULL;

        /// <summary>
        /// 暂停时间点
        /// </summary>
        private double m_pauseTimePoint = -1f;


        private void Awake()
        {
            m_playableDirector = GetComponent<PlayableDirector>();
            Duration = m_playableDirector.duration;
            m_pauseTimePoint = 0d;
            CurrentPlayStatus = Status.STOPPED;
        }

        /// <summary>
        /// 继续播放
        /// </summary>
        public void Continue()
        {
            OnContinue?.Invoke(CurrentTime, CurrentPlayDirection);
            CurrentPlayStatus = Status.PLAYING;
            CurrentPlayDirection = m_prePlayedDirectionCache;
        }

        /// <summary>
        /// 从暂停时间点正向播放
        /// </summary>
        public void ContinuePlayForwardByPausePoint()
        {
            OnContinue?.Invoke(CurrentTime, CurrentPlayDirection);
            CurrentPlayStatus = Status.PLAYING;
            m_timeCache = m_pauseTimePoint;
            CurrentPlayDirection = Direction.FORWARD;
        }

        /// <summary>
        /// 从暂停时间点反向播放
        /// </summary>
        public void ContinuePlayBackwardByPausePoint()
        {
            OnContinue?.Invoke(CurrentTime, CurrentPlayDirection);
            CurrentPlayStatus = Status.PLAYING;
            m_timeCache = m_pauseTimePoint;
            CurrentPlayDirection = Direction.BACKWARD;
        }

        /// <summary>
        /// 从开始播放
        /// </summary>
        public void PlayForward()
        {
            OnPlay?.Invoke(CurrentTime, CurrentPlayDirection);
            m_timeCache = 0d;
            CurrentPlayStatus = Status.PLAYING;
            CurrentPlayDirection = Direction.FORWARD;
            m_prePlayedDirectionCache = Direction.NULL;
            m_pauseTimePoint = 0d;
            m_timer = 0f;
        }

        /// <summary>
        /// 从结尾倒放
        /// </summary>
        public void PlayBackward()
        {
            OnPlay?.Invoke(CurrentTime, CurrentPlayDirection);
            m_timeCache = Duration;
            CurrentPlayStatus = Status.PLAYING;
            CurrentPlayDirection = Direction.BACKWARD;
            m_prePlayedDirectionCache = Direction.NULL;
            m_pauseTimePoint = 0d;
            m_timer = 0f;
        }

        /// <summary>
        /// 暂停播放
        /// </summary>
        public void Pause()
        {
            OnPause?.Invoke(CurrentTime, CurrentPlayDirection);
            CurrentPlayStatus = Status.PAUSED;
            m_pauseTimePoint = m_playableDirector.time;
            m_timeCache = m_pauseTimePoint;
            m_prePlayedDirectionCache = CurrentPlayDirection;
            CurrentPlayDirection = Direction.NULL;
            m_timer = 0f;
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public void Stop()
        {
            OnStop?.Invoke(CurrentTime, CurrentPlayDirection);
            CurrentPlayStatus = Status.STOPPED;
            m_pauseTimePoint = 0d;
            m_timeCache = m_pauseTimePoint;
            m_prePlayedDirectionCache = Direction.NULL;
            CurrentPlayDirection = Direction.NULL;
            CurrentTime = 0d;
            m_timer = 0f;
            m_playableDirector.time = CurrentTime;
            m_playableDirector.Evaluate();
        }

        /// <summary>
        /// Lerp计时器
        /// </summary>
        private float m_timer;

        /// <summary>
        /// 继续播放时计算剩余的比例 与 Lerp计时器混合计算
        /// </summary>
        private float m_continueTimerRatio = 1f;

        private void FixedUpdate()
        {
            // 播放时触发
            if (CurrentPlayStatus.Equals(Status.PLAYING))
            {
                // Lerp计时累加
                m_timer += Time.deltaTime;

                // 正播
                if (CurrentPlayDirection.Equals(Direction.FORWARD))
                {
                    // 计算播放速度比例
                    m_continueTimerRatio = (float)Math.Abs(m_timeCache - Duration) / (float)Duration;
                    CurrentTime = DoubleLerp(m_timeCache, Duration, m_timer / m_continueTimerRatio * PlaySpeed);
                }
                // 倒播
                else if (CurrentPlayDirection.Equals(Direction.BACKWARD))
                {
                    m_continueTimerRatio = (float)Math.Abs(m_timeCache - 0) / (float)Duration;
                    CurrentTime = DoubleLerp(m_timeCache, 0, m_timer / m_continueTimerRatio * PlaySpeed);
                }

                // 当播放进度到1后做播放完毕处理
                if (Mathf.Clamp01(m_timer / m_continueTimerRatio * PlaySpeed).Equals(1))
                {
                    // 本次播放完毕可能时中途继续播放，还原播放比例
                    m_continueTimerRatio = 1f;

                    // 处理各个播放模式
                    switch (WrapMode)
                    {
                        // 只播放一次， 根据翻译是 播放完毕后回到初始状态
                        case WrapMode.Once:
                            Stop();
                            break;
                        // 循环播放, 方向不变，把计时器归零 Lerp继续走
                        case WrapMode.Loop:
                            m_timer = 0f;
                            break;
                        // 乒乓，方向取反 计时器归零 Lerp继续走
                        case WrapMode.PingPong:
                            CurrentPlayDirection = CurrentPlayDirection.Equals(Direction.FORWARD)
                                ? Direction.BACKWARD
                                : Direction.FORWARD;
                            m_timer = 0f;
                            break;
                        // 和Once一样了
                        case WrapMode.Default:
                            Stop();
                            break;
                        // 根绝翻译，当前方向播放完毕后保持最后的状态
                        case WrapMode.ClampForever:
                            Pause();
                            break;
                    }
                    // 因继续播放因素存在重置 时间 缓存
                    m_timeCache = CurrentPlayDirection.Equals(Direction.FORWARD) ? 0d : Duration;

                }

                // 直接取样
                m_playableDirector.time = CurrentTime;
                m_playableDirector.Evaluate();
            }
        }

        /// <summary>
        /// Lerp 没有double 特写一个
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public double DoubleLerp(double a, double b, float t) => a + (b - a) * Mathf.Clamp01(t);

        public void SetDirectorTimeStart()
        {
            m_playableDirector.time = 0;
            m_playableDirector.Evaluate();
        }


        public void SetDirectorTimeEnd()
        {
            m_playableDirector.time = Duration;
            m_playableDirector.Evaluate();
        }
    }
}