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

        private void Start()
        {
            gridDisplay.OnMatchFoundEvent += OnMatchFound;

            displayedTimer = startTime;
        }

        private void OnMatchFound(object sender, GridDisplay.OnMatchFoundEventArgs args)
        {
            displayedTimer -= args.NewlyMatchedObjects.Length * timeLostPerMatch;
        }

        private void Update()
        {
            if (displayedTimer > 0f)
            {
                displayedTimer -= Time.deltaTime;
            }
            else
            {
                displayedTimer = 0f;
                Debug.Log("Time's Up!");
                gridDisplay.SetUnmatchedObjectsHidden(true);
            }
        }
    }
}