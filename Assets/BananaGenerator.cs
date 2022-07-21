using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaGenerator : MonoBehaviour
{
    public GameObject bananaSpawner;
    public void GenerateBananas(int[,] worldSpace, int seed)
    {
        GameObject bSpawner = Instantiate(bananaSpawner, Vector3.zero, Quaternion.identity);

    }
}
