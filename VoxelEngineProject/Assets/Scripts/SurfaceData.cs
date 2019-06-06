using System;

[System.Serializable]
public class SurfaceData
{
    
    private int size;
    private Block[] blocks;

    public SurfaceData(int size)
    {
        this.size = size;
        blocks = new Block[size * size];
    }

    public SurfaceData(int size, Block[] array)
    {
        this.size = size;
        blocks = array;
    }

    public bool SetBlocks(Block[] array)
    {
        if (array.Length > size * size)
            return false;

        blocks = array;
        return true;
    }

    public void SetBlock(int x, int y, Block block)
    {
        blocks[x + y * size] = block;
    }

    public void SetBlock(int x, int y, BlockType type, float value)
    {
        blocks[x + y * size] = new Block(type, value);
    }

    public Block GetBlock(int x, int y)
    {
        return blocks[x + y * size];
    }

    public Block GetBlockBilinear(double x, double y)
    {
        x = MathHelper.Fract(x);
        y = MathHelper.Fract(y);
        int ix = (int)Math.Floor(x * size);
        int iy = (int)Math.Floor(y * size);
        double fx = (x * size) - ix;
        double fy = (y * size) - iy;

        float a = blocks[ ix     + iy      * size ].density;
        float b = blocks[ inc(ix)+ iy      * size ].density;
        float c = blocks[ ix     + inc(iy) * size ].density;
        float d = blocks[ inc(ix)+ inc(iy) * size ].density;

        double val = MathHelper.Lerp(MathHelper.Lerp(a, b, fx), MathHelper.Lerp(c, d, fx), fy);
        return new Block(blocks[ix + iy * size].type, (float)val);
    }
    
    private int inc(int i)
    {
        return (i + 1) % size;
    }

}
