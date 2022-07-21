using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private int seed = 42;
    [SerializeField] private int worldWidth = 40;
    [SerializeField] private int worldHeight = 20;
    
    private int[,] worldSpace;
    public PlatformGenerator platformGenerator;
    private List<Vector3> spawnPoints = new List<Vector3>();

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        worldSpace = new int[worldWidth, worldHeight];
        Random.InitState(seed);
        // set players to positions
        spawnPoints = platformGenerator.GeneratePlatforms(worldSpace, seed);
        if (PlayerHandler.players.Count > 1 && spawnPoints.Count > 0)
        {
            PlayerHandler.players[0].GetGO().transform.position = spawnPoints[0];
            PlayerHandler.players[1].GetGO().transform.position = spawnPoints[spawnPoints.Count - 1];
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
