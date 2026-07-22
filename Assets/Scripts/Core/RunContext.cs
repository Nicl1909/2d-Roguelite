public class RunContext
{
    public int seed;
    public int roomIndex;
    public RunRng rng;

    public RunContext(int seed)
    {
        this.seed = seed;
        this.roomIndex = 0;
        this.rng = new RunRng(seed);
    }

    public void Reset(int newSeed)
    {
        seed = newSeed;
        roomIndex = 0;
        rng = new RunRng(newSeed);
    }

    public void AdvanceRoom(int extraSeedSalt = 0)
    {
        roomIndex++;
        if (extraSeedSalt != 0)
        {
            rng = new RunRng(seed ^ (roomIndex * 1009 + extraSeedSalt));
        }
    }
}
