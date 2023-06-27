using UnityEngine;

namespace MatchBox.Box
{
    using Grids;

    public class GridCollision : MonoBehaviour
    {
        [SerializeField] private GridDisplay gridDisplay;

        [Space]

        [SerializeField] private CompositeCollider2D compositeCollider;
        private bool queuedGenerateGeometry;

        private void Start()
        {
            gridDisplay.OnGridCollapseEvent += OnGridCollapse;
        }

        private void OnGridCollapse(object sender, GridDisplay.OnGridCollapseEventArgs args)
        {
            if (args.IsCollapsed)
            {
                compositeCollider.enabled = true;

                queuedGenerateGeometry = true;
            }
            else
            {
                compositeCollider.enabled = false;
            }
        }

        private void FixedUpdate()
        {
            if (queuedGenerateGeometry)
            {
                Debug.Log("Generated Geometry");

                compositeCollider.GenerateGeometry();
            }
        }
    }
}
