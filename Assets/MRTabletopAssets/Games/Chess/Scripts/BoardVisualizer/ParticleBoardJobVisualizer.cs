using System.Collections.Generic;
using Transmutable.UI.Content;
using Unity.Collections;
using Unity.XR.CoreUtils.Bindings;
using UnityEngine;
using UnityEngine.Jobs;

namespace UnityLabs.Slices.Games.Chess
{
    public class ParticleBoardJobVisualizer : BoardJobVisualizer
    {
        [SerializeField]
        ParticleSystem m_System = null;

        [Tooltip("Before this specified lifetime of particle, hide it. Set to -1 to disable hiding.")]
        [SerializeField]
        float m_HideBeforeLifetime = 0f;

        [SerializeField]
        float m_PerParticleSystemWeight = .05f;

        [SerializeField]
        [Tooltip("Time before initiating interactive VFX after beginning transition")]
        float m_TransitionInDuration = 1f;

        [SerializeField]
        List<ParticleSystemForceField> m_BaseForceFields = new List<ParticleSystemForceField>();

        List<VisualizerGravityWell> m_GravityWells = new List<VisualizerGravityWell>();

        readonly BindingsGroup m_GravityWellBindings = new BindingsGroup();

        // UserSpawnHandler m_SpawnHandler = null;

        float m_InitializedTime = 0f;

        NativeArray<ParticleSystem.Particle> m_ParticleBuffer;

        //[BurstCompile]
        public struct ParticleVisualizerJob : IJobParallelForTransform
        {
            [ReadOnly]
            public NativeArray<ParticleSystem.Particle> particles;

            [ReadOnly]
            public int readParticleCount;

            [ReadOnly]
            public NativeArray<Pose> basePoses;

            [ReadOnly]
            public float weight;

            [ReadOnly]
            public float hideBeforeLifetime;

            [ReadOnly]
            public float perParticleSystemWeight;

            public void Execute(int i, TransformAccess transform)
            {
                if (weight < 0) return;

                float perParticleMaxWeight = 64.0f + ((1.0f / perParticleSystemWeight) - 1.0f);
                float systemWeight = weight;
                if (perParticleSystemWeight > 0.0f)
                {
                    systemWeight = Mathf.Clamp01(((weight * perParticleMaxWeight) - i) * perParticleSystemWeight);
                    systemWeight = 1.0f - (1.0f - systemWeight) * (1.0f - systemWeight);
                }

                bool showParticle = particles[i].startLifetime - particles[i].remainingLifetime >= hideBeforeLifetime;

                if (i < readParticleCount && showParticle)
                {
                    var particlePose = new Pose(particles[i].position, Quaternion.Euler(particles[i].rotation3D));
                    transform.localPosition = Vector3.Lerp(basePoses[i].position, particlePose.position, systemWeight);
                    transform.localRotation = basePoses[i].rotation * Quaternion.Slerp(Quaternion.identity, particlePose.rotation, systemWeight);
                }
                // Hide the tile, unless hidebeforeLifetime set to negative, which negates the hiding functionality.
                else if (hideBeforeLifetime >= 0)
                {
                    transform.localPosition = new Vector3(0, 25, 0);
                }
            }
        }

        /// <summary>
        /// Before this specified lifetime of particle, hide it. Set to -1 to disable hiding.
        /// </summary>
        public float hideBeforeLifetime { get => m_HideBeforeLifetime; set => m_HideBeforeLifetime = value; }

        public float perParticleSystemWeight { get => m_PerParticleSystemWeight; set => m_PerParticleSystemWeight = value; }

        protected override void Awake()
        {
            m_ParticleBuffer = new NativeArray<ParticleSystem.Particle>(m_BoardGenerator.positionList.Count, Allocator.Persistent);
            m_GravityWells = new List<VisualizerGravityWell>(gameObject.GetComponentsInChildren<VisualizerGravityWell>());
            // m_SpawnHandler = FindObjectOfType<UserSpawnHandler>();
            base.Awake();
        }

        protected override void OnDestroy()
        {
            m_JobHandle.Complete();
            m_ParticleBuffer.Dispose();
            base.OnDestroy();
        }

