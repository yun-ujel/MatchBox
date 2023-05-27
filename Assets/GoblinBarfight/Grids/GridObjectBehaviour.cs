using UnityEngine;

namespace GoblinBarfight.Grids
{
    public class GridObjectBehaviour : MonoBehaviour
    {
        #region Parameters

        #region Moving
        private Vector3 targetPosition;
        private Vector3 velocity;

        private bool isMoving;
        #endregion

        #endregion

        private void Update()
        {
            if (isMoving)
            {
                if (transform.position != targetPosition)
                {
                    transform.position = Vector3.SmoothDamp
                    (
                        transform.position,
                        targetPosition,
                        ref velocity,
                        0.1f
                    );
                }
                else
                {
                    isMoving = false;
                }
            }
        }

        public void MoveToPosition(Vector3 position)
        {
            targetPosition = position;
            isMoving = true;
        }
    }
}