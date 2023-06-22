using UnityEngine.InputSystem;
using UnityEngine;

namespace MatchBox.Box.Capabilities
{
    [RequireComponent(typeof(BoxPlayerHandler))]
    public abstract class Capability : MonoBehaviour
    {
        protected BoxPlayerHandler BoxPlayer;

        public virtual void Awake()
        {
            BoxPlayer = GetComponent<BoxPlayerHandler>();
        }
    }
}