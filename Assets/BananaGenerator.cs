using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaGenerator : PlatformGenerator
{
    [SerializeField] private int maxPlatformWidth = 9;
    [SerializeField] private int minPlatformWidth = 3;
    private int worldWidth;
    private int worldHeight;
    public GameObject platformPrefab;
    public List<Transform> spawnTransforms = new List<Transform>();
    public override List<Vector3> GeneratePlatforms(int[,] worldSpace, int seed)
    {
        worldWidth = worldSpace.GetLength(0);
        worldHeight = worldSpace.GetLength(1);
        // left platform(s)
        int numPlatforms = Random.Range(1, 3);
        for (int i = 0; i < numPlatforms; i++)
        {
            SpawnPlatform(-7, 8, -11);
        }

        // right platforms
        numPlatforms = Random.Range(1, 3);
        for (int i = 0; i < numPlatforms; i++)
        {
            SpawnPlatform(-7, 8, 11);
        }

        // return spawns
        List<Vector3> spawns = new List<Vector3>();
        foreach (Transform t in spawnTransforms)
        {
            spawns.Add(t.position);
        }
        return spawns;
    }

    void SpawnPlatform(int min, int max, int xVal)
    {
        Vector3 center = new Vector3(xVal, Random.Range(min, max + 1), 0);
        Debug.Log(center);
        int width = Random.Range(minPlatformWidth, maxPlatformWidth);
        bool swap = false;
        int left = 1;
        int right = 0;

        for (int i = 0; i < width; i++)
        {
            GameObject platform = platformPrefab;
            if (swap)
            {
                Instantiate(platform, center + new Vector3(right, 0, 0), Quaternion.identity);
                right++;
                swap = !swap;
                continue;
            }
            else
            {
                Instantiate(platform, center + new Vector3(-left, 0, 0), Quaternion.identity);
                left++;
                swap = !swap;
                continue;
            }
        }
    }

}
