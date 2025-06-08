public static class Random
{
    /// <summary>
    /// Determines whether a random event occurs based on a specified probability.
    /// </summary>
    /// <param name="probabilityPercentage">The probability of the event occurring, expressed as a decimal (0.0 to 1.0).</param>
    /// <returns>True if the random event occurs; otherwise, false.</returns>
    public static bool ShouldOccur(double probabilityPercentage)
    {
        return new System.Random().NextDouble() < probabilityPercentage;
    }

    public static T GetFromArray<T>(T[] arr)
    {
        if (arr is null || arr.Length == 0)
            return default;

        return arr[new System.Random().Next(arr.Length)];
    }

    public static int[] DistributeNumbers(int total, int numElements)
    {
        int[] result = new int[numElements];
        System.Random rand = new();
        int sum = 0;

        // Generate random values for the first numElements - 1 elements
        for (int i = 0; i < numElements - 1; i++)
        {
            result[i] = rand.Next(0, total + 1 - sum); // Ensure the remaining total can be used for the last element
            sum += result[i];
        }

        // The last element should ensure the sum is exactly equal to total
        result[numElements - 1] = total - sum;

        return result;
    }

    /// <summary>
    /// Get a random number within a range (inclusive). E.g. if lower is 1 and upper is 5, it could return 1, 2, 3, 4, or 5
    /// </summary>
    /// <param name="lower">lower bound of random value</param>
    /// <param name="upper">upper bound of random value</param>
    /// <returns>a random value between lower and upper inclusive</returns>
    public static int GetRandomIntInRangeInclusive(int lower, int upper)
    {
        return new System.Random().Next(lower, upper + 1);
    }
}
