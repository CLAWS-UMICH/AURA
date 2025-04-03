using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityLabs.SmartUX.Interaction.Widgets
{
    public class XRToggle : XRBaseInteractable
    {
        [SerializeField]
        bool m_DefaultToggleState = false;

        public BindableVariable<bool> isToggled { get; } = new BindableVariable<bool>();

        public BindableVariable<bool> onToggleSelectChanged { get; } = new BindableVariable<bool>();

        public bool toggleEnabled
        {
            set => isToggled.Value = value;
        }

        protected override void Awake()
        {
            base.Awake();
            isToggled.Value = m_DefaultToggleState;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            selectEntered.AddListener(OnSelectEntered);
            selectExited.AddListener(OnSelectExited);
        }

        protected override void OnDisable()
        {
            selectEntered.RemoveListener(OnSelectEntered);
            selectExited.RemoveListener(OnSelectExited);
            base.OnDisable();
        }

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
        private void OnSelectEntered(SelectEnterEventArgs args)

        {
            onToggleSelectChanged.Value = true;
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            isToggled.Value = !isToggled.Value;
            onToggleSelectChanged.Value = false;
        }
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
    }
}
