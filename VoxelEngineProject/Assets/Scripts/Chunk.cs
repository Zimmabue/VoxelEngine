using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Chunk : MonoBehaviour
{

    public Vector3Int size { set; get; }
    private Vector2Int _position;
    public Vector2Int position
    {
        set
        {
            _position = value;
            transform.localPosition = new Vector3(_position.x * size.x, 0, _position.y * size.z);
        }
        get
        {
            return _position;
        }
    }
    public int tileSize { get; set; }
    public SurfaceData data { get; set; }

    private MeshRenderer _renderer;
    private MeshFilter _meshFilter;

    private Thread _thread = null;
    private object _locker = new object();

    private Voxel vox;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;
    private Vector3[] normals;
    private Mesh mesh;

    public void Initialize()
    {
        vox = new Voxel(size, 1);
        _renderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
    }

    public void RecalculateTerrain()
    {
        //if (_thread.IsAlive)
        //    _thread.Abort();

        _thread = new Thread(new ThreadStart(_recalculateTerrain));
        _thread.Start();
    }

    private void _recalculateTerrain()
    {
        Vector3Int sizePlusTwo = size + Vector3Int.one * 2;
        Block[,,] vol = new Block[sizePlusTwo.x, sizePlusTwo.y, sizePlusTwo.z];
        for (int y = 0; y < sizePlusTwo.y; y++)
        {
            for (int z = 0; z < sizePlusTwo.z; z++)
            {
                for (int x = 0; x < sizePlusTwo.x; x++)
                {
                    Vector3Int voxelPositionInWorldSpace = new Vector3Int
                        (
                            x + position.x * size.x - 1,
                            y,
                            z + position.y * size.z - 1
                        );
                    
                    Block d = World.instance.GetVoxel(voxelPositionInWorldSpace);
                    
                    vol[x, y, z] = d;
                }
            }
        }

        vox.Create(vol);
        vertices = vox.vertices.ToArray();
        triangles = vox.triangles.ToArray();
        normals = vox.normals.ToArray();
        uvs = vox.uvs.ToArray();
        
        World.instance.AddChunkToDraw(this);
    }

    public void UpdateMesh()
    {
        mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs,
            normals = normals
        };

        mesh.RecalculateTangents();

        _meshFilter.sharedMesh = mesh;
        SetRendererActive(true);
    }

    public void SetRendererActive(bool state)
    {
        _renderer.enabled = state;
    }
}
