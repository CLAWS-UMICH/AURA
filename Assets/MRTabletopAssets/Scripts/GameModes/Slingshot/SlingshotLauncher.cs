using UnityEngine;

[ExecuteInEditMode]
public class SlingshotLauncher : MonoBehaviour
{
    public Transform launchPositionTransform;
    public Vector3 launchForce => m_LaunchForce;
    private Vector3 m_LaunchForce;

    [SerializeField]
    float m_BonusForceMultiplier = 1.0f;

    [SerializeField]
    Vector2 m_MinMaxLaunchScale = new Vector2(0.35f, 1.0f);

    [SerializeField]
    private float m_MaxDistance = 0.05f;

    [SerializeField]
    SlingshotTrajectoryArc m_TrajectoryArc;

    [SerializeField]
    Transform m_Scaler;

    float m_CurrentDistancePercent = 0.0f;

    [SerializeField]
    private Renderer m_Renderer;

    [SerializeField]
    private Color m_StartColor = Color.white;

    [SerializeField]
    private Color m_EndColor = Color.red;

    [SerializeField]
    LineRenderer m_DistanceLineRenderer;

    public MaterialPropertyBlock propertyBlock
    {
        get
        {
            if (m_PropertyBlock == null)
            {
                m_PropertyBlock = new MaterialPropertyBlock();
            }
            return m_PropertyBlock;
        }
    }
    private MaterialPropertyBlock m_PropertyBlock;

    void Update()
    {
        if (m_Renderer != null)
        {
            m_Renderer.GetPropertyBlock(propertyBlock);
            Color lerpedColor = Color.Lerp(m_StartColor, m_EndColor, m_CurrentDistancePercent);
            propertyBlock.SetColor("_BaseColor", lerpedColor);
            m_Renderer.SetPropertyBlock(propertyBlock);
        }
    }

    public void ResetLaunchPosition()
    {
        if (launchPositionTransform != null)
        {
            launchPositionTransform.position = transform.position;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (m_CurrentDistancePercent <= 0.001f)
        {
            m_TrajectoryArc.lineRenderer.enabled = false;
            m_DistanceLineRenderer.enabled = false;

        }
        else
        {
            m_TrajectoryArc.lineRenderer.enabled = true;
            m_DistanceLineRenderer.enabled = true;
        }

        m_Scaler.transform.localScale = Vector3.one * Mathf.Lerp(m_MinMaxLaunchScale.x, m_MinMaxLaunchScale.y, m_CurrentDistancePercent);


        if (launchPositionTransform != null)
        {
            // Check distance, and if beyond max, move back to max
            float currentDistance = Vector3.Distance(launchPositionTransform.position, transform.position);
            if (currentDistance > m_MaxDistance)
            {
                launchPositionTransform.position = transform.position + (launchPositionTransform.position - transform.position).normalized * m_MaxDistance;
            }

            m_CurrentDistancePercent = Mathf.Clamp01(Vector3.Distance(transform.position, launchPositionTransform.position) / m_MaxDistance);
            m_DistanceLineRenderer.SetPosition(0, transform.position);
            m_DistanceLineRenderer.SetPosition(1, launchPositionTransform.position);
            Vector3 aimDirection = (transform.position - launchPositionTransform.position).normalized;

            m_LaunchForce = m_BonusForceMultiplier * m_CurrentDistancePercent * aimDirection;
            m_TrajectoryArc.CalculateTrajectory(transform.position, m_LaunchForce);

            if (aimDirection != Vector3.zero)
                m_DistanceLineRenderer.transform.forward = aimDirection;
        }
        else
        {
            m_CurrentDistancePercent = 0.0f;
            m_LaunchForce = Vector3.zero;

        }
    }
}
