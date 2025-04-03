using System;
using System.Collections;
using Unity.XR.CoreUtils.Bindings;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Content.Utils;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

/*
using UnityLabs.Slices.Table;
using UnityLabs.SmartUX.Input.Controller.Binding;
using UnityLabs.SmartUX.SmartAttributes.Attribute;
using UnityLabs.SmartUX.Utils.Wrappers;
*/

namespace UnityLabs.Slices.Games.Chess
{
    /// <summary>
    /// This class should probably be merged in ChessBoardTransition?
    /// </summary>
    public class ChessBoardStartAnimator : MonoBehaviour
    {
        /*
        [Header("References")]
        [SerializeField]
        TableInsetData m_TableInsetData;
        */

        [SerializeField]
        Image m_BoardCenterImage = null;

        [SerializeField]
        RectTransform m_BoardRectTransform = null;

        [SerializeField]
        ParticleBoardJobVisualizer m_Visualzer = null;

        [Header("Animation properties")]
        [SerializeField]
        Vector2 m_BorderWidthStartFinish = new Vector2(0.1f, 0.51f);

        [SerializeField]
        Vector2 m_BorderFillStartFinish = new Vector2(0.05f, 0.3f);

        AnimationCurve m_AnimationCurve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);

        [SerializeField]
        float m_BoardInsetDepth = 12f;

        [SerializeField]
        float m_BoardSmallConstrict = 87.5f;

        [SerializeField]
        float m_BoardLargeConstrict = 50f;

        [Header("Duration - Must add up to 1")] // todo is there way to auto-enforce properties like this adding up to 1?
        [SerializeField]
        float m_BoardOpeningDuration = 0.25f;

        [SerializeField]
        float m_BoardFillOpeningDuration = 0.25f;

        [SerializeField]
        float m_ParticleOpeningDuration = 0.5f;

#pragma warning disable CS0618 // Type or member is obsolete
        FloatTweenableVariable m_StartButtonScaleAttribute = new FloatTweenableVariable();

        FloatTweenableVariable m_BoardWidthAttribute = new FloatTweenableVariable();
        FloatTweenableVariable m_BoardFillAttribute = new FloatTweenableVariable();
        FloatTweenableVariable m_BoardInsetAttribute = new FloatTweenableVariable();
        FloatTweenableVariable m_BoardConstrictAttribute = new FloatTweenableVariable();
        FloatTweenableVariable m_FountainVisualizerWeight = new FloatTweenableVariable();
#pragma warning restore CS0618 // Type or member is obsolete

        readonly BindingsGroup m_BindingGroup = new BindingsGroup();

        void OnEnable()
        {
            m_BindingGroup.AddBinding(m_BoardWidthAttribute.SubscribeAndUpdate(width =>
            {
                m_BoardRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                m_BoardRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, width);
            }));
            m_BindingGroup.AddBinding(m_BoardFillAttribute.SubscribeAndUpdate(alpha =>
            {
                Color newColor = m_BoardCenterImage.color;
                newColor.a = alpha;
                m_BoardCenterImage.color = newColor;
            }));

            m_BoardWidthAttribute.animationCurve = m_AnimationCurve;
            m_BoardFillAttribute.animationCurve = m_AnimationCurve;
            m_BoardConstrictAttribute.animationCurve = m_AnimationCurve;
            m_FountainVisualizerWeight.animationCurve = m_AnimationCurve;

            //m_BindingGroup.AddBinding(m_BoardInsetAttribute.Subscribe(depth => m_TableInsetData.SetDepth(depth)));
            //m_BindingGroup.AddBinding(m_BoardConstrictAttribute.Subscribe(constrict => m_TableInsetData.SetConstrict(constrict)));

            m_BindingGroup.AddBinding(m_FountainVisualizerWeight.Subscribe(weight => m_Visualzer.SetWeight(weight)));
        }

        void OnDisable()
        {
            CloseInsetInstant();
            m_BindingGroup.Clear();
        }

        public IEnumerator StartGame(float duration = 1.0f, Action onComplete = null)
        {
            m_Visualzer.BeginShutdown();

            StartCoroutine(m_BoardConstrictAttribute.PlaySequence(m_BoardSmallConstrict, m_BoardLargeConstrict, duration * m_BoardOpeningDuration));

            yield return m_BoardWidthAttribute.PlaySequence(m_BorderWidthStartFinish.x, m_BorderWidthStartFinish.y, duration * m_BoardOpeningDuration);
            yield return m_BoardFillAttribute.PlaySequence(m_BorderFillStartFinish.y, m_BorderFillStartFinish.x, duration * m_BoardFillOpeningDuration);
            yield return m_FountainVisualizerWeight.PlaySequence(1, 0, duration * m_ParticleOpeningDuration);
            m_Visualzer.ShutDown();
            onComplete?.Invoke();
        }

        public void EndInstant()
        {
            m_StartButtonScaleAttribute.Initialize(1);
            m_BoardWidthAttribute.Initialize(m_BorderWidthStartFinish.y);
            m_BoardFillAttribute.Initialize(m_BorderFillStartFinish.x);
        }

        public void EndGameEnableFountainVisualizer()
        {
            m_Visualzer.hideBeforeLifetime = -1; // If transitioning from board to setup, don't hide then early in their life
            m_Visualzer.Initialize();
        }

        public IEnumerator EndGame(float duration = 1.0f, Action onComplete = null)
        {
            yield return m_FountainVisualizerWeight.PlaySequence(0, 1, duration * m_ParticleOpeningDuration);
            yield return m_BoardFillAttribute.PlaySequence(m_BorderFillStartFinish.x, m_BorderFillStartFinish.y, duration * m_BoardFillOpeningDuration);

            StartCoroutine(m_BoardWidthAttribute.PlaySequence(m_BorderWidthStartFinish.y, m_BorderWidthStartFinish.x, duration * m_BoardOpeningDuration));
            yield return m_BoardConstrictAttribute.PlaySequence(m_BoardLargeConstrict, m_BoardSmallConstrict, duration * m_BoardOpeningDuration);

            onComplete?.Invoke();
        }

        public IEnumerator OpenInset(float duration, Action onComplete = null)
        {
            m_BoardConstrictAttribute.Value = m_BoardLargeConstrict;
            yield return StartCoroutine(m_BoardInsetAttribute.PlaySequence(0, m_BoardInsetDepth, duration, onComplete));
        }

        public void OpenInsetInstant()
        {
            m_BoardInsetAttribute.Initialize(m_BoardInsetDepth);
            m_BoardConstrictAttribute.Initialize(m_BoardLargeConstrict);
        }

        public void CloseInsetInstant()
        {
            m_BoardConstrictAttribute.Initialize(m_BoardSmallConstrict);
            m_BoardInsetAttribute.Initialize(0);
        }

        public void EnableFountainVisualizer(bool state)
        {
            if (m_Visualzer.enabled == state) return;

            if (state)
            {
                m_Visualzer.Initialize();
                // Put a small hide before lifetime so you dont see the tiles on the board
                // before the fountain shoots them up
                m_Visualzer.hideBeforeLifetime = .05f;
                m_Visualzer.SetWeight(1);
            }
            else
            {
                m_Visualzer.ShutDown();
            }
        }
    }
}
