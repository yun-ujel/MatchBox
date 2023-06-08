using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ActionRebinderDisplay : MonoBehaviour
{
    [SerializeField] private ActionRebinder actionRebinder;

    [Space]

    [SerializeField] private TextMeshProUGUI bindingText;

    private void Awake()
    {
        actionRebinder.OnStartRebinding += OnStartRebind;
        actionRebinder.OnStopRebinding += OnCompleteRebind;
    }

    private void OnStartRebind(object sender, ActionRebinder.InteractiveRebindingEventArgs args)
    {
        bindingText.text = "Waiting For Input...";
    }

    private void Start()
    {
        UpdateDisplay(actionRebinder.InputAction, actionRebinder.BindingIndex);
    }

    private void OnCompleteRebind(object sender, ActionRebinder.InteractiveRebindingEventArgs args)
    {
        UpdateDisplay(args.action, args.bindingIndex);
    }

    private void UpdateDisplay(InputAction action, int bindingIndex)
    {
        if (action != null)
        {
            bindingText.text = action.GetBindingDisplayString(bindingIndex);
        }
    }
}
