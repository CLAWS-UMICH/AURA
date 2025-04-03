using Unity.XR.CoreUtils.Bindings;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine;
using UnityEngine.XR.Content.Utils;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

namespace UnityLabs.Slices.Games.Chess
{
    public class ChessBoardTile : MonoBehaviour
    {
        [SerializeField]
        ChessColor m_ChessColor = ChessColor.Black;

        public ChessColor chessColor { get => m_ChessColor; }

        [SerializeField]
        Renderer m_Renderer = null;

        [SerializeField]
        Color m_WhiteColor = Color.white;

        [SerializeField]
        Color m_WhiteHighlightColor = Color.cyan;

        [SerializeField]
        Color m_BlackColor = Color.black;

        [SerializeField]
        Color m_BlackHighlightColor = Color.grey;

        [SerializeField]
        float m_TileWidth = 0.05f;

        ChessSquare m_Square;

        public ChessSquare chessSquare { get => m_Square; }

        Vector3 m_InitialLocalPosition;

        public Vector3 initialLocalPosition { get => m_InitialLocalPosition; }

        public float tileWidth => m_TileWidth;

#pragma warning disable CS0618 // Type or member is obsolete
        readonly ColorTweenableVariable m_ColorAttribute = new ColorTweenableVariable();
#pragma warning restore CS0618 // Type or member is obsolete


        readonly BindingsGroup m_BindingGroup = new BindingsGroup();

        MaterialPropertyBlock m_PropertyBlock;

        int m_ColorPropertyId;

        Color GetColorTarget()
        {
            Color target;
            if (m_ChessColor == ChessColor.Black)
            {
                target = m_IsHighlighted ? m_BlackHighlightColor : m_BlackColor;
            }
            else
            {
                target = m_IsHighlighted ? m_WhiteHighlightColor : m_WhiteColor;
            }

            return target;
        }

        bool m_IsHighlighted = false;

        public bool isHighlighted
        {
            get => m_IsHighlighted;
            set
            {
                if (m_IsHighlighted == value)
                    return;

                m_IsHighlighted = value;
                m_ColorAttribute.target = GetColorTarget();
                //Debug.Log("Square Highlighted " + m_Square + " - " + m_IsHighlighted);
            }
        }

        public void Initialize(ChessSquare chessSquare, ChessColor color, Vector3 localPosition, Transform parent)
        {
            transform.SetParent(parent);
            transform.localPosition = localPosition;
            m_InitialLocalPosition = localPosition;
            m_Square = chessSquare;
            m_ChessColor = color;
            m_ColorAttribute.Initialize(GetColorTarget());
        }

        void Awake()
        {
            m_PropertyBlock = new MaterialPropertyBlock();
            m_ColorPropertyId = Shader.PropertyToID("_Color");
        }

        void OnEnable()
        {
            m_BindingGroup.AddBinding(m_ColorAttribute.SubscribeAndUpdate(newColor =>
            {
                m_Renderer.GetPropertyBlock(m_PropertyBlock);
                m_PropertyBlock.SetColor(m_ColorPropertyId, newColor);
                m_Renderer.SetPropertyBlock(m_PropertyBlock);
            }));
        }

        void OnDisable()
        {
            m_BindingGroup.Clear();
        }

        void Update()
        {
            m_ColorAttribute.HandleTween(Time.deltaTime * 8f);
        }
    }
}
