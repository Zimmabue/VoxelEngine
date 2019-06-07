using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel {

    //----------------------------------------------------------
    //----------------------------------------------------------
    //----------------------------------------------------------
    /*

 __      __           _ 
 \ \    / /          | |
  \ \  / /____  _____| |
   \ \/ / _ \ \/ / _ \ |
    \  / (_) >  <  __/ |
     \/ \___/_/\_\___|_|

     */
    //----------------------------------------------------------
    //----------------------------------------------------------
    //----------------------------------------------------------

    public List<Vector3> vertices { get; private set; }
    public List<int> triangles { get; private set; }
    public List<Vector2> uvs { get; private set; }
    public List<Vector3> normals { get; private set; }

    private Vector3Int size;
    private float scale;

    public Voxel(Vector3Int size, float scale)
    {
        this.size = size;
        this.scale = scale; // some issues with this.. set always to 1, for now

        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
        normals = new List<Vector3>();
    }

    public void Create(Block[,,] volume)
    {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        normals.Clear();
        
        for (int y = 1; y < size.y + 2 - 1; y++)
        {
            for (int z = 1; z < size.z + 2 - 1; z++)
            {
                for (int x = 1; x < size.x + 2 - 1; x++)
                {
                    if (volume[x, y, z].density == 0)
                        continue;

                    float top = volume[x, y + 1, z].density;
                    float bottom = y == 0 ? 1 : volume[x, y - 1, z].density;

                    float front = z == 0 ? 1 : volume[x, y, z - 1].density;
                    float back = volume[x, y, z + 1].density;

                    float right = volume[x + 1, y, z].density;
                    float left = x == 0 ? 1 : volume[x - 1, y, z].density;

                    int faces = 0;
                    if (top == 0)
                        faces |= FACE_TOP;
                    if (bottom == 0)
                        faces |= FACE_BOTTOM;
                    if (front == 0)
                        faces |= FACE_FRONT;
                    if (back == 0)
                        faces |= FACE_BACK;
                    if (right == 0)
                        faces |= FACE_RIGHT;
                    if (left == 0)
                        faces |= FACE_LEFT;

                    if (faces == 0)
                        continue;
                    
                    Cube(volume[x, y, z].type, faces, new Vector3(x, y, z), scale * 0.5f);

                }
            }
        }
        
    }


    //----------------------------------------------------------
    //----------------------------------------------------------
    //----------------------------------------------------------
    /*
    
  ____  _            _        
 |  _ \| |          | |       
 | |_) | | ___   ___| | _____ 
 |  _ <| |/ _ \ / __| |/ / __|
 | |_) | | (_) | (__|   <\__ \
 |____/|_|\___/ \___|_|\_\___/
      
     */
    //----------------------------------------------------------
    //----------------------------------------------------------
    //----------------------------------------------------------

    public static int FACE_TOP = 1;
    public static int FACE_BOTTOM = 2;
    public static int FACE_FRONT = 4;
    public static int FACE_BACK = 8;
    public static int FACE_RIGHT = 16;
    public static int FACE_LEFT = 32;

    private void Face(BlockType block, int direction, Vector3 position, float scale)
    {
        // TRIANGLES
        int tri = vertices.Count;
        triangles.Add(tri + 0);
        triangles.Add(tri + 2);
        triangles.Add(tri + 1);

        triangles.Add(tri + 2);
        triangles.Add(tri + 3);
        triangles.Add(tri + 1);

        // UVS
        Vector2 uvPos = BlockFabric.GetUV(block);

        float unit = BlockFabric.TEXTURE_SIZE;
        uvs.Add(uvPos + Vector2.zero / unit);
        uvs.Add(uvPos + Vector2.right / unit);
        uvs.Add(uvPos + Vector2.up / unit);
        uvs.Add(uvPos + Vector2.one / unit);

        // VERTICES & NORMALS
        if (direction == FACE_TOP)
        {

            vertices.Add(new Vector3(-1, 1, -1) * scale + position); vertices.Add(new Vector3(1, 1, -1) * scale + position);
            vertices.Add(new Vector3(-1, 1, 1) * scale + position); vertices.Add(new Vector3(1, 1, 1) * scale + position);

            normals.Add(Vector3.up);
            normals.Add(Vector3.up);
            normals.Add(Vector3.up);
            normals.Add(Vector3.up);

        }

        if (direction == FACE_FRONT)
        {

            vertices.Add(new Vector3(-1, -1, -1) * scale + position); vertices.Add(new Vector3(1, -1, -1) * scale + position);
            vertices.Add(new Vector3(-1, 1, -1) * scale + position); vertices.Add(new Vector3(1, 1, -1) * scale + position);

            normals.Add(Vector3.back);
            normals.Add(Vector3.back);
            normals.Add(Vector3.back);
            normals.Add(Vector3.back);

        }

        if (direction == FACE_RIGHT)
        {

            vertices.Add(new Vector3(1, -1, -1) * scale + position); vertices.Add(new Vector3(1, -1, 1) * scale + position);
            vertices.Add(new Vector3(1, 1, -1) * scale + position); vertices.Add(new Vector3(1, 1, 1) * scale + position);

            normals.Add(Vector3.right);
            normals.Add(Vector3.right);
            normals.Add(Vector3.right);
            normals.Add(Vector3.right);

        }

        if (direction == FACE_LEFT)
        {

            vertices.Add(new Vector3(-1, -1, 1) * scale + position); vertices.Add(new Vector3(-1, -1, -1) * scale + position);
            vertices.Add(new Vector3(-1, 1, 1) * scale + position); vertices.Add(new Vector3(-1, 1, -1) * scale + position);

            normals.Add(Vector3.left);
            normals.Add(Vector3.left);
            normals.Add(Vector3.left);
            normals.Add(Vector3.left);

        }

        if (direction == FACE_BACK)
        {

            vertices.Add(new Vector3(1, -1, 1) * scale + position); vertices.Add(new Vector3(-1, -1, 1) * scale + position);
            vertices.Add(new Vector3(1, 1, 1) * scale + position); vertices.Add(new Vector3(-1, 1, 1) * scale + position);

            normals.Add(Vector3.forward);
            normals.Add(Vector3.forward);
            normals.Add(Vector3.forward);
            normals.Add(Vector3.forward);

        }

        if (direction == FACE_BOTTOM)
        {

            vertices.Add(new Vector3(1, -1, -1) * scale + position); vertices.Add(new Vector3(-1, -1, -1) * scale + position);
            vertices.Add(new Vector3(1, -1, 1) * scale + position); vertices.Add(new Vector3(-1, -1, 1) * scale + position);

            normals.Add(Vector3.down);
            normals.Add(Vector3.down);
            normals.Add(Vector3.down);
            normals.Add(Vector3.down);

        }

    }

    public void Cube(BlockType block, int face, Vector3 position, float scale)
    {

        //FACE_TOP
        if ((face & FACE_TOP) == FACE_TOP)
        {
            Face(block, FACE_TOP, position, scale);
        }

        //FACE_BOTTOM
        if ((face & FACE_BOTTOM) == FACE_BOTTOM)
        {
            Face(block, FACE_BOTTOM, position, scale);
        }

        //FACE_FRONT
        if ((face & FACE_FRONT) == FACE_FRONT)
        {
            Face(block, FACE_FRONT, position, scale);
        }

        //FACE_BACK
        if ((face & FACE_BACK) == FACE_BACK)
        {
            Face(block, FACE_BACK, position, scale);
        }

        //FACE_RIGHT
        if ((face & FACE_RIGHT) == FACE_RIGHT)
        {
            Face(block, FACE_RIGHT, position, scale);
        }

        //FACE_LEFT
        if ((face & FACE_LEFT) == FACE_LEFT)
        {
            Face(block, FACE_LEFT, position, scale);
        }
        
    }
    
}
