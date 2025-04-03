using Unity.XR.CoreUtils.Bindings;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;
using UnityLabs.Slices.Systems;

namespace UnityLabs.Slices.Environment
{
    /// <summary>
    /// Handles the animated showing/hiding of the default scene background when an additive scene is either loaded or unloaded
    /// </summary>
    public class DefaultBackground : MonoBehaviour
    {
        /*
        [SerializeField]
        AppearanceReplacementSystem m_AppearanceReplacementSystem = null;
        */

        [SerializeField]
        Renderer m_BackgroundRenderer = null;

        /*
        [SerializeField]
        AppearanceSystem m_ApperanceSystem = null;
        */

        [SerializeField]
        [Tooltip("A color other than clear/transparent, will assign a new global override color when replacing background.")]
        Color m_CustomGlobalOverrideColor = Color.clear;

        BindingsGroup m_BindingGroup = new();

        bool m_DisableAfterFadingOut;
        GameObject m_BackgroundContainer;
        Material m_BackgroundMaterialClone;
#pragma warning disable CS0618 // Type or member is obsolete
        FloatTweenableVariable m_BackgroundOpacityAttribute = new FloatTweenableVariable();


        Transform m_BackgroundContainerTransform;
        Vector3 m_HiddenScale = new Vector3(2f, 1.25f, 2f);
        Vector3TweenableVariable m_BackgroundScaleAttribute = new Vector3TweenableVariable();
#pragma warning restore CS0618 // Type or member is obsolete

        readonly int m_BackgroundOpacityPropertyID = Shader.PropertyToID("_Opacity");

        /// <summary>
        /// Make the background visible when no additive scene is loaded, or hidden when an additive scene is loaded
        /// </summary>
        public bool visible
        {
            set
            {
                m_BackgroundOpacityAttribute.target = value ? 1f : 0f;

                m_DisableAfterFadingOut = value ? false : true;

                m_BackgroundScaleAttribute.target = value ? Vector3.one : m_HiddenScale;

                if (!m_DisableAfterFadingOut && !m_BackgroundContainer.activeSelf)
                    m_BackgroundContainer.SetActive(true);
            }
        }

        void OnEnable()
        {
            /*
            if (m_AppearanceReplacementSystem == null)
            {
                Debug.LogError($"Missing appearance replacement system on {this}.");
                enabled = false;
            }
            */
        }

        void Start()
        {
            /*
            m_BindingGroup.AddBinding(m_AppearanceReplacementSystem.hasReplacementBgActive.SubscribeAndUpdate(hasReplacement =>
            {
                if (!hasReplacement && m_ApperanceSystem != null)
                {
                    if (m_CustomGlobalOverrideColor.a != 0f)
                        m_ApperanceSystem.AssignGlobalColor(m_CustomGlobalOverrideColor);

                    // TODO: verify if still needed
                    m_ApperanceSystem.ClearSceneLightColor();
                }

                m_BackgroundOpacityAttribute.target = hasReplacement ? 0f : 1f; // Hide background if a replacement background is present
            }));
            */

            m_BackgroundContainer = m_BackgroundRenderer.gameObject;
            m_BackgroundContainerTransform = m_BackgroundContainer.transform;

            m_BackgroundMaterialClone = m_BackgroundRenderer.material;
            m_BackgroundRenderer.material = m_BackgroundMaterialClone;
            m_BackgroundOpacityAttribute.Value = 1f; // Begin app with background visible
            m_BindingGroup.AddBinding(m_BackgroundOpacityAttribute.SubscribeAndUpdate(newOpacity => m_BackgroundMaterialClone.SetFloat(m_BackgroundOpacityPropertyID, newOpacity)));

            m_BackgroundScaleAttribute.Value = Vector3.one;
            m_BindingGroup.AddBinding(m_BackgroundScaleAttribute.SubscribeAndUpdate(newScale => m_BackgroundContainerTransform.localScale = newScale));
        }

        void OnDestroy()
        {
            m_BindingGroup.Clear();
        }

        void Update()
        {
            if (m_DisableAfterFadingOut && m_BackgroundOpacityAttribute.Value == 0f)
            {
                m_BackgroundContainer.SetActive(false);
                m_BackgroundContainerTransform.localScale = Vector3.one;
                m_BackgroundScaleAttribute.target = Vector3.one;
            }

            m_BackgroundOpacityAttribute.HandleTween(Time.deltaTime * 4f);
            m_BackgroundScaleAttribute.HandleTween(Time.deltaTime * 4f);
        }

        /// <summary>
        /// Allow the AppearanceManager to externally clear any non-default scene appearance overrides
        /// </summary>
        public void AssignDefaultSceneAppearanceProperties()
        {
            /*
            m_ApperanceSystem.AssignGlobalColor(m_CustomGlobalOverrideColor);
            m_ApperanceSystem.ClearSceneLightColor();
            */
        }
    }
}
