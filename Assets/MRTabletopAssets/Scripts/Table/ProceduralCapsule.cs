using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace XRMultiplayer
{
    [ExecuteAlways]
    [BurstCompile]
    public class ProceduralCapsule : MonoBehaviour
    {
        [Header("Child Mesh Components")]
        [SerializeField]
        Transform m_CapsuleCentroidChildTransform = null;

        [SerializeField]
        MeshFilter m_MeshFilter;

        [SerializeField]
        MeshRenderer m_MeshRenderer;

        public MeshRenderer meshRenderer
        {
            get => m_MeshRenderer;
            set => m_MeshRenderer = value;
        }

        [Header("Capsule Settings")]
        [Range(0.001f, 10f)]
        [SerializeField]
        float m_Radius = 0.5f;

        public float radius
        {
            get => m_Radius;
            set => m_Radius = value;
        }

        [SerializeField, Range(0.001f, 100f)]
        float m_Height = 2f;

        public float height
        {
            get => m_Height;
            set => m_Height = value;
        }

        [SerializeField, Range(3, 100)]
        int m_Segments = 16; // Number of sides around the capsule

        public int segments
        {
            get => m_Segments;
            set => m_Segments = value;
        }

        [SerializeField, Range(2, 100)]
        int m_Rings = 8; // Number of segments along the capsule

        public int rings
        {
            get => m_Rings;
            set => m_Rings = value;
        }

        [Header("Bending Settings")]
        [SerializeField, Range(-1f, 1f)]
        float m_BendAmount = 0f; // Bend amount parameter (-1 to 1)

        public float bendAmount
        {
            get => m_BendAmount;
            set { m_BendAmount = value; }
        }

        [SerializeField, Range(0.001f, float.MaxValue)]
        float m_CornerRadius = 4f;

        public float cornerRadius
        {
            get => m_CornerRadius;
            set => m_CornerRadius = value;
        }

        [SerializeField, Range(0f, 180f)]
        float m_MaxBendAngle = 90f;

        public float maxBendAngle
        {
            get => m_MaxBendAngle;
            set => m_MaxBendAngle = value;
        }

        public enum BendDirection
        {
            Left,
            Right
        }

        [SerializeField]
        BendDirection m_BendDirection = BendDirection.Right;

        public BendDirection bendDirection
        {
            get => m_BendDirection;
            set => m_BendDirection = value;
        }

        public bool isVisible
        {
            get
            {
                return m_MeshRenderer != null && m_MeshRenderer.enabled;
            }
            set => m_MeshRenderer.enabled = value;
        }

        Spline m_Spline;

        // Caching previous values for dirty checks
        float m_PrevRadius;
        float m_PrevHeight;
        int m_PrevSegments;
        int m_PrevRings;
        float m_PrevBendAmount;
        float m_PrevCornerRadius;
        float m_PrevMaxBendAngle;
        BendDirection m_PrevBendDirection;

        Mesh m_CylinderMesh;
        Mesh m_FinalMesh;
        Mesh m_StartHemisphereMesh;
        Mesh m_EndHemisphereMesh;

        bool m_IsDirty = true;
        float m_TotalLength;

        NativeArray<float3> m_HemisphereVertices;
        NativeArray<float3> m_HemisphereNormals;
        NativeArray<float2> m_HemisphereUVs;
        NativeArray<int> m_HemisphereIndices;

        int m_PrevStartHemisphereHash;
        int m_PrevEndHemisphereHash;

        void Start()
        {
            CreateCapsuleMesh();
        }

#if UNITY_EDITOR
        bool m_IsEditorDirty = false;

        void OnValidate()
        {
            m_IsEditorDirty = true;
            if (!Application.isPlaying)
                UnityEditor.EditorApplication.update += EditorUpdate;
        }

        void EditorUpdate()
        {
            if (!m_IsEditorDirty)
                return;

            CreateCapsuleMesh();
            m_IsEditorDirty = false;

            if (!Application.isPlaying)
                UnityEditor.EditorApplication.update -= EditorUpdate;
        }
#endif

        void OnDestroy()
        {
            if (m_FinalMesh != null)
                DestroyUnityObject(m_FinalMesh);
            if (m_CylinderMesh != null)
                DestroyUnityObject(m_CylinderMesh);

            if (m_StartHemisphereMesh != null)
                DestroyUnityObject(m_StartHemisphereMesh);
            if (m_EndHemisphereMesh != null)
                DestroyUnityObject(m_EndHemisphereMesh);

            if (m_HemisphereVertices.IsCreated)
                m_HemisphereVertices.Dispose();
            if (m_HemisphereNormals.IsCreated)
                m_HemisphereNormals.Dispose();
            if (m_HemisphereUVs.IsCreated)
                m_HemisphereUVs.Dispose();
            if (m_HemisphereIndices.IsCreated)
                m_HemisphereIndices.Dispose();
        }

        void DestroyUnityObject(UnityEngine.Object obj)
        {
            if (Application.isPlaying)
                Destroy(obj);
#if UNITY_EDITOR
            else
                DestroyImmediate(obj);
#endif
        }

        void Update()
        {
            CreateCapsuleMesh();
        }

        bool CheckDirty()
        {
            if (!Mathf.Approximately(m_Radius, m_PrevRadius) ||
                !Mathf.Approximately(m_Height, m_PrevHeight) ||
                m_Segments != m_PrevSegments ||
                m_Rings != m_PrevRings ||
                !Mathf.Approximately(m_BendAmount, m_PrevBendAmount) ||
                !Mathf.Approximately(m_CornerRadius, m_PrevCornerRadius) ||
                !Mathf.Approximately(m_MaxBendAngle, m_PrevMaxBendAngle) ||
                m_BendDirection != m_PrevBendDirection)
            {
                m_IsDirty = true;
                m_PrevRadius = m_Radius;
                m_PrevHeight = m_Height;
                m_PrevSegments = m_Segments;
                m_PrevRings = m_Rings;
                m_PrevBendAmount = m_BendAmount;
                m_PrevCornerRadius = m_CornerRadius;
                m_PrevMaxBendAngle = m_MaxBendAngle;
                m_PrevBendDirection = m_BendDirection;
            }

            return m_IsDirty;
        }

        int ComputeHemisphereHash(float4x4 matrix)
        {
            unchecked // Allow integer overflow
            {
                int hash = 17;
                hash = hash * 31 + m_Radius.GetHashCode();
                hash = hash * 31 + m_Segments.GetHashCode();
                hash = hash * 31 + matrix.GetHashCode();
                return hash;
            }
        }

        void CreateCapsuleMesh()
        {
            if (!isVisible || !CheckDirty())
                return;

            if (m_FinalMesh == null)
                m_FinalMesh = new Mesh();

            if (m_CylinderMesh == null)
                m_CylinderMesh = new Mesh();

            if (m_MeshRenderer == null)
                m_MeshRenderer = m_CapsuleCentroidChildTransform.GetComponent<MeshRenderer>();

            if (m_MeshFilter == null)
                m_MeshFilter = m_CapsuleCentroidChildTransform.GetComponent<MeshFilter>();

            CreateSpline();

            int sides = m_Segments;
            int segmentsAlongSpline = m_Rings;
            bool capped = false;

            SplineMesh.Extrude(m_Spline, m_CylinderMesh, m_Radius, sides, segmentsAlongSpline, capped);

            float3 startPosition = m_Spline.EvaluatePosition(0f);
            float3 startTangent = m_Spline.EvaluateTangent(0f);
            float3 endPosition = m_Spline.EvaluatePosition(1f);
            float3 endTangent = m_Spline.EvaluateTangent(1f);

            // Transform the hemispheres to align with the cylinder's ends
            quaternion startRotation = quaternion.LookRotationSafe(-startTangent, math.up());
            float4x4 startMatrix = float4x4.TRS(startPosition, startRotation, new float3(1f));

            quaternion endRotation = quaternion.LookRotationSafe(endTangent, math.up());
            float4x4 endMatrix = float4x4.TRS(endPosition, endRotation, new float3(1f));

            // Compute hashes for start and end hemispheres
            int startHemisphereHash = ComputeHemisphereHash(startMatrix);
            int endHemisphereHash = ComputeHemisphereHash(endMatrix);

            // Check if we need to regenerate the start hemisphere
            if (m_StartHemisphereMesh == null || startHemisphereHash != m_PrevStartHemisphereHash)
            {
                GenerateHemisphere(ref m_StartHemisphereMesh, m_Radius, m_Segments, startMatrix);
                m_PrevStartHemisphereHash = startHemisphereHash;
            }

            // Check if we need to regenerate the end hemisphere
            if (m_EndHemisphereMesh == null || endHemisphereHash != m_PrevEndHemisphereHash)
            {
                GenerateHemisphere(ref m_EndHemisphereMesh, m_Radius, m_Segments, endMatrix);
                m_PrevEndHemisphereHash = endHemisphereHash;
            }

            // Combine the cylinder and hemisphere meshes
            CombineInstance[] combine = new CombineInstance[3];
            combine[0].mesh = m_CylinderMesh;
            combine[0].transform = Matrix4x4.identity;
            combine[1].mesh = m_StartHemisphereMesh;
            combine[1].transform = Matrix4x4.identity;
            combine[2].mesh = m_EndHemisphereMesh;
            combine[2].transform = Matrix4x4.identity;

            m_FinalMesh.CombineMeshes(combine, true, true, false);
            m_FinalMesh.RecalculateBounds();

            // Compute the centroid of the capsule
            float3 centroid = m_Spline.EvaluatePosition(0.5f);

            m_CapsuleCentroidChildTransform.localPosition = -centroid;

            // Assign the final mesh to the MeshFilter
            m_MeshFilter.sharedMesh = m_FinalMesh;

            m_IsDirty = false;
        }

        void CreateSpline()
        {
            m_Spline = new Spline
            {
                Closed = false
            };

            float bendDirMultiplier = math.sign(m_BendAmount) * (m_BendDirection == BendDirection.Right ? -1f : 1f);

            // Calculate the actual bend angle based on the absolute value of bendAmount
            float actualBendAngle = math.abs(m_BendAmount) * m_MaxBendAngle;
            float actualBendAngleRad = math.radians(actualBendAngle);

            float bendRadius = m_CornerRadius;

            // Ensure bend radius is sufficient
            bendRadius = math.max(bendRadius, 0.001f);

            // Total length to be distributed between the straight segments and the bend
            m_TotalLength = m_Height;

            // Calculate the maximum possible angle
            float maxPossibleAngleRad = m_TotalLength / bendRadius;
            float maxPossibleAngleDegrees = math.degrees(maxPossibleAngleRad);

            // Clamp the bend angle
            actualBendAngle = math.clamp(actualBendAngle, 0f, maxPossibleAngleDegrees);
            actualBendAngleRad = math.radians(actualBendAngle);

            // Recalculate arc length after clamping
            float arcLength = bendRadius * actualBendAngleRad;

            // Adjust the straight segments' lengths to keep total length constant
            float remainingLength = math.max(0f, m_TotalLength - arcLength);
            float straightLength = remainingLength * 0.5f;

            int totalKnots = m_Rings + 1;
            var knotPositions = new NativeArray<float3>(totalKnots, Allocator.Temp);

            if (math.abs(actualBendAngle) < 0.0001f)
            {
                // If bend angle is zero, the path is a straight line
                float3 pointA = new float3(-0.5f * m_TotalLength, 0f, 0f);
                float3 pointB = new float3(0.5f * m_TotalLength, 0f, 0f);

                for (int i = 0; i < totalKnots; i++)
                {
                    float t = (float)i / (totalKnots - 1); // t from 0 to 1
                    float3 position = math.lerp(pointA, pointB, t);
                    knotPositions[i] = position;
                }
            }
            else if (m_BendAmount > 0f)
            {
                // Positive bendAmount
                float3 pointA = new float3(-0.5f * m_TotalLength, 0f, 0f);
                float3 pointC = pointA + new float3(straightLength, 0f, 0f);

                float bendEndX = bendRadius * math.sin(actualBendAngleRad);
                float bendEndZ = bendDirMultiplier * bendRadius * (1f - math.cos(actualBendAngleRad));

                float3 bendEndPoint = pointC + new float3(bendEndX, 0f, bendEndZ);
                float3 pointB = bendEndPoint + new float3(straightLength * math.cos(actualBendAngleRad), 0f,
                    straightLength * math.sin(actualBendAngleRad) * bendDirMultiplier);

                // Accumulated lengths to determine the segment transitions
                float totalPathLength = straightLength + arcLength + straightLength;
                float accumulatedLength1 = straightLength;
                float accumulatedLength2 = accumulatedLength1 + arcLength;

                // Generate knot positions using the Burst-compiled static method
                GenerateKnotPositions(pointA, pointC, bendEndPoint, pointB, bendRadius, actualBendAngleRad, bendDirMultiplier,
                    straightLength, arcLength, totalPathLength, accumulatedLength1, accumulatedLength2, totalKnots, false, ref knotPositions);
            }
            else
            {
                // Negative bendAmount
                float3 pointA = new float3(0.5f * m_TotalLength, 0f, 0f);
                float3 pointC = pointA - new float3(straightLength, 0f, 0f);

                float bendEndX = bendRadius * math.sin(actualBendAngleRad);
                float bendEndZ = bendDirMultiplier * bendRadius * (1f - math.cos(actualBendAngleRad));

                float3 bendEndPoint = pointC - new float3(bendEndX, 0f, bendEndZ);
                float3 pointB = bendEndPoint - new float3(straightLength * math.cos(actualBendAngleRad), 0f,
                    straightLength * math.sin(actualBendAngleRad) * bendDirMultiplier);

                // Accumulated lengths to determine the segment transitions
                float totalPathLength = straightLength + arcLength + straightLength;
                float accumulatedLength1 = straightLength;
                float accumulatedLength2 = accumulatedLength1 + arcLength;

                // Generate knot positions using the Burst-compiled static method
                GenerateKnotPositions(pointA, pointC, bendEndPoint, pointB, bendRadius, actualBendAngleRad, bendDirMultiplier,
                    straightLength, arcLength, totalPathLength, accumulatedLength1, accumulatedLength2, totalKnots, true, ref knotPositions);
            }

            // === Build the Spline with the Knot Positions ===

            m_Spline.Clear();

            for (int i = 0; i < knotPositions.Length; i++)
            {
                BezierKnot knot = new BezierKnot(knotPositions[i]);
                m_Spline.Add(knot, TangentMode.AutoSmooth);
            }

            // Dispose the NativeArray to free up memory
            knotPositions.Dispose();

            // Recalculate rotations based on spline tangents
            for (int i = 0; i < m_Spline.Count; i++)
            {
                BezierKnot knot = m_Spline[i];

                // Calculate the parameter t along the spline (0 to 1)
                float t = m_Spline.Count > 1 ? (float)i / (m_Spline.Count - 1) : 0f;

                // Evaluate the tangent at this point along the spline
                float3 tangent = m_Spline.EvaluateTangent(t);

                // Ensure the tangent is not zero to avoid errors
                if (math.lengthsq(tangent) == 0f)
                {
                    // Use the previous valid tangent or default forward vector
                    if (i > 0)
                        tangent = m_Spline.EvaluateTangent((float)(i - 1) / (m_Spline.Count - 1));
                    else
                        tangent = new float3(1f, 0f, 0f);
                }

                quaternion rotation = quaternion.LookRotationSafe(tangent, math.up());
                knot.Rotation = rotation;

                m_Spline.SetKnotNoNotify(i, knot);
            }
        }

        void GenerateHemisphere(ref Mesh hemiSphereMesh, float sphereRadius, int sphereSegments, float4x4 matrix)
        {
            if (hemiSphereMesh == null)
                hemiSphereMesh = new Mesh();

            hemiSphereMesh.Clear();

            int vertexCount = (sphereSegments + 1) * (sphereSegments + 1);
            int indexCount = sphereSegments * sphereSegments * 6;

            if (!m_HemisphereVertices.IsCreated || m_HemisphereVertices.Length != vertexCount)
            {
                if (m_HemisphereVertices.IsCreated) m_HemisphereVertices.Dispose();
                m_HemisphereVertices = new NativeArray<float3>(vertexCount, Allocator.Persistent);
            }

            if (!m_HemisphereNormals.IsCreated || m_HemisphereNormals.Length != vertexCount)
            {
                if (m_HemisphereNormals.IsCreated) m_HemisphereNormals.Dispose();
                m_HemisphereNormals = new NativeArray<float3>(vertexCount, Allocator.Persistent);
            }

            if (!m_HemisphereUVs.IsCreated || m_HemisphereUVs.Length != vertexCount)
            {
                if (m_HemisphereUVs.IsCreated) m_HemisphereUVs.Dispose();
                m_HemisphereUVs = new NativeArray<float2>(vertexCount, Allocator.Persistent);
            }

            if (!m_HemisphereIndices.IsCreated || m_HemisphereIndices.Length != indexCount)
            {
                if (m_HemisphereIndices.IsCreated) m_HemisphereIndices.Dispose();
                m_HemisphereIndices = new NativeArray<int>(indexCount, Allocator.Persistent);
            }

            GenerateHemisphereData(sphereRadius, sphereSegments, matrix, ref m_HemisphereVertices, ref m_HemisphereNormals, ref m_HemisphereUVs, ref m_HemisphereIndices);

            hemiSphereMesh.SetVertices(m_HemisphereVertices.Reinterpret<Vector3>());
            hemiSphereMesh.SetNormals(m_HemisphereNormals.Reinterpret<Vector3>());
            hemiSphereMesh.SetUVs(0, m_HemisphereUVs);
            hemiSphereMesh.SetIndices(m_HemisphereIndices, MeshTopology.Triangles, 0);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                m_HemisphereVertices.Dispose();
                m_HemisphereNormals.Dispose();
                m_HemisphereUVs.Dispose();
                m_HemisphereIndices.Dispose();
            }
#endif
        }

        [BurstCompile]
        static void GenerateKnotPositions(
            in float3 pointA, in float3 pointC, in float3 bendEndPoint, in float3 pointB,
            float bendRadius, float actualBendAngleRad, float bendDirMultiplier,
            float straightLength, float arcLength, float totalPathLength,
            float accumulatedLength1, float accumulatedLength2,
            int totalKnots, bool invertBend, ref NativeArray<float3> knotPositions)
        {
            for (int i = 0; i < totalKnots; i++)
            {
                float t = (float)i / (totalKnots - 1); // t from 0 to 1
                float targetDistance = t * totalPathLength;

                float3 position;

                if (straightLength > 0f && targetDistance <= accumulatedLength1)
                {
                    // On the straight segment before the bend
                    float segmentT = targetDistance / straightLength;
                    position = math.lerp(pointA, pointC, segmentT);
                }
                else if (targetDistance <= accumulatedLength2)
                {
                    // On the bend
                    float bendDistance = targetDistance - accumulatedLength1;
                    float angle = bendDistance / arcLength * actualBendAngleRad;

                    float x = bendRadius * math.sin(angle);
                    float z = bendDirMultiplier * bendRadius * (1f - math.cos(angle));

                    if (invertBend)
                    {
                        // Adjust positions for negative bendAmount
                        position = pointC - new float3(x, 0f, z);
                    }
                    else
                    {
                        position = pointC + new float3(x, 0f, z);
                    }
                }
                else if (straightLength > 0f)
                {
                    // On the straight segment after the bend
                    float segmentDistance = targetDistance - accumulatedLength2;
                    float segmentT = segmentDistance / straightLength;

                    float3 start = bendEndPoint;
                    float3 end = pointB;

                    position = math.lerp(start, end, segmentT);
                }
                else
                {
                    // If straightLength is zero, position remains at the end of the bend
                    position = bendEndPoint;
                }

                knotPositions[i] = position;
            }
        }

        [BurstCompile]
        static void GenerateHemisphereData(
            float radius,
            int segments,
            in float4x4 matrix,
            ref NativeArray<float3> vertices,
            ref NativeArray<float3> normals,
            ref NativeArray<float2> uvs,
            ref NativeArray<int> indices)
        {
            int vertexCount = 0;
            int indexCount = 0;

            for (int y = 0; y <= segments; y++)
            {
                float phi = math.PI * y / (2 * segments); // from 0 to PI/2

                for (int x = 0; x <= segments; x++)
                {
                    float theta = 2 * math.PI * x / segments;

                    // Positions
                    float xPos = radius * math.sin(phi) * math.cos(theta);
                    float yPos = radius * math.sin(phi) * math.sin(theta);
                    float zPos = radius * math.cos(phi);

                    float3 vertex = new float3(xPos, yPos, zPos);
                    // Apply transformation to the vertex
                    float4 transformedVertex = math.mul(matrix, new float4(vertex, 1f));
                    vertices[vertexCount] = transformedVertex.xyz;

                    // Normals
                    float3 normal = math.normalize(vertex);
                    // Apply transformation to the normal (without translation)
                    float4 transformedNormal = math.mul(matrix, new float4(normal, 0f));
                    normals[vertexCount] = math.normalize(transformedNormal.xyz);

                    // UVs
                    uvs[vertexCount] = new float2((float)x / segments, (float)y / segments);
                    vertexCount++;
                }
            }

            // Indices generation remains the same
            for (int y = 0; y < segments; y++)
            {
                for (int x = 0; x < segments; x++)
                {
                    int current = y * (segments + 1) + x;
                    int next = current + segments + 1;

                    // First triangle of quad
                    indices[indexCount++] = current;
                    indices[indexCount++] = next;
                    indices[indexCount++] = current + 1;

                    // Second triangle of quad
                    indices[indexCount++] = current + 1;
                    indices[indexCount++] = next;
                    indices[indexCount++] = next + 1;
                }
            }
        }
    }
}
