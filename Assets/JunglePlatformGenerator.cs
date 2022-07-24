using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunglePlatformGenerator : PlatformGenerator
{
    [SerializeField] private int maxPlatformWidth = 9;
    [SerializeField] private int minPlatformWidth = 3;
    private int numPlatforms = 8;
    private int worldWidth;
    private int worldHeight;
    public GameObject platformPrefab;
    public GameObject venusFlyTrapPrefab;
    public GameObject weaponSpawnerPrefab;
    public float chanceOfFlyTrap = 0.05f;
    public float chanceOfWeaponSpawner = 0.05f;
    public List<Transform> spawnTransforms = new List<Transform>();
    public override List<Vector3> GeneratePlatforms(int[,] worldSpace, int seed)
    {
        worldWidth = worldSpace.GetLength(0);
        worldHeight = worldSpace.GetLength(1);
        // left platforms
        int currY = worldHeight / 2;
        SpawnPlatform(-worldWidth / 2, 0, currY);
        currY -= 3;
        for (int i=0; i < numPlatforms/2 - 2; i++)
        {
            SpawnPlatform(-worldWidth / 2, 0, currY);
            currY -= 4;
        }
        SpawnPlatform(-worldWidth / 2, -5, currY);

        // right platforms
        currY = worldHeight / 2;
        SpawnPlatform(0, worldWidth / 2, currY);
        currY -= 3;
        for (int i = 0; i < numPlatforms / 2 - 2; i++)
        {
            SpawnPlatform(0, worldWidth / 2, currY);
            currY -= 4;
        }
        SpawnPlatform(5, worldWidth / 2, currY);


        // return spawns
        List<Vector3> spawns = new List<Vector3>();
        foreach(Transform t in spawnTransforms)
        {
            spawns.Add(t.position);
        }
        return spawns;
    }

    void SpawnPlatform(int min, int max, int height)
    {
        Vector3 center = new Vector3(Random.Range(min, max+1), height, 0);
        int width = Random.Range(minPlatformWidth, maxPlatformWidth);
        bool swap = false;
        int left = 1;
        int right = 0;
        // bool hasflytrap to ensure at most one trap spawns on any given platform
        bool hasFlytrap = false;

        for(int i=0; i<width; i++)
        {
            GameObject platform = platformPrefab;
            if (Random.Range(0f, 1f) < chanceOfFlyTrap && !hasFlytrap)
            {
                platform = venusFlyTrapPrefab;
                hasFlytrap = true;
            }else if(Random.Range(0f, 1f) < chanceOfWeaponSpawner)
            {
                platform = weaponSpawnerPrefab;
            }
            
            if (swap && center.x + right < max)
            {
                Instantiate(platform, center + new Vector3(right, 0, 0), Quaternion.identity);
                right++;
                swap = !swap;
                continue;
            }
            if (!swap && center.x - left > min)
            {
                Instantiate(platform, center + new Vector3(-left, 0, 0), Quaternion.identity);
                left++;
                swap = !swap;
                continue;
            }
            if(center.x + right < max)
            {
                Instantiate(platform, center + new Vector3(right, 0, 0), Quaternion.identity);
                right++;
                swap = !swap;
                continue;
            }
            if(center.x - left > min)
            {
                Instantiate(platform, center + new Vector3(-left, 0, 0), Quaternion.identity);
                left++;
                swap = !swap;
                continue;
            }
        }
    }
}
