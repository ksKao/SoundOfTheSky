public static class Random
{
    /// <summary>
    /// Determines whether a random event occurs based on a specified probability.
    /// </summary>
    /// <param name="probabilityPercentage">The probability of the event occurring, expressed as a decimal (0.0 to 1.0).</param>
    /// <returns>True if the random event occurs; otherwise, false.</returns>
    public static bool ShouldOccur(double probabilityPercentage)
    {
        return new System.Random().NextDouble() <= probabilityPercentage;
    }

    public static T GetFromArray<T>(T[] arr)
    {
        return arr[new System.Random().Next(arr.Length)];
    }
}
