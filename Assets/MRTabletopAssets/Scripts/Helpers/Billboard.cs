using UnityEngine;

namespace XRMultiplayer
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField] bool m_WorldUp;
        [SerializeField] bool m_FlipForward;

        [Header("Rotation Thresholds")]
        [SerializeField]
        bool m_UseThresholds = true;

        [SerializeField]
        float m_RotationThresholdInDegrees = 15.0f;

        [SerializeField]
        float m_RotationSpeed = 5.0f;

        Quaternion m_DestinationRotation;

        protected Camera m_Camera;

        private void Awake()
        {
            m_Camera = Camera.main;
        }

        protected virtual void Update()
        {
            Quaternion lookRot = Quaternion.LookRotation(m_Camera.transform.position - transform.position);

            if (m_WorldUp)
            {
                Vector3 offset = lookRot.eulerAngles;
                offset.x = 0;
                offset.z = 0;

                if (m_FlipForward)
                    offset.y += 180;

                lookRot = Quaternion.Euler(offset);
            }

            if (m_UseThresholds)
            {
                if (Quaternion.Angle(transform.rotation, lookRot) > m_RotationThresholdInDegrees)
                    m_DestinationRotation = lookRot;

                if (Quaternion.Angle(transform.rotation, m_DestinationRotation) > 0.01f)
                    transform.rotation = Quaternion.Slerp(transform.rotation, m_DestinationRotation, Time.deltaTime * m_RotationSpeed);
            }
            else
                transform.rotation = lookRot;
        }
    }
}
