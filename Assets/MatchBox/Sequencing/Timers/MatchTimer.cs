using UnityEngine;
using Grids;

namespace MatchBox.Sequencing.Timers
{
    using Grids;

    public class MatchTimer : MonoBehaviour
    {
        [SerializeField] private GridDisplay gridDisplay;

        [Header("Timer")]
        [SerializeField] private float startTime;
        [SerializeField] private float timeLostPerMatch;

        [Header("Debug")]
        [SerializeField] private float displayedTimer;
        private bool timerRunning;

        private void Start()
        {
            gridDisplay.OnMatchFoundEvent += OnMatchFound;

            displayedTimer = startTime;
            timerRunning = true;
        }

        private void OnMatchFound(object sender, GridDisplay.OnMatchFoundEventArgs args)
        {
            if (timerRunning)
            {
                displayedTimer -= args.NewlyMatchedObjects.Length * timeLostPerMatch;
            }
        }

        private void Update()
        {
            if (timerRunning)
            {
                if (displayedTimer > 0f)
                {
                    displayedTimer -= Time.deltaTime;
                }
                else
                {
                    displayedTimer = 0f;
                    
                    Debug.Log("Time's Up!");
                    gridDisplay.CollapseGrid(true);

                    timerRunning = false;
                }
            }
        }
    }
}