using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Le0der.Toolbox
{
    public class TimelineController : MonoBehaviour
    {

        public PlayableDirector PlayableDirector;

        public Button PlayForward;

        public Button PlayBackward;

        public Button Pause;

        public Button Stop;

        public Button Continue;

        public Button ContinueForward;

        public Button ContinueBackward;

        private TimelineDirector Director;

        public void Start()
        {
            Director = TimelineHelper.CreateTimelineDirector(PlayableDirector);

            Director.OnPlay = (t, d) =>
            {
                Debug.Log($"OnPlay time {t} dir {d}");
            };

            Director.OnPause = (t, d) =>
            {
                Debug.Log($"OnPause time {t} dir {d}");
            };

            Director.OnContinue = (t, d) =>
            {
                Debug.Log($"OnContinue time {t} dir {d}");
            };

            Director.OnStop = (t, d) =>
            {
                Debug.Log($"OnStop time {t} dir {d}");
            };

            PlayForward.onClick.AddListener(() =>
            {
                Director.PlayForward();
            });

            PlayBackward.onClick.AddListener(() =>
            {
                Director.PlayBackward();
            });

            Pause.onClick.AddListener(() =>
            {
                Director.Pause();
            });

            Stop.onClick.AddListener(() =>
            {
                Director.Stop();
            });

            Continue.onClick.AddListener(() =>
            {
                Director.Continue();
            });

            ContinueForward.onClick.AddListener(() =>
            {
                Director.ContinuePlayForwardByPausePoint();
            });

            ContinueBackward.onClick.AddListener(() =>
            {
                Director.ContinuePlayBackwardByPausePoint();
            });
        }
    }
}