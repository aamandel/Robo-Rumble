using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public bool titleScreen = false, joinPlayersScene = false, trafficScene = true, jungleTempleScene = true, bananaScene = true, rockCliffScene = true;
    public float chanceOfBanana = 0.1f;
    private List<string> scenes;

    public void ChangeScene()
    {
        scenes = new List<string>();
        if (joinPlayersScene)
        {
            scenes.Add("JoinPlayersScene");
        }
        if (trafficScene)
        {
            scenes.Add("Traffic");
        }
        if (jungleTempleScene)
        {
            scenes.Add("Jungle Temple");
        }
        if (rockCliffScene)
        {
            scenes.Add("Rock Cliffs");
        }
        if(bananaScene && Random.Range(0f, 1f) < chanceOfBanana)
        {
            SceneManager.LoadScene("Banana");
            return;
        }
        SceneManager.LoadScene(scenes[Random.Range(0, scenes.Count)]);
    }
}
