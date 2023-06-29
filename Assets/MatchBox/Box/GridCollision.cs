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
            gridDisplay.OnGridObjectRegeneratedEvent += OnObjectRegenerated;
        }

        private void OnObjectRegenerated(object sender, GridDisplay.OnGridObjectRegeneratedEventArgs args)
        {
            _ = StartCoroutine(nameof(GenerateGrid));
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
