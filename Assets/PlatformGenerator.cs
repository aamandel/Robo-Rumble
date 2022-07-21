using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlatformGenerator : MonoBehaviour
{
    public abstract List<Vector3> GeneratePlatforms(int[,] worldSpace, int seed);
}

class Platform
{
    public int cornerX, cornerY, width;
    public PlatformContainer myContainer;
    public Platform(int _x, int _y, int _width)
    {
        cornerX = _x;
        cornerY = _y;
        width = _width;
        myContainer = null;
    }

    public Vector2 GetPoint()
    {
        return new Vector2(cornerX, cornerY);
    }
}
class PlatformContainer
{
    public List<Platform> platforms = new List<Platform>();

    public void addPlatform(Platform _platform)
    {
        platforms.Add(_platform);
    }
}

