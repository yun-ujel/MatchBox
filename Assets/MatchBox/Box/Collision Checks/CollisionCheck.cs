using UnityEngine;

namespace MatchBox.Box.CollisionChecks
{
    public abstract class CollisionCheck : ScriptableObject
    {
        public abstract void OnCollisionEnter(Collision2D collision);
        public abstract void OnCollisionStay(Collision2D collision);
        public abstract void OnCollisionExit(Collision2D collision);
    }
}
