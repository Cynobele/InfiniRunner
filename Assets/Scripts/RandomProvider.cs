using System.Threading;
using System;

//Ref: https://stackoverflow.com/a/7251724/15562413

public class RandomProvider //gets a better random value than System.Random()
{                           //increments seed whenever a new number is needed

    private static int seed = Environment.TickCount;

    private static ThreadLocal<Random> random_val = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

    public static Random GetRandomVal()
    {
        return random_val.Value;
    }
}
