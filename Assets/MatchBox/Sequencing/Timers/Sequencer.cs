using UnityEngine;
using Grids;

namespace MatchBox.Sequencing.Timers
{
    using Grids;

    public class Sequencer : MonoBehaviour
    {
        [SerializeField] private GridDisplay gridDisplay;

        [Header("Match Time")]
        [SerializeField] private float matchStartTime;
        [SerializeField] private float timeLostPerMatch;

        [Header("Box Time")]
        [SerializeField] private float boxStartTime;

        [Header("Debug")]
        [SerializeField] private float displayedTimer;

        private TimerMode timerMode;
        private enum TimerMode
        {
            none,
            match,
            box
        }

        private void Start()
        {
            gridDisplay.OnMatchFoundEvent += OnMatchFound;
            StartMatchTimer();
        }

        private void OnMatchFound(object sender, GridDisplay.OnMatchFoundEventArgs args)
        {
            if (timerMode == TimerMode.match)
            {
                displayedTimer -= args.NewlyMatchedObjects.Length * timeLostPerMatch;
            }
        }

        private void Update()
        {
            if (timerMode != TimerMode.none)
            {
                if (displayedTimer > 0f)
                {
                    displayedTimer -= Time.deltaTime;
                }
                else
                {
                    if (timerMode == TimerMode.box)
                    {
                        BoxTimerOut();
                    }
                    else if (timerMode == TimerMode.match)
                    {
                        MatchTimerOut();
                    }
                }
            }
        }

        private void StartMatchTimer()
        {
            displayedTimer = matchStartTime;
            timerMode = TimerMode.match;
        }

        private void MatchTimerOut()
        {
            gridDisplay.CollapseGrid();

            StartBoxTimer();
        }

        private void StartBoxTimer()
        {
            displayedTimer = boxStartTime;
            timerMode = TimerMode.box;
        }

        private void BoxTimerOut()
        {
            gridDisplay.RestoreGrid();

            StartMatchTimer();
        }
    }
}