using UnityEngine;

namespace MatchBox.Grids
{
    public class GridSelectDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridSelect gridSelect;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Movement")]
        [SerializeField] private float smoothTime = 0.02f;
        private Vector3 targetPosition;
        private Vector3 smoothDampVelocity;

        [Header("Sprites")]
        [SerializeField] private SelectDisplay unaligned;
        [SerializeField] private SelectDisplay aligned;
        [SerializeField] private SelectDisplay selected;

        private SelectDisplay lastSetDisplay;
        private bool isSelected;

        [System.Serializable]
        private class SelectDisplay
        {
            public Sprite sprite;
            public Vector2 size;
            public Color color;
        }

        private void Start()
        {
            gridSelect.OnMoveEvent += OnMove;
            gridSelect.OnSelectEvent += OnSelect;
        }

        private void OnSelect(object sender, GridSelect.OnSelectEventArgs args)
        {
            transform.position = targetPosition;
            isSelected = args.IsSelected;
        }

        private void OnMove(object sender, GridSelect.OnMoveEventArgs args)
        {
            targetPosition = args.targetWorldPosition;

            if ((args.startGridPosition - args.targetGridPosition).sqrMagnitude > 4f)
            {
                transform.position = targetPosition;
            }
        }

        private void Update()
        {
            transform.position = Vector3.SmoothDamp
                (
                    transform.position,
                    targetPosition,
                    ref smoothDampVelocity,
                    smoothTime
                );
            if (transform.position != targetPosition)
            {
                SetDisplay(unaligned);
            }
            else
            {
                if (isSelected)
                {
                    SetDisplay(selected);
                }
                else
                {
                    SetDisplay(aligned);
                }
            }
        }

        private void SetDisplay(SelectDisplay display)
        {
            if (lastSetDisplay != display)
            {
                spriteRenderer.sprite = display.sprite;
                spriteRenderer.size = display.size;
                spriteRenderer.color = display.color;

                lastSetDisplay = display;
            }
        }
    }

}