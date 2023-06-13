using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using System.Linq;

[CustomEditor(typeof(ActionRebinder))]
public class ActionRebinderEditor : Editor
{
    #region Properties
    private SerializedProperty actionProperty;
    private SerializedProperty bindingIdProperty;

    private GUIContent[] bindingOptions;
    private string[] bindingOptionIds;
    private int selectedBindingIndex;
    #endregion

    private void OnEnable()
    {
        actionProperty = serializedObject.FindProperty("actionReference");
        bindingIdProperty = serializedObject.FindProperty("bindingId");

        RefreshBindingOptions();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(actionProperty);

        int newSelectedBinding = EditorGUILayout.Popup
        (
            new GUIContent("Binding"),
            selectedBindingIndex,
            bindingOptions
        );

        if (newSelectedBinding != selectedBindingIndex)
        {
            string bindingId = bindingOptionIds[newSelectedBinding];
            bindingIdProperty.stringValue = bindingId;
            selectedBindingIndex = newSelectedBinding;
        }

        // Binding Id section

        if (EditorGUI.EndChangeCheck())
        {
            _ = serializedObject.ApplyModifiedProperties();
            RefreshBindingOptions();
        }
    }

    private void RefreshBindingOptions()
    {
        InputActionReference actionReference = (InputActionReference)actionProperty.objectReferenceValue;
        InputAction action = actionReference?.action;

        if (action == null)
        {
            bindingOptions = new GUIContent[0];
            bindingOptionIds = new string[0];
            selectedBindingIndex = -1;
            return;
        }

        ReadOnlyArray<InputBinding> bindings = action.bindings;
        int bindingCount = bindings.Count;

        bindingOptions = new GUIContent[bindingCount];
        bindingOptionIds = new string[bindingCount];
        selectedBindingIndex = -1;

        string currentBindingId = bindingIdProperty.stringValue;
        for (var i = 0; i < bindingCount; ++i)
        {
            InputBinding binding = bindings[i];
            string bindingId = binding.id.ToString();

            #region Configure Display String

            bool bindingInControlSchemes = !string.IsNullOrEmpty(binding.groups);

            #region Set Display String Options
            // If we don't have a binding groups (control schemes), show the device that if there are, for example,
            // there are two bindings with the display string "A", the user can see that one is for the keyboard
            // and the other for the gamepad.
            InputBinding.DisplayStringOptions displayOptions =
                InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.IgnoreBindingOverrides;

            if (!bindingInControlSchemes)
            {
                displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;
            }
            #endregion

            // Create display string.
            string displayString = action.GetBindingDisplayString(i, displayOptions);

            #region Configure for Composites
            // If binding is part of a composite, include the part name.
            if (binding.isPartOfComposite)
            {
                displayString = $"{ObjectNames.NicifyVariableName(binding.name)}: {displayString}";
            }

            // Some composites use '/' as a separator. When used in popup, this will lead to to submenus. Prevent
            // by instead using a backlash.
            displayString = displayString.Replace('/', '\\');
            #endregion

            #region Configure for Control Schemes
            // If the binding is part of control schemes, mention them.
            if (bindingInControlSchemes)
            {
                var asset = action.actionMap?.asset;
                if (asset != null)
                {
                    var controlSchemes = string.Join(", ",
                        binding.groups.Split(InputBinding.Separator)
                            .Select(x => asset.controlSchemes.FirstOrDefault(c => c.bindingGroup == x).name));

                    displayString = $"{displayString} ({controlSchemes})";
                }
            }
            #endregion

            #endregion

            bindingOptions[i] = new GUIContent(displayString);
            bindingOptionIds[i] = bindingId;

            if (currentBindingId == bindingId)
            {
                selectedBindingIndex = i;
            }
        }
    }
}
