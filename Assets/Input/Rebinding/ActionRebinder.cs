using UnityEngine;
using UnityEngine.InputSystem;

public class ActionRebinder : MonoBehaviour
{
    #region Properties

    #region Serialized Properties
    [Tooltip("Reference to the Input Action that can have one of its bindings rebound.")]
    [SerializeField] private InputActionReference actionReference;

    public InputAction InputAction => actionReference.action;

    [Tooltip("Reference to the specific binding that will be replaced.")]
    [SerializeField] private string bindingId;

    public int BindingIndex => InputAction.bindings.IndexOf(x => x.id.ToString() == bindingId);
    #endregion

    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    public class InteractiveRebindingEventArgs : System.EventArgs
    {
        public InputAction action;
        public int bindingIndex;

        public InteractiveRebindingEventArgs(InputAction action, int bindingIndex)
        {
            this.action = action;
            this.bindingIndex = bindingIndex;
        }
    }

    public event System.EventHandler<InteractiveRebindingEventArgs> OnStartRebinding;

    public event System.EventHandler<InteractiveRebindingEventArgs> OnStopRebinding;

    #endregion

    public void ResetToDefault()
    {
        if (!TryGetActionAndBinding(out InputAction action, out int bindingIndex))
        {
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            for (int i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
            {
                action.RemoveBindingOverride(i);
            }
        }
        else
        {
            action.RemoveBindingOverride(bindingIndex);
        }

        OnStopRebinding?.Invoke(this, new InteractiveRebindingEventArgs(action, bindingIndex));
    }

    public void StartInteractiveRebind()
    {
        if (!TryGetActionAndBinding(out InputAction action, out int bindingIndex))
        {
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            int firstPartIndex = bindingIndex + 1;
            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
            {
                PerformInteractiveRebind(action, firstPartIndex, true);
            }
        }
        else
        {
            PerformInteractiveRebind(action, bindingIndex);
        }
    }

    private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool runThroughFullComposite = false)
    {
        rebindOperation?.Cancel();

        void CleanUp()
        {
            rebindOperation?.Dispose();
            rebindOperation = null;
        }

        action.Disable();

        rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .OnMatchWaitForAnother(0.01f)
            .OnCancel
            (
                operation =>
                {
                    action.Enable();
                    OnStopRebinding?.Invoke(this, new InteractiveRebindingEventArgs(action, bindingIndex));
                    CleanUp();
                }
            )
            .OnComplete
            (
                operation =>
                {
                    action.Enable();
                    OnStopRebinding?.Invoke(this, new InteractiveRebindingEventArgs(action, bindingIndex));
                    CleanUp();

                    if (runThroughFullComposite)
                    {
                        int nextBindingIndex = bindingIndex + 1;
                        if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                        {
                            PerformInteractiveRebind(action, nextBindingIndex, true);
                        }
                    }
                }
            );

        OnStartRebinding?.Invoke(this, new InteractiveRebindingEventArgs(action, bindingIndex));
        rebindOperation.Start();
    }

    private bool TryGetActionAndBinding(out InputAction action, out int bindingIndex)
    {
        bindingIndex = -1;

        action = actionReference?.action;
        if (action == null)
        {
            Debug.LogError("Cannot find an Input Action to rebind", this);
            return false;
        }

        if (string.IsNullOrEmpty(bindingId))
        {
            Debug.LogError($"Cannot find an Binding to replace on '{action}'", this);
            return false;
        }

        // Get Binding Index
        System.Guid searchBindingId = new System.Guid(bindingId);
        bindingIndex = action.bindings.IndexOf(x => x.id == searchBindingId);
        if (bindingIndex == -1)
        {
            Debug.LogError($"Cannot find binding with ID '{bindingId}' on '{action}'", this);
            return false;
        }

        return true;
    }
}
