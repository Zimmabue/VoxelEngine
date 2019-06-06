using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
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
    private MeshCollider _meshCollider;

    private Thread _thread = null;
    private object _locker = new object();

    private Voxel vox;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;
    private Vector3[] normals;
    
    public void Initialize()
    {
        vox = new Voxel(size, 1);
        _renderer = GetComponent<MeshRenderer>();
        _meshCollider = GetComponent<MeshCollider>();
        _meshFilter = GetComponent<MeshFilter>();
    }

    public void RecalculateTerrain()
    {
        _thread = new Thread(new ThreadStart(_recalculateTerrain));
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
                    int ix = x + position.x * size.x;
                    int iz = z + position.y * size.z;
                    int iy = y * size.y;

                    //double xCoord = (ix - 1) / (double)tileSize;
                    //double zCoord = (iz - 1) / (double)tileSize;
                    Block d = World.instance.GetVoxel(new Vector3Int(ix - 1, iy, iz - 1));//surface.GetBlockBilinear(xCoord, zCoord);
                    //float fh = Mathf.Floor(d.density * tileSize);

                    vol[x, y, z] = d;//iy > fh ? new Block(BlockType.Air, 0) : new Block(d.type, 1);
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
        Mesh mesh = new Mesh
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
