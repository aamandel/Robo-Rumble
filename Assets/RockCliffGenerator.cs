using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockCliffGenerator : PlatformGenerator
{
    public GameObject rockCliff1, rockCliff2, rockCliff3;
    public List<Transform> spawnTransforms = new List<Transform>();
    private float[] platformx = { 0f, -12f, 12f };//, platformx2 = -12f, platformx3 = 12f;
    public float minPlatformHeight = -12f;
    public float maxPlatformHeight = 0f;
    public override List<Vector3> GeneratePlatforms(int[,] worldSpace, int seed)
    {
        List<GameObject> platforms = new List<GameObject>();
        platforms.Add(rockCliff1);
        platforms.Add(rockCliff2);
        platforms.Add(rockCliff3);

        for(int i=0; i<platformx.Length; i++)
        {
            GameObject first = platforms[Random.Range(0, platforms.Count)];
            first.transform.position = new Vector3(platformx[i], Random.Range(minPlatformHeight, maxPlatformHeight), 0);
            platforms.Remove(first);
        }
        


        // return spawns
        List<Vector3> spawns = new List<Vector3>();
        foreach (Transform t in spawnTransforms)
        {
            spawns.Add(t.position);
        }
        return spawns;
    }

   /* void SpawnPlatform(int min, int max, int height)
    {
        Vector3 center = new Vector3(Random.Range(min, max + 1), height, 0);
        int width = Random.Range(minPlatformWidth, maxPlatformWidth);
        bool swap = false;
        int left = 1;
        int right = 0;
        // bool hasflytrap to ensure at most one trap spawns on any given platform
        bool hasFlytrap = false;

        for (int i = 0; i < width; i++)
        {
            GameObject platform = platformPrefab;
            if (Random.Range(0f, 1f) < chanceOfFlyTrap && !hasFlytrap)
            {
                platform = venusFlyTrapPrefab;
                hasFlytrap = true;
            }
            else if (Random.Range(0f, 1f) < chanceOfWeaponSpawner)
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
            if (center.x + right < max)
            {
                Instantiate(platform, center + new Vector3(right, 0, 0), Quaternion.identity);
                right++;
                swap = !swap;
                continue;
            }
            if (center.x - left > min)
            {
                Instantiate(platform, center + new Vector3(-left, 0, 0), Quaternion.identity);
                left++;
                swap = !swap;
                continue;
            }
        }
    }*/
}
