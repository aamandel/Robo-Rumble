using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    private int seed;
    [SerializeField] private int worldWidth = 40;
    [SerializeField] private int worldHeight = 20;
    
    private int[,] worldSpace;
    public PlatformGenerator platformGenerator;
    private List<Vector3> spawnPoints = new List<Vector3>();

    void Start()
    {
        seed = Random.Range(0, 500);
        Debug.Log("seed: " + seed);
        Initialize();
    }

    public void Initialize()
    {
        worldSpace = new int[worldWidth, worldHeight];
        Random.InitState(seed);
        // set players to positions
        spawnPoints = platformGenerator.GeneratePlatforms(worldSpace, seed);
        if (StaticData.p1GO != null && StaticData.p2GO != null && spawnPoints.Count > 0)
        {
            StaticData.p1GO.transform.position = spawnPoints[0];
            StaticData.p2GO.transform.position = spawnPoints[spawnPoints.Count - 1];
        }
    }

    public List<Vector3> GetSpawnPoints()
    {
        if(spawnPoints.Count > 0)
        {
            return spawnPoints;
        }
        List<Vector3> substitute = new List<Vector3>();
        substitute.Add(Vector3.zero);
        return substitute;
    }
}
