using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCut : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject objectToCut;

    private MeshFilter _meshFilter;

    void Start()
    {
        _meshFilter = objectToCut.GetComponent<MeshFilter>();
        if (_meshFilter != null)
        {
            Mesh originalMesh = _meshFilter.mesh;
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

            // 절두체로 메시를 자름
            Mesh insideMesh;
            CutMeshWithFrustum(planes, originalMesh, out insideMesh);

            // 새로운 메시 적용 (절두체 내부)
            if (insideMesh != null)
            {
                GameObject insideObject = new GameObject("InsideMesh");
                insideObject.AddComponent<MeshFilter>().mesh = insideMesh;
                insideObject.AddComponent<MeshRenderer>().material = objectToCut.GetComponent<MeshRenderer>().material;
            }
        }
    }

    // 절두체의 평면과 메시를 사용하여 절단된 메시 생성
    void CutMeshWithFrustum(Plane[] planes, Mesh originalMesh, out Mesh insideMesh)
    {
        // 각 절두체 평면을 사용해 메시를 자름
        // 교차점과 절단 알고리즘을 여기서 구현 (복잡한 부분)

        List<Vector3> insideVertices = new List<Vector3>();
        List<int> insideTriangles = new List<int>();


        Vector3[] vertices = originalMesh.vertices;
        int[] triangles = originalMesh.triangles;

        // 각 삼각형을 검사
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            // 삼각형과 모든 절두체 평면의 교차 여부 확인
            bool isInside = true;
            for (int j = 0; j < planes.Length; j++)
            {
                Plane plane = planes[j];

                if (plane.GetDistanceToPoint(v0) < 0 &&
                    plane.GetDistanceToPoint(v1) < 0 &&
                    plane.GetDistanceToPoint(v2) < 0)
                {
                    // 삼각형의 모든 버텍스가 절두체 바깥에 있으면 무시
                    isInside = false;
                    break;
                }
            }

            // 절두체 내부에 있는 삼각형만 저장
            if (isInside)
            {
                int currentIndex = insideVertices.Count;

                insideVertices.Add(v0);
                insideVertices.Add(v1);
                insideVertices.Add(v2);

                insideTriangles.Add(currentIndex);
                insideTriangles.Add(currentIndex + 1);
                insideTriangles.Add(currentIndex + 2);
            }
        }

        // 자른 메시를 각각 내부/외부 메시로 반환
        insideMesh = new Mesh();
        insideMesh.vertices = insideVertices.ToArray();
        insideMesh.triangles = insideTriangles.ToArray();
    }
}
