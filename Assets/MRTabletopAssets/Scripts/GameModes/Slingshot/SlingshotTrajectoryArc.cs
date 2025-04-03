using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class SlingshotTrajectoryArc : MonoBehaviour
{
    [SerializeField]
    private int m_NumberOfPoints = 50;

    public LineRenderer lineRenderer
    {
        get
        {
            if (m_LineRenderer == null)
                m_LineRenderer = GetComponent<LineRenderer>();
            return m_LineRenderer;
        }
    }

    [SerializeField]
    private LineRenderer m_LineRenderer;

    [SerializeField]
    [Range(0.001f, .1f)]
    private float m_TimeBetweenPoints = 0.1f;

    [SerializeField]
    private float m_GravityModifier = 0.075f;

    [SerializeField]
    private float m_PointRadius = .01f;

    [SerializeField]
    private float m_Mass = 1f;

    [SerializeField]
    Vector3 m_Force;

    [SerializeField]
    private LayerMask m_CollisionMask;

    [SerializeField]
    bool m_CalculateEachFrame = false;

    private void Awake()
    {
        if (m_LineRenderer == null)
            m_LineRenderer = GetComponent<LineRenderer>();
    }

    void OnValidate()
    {
        m_TimeBetweenPoints = Mathf.Clamp(m_TimeBetweenPoints, 0.001f, .1f);
    }

    void Update()
    {
        if (m_CalculateEachFrame)
        {
            CalculateTrajectory(transform.position, m_Force);
        }
    }

    [ContextMenu("Test Trajectory")]
    public void TestTrajectory()
    {
        CalculateTrajectory(transform.position, transform.forward * 10);
    }

    [SerializeField]
    private float m_MaxDistance = 10f;

    public void CalculateTrajectory(Vector3 startPosition, Vector3 force)
    {
        Vector3 velocity = force / m_Mass;
        List<Vector3> points = new List<Vector3>();
        lineRenderer.positionCount = 0;
        float distanceTraveled = 0f;

        for (float t = 0; t < m_NumberOfPoints; t += m_TimeBetweenPoints)
        {
            Vector3 point = startPosition + t * velocity;

            point.y = startPosition.y + velocity.y * t + Physics.gravity.y * m_GravityModifier / 2f * t * t;

            if (points.Count > 0)
                distanceTraveled += Vector3.Distance(points[points.Count - 1], point);

            if (distanceTraveled > m_MaxDistance)
                break;

            points.Add(point);

            if (Physics.OverlapSphere(point, m_PointRadius, m_CollisionMask).Length > 0)
            {
                break;
            }
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
