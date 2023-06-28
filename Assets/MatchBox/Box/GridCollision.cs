using UnityEngine;
using System.Collections;

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
            _ = StartCoroutine(nameof(GenerateGrid));
        }

        private IEnumerator GenerateGrid()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            Debug.Log($"Generated Geometry");
            compositeCollider.GenerateGeometry();
        }
    }
}
