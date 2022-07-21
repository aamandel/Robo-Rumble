using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficPlatformGenerator : PlatformGenerator
{
    [SerializeField] private int numPlatforms = 5;
    [SerializeField] private int maxMovingPlatforms = 5;
    private int worldWidth;
    private int worldHeight;
    [SerializeField] private int maxPlatformWidth = 7;
    [SerializeField] private int maxPlatformHeight = 2;
    [SerializeField] private int maxMovingPlatformWidth = 7;
    [SerializeField] private int maxMovingPlatformHeight = 1;
    [SerializeField] private int maxMovingPlatformDist = 50; // must be larger than 5
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private GameObject weaponSpawnerPrefab;
    [SerializeField] [Range(0f, 1f)] private float chanceOfWeaponSpawner;
    [SerializeField] private GameObject movingPlatformPrefab;
    [SerializeField] float scale = 5f;
    public GameObject marker;
    private List<Platform> platforms = new List<Platform>();
    private List<PlatformContainer> platformContainers = new List<PlatformContainer>();
    [SerializeField] private float platformConnectDist = 10f;
    //spawn points
    [SerializeField] public int numSpawnPoints = 4;
    private List<Vector3> spawnPoints = new List<Vector3>();
    //cars stuff
    private float carWait = 0f;
    [SerializeField] private float carFrequency = 5f;
    [SerializeField] private List<GameObject> carsPrefabs;
    // road
    [SerializeField] private GameObject roadFloorPrefab;


    // debugging
    [SerializeField] private bool debugMode = false;

    public override List<Vector3> GeneratePlatforms(int[,] worldSpace, int seed)
    {
        // instantiate road
        GameObject floor = Instantiate(roadFloorPrefab, new Vector3(0, -worldHeight / 2 - 35f, 0), Quaternion.identity);

        worldWidth = worldSpace.GetLength(0);
        worldHeight = worldSpace.GetLength(1);
        // populate worldspace with 1's for platforms, 3's for weapon spawners, 4's for spawn points
        for (int i = 0; i < numPlatforms; i++)
        {
            int platformCornerX = Random.Range(0, worldWidth - maxPlatformWidth / 2);
            int platformCornerY = Random.Range(0, worldHeight);
            int pWidth = Random.Range(10, maxPlatformWidth);
            int trueX = 0;
            int trueY = 0;
            bool first = true;
            int pHeight = Random.Range(1, maxPlatformHeight);
            float fSeed = Random.Range(1f, (float)seed);
            for (int x = 0; x <= pWidth; x++)
            {
                int py = Mathf.RoundToInt(Mathf.PerlinNoise(x / scale, fSeed) * 17);
                // guard clause if index out of bounds for worldspace
                if (platformCornerY + py >= worldHeight || platformCornerY + py < 0 || platformCornerX + x >= worldWidth)
                {
                    continue;
                }
                // guard clause if worldspace is already occupied
                if (!(worldSpace[platformCornerX + x, platformCornerY + py] == 0))
                {
                    continue;
                }

                // populate worldSpace
                if (platformCornerY + py - pHeight >= 0)
                {
                    for (int index = platformCornerY + py - pHeight; index < platformCornerY + py; index++)
                    {
                        // 1 encodes normal platform
                        worldSpace[platformCornerX + x, index] = 1;

                        // add truecorner for platforms list
                        if (first)
                        {
                            trueX = platformCornerX + x;
                            trueY = index;
                            platforms.Add(new Platform(trueX, trueY, pWidth));
                            first = false;
                        }

                        // if top of platform
                        if (index == platformCornerY + py - 1)
                        {
                            // chance of weapon spawner
                            if (Random.Range(0f, 1f) < chanceOfWeaponSpawner)
                            {
                                // 3 encodes weapon spawner
                                worldSpace[platformCornerX + x, index] = 3;
                            }
                        }
                    }
                }
                else
                {
                    for (int index = platformCornerY; index < platformCornerY + py; index++)
                    {
                        // add truecorner for platforms list
                        if (first)
                        {
                            trueX = platformCornerX + x;
                            trueY = index;
                            platforms.Add(new Platform(trueX, trueY, pWidth));
                            first = false;
                        }
                        worldSpace[platformCornerX + x, index] = 1;
                    }
                }
            }

            // reference for working perlin:
            /*int perlinHeight;
            for (int x=0; x< 70; x++)
            {
                perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x / 30f, 0) * 17);
                perlinHeight += 17;
                for (int y = 0; y < perlinHeight; y++)
                {
                    worldSpace[x, y] = 1;
                }
            }*/


        }

        //add spawn platform
        for (int i = 0; i < 10; i++)
        {
            worldSpace[worldWidth / 2 - 4 + i, worldHeight / 2 - 4] = 1;
        }

        // add moving platforms
        ConnectPlatforms(worldSpace);
        AddMovingPlatforms(worldSpace);

        // select player spawn points
        FindSpawnPoints(worldSpace);

        // spawn remaining items from worldSpace map
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                switch (worldSpace[x, y])
                {
                    case 1:
                        Instantiate(platformPrefab, new Vector3(x - (worldWidth / 2), y - (worldHeight / 2), 0), Quaternion.identity);
                        break;
                    case 3:
                        if (worldSpace[x, y + 1] == 0)
                        {
                            Instantiate(weaponSpawnerPrefab, new Vector3(x - (worldWidth / 2), y - (worldHeight / 2), 0), Quaternion.identity);
                        }
                        else
                        {
                            Instantiate(platformPrefab, new Vector3(x - (worldWidth / 2), y - (worldHeight / 2), 0), Quaternion.identity);
                        }
                        break;
                    case 5:
                        Instantiate(marker, new Vector3(x - (worldWidth / 2), y - (worldHeight / 2), 1), Quaternion.identity);
                        break;
                }
            }
        }

        return spawnPoints;
    }

    private void FindSpawnPoints(int[,] worldSpace)
    {
        int numLeft = numSpawnPoints / 2;
        int numRight = numLeft;

        if (numSpawnPoints % 2 == 1)
        {
            numLeft++;
        }

        // find left spawnpoints
        for (int i = 0; i < numLeft; i++)
        {
            if (i >= platforms.Count)
            {
                return;
            }
            Vector2 corner = platforms[i].GetPoint();
            int centerX = (int)(corner.x + platforms[i].width / 2);
            int cornerY = (int)corner.y;
            int height = maxPlatformHeight;
            bool found = false;
            while (!found)
            {
                bool valid = centerX < worldWidth && cornerY + height < worldHeight;
                if ((valid && worldSpace[centerX, cornerY + height] == 0) || !valid)
                {
                    found = true;
                    Vector3 actual = new Vector3(centerX - worldWidth / 2, cornerY + height - worldHeight / 2, 0);
                    spawnPoints.Add(actual);
                }
                height++;
            }
        }

        // find right spawnpoints
        for (int i = platforms.Count - 1; i > platforms.Count - 1 - numRight; i--)
        {
            if (i < 0)
            {
                return;
            }
            Vector2 corner = platforms[i].GetPoint();
            int centerX = (int)(corner.x + platforms[i].width / 2);
            int cornerY = (int)corner.y;
            int height = maxPlatformHeight;
            bool found = false;
            while (!found)
            {
                bool valid = centerX < worldWidth && cornerY + height < worldHeight;
                if ((valid && worldSpace[centerX, cornerY + height] == 0) || !valid)
                {
                    found = true;
                    Vector3 actual = new Vector3(centerX - worldWidth / 2, cornerY + height - worldHeight / 2, 0);
                    spawnPoints.Add(actual);
                }
                height++;
            }
        }

        foreach (Vector3 spawn in spawnPoints)
        {
            Debug.Log(spawn);
        }
    }


    private void AddMovingPlatforms(int[,] worldSpace)
    {
        int numMovingPlatforms = Random.Range(0, maxMovingPlatforms);
        for (int i = 0; i < numMovingPlatforms; i++)
        {
            int platformCornerX = Random.Range(0, worldWidth);
            int platformCornerY = Random.Range(0, worldHeight);
            worldSpace[platformCornerX, platformCornerY] = 2;
            GameObject newMovingPlatform = Instantiate(movingPlatformPrefab, new Vector3(platformCornerX - (worldWidth / 2), platformCornerY - (worldHeight / 2), 0), Quaternion.identity);
            MovingPlatformAssigner MPA = newMovingPlatform.GetComponent<MovingPlatformAssigner>();
            if (!MPA)
            {
                Debug.LogError("Moving Platform Assigner not found!");
                continue;
            }
            int MPWidth = Random.Range(2, maxMovingPlatformWidth);
            int MPHeight = Random.Range(1, maxMovingPlatformHeight + 1);
            int MPDist = Random.Range(5, maxMovingPlatformDist);
            float MPSpeed = Random.Range(1, 8);
            bool MPVertical = Random.value > 0.5f;
            MPA.Assign(MPWidth, MPHeight, MPDist, MPSpeed, MPVertical);
            MPA.SetPoints(new Vector3(platformCornerX - worldWidth / 2, platformCornerY - worldHeight / 2, 0));

            // if set height or width to include the distance the moving platform will travel
            if (MPVertical)
            {
                MPHeight += MPDist;

                // if the endpoint of the moving platform is out of bounds
                if (platformCornerY + MPHeight > worldHeight)
                {
                    // then spawn a platform at the end
                    SpawnLandingToRight(platformCornerX + MPWidth, platformCornerY + MPHeight);
                }

            }
            else
            {
                MPWidth += MPDist;
                // if the endpoint of the moving platform is out of bounds
                if (platformCornerX + MPWidth > worldWidth)
                {
                    // then spawn a platform at the end
                    SpawnLandingToRight(platformCornerX + MPWidth, platformCornerY + MPHeight);
                }
            }
            for (int x = 0; x < MPWidth; x++)
            {
                for (int y = 0; y < MPHeight; y++)
                {
                    if (platformCornerX + x >= worldWidth || platformCornerY + y >= worldHeight)
                    {
                        continue;
                    }
                    worldSpace[platformCornerX + x, platformCornerY + y] = 2;
                }
            }
        }
    }

    private void SpawnLandingToRight(int x, int y)
    {
        int convertedX = x - (worldWidth / 2) - 1;
        int convertedY = y - (worldHeight / 2) - 1;

        Instantiate(platformPrefab, new Vector3(convertedX + 1, convertedY, 0), Quaternion.identity);
        Instantiate(weaponSpawnerPrefab, new Vector3(convertedX + 2, convertedY, 0), Quaternion.identity);
        Instantiate(platformPrefab, new Vector3(convertedX + 3, convertedY, 0), Quaternion.identity);

    }

    private void ConnectPlatforms(int[,] worldSpace)
    {
        foreach (Platform p1 in platforms)
        {
            foreach (Platform p2 in platforms)
            {
                if (p1 == p2)
                {
                    continue;
                }
                if (p1.myContainer == null)
                {
                    p1.myContainer = new PlatformContainer();
                    p1.myContainer.addPlatform(p1);
                    platformContainers.Add(p1.myContainer);
                }
                if (p2.myContainer == null)
                {
                    p2.myContainer = new PlatformContainer();
                    p2.myContainer.addPlatform(p2);
                    platformContainers.Add(p2.myContainer);
                }
                if (GetPlatformDist(p1, p2) > platformConnectDist)
                {
                    continue;
                }
                if (p1.myContainer == p2.myContainer)
                {
                    continue;
                }

                // join containers
                PlatformContainer oldContainer = p2.myContainer;
                foreach (Platform pMoving in p2.myContainer.platforms)
                {
                    pMoving.myContainer = p1.myContainer;
                    p1.myContainer.addPlatform(pMoving);
                }
                platformContainers.Remove(oldContainer);
            }
        }

        OrderByXVal(platformContainers);

        for (int index = 0; index < platformContainers.Count; index++)
        {
            if (index == platformContainers.Count - 1)
            {
                continue;
            }
            int MPWidth = 3;
            int MPHeight = 1;
            int MPDist = 0;
            float MPSpeed = 3f;
            bool MPVertical = false;
            float x1 = platformContainers[index].platforms[0].cornerX - worldWidth / 2 + platformContainers[index].platforms[0].width + 1;
            float y1 = platformContainers[index].platforms[0].cornerY - worldHeight / 2;
            float x2 = platformContainers[index + 1].platforms[0].cornerX - worldWidth / 2 - MPWidth;
            float y2 = platformContainers[index + 1].platforms[0].cornerY - worldHeight / 2;

            Vector3 p1 = new Vector3(x1, y1, 0);
            Vector3 p2 = new Vector3(x2, y2, 0);

            GameObject newMovingPlatform = Instantiate(movingPlatformPrefab, p1, Quaternion.identity);
            MovingPlatformAssigner MPA = newMovingPlatform.GetComponent<MovingPlatformAssigner>();
            if (!MPA)
            {
                Debug.LogError("Moving Platform Assigner not found!");
                continue;
            }
            MPA.Assign(MPWidth, MPHeight, MPDist, MPSpeed, MPVertical);
            MPA.SetPointsRaw(p1, p2);
        }

        // debugging below this point:
        if (!debugMode)
        {
            return;
        }

        // draw lines
        foreach (PlatformContainer PC in platformContainers)
        {
            foreach (Platform p in PC.platforms)
            {
                Vector3 start = new Vector3(PC.platforms[0].GetPoint().x - 5 - worldWidth / 2, PC.platforms[0].GetPoint().y - 5 - worldHeight / 2, 0);
                Vector3 end = new Vector3(p.GetPoint().x - worldWidth / 2, p.GetPoint().y - worldHeight / 2, 0);
                DrawLine(start, end, Color.red, 100f);
            }
        }
    }

    void OrderByXVal(List<PlatformContainer> containers)
    {
        foreach (PlatformContainer PC in containers)
        {
            PC.platforms.Sort(SortPlatforms);
        }
        containers.Sort(SortContainers);
    }

    int SortPlatforms(Platform p1, Platform p2)
    {
        return p1.cornerX.CompareTo(p2.cornerX);
    }

    int SortContainers(PlatformContainer PC1, PlatformContainer PC2)
    {
        return PC1.platforms[0].cornerX.CompareTo(PC2.platforms[0].cornerX);
    }

    float GetPlatformDist(Platform p1, Platform p2)
    {
        float actualDist = (p1.GetPoint() - p2.GetPoint()).magnitude;
        float otherDist;
        if (p1.GetPoint().x > p2.GetPoint().x)
        {
            otherDist = (p1.GetPoint() - new Vector2(p2.GetPoint().x + p2.width, p2.GetPoint().y)).magnitude;
        }
        else
        {
            otherDist = (p2.GetPoint() - new Vector2(p1.GetPoint().x + p1.width, p1.GetPoint().y)).magnitude;
        }
        return Mathf.Min(actualDist, otherDist);
    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        //lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }

    private void Update()
    {
        if (carWait <= 0)
        {
            carWait = Random.Range(0.1f, carFrequency);
            GameObject car = Instantiate(carsPrefabs[Random.Range(0, carsPrefabs.Count)], new Vector3(roadFloorPrefab.GetComponent<SpriteRenderer>().size.x / 2 - 5f, -worldHeight / 2, 0), Quaternion.identity);
            //car.GetComponent<Rigidbody2D>().velocity = (Vector2.left * carSpeed);
        }
        carWait -= Time.deltaTime;
    }
}
