public class RunContext
{
    public int seed;
    public int roomIndex;

    public RunContext(int seed)
    {
        this.seed = seed;
        this.roomIndex = 0;
    }

    public void Reset(int newSeed)
    {
        seed = newSeed;
        roomIndex = 0;
    }
}
