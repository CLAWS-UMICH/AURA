using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace UnityLabs.Slices.Games.Chess
{
    public abstract class BoardJobVisualizer : MonoBehaviour
    {
        [SerializeField]
        protected float m_Weight = 0;

        [SerializeField]
        protected ChessBoardGenerator m_BoardGenerator = null;

        [SerializeField]
        protected Transform m_BoardBaseTransform = null;

        protected TransformAccessArray m_TransformsAccessArray;
        protected JobHandle m_JobHandle;
        protected NativeArray<Pose> m_BasePoses;
        Transform[] m_SquareTransforms;

        bool m_IsShuttingDown = false;

        protected virtual void Awake()
        {
            m_BasePoses = new NativeArray<Pose>(m_BoardGenerator.positionList.Count, Allocator.Persistent);
            m_SquareTransforms = new Transform[m_BoardGenerator.positionList.Count];
            for (var i = 0; i < m_BoardGenerator.positionList.Count; i++)
            {
                m_SquareTransforms[i] = m_BoardGenerator.positionList[i].transform;
                var basePose = m_BasePoses[i];
                basePose.position = m_SquareTransforms[i].localPosition;
                basePose.rotation = m_SquareTransforms[i].localRotation;
                m_BasePoses[i] = basePose;
            }

            m_TransformsAccessArray = new TransformAccessArray(m_SquareTransforms);

            // Set weight to -1 initially so that the job that does not actually move the transforms
            // otherwise it may mess up initialization for other transforms
            m_Weight = -1;

            // Schedule first job to cache all the GC, otherwise makes a spike when Initialize is called.
            ScheduleJob();

            enabled = false;
        }

        protected virtual void OnDestroy()
        {
            m_JobHandle.Complete();
            m_BasePoses.Dispose();
            m_TransformsAccessArray.Dispose();
        }

        public virtual void Initialize()
        {
            enabled = true;
            m_IsShuttingDown = false;
        }

        public virtual void BeginShutdown()
        {
            ShutDownVFX(m_BoardGenerator.positionList);
            m_IsShuttingDown = true;
        }

        public virtual void ShutDown()
        {
            enabled = false;
            m_JobHandle.Complete();
            ShutDownVFX(m_BoardGenerator.positionList);
            for (var i = 0; i < m_SquareTransforms.Length; i++)
            {
                var child = m_SquareTransforms[i];
                child.localPosition = m_BasePoses[i].position;
                child.localRotation = m_BasePoses[i].rotation;
                child.localScale = Vector3.one;
                m_SquareTransforms[i] = child;
            }
        }

        public void SetWeight(float weight)
        {
            m_Weight = weight;
        }

        protected virtual void LateUpdate()
        {
            // Call complete before schedule because its actually completing the job
            // from the prior frame, done so it lets the job run one whole cycle
            m_JobHandle.Complete();
            if (!m_IsShuttingDown)
            {
                UpdateVFX(m_BoardGenerator.positionList);
            }
            ScheduleJob();
        }

        protected abstract void UpdateVFX(List<ChessBoardTile> boardList);

        protected abstract void ShutDownVFX(List<ChessBoardTile> boardList);

        protected abstract void ScheduleJob();
    }
}