        public override void Initialize()
        {
            base.Initialize();
            m_System.Play();
            m_InitializedTime = Time.time;
            DestroyGravityWells(true);
        }

        public override void ShutDown()
        {
            base.ShutDown();
            m_System.Clear();
            m_System.Stop();
            DestroyGravityWells();
        }

        void DestroyGravityWells(bool keepBaseFields = false)
        {
            m_System.externalForces.RemoveAllInfluences();
            for (int i = 0; i < m_GravityWells.Count; i++)
            {
                Destroy(m_GravityWells[i].gameObject);
            }

            m_GravityWells.Clear();
            m_GravityWellBindings.Clear();

            if (keepBaseFields)
            {
                for (int i = 0; i < m_BaseForceFields.Count; i++)
                {
                    m_System.externalForces.AddInfluence(m_BaseForceFields[i]);
                }
            }
        }

        void RebuildGravityWells(bool forceReset = false, bool ignoreUserInputWells = false)
        {
            /*
            if (NetworkAvatarManager.Instance == null)
                return;

            if (((NetworkAvatarManager.Instance.AvatarCount * 2) != m_GravityWells.Count))
            {
                DestroyGravityWells(true);

                foreach (NetworkAvatar avatar in NetworkAvatarManager.Instance.Avatars)
                {
                    SlicesNetworkAvatar slicesAvatar = avatar as SlicesNetworkAvatar;

                    if (slicesAvatar == null)
                    {
                        Debug.LogWarning("Encountered non-slices avatar?");
                        continue;
                    }

                    var leftGravityWell = Instantiate(m_VisualizerGravityWellPrefab, m_System.transform);
                    leftGravityWell.isLocallyControlled = slicesAvatar.IsLocalPlayer;
                    m_GravityWellBindings.AddBinding(slicesAvatar.UserLeftHandPose.SubscribeAndUpdate(leftGravityWell.UpdatePoseData));
                    m_GravityWells.Add(leftGravityWell);

                    var rightGravityWell = Instantiate(m_VisualizerGravityWellPrefab, m_System.transform);
                    rightGravityWell.isLocallyControlled = slicesAvatar.IsLocalPlayer;
                    m_GravityWellBindings.AddBinding(slicesAvatar.UserRightHandPose.SubscribeAndUpdate(rightGravityWell.UpdatePoseData));
                    m_GravityWells.Add(rightGravityWell);
                }

                for (int i = 0; i < m_GravityWells.Count; i++)
                {
                    if (m_GravityWells[i].TryGetComponent(out ParticleSystemForceField field))
                    {
                        m_System.externalForces.AddInfluence(field);
                    }
                }
            }
            */
        }

        protected override void UpdateVFX(List<ChessBoardTile> boardList)
        {
            RebuildGravityWells();

            if (Time.time - m_InitializedTime < m_TransitionInDuration)
                return;

            for (int i = 0; i < boardList.Count; i++)
            {
                bool anyAffected = false;
                for (int j = 0; j < m_GravityWells.Count; j++)
                {
                    float sqDist = (m_GravityWells[j].transform.position - boardList[i].transform.position).sqrMagnitude;
                    bool affected = sqDist < m_GravityWells[j].sqEffectRadius * 1.05f;
                    anyAffected = anyAffected || affected;
                }
                boardList[i].isHighlighted = anyAffected;
            }
        }

        protected override void ShutDownVFX(List<ChessBoardTile> boardList)
        {
            DestroyGravityWells(true);
            for (int i = 0; i < boardList.Count; i++)
            {
                boardList[i].isHighlighted = false;
            }
        }

        protected override void ScheduleJob()
        {
            int readParticleCount = m_System.GetParticles(m_ParticleBuffer);
            ParticleVisualizerJob jobData = new ParticleVisualizerJob
            {
                particles = m_ParticleBuffer,
                readParticleCount = readParticleCount,
                weight = m_Weight,
                basePoses = m_BasePoses,
                hideBeforeLifetime = m_HideBeforeLifetime,
                perParticleSystemWeight = m_PerParticleSystemWeight
            };
            m_JobHandle = jobData.Schedule(m_TransformsAccessArray);
        }
    }
}
