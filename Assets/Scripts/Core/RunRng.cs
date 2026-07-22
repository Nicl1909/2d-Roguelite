using UnityEngine;

public class RunRng
{
    private System.Random rng;

    public RunRng(int seed)
    {
        rng = new System.Random(seed);
    }

    public int Next()
    {
        return rng.Next();
    }

    public int Range(int minInclusive, int maxExclusive)
    {
        return rng.Next(minInclusive, maxExclusive);
    }

    public float Range(float minInclusive, float maxInclusive)
    {
        float t = (float)rng.NextDouble();
        return Mathf.Lerp(minInclusive, maxInclusive, t);
    }

    public float Value()
    {
        return (float)rng.NextDouble();
    }
}
