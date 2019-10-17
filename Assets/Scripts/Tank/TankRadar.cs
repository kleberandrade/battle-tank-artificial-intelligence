using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankRadar : MonoBehaviour
{
    [SerializeField]
    private float m_ViewRadius;

    [Range(0, 360)]
    public float m_ViewAngle;

    [SerializeField]
    private LayerMask m_TargetMask;
    [SerializeField]
    private LayerMask m_ObstacleMask;

    public readonly List<Vector3> m_Targets = new List<Vector3>();

    [SerializeField]
    private float m_MeshResolution;
    [SerializeField]
    private int m_EdgeResolveIterations;
    [SerializeField]
    private float m_EdgeDstThreshold;

    [SerializeField]
    private float m_MaskCutawayDst = .1f;

    public MeshFilter m_ViewMeshFilter;
    private Mesh m_ViewMesh;

    private void OnEnable()
    {
        m_Targets.Clear();

        StopAllCoroutines();
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    private void OnDisable()
    {
        m_Targets.Clear();

        StopAllCoroutines();
    }

    private void Start()
    {
        m_ViewMesh = new Mesh();
        m_ViewMesh.name = "View Mesh";

        m_ViewMeshFilter.mesh = m_ViewMesh;

        StopAllCoroutines();
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    private IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    private void FindVisibleTargets()
    {
        m_Targets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, m_ViewRadius, m_TargetMask);

        // for (int i = 1 ===> 0
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < m_ViewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, m_ObstacleMask))
                {
                    if (!transform.position.Equals(target.position)) 
                        m_Targets.Add(target.position);
                }
            }
        }
    }

    private void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(m_ViewAngle * m_MeshResolution);
        float stepAngleSize = m_ViewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - m_ViewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > m_EdgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }

            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + Vector3.forward * m_MaskCutawayDst;

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        m_ViewMesh.Clear();

        m_ViewMesh.vertices = vertices;
        m_ViewMesh.triangles = triangles;
        m_ViewMesh.RecalculateNormals();
    }


    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < m_EdgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > m_EdgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }


    private ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, m_ViewRadius, m_ObstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * m_ViewRadius, m_ViewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
