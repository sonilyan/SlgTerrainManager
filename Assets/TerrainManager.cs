using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

//[ExecuteInEditMode]
public class TerrainManager : MonoBehaviour
{
    private Camera _camera;
    private Plane[] _planes;

    private QTree root;

    public int minSize = 10;
    public int depth = 5;
    
    public int count = 200;
    public bool debugAlways = true;
    public bool debugHideDraw = false;
    public int TestCount;

    public Mesh mesh;
    public Material material;
    
    void Start()
    {
        int mapSize = minSize * (int)Mathf.Pow(2,depth);
        
        Debug.Log($"map size = {mapSize}");
        root = new QTree(Vector2.zero, mapSize, 0);

        IEntity[] es = new IEntity[count];
        for (int i = 0; i < count; i++)
        {
            es[i] = new TestEntity()
            {
                Pos = new Vector2(Random.Range(-mapSize/2, mapSize/2), Random.Range(-mapSize/2, mapSize/2)),
                Type = Random.Range(0, 100),
                Depth = depth,
            };
        }

        _camera = GetComponent<Camera>();

        root.BuildQTree(es);
    }

    void Update()
    {
        _planes = GeometryUtility.CalculateFrustumPlanes(_camera);

        testCount = 0;
        DrawQtree(root);
        TestCount = testCount;
    }

    int testCount = 0;

    private void DrawQtree(QTree qTree)
    {
        if (GeometryUtility.TestPlanesAABB(_planes, qTree.Bounds))
        {
            testCount++;
            foreach (var e in qTree.es)
            {
                DrawEntity(e);
            }

            if (qTree.Child != null)
            {
                foreach (var tree in qTree.Child)
                {
                    DrawQtree(tree);
                }
            }
        }
    }

    private void DrawEntity(IEntity entity)
    {
        if (!debugHideDraw)
            Graphics.DrawMesh(mesh, new Vector3(entity.Pos.x, 0.1f * depth, entity.Pos.y), Quaternion.identity,
                material, 0);
    }

    private void OnDrawGizmos()
    {
        if (_camera == null)
            return;

        GizomoDrawQtree(root);

        Gizmos.color = Color.yellow;

        var pos = transform.position;
        Gizmos.DrawLine(pos, new Vector3(pos.x, 0, pos.z));

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(_camera.aspect, 1.0f, 1.0f));
        Gizmos.DrawFrustum(Vector3.zero, _camera.fieldOfView, _camera.farClipPlane, _camera.nearClipPlane, 1);
    }

    private void GizomoDrawQtree(QTree root)
    {
        if (!GeometryUtility.TestPlanesAABB(_planes, root.Bounds))
            return;

        if (root.depth == depth || debugAlways)
        {
            if (root.depth == depth)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = debugColor[root.depth];
            }

            Gizmos.DrawCube(new Vector3(root.Bounds.center.x, 0.01f * root.depth, root.Bounds.center.z),
                root.Bounds.size - new Vector3(0.1f,0,0.1f));
        }

        if (root.Child == null)
            return;
        foreach (var qTree in root.Child)
        {
            GizomoDrawQtree(qTree);
        }
    }

    private Color[] debugColor = new[]
    {
        Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.grey, Color.magenta, Color.white,
        Color.yellow
    };
}
