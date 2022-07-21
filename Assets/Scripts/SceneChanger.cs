using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private Button continueButton;

    private void Awake()
    {
        continueButton = GetComponent<Button>();
    }
    public void ContinueToGame()
    {
        SceneManager.LoadScene("Main Game");
    }

    private void Update()
    {
        if( PlayerHandler.GetNumPlayers() == 2)
        {
            continueButton.interactable = true;
        }
    }
}
