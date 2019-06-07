using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    #region Singleton
    public static World instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    public Transform target;
    public Chunk prefab;
    public Vector3Int chunkSize;
    public int viewDistance;
    public int tileSize;
    public int worldHeight;

    public float timerUpdate;

    public SurfaceData surface { get; set; }

    private Vector2Int currentTargetChunkPosition;
    private List<Chunk> chunks;
    private Stack<Chunk> notVisibleChunks;
    private Queue<Chunk> chunksToProcess;
    private Queue<Chunk> chunksToDraw;
    private float currentCountDownUpdate;

    // Start is called before the first frame update
    void Start()
    {
        currentCountDownUpdate = timerUpdate;
        currentTargetChunkPosition = Vector2Int.one * int.MaxValue;

        chunks = new List<Chunk>();
        notVisibleChunks = new Stack<Chunk>();
        chunksToProcess = new Queue<Chunk>();
        chunksToDraw = new Queue<Chunk>();
    }

    private object _locker = new object();
    public void AddChunkToDraw(Chunk chunk)
    {
        lock (_locker)
            chunksToDraw.Enqueue(chunk);
    }

    // Update is called once per frame
    void Update()
    {
        //--------------------------------------------------
        //--------PROCESSING-AND-SHOWING-CHUNKS-------------
        //--------------------------------------------------
        if (chunksToDraw.Count > 0)
        {
            // draw a chunk
            chunksToDraw.Dequeue().UpdateMesh();

            // process the next chunk
            if (chunksToProcess.Count > 0)
                chunksToProcess.Dequeue().RecalculateTerrain();
        }

        //--------------------------------------------------
        //--------CHEKING-FOR-CHUNKS-UPDATE-----------------
        //--------------------------------------------------
        Vector2Int nextTargetChunkPosition = new Vector2Int
           (
               Mathf.FloorToInt(target.position.x / chunkSize.x),
               Mathf.FloorToInt(target.position.z / chunkSize.z)
           );
        
        if (nextTargetChunkPosition == currentTargetChunkPosition)
        {
            currentCountDownUpdate = timerUpdate;
            return;
        }

        //
        currentCountDownUpdate -= Time.deltaTime;
        if (currentCountDownUpdate > 0)
            return;
        currentCountDownUpdate = timerUpdate;

        //--------------------------------------------------
        //--------UPDATE-NOT-VISIBLE-CHUNKS-----------------
        //--------------------------------------------------
        for (int i = 0; i < chunks.Count; i++)
        {
            var c = chunks[i];
            if (IsInBound(nextTargetChunkPosition, viewDistance, c.position))
                continue;

            c.SetRendererActive(false);
            chunks.RemoveAt(i);
            notVisibleChunks.Push(c);
        }
        
        //--------------------------------------------------
        //--------PLACING-NEW-CHUNKS------------------------
        //--------------------------------------------------
        for (int z = 0; z < viewDistance * 2; z++)
        {
            for (int x = 0; x < viewDistance * 2; x++)
            {
                Vector2Int positionToAnalize = new Vector2Int
                    (
                        x - viewDistance,
                        z - viewDistance
                    ) + nextTargetChunkPosition;

                if (IsInBound(nextTargetChunkPosition, viewDistance, positionToAnalize) &&
                    IsInBound(currentTargetChunkPosition, viewDistance, positionToAnalize))
                    continue;

                SetChunk(positionToAnalize);
            }
        }
        
        // this starts the chunks update process
        if (chunksToDraw.Count == 0)
            chunksToProcess.Dequeue().RecalculateTerrain();

        currentTargetChunkPosition = nextTargetChunkPosition;
    }

    private void SetChunk(Vector2Int position)
    {
        Chunk chunk;

        // check if there are any chunks in the notVisibleChunks pool, than re-use the first
        if (notVisibleChunks.Count > 0)
            chunk = notVisibleChunks.Pop();
        else
        // otherwise, create a new one
        {
            chunk = Instantiate(prefab);
            chunk.tileSize = tileSize;
            chunk.size = chunkSize;
            chunk.transform.SetParent(transform);
            chunk.Initialize();
        }
        
        chunk.data = surface;
        chunk.position = position;
        chunks.Add(chunk);

        chunksToProcess.Enqueue(chunk);
    }

    private bool IsInBound(Vector2Int center, int size, Vector2Int contain)
    {
        return
            contain.x > center.x - size && contain.x < center.x + size &&
            contain.y > center.y - size && contain.y < center.y + size;
    }

    public Block GetVoxel(Vector3Int position)
    {
        // Surface pass
        double xCoord = position.x / (double)tileSize;
        double zCoord = position.z / (double)tileSize;
        Block surfaceBlock = surface.GetBlockBilinear(xCoord, zCoord);

        float heightOfTheSurfaceBlock = Mathf.Floor(surfaceBlock.density * worldHeight);
        
        // hard-coded for now
        // below the surface
        if (position.y < heightOfTheSurfaceBlock)
            return new Block(BlockType.Rock, 1);

        //above the surface
        if (position.y > heightOfTheSurfaceBlock)
            return new Block(BlockType.Air, 0);

        // on the surface
        return new Block(surfaceBlock.type, 1);
    }

    public void LoadSurfaceData(string path)
    {

        float[] data = Storage.StorageManager.GetDataAccess().Load<float[]>(path);
        Block[] blocks = new Block[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            blocks[i] = new Block((BlockType)Mathf.FloorToInt(data[i]), MathHelper.Fract(data[i]));
        }

        // hard-coded size, it will be fix soon
        surface = new SurfaceData(1024, blocks);

    }

}
