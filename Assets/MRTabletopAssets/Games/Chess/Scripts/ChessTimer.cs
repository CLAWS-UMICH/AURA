using System;
using TMPro;
using Unity.XR.CoreUtils.Bindings;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.XR.Content.Utils;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

namespace UnityLabs.Slices.Games.Chess
{
    public class ChessTimer : MonoBehaviour
    {
        [SerializeField]
        Transform m_ButtonTransform = null;

        [SerializeField]
        Transform m_ButtonCheckTransform = null;

        [SerializeField]
        float m_TurnButtonOffset;

        [SerializeField]
        float m_TurnCheckButtonOffset;

        [SerializeField]
        float m_OnTurnTextOffset;

        [SerializeField]
        float m_OffTurnTextOffset;

        [SerializeField]
        float m_OnTurnTextSize;

        [SerializeField]
        float m_OffTurnTextSize;

        [SerializeField]
        TextMeshProUGUI m_BlackChessTimer = null;

        [SerializeField]
        TextMeshProUGUI m_WhiteChessTimer = null;

        [SerializeField]
        Color m_TextColor;

        [SerializeField]
        Color m_SubTextColor;

#pragma warning disable CS0618 // Type or member is obsolete
        readonly FloatTweenableVariable m_ButtonPositionAttribute = new FloatTweenableVariable();

        readonly FloatTweenableVariable m_CheckButtonPositionAttribute = new FloatTweenableVariable();
        readonly FloatTweenableVariable m_WhiteTextPositionAttribute = new FloatTweenableVariable();
        readonly FloatTweenableVariable m_BlackTextPositionAttribute = new FloatTweenableVariable();
        readonly FloatTweenableVariable m_WhiteTextSizeAttribute = new FloatTweenableVariable();
        readonly FloatTweenableVariable m_BlackTextSizeAttribute = new FloatTweenableVariable();
        readonly BindingsGroup m_BindingGroup = new BindingsGroup();
#pragma warning restore CS0618 // Type or member is obsolete

        void OnEnable()
        {
            m_BindingGroup.AddBinding(m_ButtonPositionAttribute.SubscribeAndUpdate(newPos => m_ButtonTransform.localPosition = m_ButtonTransform.localPosition.SetAxis(newPos, Axis.Y)));

            m_BindingGroup.AddBinding(m_CheckButtonPositionAttribute.SubscribeAndUpdate(newPos => m_ButtonCheckTransform.localPosition = m_ButtonCheckTransform.localPosition.SetAxis(newPos, Axis.Y)));

            m_BindingGroup.AddBinding(m_WhiteTextPositionAttribute.SubscribeAndUpdate(newPos => m_WhiteChessTimer.transform.localPosition = m_WhiteChessTimer.transform.localPosition.SetAxis(newPos, Axis.X)));
            m_BindingGroup.AddBinding(m_WhiteTextSizeAttribute.SubscribeAndUpdate(newSize => m_WhiteChessTimer.fontSize = newSize));

            m_BindingGroup.AddBinding(m_BlackTextPositionAttribute.SubscribeAndUpdate(newPos => m_BlackChessTimer.transform.localPosition = m_BlackChessTimer.transform.localPosition.SetAxis(newPos, Axis.X)));
            m_BindingGroup.AddBinding(m_BlackTextSizeAttribute.SubscribeAndUpdate(newSize => m_BlackChessTimer.fontSize = newSize));

            m_WhiteChessTimer.text = string.Empty;
            m_BlackChessTimer.text = string.Empty;
        }

        void OnDisable()
        {
            m_BindingGroup.Clear();
        }

        void Update()
        {
            float lerpAmt = Time.deltaTime * 8f;
            m_ButtonPositionAttribute.HandleTween(lerpAmt);
            m_CheckButtonPositionAttribute.HandleTween(lerpAmt);
            m_WhiteTextPositionAttribute.HandleTween(lerpAmt);
            m_BlackTextPositionAttribute.HandleTween(lerpAmt);
            m_WhiteTextSizeAttribute.HandleTween(lerpAmt);
            m_BlackTextSizeAttribute.HandleTween(lerpAmt);
        }

        public void RefreshVisuals(ChessColor currentTurn)
        {
            m_ButtonPositionAttribute.target = currentTurn == ChessColor.Black ? m_TurnButtonOffset : -m_TurnButtonOffset;
            m_CheckButtonPositionAttribute.target = currentTurn == ChessColor.Black ? m_TurnCheckButtonOffset : -m_TurnCheckButtonOffset;
            m_WhiteTextPositionAttribute.target = currentTurn == ChessColor.Black ? m_OffTurnTextOffset : m_OnTurnTextOffset;
            m_WhiteTextSizeAttribute.target = currentTurn == ChessColor.Black ? m_OffTurnTextSize : m_OnTurnTextSize;
            m_BlackTextPositionAttribute.target = currentTurn == ChessColor.Black ? m_OnTurnTextOffset : m_OffTurnTextOffset;
            m_BlackTextSizeAttribute.target = currentTurn == ChessColor.Black ? m_OnTurnTextSize : m_OffTurnTextSize;
        }

        public void UpdateTimerText(ChessColor currentTurn, float time)
        {
            var span = TimeSpan.FromSeconds(time);
            string text = $"{(int)span.TotalMinutes}:{span.Seconds:00}";
            if (currentTurn == ChessColor.White)
                m_WhiteChessTimer.text = text;
            else
                m_BlackChessTimer.text = text;
        }

        public void SetInfinityText()
        {
            const string infinity = "∞";
            m_WhiteChessTimer.text = infinity;
            m_BlackChessTimer.text = infinity;
            m_WhiteChessTimer.color = m_SubTextColor;
            m_BlackChessTimer.color = m_SubTextColor;
        }
    }
}
