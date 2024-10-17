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

            // ����ü�� �޽ø� �ڸ�
            Mesh insideMesh;
            CutMeshWithFrustum(planes, originalMesh, out insideMesh);

            // ���ο� �޽� ���� (����ü ����)
            if (insideMesh != null)
            {
                GameObject insideObject = new GameObject("InsideMesh");
                insideObject.AddComponent<MeshFilter>().mesh = insideMesh;
                insideObject.AddComponent<MeshRenderer>().material = objectToCut.GetComponent<MeshRenderer>().material;
            }
        }
    }

    // ����ü�� ���� �޽ø� ����Ͽ� ���ܵ� �޽� ����
    void CutMeshWithFrustum(Plane[] planes, Mesh originalMesh, out Mesh insideMesh)
    {
        // �� ����ü ����� ����� �޽ø� �ڸ�
        // �������� ���� �˰����� ���⼭ ���� (������ �κ�)

        List<Vector3> insideVertices = new List<Vector3>();
        List<int> insideTriangles = new List<int>();


        Vector3[] vertices = originalMesh.vertices;
        int[] triangles = originalMesh.triangles;

        // �� �ﰢ���� �˻�
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            // �ﰢ���� ��� ����ü ����� ���� ���� Ȯ��
            bool isInside = true;
            for (int j = 0; j < planes.Length; j++)
            {
                Plane plane = planes[j];

                if (plane.GetDistanceToPoint(v0) < 0 &&
                    plane.GetDistanceToPoint(v1) < 0 &&
                    plane.GetDistanceToPoint(v2) < 0)
                {
                    // �ﰢ���� ��� ���ؽ��� ����ü �ٱ��� ������ ����
                    isInside = false;
                    break;
                }
            }

            // ����ü ���ο� �ִ� �ﰢ���� ����
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

        // �ڸ� �޽ø� ���� ����/�ܺ� �޽÷� ��ȯ
        insideMesh = new Mesh();
        insideMesh.vertices = insideVertices.ToArray();
        insideMesh.triangles = insideTriangles.ToArray();
    }
}
