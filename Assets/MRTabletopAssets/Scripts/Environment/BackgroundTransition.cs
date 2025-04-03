using Unity.XR.CoreUtils.Bindings;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

namespace UnityLabs.Slices.Environment
{
    public class BackgroundTransition : MonoBehaviour
    {
        /*
        [SerializeField]
        AppearanceSystem m_AppearanceSystem = null;

        [SerializeField]
        AppearanceReplacer m_AppearanceReplacer = null;
        */

        [SerializeField]
        Renderer[] m_BackgroundRenderers = null;

        BindingsGroup m_BindingGroup = new();

        bool m_DestoryAfterFadingOut;
        Material m_BackgroundMaterialClone;

#pragma warning disable CS0618 // Type or member is obsolete
        FloatTweenableVariable m_BackgroundOpacityAttribute = new FloatTweenableVariable();


        Transform m_BackgroundContainerTransform;
        Vector3 m_HiddenScale = new Vector3(2f, 1.25f, 2f);
        Vector3TweenableVariable m_BackgroundScaleAttribute = new Vector3TweenableVariable();
#pragma warning restore CS0618 // Type or member is obsolete

        readonly int m_BackgroundOpacityPropertyID = Shader.PropertyToID("_Opacity");

        /*
        public bool preventAppearanceChangesOnDestory { set { m_AppearanceReplacer.preventAppearanceChangesOnDestory = value; } }
        */

        public bool visible
        {
            set
            {
                m_BackgroundOpacityAttribute.target = value ? 1f : 0f;
                m_BackgroundScaleAttribute.target = value ? Vector3.one : m_HiddenScale;
                m_DestoryAfterFadingOut = !value;
            }
        }

        void Start()
        {
            m_BackgroundContainerTransform = transform;

            m_BackgroundMaterialClone = m_BackgroundRenderers[0].material;
            m_BackgroundOpacityAttribute.Value = 0f;
            m_BindingGroup.AddBinding(m_BackgroundOpacityAttribute.SubscribeAndUpdate(newOpacity => m_BackgroundMaterialClone.SetFloat(m_BackgroundOpacityPropertyID, newOpacity)));

            m_BackgroundScaleAttribute.Value = Vector3.one;
            m_BindingGroup.AddBinding(m_BackgroundScaleAttribute.SubscribeAndUpdate(newScale => m_BackgroundContainerTransform.localScale = newScale));

            foreach (var backgroundRenderer in m_BackgroundRenderers)
            {
                backgroundRenderer.material = m_BackgroundMaterialClone;
            }

            //m_AppearanceSystem.PrepareAdditiveSceneBackground(this);
        }

        void OnDestroy()
        {
            m_BindingGroup.Clear();
        }

        void Update()
        {
            m_BackgroundOpacityAttribute.HandleTween(Time.deltaTime * 4f);
            m_BackgroundScaleAttribute.HandleTween(Time.deltaTime * 4f);

            if (m_DestoryAfterFadingOut && m_BackgroundOpacityAttribute.Value == 0f)
                Destroy(gameObject);
        }
    }
}
