using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    public TextMeshProUGUI p1WeaponTMP, p2WeaponTMP;
    public TextMeshProUGUI p1AmmoTMP, p2AmmoTMP;
    public Image[] p1Lives;
    public Image[] p2Lives;
    private GameObject player1, player2;
    public Animator roundOverAnimator;
    public TextMeshProUGUI roundOverTMP;
    private bool gameOver;
    public SceneChanger sceneChanger;

    private void Awake()
    {
        gameOver = false;
    }


    public void connectToPlayers()
    {
        player1 = StaticData.p1GO;
        player2 = StaticData.p2GO;

    }

    public void SetUI()
    {
        SetWeaponText();
        SetLives();
    }

    private void SetLives()
    {
        for(int i=0; i<p1Lives.Length; i++)
        {
            if(i > StaticData.p1Lives - 1)
            {
                p1Lives[i].enabled = false;
                continue;
            }
            p1Lives[i].enabled = true;
        }
        for (int i = 0; i < p2Lives.Length; i++)
        {
            if (i > StaticData.p2Lives - 1)
            {
                p2Lives[i].enabled = false;
                continue;
            }
            p2Lives[i].enabled = true;
        }

        if(StaticData.p1Lives <= 0 && !gameOver)
        {
            //EndRoundUI(StaticData.p2GO.name, StaticData.p1GO.name);
            EndRoundUI("Player2", "Player1");
            Time.timeScale = 0.5f;
            gameOver = true;
        }
        if (StaticData.p2Lives <= 0 && !gameOver)
        {
            //EndRoundUI(StaticData.p1GO.name, StaticData.p2GO.name);
            EndRoundUI("Player1", "Player2");
            Time.timeScale = 0.5f;
            gameOver = true;
        }
    }

    

    private void SetWeaponText()
    {
        WeaponController WC = null;
        Weapon weapon = null;
        if (player1)
        {
            WC = player1.GetComponent<WeaponController>();
        }
        if (WC)
        {
            weapon = WC.GetWeaponScript();
        }
        if (weapon)
        {
            p1WeaponTMP.text = weapon.name.Replace("(Clone)", " ");
            int currshots = weapon.GetCurrShots();
            int capacity = weapon.GetShotCapacity();
            p1AmmoTMP.text = currshots + "/" + capacity;
        }
        else
        {
            p1WeaponTMP.text = "fists";
            p1AmmoTMP.text = "0/0";
        }

        WC = null;
        weapon = null;
        if (player2)
        {
            WC = player2.GetComponent<WeaponController>();
        }
        if (WC)
        {
            weapon = WC.GetWeaponScript();
        }
        if (weapon)
        {
            p2WeaponTMP.text = weapon.name.Replace("(Clone)", " ");
            int currshots = weapon.GetCurrShots();
            int capacity = weapon.GetShotCapacity();
            p2AmmoTMP.text = currshots + "/" + capacity;
        }
        else
        {
            p2WeaponTMP.text = "fists";
            p2AmmoTMP.text = "0/0";
        }
    }

    private void EndRoundUI(string winnerName, string loserName)
    {
        string[] roundOverMessages = {
            $"{winnerName} is cracked outta their jorts",
            $"{winnerName} dominated",
            "ez",
            $"{loserName} LLLLLLL",
            $"{winnerName} is a sweaty gamer",
            $"{winnerName} > {loserName}",
            $"It's ok {loserName}, you learn more from losing than winning",
            $"{loserName} drinks!"
        };
        roundOverAnimator.SetBool("RoundIsOver", true);
        roundOverTMP.text = roundOverMessages[Random.Range(0, roundOverMessages.Length)];
        Invoker.InvokeDelayed(LoadNextScene, 6f);
    }

    private void LoadNextScene()
    {
        Time.timeScale = 1;
        sceneChanger.ChangeScene();
    }
}
