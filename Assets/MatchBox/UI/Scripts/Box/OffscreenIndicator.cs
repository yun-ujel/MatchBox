using UnityEngine.UI;
using UnityEngine;

namespace MatchBox.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class OffscreenIndicator : MonoBehaviour
    {
        #region Parameters
        private RectTransform rectTransform;

        #region Serialized
        [SerializeField] private Canvas canvas;
        [SerializeField] private Camera uICamera;

        [Space]

        [SerializeField] private Graphic graphic;

        [Space]

        [SerializeField] private Transform referenceTransform;
        [SerializeField] private float displayedTargetDistance;
        #endregion

        #region Calculations

        #region Camera Bounds / Clamping
        private Vector2 targetWorldPosition;

        private Vector2 clampedPosition;
        private Vector2 displayedClampPosition;

        private Vector2 worldScale;

        private Vector2 worldCameraBoundsMax;
        private Vector2 worldCameraBoundsMin;
        #endregion

        private Vector2 smoothDampVelocity;
        
        private float targetDistance;
        private float smoothDampTargetDistanceVelocity;

        #endregion


        #endregion
        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();

            Vector2 cameraPosition = uICamera.transform.position;
            Vector2 worldCameraSize = uICamera.GetWorldSize();

            worldCameraBoundsMax = (worldCameraSize * 0.5f) + cameraPosition;
            worldCameraBoundsMin = -(worldCameraSize * 0.5f) + cameraPosition;

            worldScale = rectTransform.sizeDelta / GetCanvasPixelsPerUnit(canvas);
        }

        private void Update()
        {
            targetWorldPosition = referenceTransform.position;

            if (TargetPositionIsOffscreen())
            {
                graphic.enabled = true;
            }
            else
            {
                graphic.enabled = false;
            }
        }

        private void FixedUpdate()
        {
            clampedPosition = new Vector2
            (
                Mathf.Clamp(targetWorldPosition.x, worldCameraBoundsMin.x, worldCameraBoundsMax.x),
                Mathf.Clamp(targetWorldPosition.y, worldCameraBoundsMin.y, worldCameraBoundsMax.y)
            );

            displayedClampPosition = new Vector2
            (
                Mathf.Clamp(targetWorldPosition.x, worldCameraBoundsMin.x + (worldScale.x * 0.5f), worldCameraBoundsMax.x - (worldScale.x * 0.5f)),
                Mathf.Clamp(targetWorldPosition.y, worldCameraBoundsMin.y + (worldScale.y * 0.5f), worldCameraBoundsMax.y - (worldScale.y * 0.5f))
            );

            transform.position = Vector2.SmoothDamp
            (
                transform.position,
                displayedClampPosition,
                ref smoothDampVelocity,
                0.02f
            );

            if (TargetPositionIsOffscreen())
            {
                GetDirectionAndMagnitude((targetWorldPosition - clampedPosition), out float distance, out Vector2 normalized);

                targetDistance = distance;
                transform.right = normalized;

                displayedTargetDistance = Mathf.SmoothDamp(displayedTargetDistance, targetDistance, ref smoothDampTargetDistanceVelocity, 0.01f);
            }
        }

        private Vector2 GetCanvasPixelsPerUnit(Canvas canvas)
        {
            RectTransform rectTransform = canvas.GetComponent<RectTransform>();
            return rectTransform.sizeDelta / canvas.worldCamera.GetWorldSize();
        }

        private bool TargetPositionIsOffscreen()
        {
            return targetWorldPosition.x < worldCameraBoundsMin.x
                || targetWorldPosition.y < worldCameraBoundsMin.y
                || targetWorldPosition.x > worldCameraBoundsMax.x
                || targetWorldPosition.y > worldCameraBoundsMax.y;
        }

        private void GetDirectionAndMagnitude(Vector2 vector, out float magnitude, out Vector2 normalized)
        {
            magnitude = vector.magnitude;
            normalized = vector / magnitude;
        }
    }

}