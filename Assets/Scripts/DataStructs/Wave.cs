using System;

[System.Serializable]
public class Wave
{
    public int spawnerCount;
    public float spawnRate;
    public int lengthOfWave;

    public Wave(int spawnerCount, float spawnRate, int lengthOfWave)
    {
        this.spawnerCount = spawnerCount;
        this.spawnRate = spawnRate;
        this.lengthOfWave = lengthOfWave;
    }

    public static Wave GenerateWave()
    {
        var random = new Random();
        var spawnerCount = random.Next(1, 7);
        var spawnRate = (float)(Math.Pow(random.NextDouble(), 2) * (1 - 0.5) + 1);

        var lengthOfWave = random.Next(10, 21);

        return new Wave(spawnerCount, spawnRate, lengthOfWave);
    }
}