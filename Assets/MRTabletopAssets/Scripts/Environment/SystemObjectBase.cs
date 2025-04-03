using System.Threading.Tasks;
using Unity.XR.CoreUtils.Bindings;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityLabs.Slices.Systems
{
    public abstract class SystemObjectBase : ScriptableObject
    {
        public IReadOnlyBindableVariable<InitializationState> Initialized => m_Initialized;
        protected const string LoggerCategory = "SlicesSystem";
        readonly BindableEnum<InitializationState> m_Initialized = new();
        readonly BindingsGroup m_BindingGroup = new();

        public enum InitializationState
        {
            Initializing,
            Initialized,
            Deinitializing,
            Deinitialized,
        }

        protected void AddBinding(IEventBinding binding)
        {
            m_BindingGroup.AddBinding(binding);
        }

        protected void ClearBindings()
        {
            m_BindingGroup.Clear();
        }

        protected virtual Task OnServiceStart()
        {
            Debug.Log($"OnSystemStart Completed {name}");
            m_Initialized.Value = InitializationState.Initialized;
            return Task.CompletedTask;
        }

        void ServiceStart()
        {
            Debug.Log($"OnSystemStart {name}");
            m_Initialized.Value = InitializationState.Initializing;
            OnServiceStart();
        }

        protected virtual Task OnServiceEnd()
        {
            Debug.Log($"OnSystemEnd Completed {name}");
            ClearBindings();
            m_Initialized.Value = InitializationState.Deinitialized;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Override to implement service cleanup. Settings of service will
        /// not clear between 'Plays' in Unity when Addressable is set to 'Use Asset Database'.
        ///
        /// I think we could do something like save/reload serialized state in the editor if we want to automate?
        /// </summary>
        Task ServiceEnd()
        {
            Debug.Log($"OnSystemEnd {name}");

            // This might happen? It'd be rare. But start logging so we know if it does.
            if (m_Initialized.Value != InitializationState.Initialized)
                Debug.LogError($"Trying to end {name} which is in state { m_Initialized.Value}.");

            m_Initialized.Value = InitializationState.Deinitializing;
            return OnServiceEnd();
        }

#if UNITY_EDITOR
        protected void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayStateChange;
        }

        protected void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayStateChange;
        }

        void OnPlayStateChange(PlayModeStateChange state)
        {
            // To play mode
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                ServiceStart();
            }
            // To editor mode
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                ServiceEnd();
            }
        }
#else
        protected void OnEnable()
        {
            ServiceStart();
        }

        protected void OnDisable()
        {
            OnServiceEnd();
        }
#endif
    }
}
