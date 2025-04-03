using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace UnityLabs.Slices.Games.Chess
{
    public class WinBoardJobVisualizer : BoardJobVisualizer
    {
        [SerializeField]
        float m_HeightMultiplier = .01f;

        [SerializeField]
        float m_curveMultiplier = 2f;

        [SerializeField]
        float m_CurvePower = 3f;

        [SerializeField]
        float m_RippleMultiplier = 1f;

        [SerializeField]
        float m_RippleHeight = 0f;

        [SerializeField]
        float m_RippleOffset = 0f;

        public float rippleOffset
        {
            get => m_RippleOffset;
            set => m_RippleOffset = value;
        }

        public float rippleHeight
        {
            get => m_RippleHeight;
            set => m_RippleHeight = value;
        }

        Transform m_WinningPiece;

        //[BurstCompile]
        public struct WinJob : IJobParallelForTransform
        {
            [ReadOnly] public float weight;
            [ReadOnly] public float heightMultiplier;
            [ReadOnly] public Vector3 losingPiecePosition;
            [ReadOnly] public float curveMultiplier;
            [ReadOnly] public float curvePower;

            [ReadOnly] public float rippleMultipler;
            [ReadOnly] public float rippleHeight;
            [ReadOnly] public float rippleOffset;

            public void Execute(int i, TransformAccess transform)
            {
                if (weight < 0) return;

                float distance = (transform.localPosition - losingPiecePosition).sqrMagnitude;
                float heightFromPiece = Mathf.Pow(distance * curveMultiplier, curvePower) * heightMultiplier;

                float putMult = Mathf.PI / rippleMultipler;
                float rippleCos = 0f;
                if (distance < rippleOffset + putMult && distance > rippleOffset - putMult)
                {
                    rippleCos = Mathf.Cos((distance - rippleOffset) * rippleMultipler) + 1f;
                    rippleCos *= rippleHeight;
                }

                transform.localPosition = new Vector3(transform.localPosition.x, (heightFromPiece * weight) + rippleCos, transform.localPosition.z);
            }
        }

        public void SetWinningPiece(Transform winningPiece)
        {
            m_WinningPiece = winningPiece;
        }

        protected override void UpdateVFX(List<ChessBoardTile> boardList)
        {
        }

        protected override void ShutDownVFX(List<ChessBoardTile> boardList)
        {
        }

        protected override void ScheduleJob()
        {
            WinJob jobData = new WinJob
            {
                weight = m_Weight,
                heightMultiplier = m_HeightMultiplier,
                losingPiecePosition = m_WinningPiece == null ? Vector3.zero : m_WinningPiece.localPosition,
                curveMultiplier = m_curveMultiplier,
                curvePower = m_CurvePower,
                rippleMultipler = m_RippleMultiplier,
                rippleHeight = m_RippleHeight,
                rippleOffset = m_RippleOffset,
            };
            m_JobHandle = jobData.Schedule(m_TransformsAccessArray);
        }
    }
}
