using UnityEngine;

public enum BlockType : byte
{
    Air,
    Grass,
    Cobblestone,
    Rock,
    RockDark,
    Sand
}

[System.Serializable]
public struct Block
{
    public BlockType type;
    public float density;

    public Block(BlockType type, float density)
    {
        this.type = type;
        this.density = density;
    }
}

public static class BlockType_Extention
{
    public static bool IsTransparent(this BlockType block)
    {
        return block == BlockType.Air;
    }
}

public static class BlockFabric
{

    public static readonly float TEXTURE_SIZE = 16;

    public static Vector2 GetUV(BlockType block)
    {

        switch (block)
        {
            case BlockType.Grass:
                return new Vector2(0, 15 / TEXTURE_SIZE);
            case BlockType.Cobblestone:
                return new Vector2(0, 14 / TEXTURE_SIZE);
            case BlockType.Rock:
                return new Vector2(1 / TEXTURE_SIZE, 15 / TEXTURE_SIZE);
            case BlockType.RockDark:
                return new Vector2(1 / TEXTURE_SIZE, 14 / TEXTURE_SIZE);
            case BlockType.Sand:
                return new Vector2(2 / TEXTURE_SIZE, 14 / TEXTURE_SIZE);
            default:
                return Vector2.zero;
        }

    }

    /*
    public static Block GetBlock(BlockType type)
    {
        return new Block(type);
    }
    */
}